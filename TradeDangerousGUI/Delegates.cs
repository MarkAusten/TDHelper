﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using SharpConfig;

namespace TDHelper
{
    public partial class MainForm : Form
    {
        /*
         * Mostly worker delegate related stuff goes here
         */

        private void CleanUpdatedPricesFile()
        {
            /*
                * This should search for Buy/Sell price for each commodity and set them to 0
                */

            string pricesFilePath = Path.Combine(settingsRef.TDPath, "prices.last");
            string pricesFileOutputPath = Path.Combine(settingsRef.TDPath, "prices.updated");
            string match = @"(\d+)\s+(\d+)\s+";
            string replace = "0        0        ";
            string contents = "";

            if (File.Exists(pricesFilePath))
            {
                using (StreamReader reader = new StreamReader(pricesFilePath))
                {
                    contents = reader.ReadToEnd();
                    reader.Close();
                }
                using (StreamWriter writer = new StreamWriter(pricesFileOutputPath))
                {
                    writer.Write(Regex.Replace(contents, match, replace));
                    writer.Close();
                }
            }
            else
                throw new Exception("Cannot open the prices file for some reason");
        }

        private void DecompressUpdate(string zipFileURL, string path)
        {
            string filePattern = @"[\-A-za-z0-9_\.]+\.zip";
            // parse the archive name from the url given
            string remoteArchiveName = Regex.Match(zipFileURL, filePattern).ToString();
            remoteArchiveLocalPath = Path.Combine(MainForm.assemblyPath, remoteArchiveName + ".tmp");

            // download the archive mentioned in the manifest
            if (UpdateClass.DownloadFile(zipFileURL, remoteArchiveLocalPath))
            {
                UpdateClass.WriteToLog(MainForm.updateLogPath, "Downloaded a dependent archive from URL: " + zipFileURL);

                // rename our conflicting files by making a list then enumerating
                foreach (string s in UpdateClass.ManifestFileList(localManifestPath))
                {
                    string localFilePath = Path.Combine(assemblyPath, s);
                    string localFileRenamed = localFilePath + ".REMOVE";

                    if (File.Exists(localFilePath))
                    {
                        File.Move(localFilePath, localFileRenamed);
                    }
                }

                UpdateClass.DecompressZip(remoteArchiveLocalPath, assemblyPath);
                UpdateClass.WriteToLog(MainForm.updateLogPath, "Attempted decompression of " + Path.GetFileName(remoteArchiveName) + " to our working directory: " + assemblyPath);

                // change flag indicator and serialize it
                settingsRef.HasUpdated = true;
                SaveSettingsToIniFile();
            }
        }

        /// <summary>
        /// Compare the two ship lists and find any in the first string that are not in the second.
        /// </summary>
        /// <param name="currentShips">The list of current ships.</param>
        /// <param name="availableShips">The list of now available ships from the profile.</param>
        /// <returns>A list of missing ships.</returns>
        private IList<string> DeterminMissingShips(
            string currentShips,
            string availableShips)
        {
            IList<string> missing = new List<string>();

            if (!string.IsNullOrEmpty(currentShips) &&
                !string.IsNullOrEmpty(availableShips))
            {
                IList<string> available = new List<string>(availableShips.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                foreach (string ship in currentShips.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!available.Contains(ship))
                    {
                        missing.Add(ship);
                    }
                }
            }

            return missing;
        }

        private void DoHotSwap()
        {
            /*
             * This delegate is for the auto-updater, it does the following:
             * 1) Downloads remote manifest to a .manifest.tmp file
             * 3) Compares the existing assemblies to the manifest contents
             * 4) Parses for the archive URL
             * 5) Downloads the archive to a .zip.tmp file
             * 6) Renames conflicting filenames (*.REMOVE)
             * 7) Unpacks to the assembly dir
             * 8) Cleans up when done
             */
            // grab the remote manifest to a tmp file
            UpdateClass.DownloadFile(remoteManifestPath, localManifestPath);

            if (File.Exists(localManifestPath))
            {
                // only grab new archive if assembly doesn't match
                if (!UpdateClass.CompareAssemblyToManifest(localManifestPath, assemblyPath) &&
                    UpdateClass.CompareVersionNumbers(localManifestPath, assemblyPath))
                {
                    DialogResult d = TopMostMessageBox.Show(
                        true,
                        true,
                        "An update is available, should we download it?",
                        "TD Helper - Confirmation",
                        MessageBoxButtons.YesNo);

                    if (d == DialogResult.Yes)
                    {
                        XDocument doc = XDocument.Load(localManifestPath);
                        XElement urlRoot = doc.Element("Manifest").Element("Assembly").Element("URL");

                        if (urlRoot != null)
                        {
                            string remoteArchiveURL = urlRoot.Value;
                            DecompressUpdate(remoteArchiveURL, assemblyPath);
                        }
                        else
                        {
                            Debug.WriteLine(doc.ToString());
                            UpdateClass.WriteToLog(MainForm.updateLogPath, "The manifest does not contain a URL tag, cannot parse for remote archive");
                        }
                    }
                    else
                    {
                        UpdateClass.WriteToLog(MainForm.updateLogPath, "The user cancelled the auto-update download");
                    }
                }
            }
        }

        private void DoHotSwapCleanup()
        {
            // clean up the working directory after a doHotSwap()
            try
            {
                // we include replaced and tmp files just to be thorough
                string[] cleanupFiles = Directory.EnumerateFiles(assemblyPath, "*.*").Where(x => x.EndsWith(".tmp") || x.EndsWith(".REMOVE")).Reverse().ToArray();

                foreach (string s in cleanupFiles)
                {
                    if (File.Exists(s))
                    {
                        File.Delete(s);
                    }
                }
            }
            catch (UnauthorizedAccessException) { /* eat it */ }
            catch (Exception e)
            {
                UpdateClass.WriteToLog(updateLogPath, "Exception: " + e.Message);
            }
        }

        private void DoRunEvent()
        {
            // before we do the thing
            // let's push all the boxes into variables
            CopySettingsFromForm();

            // Run button
            if (!backgroundWorker2.IsBusy)
            {
                // disable other worker callers
                DisablebtnStarts();

                backgroundWorker2.RunWorkerAsync();
            }
            else
            {
                if (!td_proc.HasExited)
                {
                    td_proc.Kill();
                    td_proc.Close();

                    buttonCaller = 5; // mark as the cancel button
                }
            }
        }

        private void DoTDProc(string path)
        {
            //
            // Assume we are calling this from a non-UI thread
            //

            // only run the delegate if we have a path
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    procCode = -1; // reset the exit code
                    td_proc.StartInfo.Arguments = path;

                    if (buttonCaller == 12)
                    {
                        td_proc.StartInfo.UseShellExecute = true;
                        td_proc.StartInfo.CreateNoWindow = false;
                    }
                    else
                    {
                        td_proc.StartInfo.RedirectStandardOutput = true;
                        td_proc.StartInfo.RedirectStandardInput = false;
                        td_proc.StartInfo.RedirectStandardError = true;
                        td_proc.StartInfo.UseShellExecute = false;
                        td_proc.StartInfo.CreateNoWindow = true;
                        td_proc.EnableRaisingEvents = true;
                        td_proc.SynchronizingObject = this;
                        td_proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    }

                    td_proc.OutputDataReceived += new DataReceivedEventHandler(ProcOutputDataHandler);
                    td_proc.ErrorDataReceived += new DataReceivedEventHandler(ProcErrorDataHandler);

                    // pre-invoke
                    if (circularBuffer.Length == 0)
                    {
                        StackCircularBuffer("Command line: " + path + "\n");
                    }
                    else
                    {
                        StackCircularBuffer("\nCommand line: " + path + "\n");
                    }

                    Invoke(new Action(() =>
                    {
                        if (buttonCaller != 5 && buttonCaller != 10 && buttonCaller != 11
                            && buttonCaller != 12 && buttonCaller != 13)
                        {
                            // don't show cancelling for UpdateDB/Import/Upload/Editor
                            btnStart.Font = new Font(btnStart.Font, FontStyle.Bold);
                            btnStart.Text = "&Cancel";
                            btnStart.Enabled = true;
                        }
                    }));

                    // only start the stopwatch for callers that run in the background
                    if (!backgroundWorker3.IsBusy)
                    {
                        backgroundWorker3.RunWorkerAsync();
                    }
                    else
                    {
                        stopwatch.Start();
                    }

                    td_proc.Start();
                    td_proc.Refresh(); // clear process cache between instances

                    if (buttonCaller != 12)
                    {
                        td_proc.BeginOutputReadLine();
                        td_proc.BeginErrorReadLine();
                    }

                    td_proc.WaitForExit();

                    try
                    {
                        if (td_proc.HasExited)
                        {
                            procCode = td_proc.ExitCode; // save our exit code
                        }
                    }
                    catch (Exception ex)
                    {
                        // swallow the exception for the moment.
                        Debug.WriteLine(ex.Message);
                    }
                }
                finally
                {
                    td_proc.Close();
                    td_proc.Dispose();
                }
            }
            else
            {
                buttonCaller = 20; // flag to play an error sound if we can't execute the command
            }

            // catch a few outcomes

            if (buttonCaller == 5)
            {
                if (hasUpdated == 0)
                {
                    StackCircularBuffer("\nData updated.");
                }
            }
            else if (buttonCaller == 11)
            {
                if (procCode == 0) // exit code should be 0
                {
                    StackCircularBuffer("\nZero'ing all commodities in the prices.last file, and saving to: " + settingsRef.TDPath + "\\updated.prices\nNOTE: This will -NOT- import/upload the changes, you must do so manually.");
                }
            }

            commandString = string.Empty; // reset the path for thread safety
        }

        private void GetDataUpdates()
        {
            /*
             * UPDATE DB BUTTON: called from Worker4
             */
            td_proc = new Process();
            td_proc.StartInfo.FileName = settingsRef.PythonPath;

            commandString = settingsRef.PythonPath.EndsWith("trade.exe", StringComparison.OrdinalIgnoreCase)
                ? "" // go in blank so we don't pass silliness to trade.exe
                : "-u \"" + Path.Combine(settingsRef.TDPath, "trade.py") + "\" ";

            // catch the method drop down here
            if (buttonCaller == 5)
            {
                // catch the database update button
                commandString += "import -P eddblink -O {0},progbar".With(DBUpdateCommandString);
            }
        }

        /// <summary>
        /// Get the padsizes upon which the ship may land.
        /// </summary>
        /// <param name="shipType">The internal ship type.</param>
        /// <returns>The padsizes upon which the ship may land.</returns>
        private string GetPadSizes(string shipType)
        {
            string padSizes = string.Empty;

            switch (shipType)
            {
                case "Adder":
                case "CobraMkIII":
                case "CobraMkIV":
                case "DiamondBack":
                case "DiamondBackXL":
                case "Dolphin":
                case "Eagle":
                case "Empire_Courier":
                case "Empire_Eagle":
                case "Hauler":
                case "SideWinder":
                case "Viper":
                case "Viper_MkIV":
                case "Vulture":
                    padSizes = "SML";
                    break;

                case "Anaconda":
                case "BelugaLiner":
                case "Cutter":
                case "Empire_Trader":
                case "Federation_Corvette":
                case "Orca":
                case "Type7":
                case "Type9":
                case "Type9_Military": // Type 10
                    padSizes = "L";
                    break;

                case "Asp":
                case "Asp_Scout":
                case "Federation_Dropship":
                case "Federation_Dropship_MkII":
                case "Federation_Gunship":
                case "FerDeLance":
                case "Independant_Trader": // Keelback
                case "Krait_MkII":
                case "Python":
                case "Type6":
                case "TypeX": // Chieftain
                case "TypeX_3": // Challenger
                    padSizes = "ML";
                    break;

                default:
                    padSizes = string.Empty;
                    break;
            }

            return padSizes;
        }

        private string GetShipName(JToken ship)
        {
            string shipId = string.Empty;
            string shipName = string.Empty;

            // Get the ship ID if assigned otherwise use the internal ID.
            try
            {
                shipId = (string)ship["shipID"];
            }
            catch
            {
                shipId = string.Empty;
            }

            if (string.IsNullOrEmpty(shipId))
            {
                shipId = (string)ship["id"];
            }

            // Get the ship name if it has one.
            try
            {
                shipName = (string)ship["shipName"];
            }
            catch
            {
                shipName = string.Empty;
            }

            if (!string.IsNullOrEmpty(shipName))
            {
                shipName = " ({0})".With(shipName);
            }

            // Finally, get the ship type.
            string shipType = (string)ship["name"];

            // Put the component parts together to form the ship name and type.
            return "{0} {1}{2}".With(
                TranslateShipType(shipType),
                shipId,
                shipName);
        }

        private void GetUpdatedPricesFile()
        {
            buttonCaller = 11;  // mark us as coming from the commodities editor (ctrl+click)
            string pricesFilePath = Path.Combine(settingsRef.TDPath, "prices.last");

            // first check if the input prices.last file already exists, if so delete it
            if (File.Exists(pricesFilePath))
            {
                File.Delete(pricesFilePath);
            }

            // hop to the worker delegate to grab updated prices for a station
            if (!backgroundWorker2.IsBusy)
            {
                DisablebtnStarts();
                backgroundWorker2.RunWorkerAsync();
                // head over to the worker delegate event RunWorkerCompleted for the next step
            }
        }

        private int IsValidRunOutput(string input)
        {
            // return an int if we recognize the input as valid Run output
            if (!string.IsNullOrEmpty(input) && input.Length > 1)
            {
                // test for -vvv
                string pattern1 = @"(.+?)\s->\s(.+?)\s\("; // route summary
                string pattern2 = @"Finish at.+gaining\s(.+?cr).+est\s(.+?cr)"; // total gains
                string pattern3 = @"Load from (.+?)\s\((.+?),\s(.+?)\)"; // load
                string pattern4 = @"(?>(\d+)\sx\s(.+?)\s\s+(.+?)cr\svs\s+(.+?)cr,\s(.+?)\s(\w+))(?>\svs\s(.+?)\s(\w+),\stotal:\s+(.+)|,\stotal:\s+(.+))"; // commodities
                string pattern5 = @"Unload at (.+?)\s\((.+?ls),\s(.+?)\) => Gain (.+?cr) \((.+?)(?>cr|\.)"; // unload

                MatchCollection matches1 = Regex.Matches(input, pattern1, RegexOptions.Compiled);
                MatchCollection matches2 = Regex.Matches(input, pattern2, RegexOptions.Compiled);
                MatchCollection matches3 = Regex.Matches(input, pattern3, RegexOptions.Compiled);
                MatchCollection matches4 = Regex.Matches(input, pattern4, RegexOptions.Compiled);
                MatchCollection matches5 = Regex.Matches(input, pattern5, RegexOptions.Compiled);

                if (matches1.Count > 0 & matches2.Count > 0 & matches3.Count > 0
                    & matches4.Count > 0 & matches5.Count > 0)
                    return 3;

                // next we test for -vv
                pattern4 = @"(?>(\d+?)\sx\s(.+?)\s\s+(.+?cr)\svs\s+(.+?cr),)(?>\s(.+?)\s(\w+)\svs\s(.+?)\s(\w+)|\s(.+?)\s(\w+))(?!,\stotal:)"; // only the commodity filter changes
                matches4 = Regex.Matches(input, pattern4, RegexOptions.Compiled);

                if (matches1.Count > 0 & matches2.Count > 0 & matches3.Count > 0
                    & matches4.Count > 0 & matches5.Count > 0)
                    return 2;

                // then we test for -v
                pattern1 = @"^(?!\s+Jump)(.+?)\s->\s(.+?)\s\(";
                pattern2 = @"Finish\s(.+?)\s\+\s*(.+?cr).+=>\s(.+?cr)";
                pattern3 = @"Load from (.+?):";
                pattern4 = @"(?:\:|\,)\s(.+?)\sx\s(.+?)\s\(\@(.+?cr)";
                pattern5 = @"Dock at (.+?)(?>\r\n|\r|\n)";

                matches1 = Regex.Matches(input, pattern1, RegexOptions.Compiled);
                matches2 = Regex.Matches(input, pattern2, RegexOptions.Compiled);
                matches3 = Regex.Matches(input, pattern3, RegexOptions.Compiled);
                matches4 = Regex.Matches(input, pattern4, RegexOptions.Compiled);
                matches5 = Regex.Matches(input, pattern5, RegexOptions.Compiled);

                if (matches1.Count > 0 & matches2.Count > 0 & matches3.Count > 0
                    & matches4.Count > 0 & matches5.Count > 0)
                    return 1;

                // finally, we test for null
                pattern1 = @"^(?!\s+Jump)(.+?)\s->\s(.+)";
                pattern2 = @"\+\s*(.+?)cr";
                pattern3 = @"\s+(.+?(?<!SORRY))(?>:\s|\s\+)";
                pattern4 = @"(?:\:|\,)\s(.+?)\sx\s(.+?(?=,))";

                matches1 = Regex.Matches(input, pattern1, RegexOptions.Compiled);
                matches2 = Regex.Matches(input, pattern2, RegexOptions.Compiled);
                matches3 = Regex.Matches(input, pattern3, RegexOptions.Compiled);
                matches4 = Regex.Matches(input, pattern4, RegexOptions.Compiled);

                if (matches1.Count > 0 & matches2.Count > 0 & matches3.Count > 0
                    & matches4.Count > 0 & matches5.Count == 0)
                    return 0;
            }

            // we don't recognize the output at all
            return -1;
        }

        private void ParseRunOutput(string input, TreeView tvOutput)
        {
            /*
             * This method parses the output of Run line by line and collects
             * it into XML for use later in populating the TreeView.
             */
            XDocument doc = new XDocument();
            int t_Capacity = 0;
            t_meanDist = 0;

            #region verbosity0

            if (runOutputState == 0)
            {
                // for null outputVerbosity
                // Step 1: catch System/Station -> System/Station
                // Reshape lines into:
                // {1.1} -> {1.2}
                // Details: Gain: {2.1} -> {2.2}
                string pattern1 = @"^(?!\s+Jump)(.+?)\s->\s(.+)";
                string pattern2 = @"\+\s*(.+?)cr";

                // Step 2 & 3: catch Load From and commodities
                // [3.1] System/Station, [4.1] amount of commodity, [4.2] name of commodity, [4.3] cost of commodity
                // Reshape lines into:
                // Load @ {3.1}
                // {4.1}x {4.2} @ {4.3}cr
                // ...
                string pattern3 = @"\s+(.+?(?<!SORRY))(?>:\s|\s\+)";
                string pattern4 = @"(?:\:|\,)\s(.+?)\sx\s(.+?(?=,))";

                // Step 4: catch jumps between dock/undock
                // SYSTEM -> SYSTEM
                // NOTE: We must filter the first match to prevent disorder
                string pattern5 = @"((?<=>\s).+?(?=\s-|\s\s))"; // short form for jumps

                // Compile our patterns
                MatchCollection matches1 = Regex.Matches(input, pattern1, RegexOptions.Compiled);
                MatchCollection matches2 = Regex.Matches(input, pattern2, RegexOptions.Compiled);
                MatchCollection matches3 = Regex.Matches(input, pattern3, RegexOptions.Compiled);
                MatchCollection matches4 = Regex.Matches(input, pattern4, RegexOptions.Compiled);
                MatchCollection matches5 = Regex.Matches(input, pattern5, RegexOptions.Compiled);

                // Now we compile the output into XML
                doc = new XDocument(new XElement("Route", new XElement("Summary", string.Format("{0} -> {1}", matches1[0].Groups[1].Value, matches1[0].Groups[2].Value))));

                XElement element = doc.Descendants("Route").FirstOrDefault(); // move to our list root

                // put our details in a child node
                element.Add(new XElement("SummaryDetails", string.Format("Gain: {0}cr", matches2[0].Groups[1].Value)));

                int j = 0, k = 0, l = 1; // external incrementers
                for (int i = 0; i < matches4.Count; i++)
                {
                    if (matches3[j].Index < matches4[i].Index)
                    {
                        element.Add(new XElement("Hop"));
                        element = doc.Descendants("Hop").Last();
                        element.Add(new XElement("LoadAt", string.Format("Load @ {0}", matches3[j].Groups[1].Value)));
                        j++;
                    }

                    while (i < matches4.Count && matches4[i].Index < matches3[k + 1].Index)
                    {
                        element.Add(new XElement("Load", string.Format("{0}x {1}", matches4[i].Groups[1].Value, matches4[i].Groups[2].Value)));

                        t_Capacity += int.Parse(matches4[i].Groups[1].Value);

                        // intelligently increment to next match index
                        if (i + 1 < matches4.Count && matches4[i + 1].Index < matches3[k + 1].Index)
                            i++;
                        else
                            break;
                    }

                    if (matches5.Count > 0 && l < matches5.Count)
                    {
                        element.Add(new XElement("Jumps"));
                        element = doc.Descendants("Jumps").Last();

                        while (matches5[l].Index < matches3[k + 1].Index)
                        {
                            element.Add(new XElement("Jump", string.Format("{0}", matches5[l].Groups[1].Value)));

                            if (l + 1 < matches5.Count)
                                l++;
                            else
                                break; // prevent OOR
                        }
                    }

                    if (k < matches3.Count && matches3[k + 1].Index > matches4[i].Index)
                    {
                        // only print if we're beyond the last commodity statement
                        element = doc.Descendants("Hop").Last();
                        element.Add(new XElement("UnloadAt", string.Format("Unload @ {0}", matches3[k + 1].Groups[1].Value)));

                        if (k + 1 < matches3.Count)
                            k++;
                        else
                            break; // break the loop to avoid OOR
                    }

                    element = doc.Descendants("Route").FirstOrDefault(); // reset the element
                }

                double savedGain = int.Parse(matches2[0].Groups[1].Value.Replace(",", "").Replace(".", "").Replace("cr", ""));
                double savedCapacity = Math.Round(t_Capacity / ((double)matches3.Count - 1));
                t_CrTonTally = savedGain / savedCapacity / (matches3.Count - 1);
                t_childTitle = string.Format("Est. Profit: {0:n0}cr/t", t_CrTonTally);
            }

            #endregion verbosity0

            #region verbosity1

            if (runOutputState == 1)
            {
                // for outputVerbosity == -v
                // Step 1: catch System/Station -> System/Station
                // Reshape lines into:
                // {1.1} -> {1.2}
                // Details: Gain: {2.1} -> {2.2}
                string pattern1 = @"^(?!\s+Jump)(.+?)\s->\s(.+?)\s\(";
                string pattern2 = @"Finish\s(.+?)\s\+\s*(.+?cr).+=>\s(.+?cr)";

                // Step 2: catch Load From and commodities
                // [3.1] System/Station, [4.1] amount of commodity, [4.2] name of commodity, [4.3] cost of commodity
                // Reshape lines into:
                // Load @ {3.1}
                // {4.1}x {4.2} @ {4.3}cr
                // ...
                string pattern3 = @"Load from (.+?):";
                string pattern4 = @"(?:\:|\,)\s(.+?)\sx\s(.+?)\s\(\@(.+?cr)";

                // Step 3: catch Dock at
                // [5.1] System/Station
                // Reshape lines into: Unload @ {5.1}
                string pattern5 = @"Dock at (.+?)(?>\r\n|\r|\n)";

                // Step 4: catch jumps between dock/undock
                // xx.xxly -> SYSTEM
                // {1.2} [{1.1}]
                string pattern6 = @"(?:,)?\s(\d*\.\d*ly)\s->\s(.+?)(?:,|\s\s)"; // long form for jumps

                // Compile our patterns
                MatchCollection matches1 = Regex.Matches(input, pattern1, RegexOptions.Compiled);
                MatchCollection matches2 = Regex.Matches(input, pattern2, RegexOptions.Compiled);
                MatchCollection matches3 = Regex.Matches(input, pattern3, RegexOptions.Compiled);
                MatchCollection matches4 = Regex.Matches(input, pattern4, RegexOptions.Compiled);
                MatchCollection matches5 = Regex.Matches(input, pattern5, RegexOptions.Compiled);
                MatchCollection matches6 = Regex.Matches(input, pattern6, RegexOptions.Compiled);

                // Now we compile the output into XML
                doc = new XDocument(new XElement("Route", new XElement("Summary", string.Format("{0} -> {1}", matches1[0].Groups[1].Value, matches1[0].Groups[2].Value))));

                XElement element = doc.Descendants("Route").FirstOrDefault(); // move to our list root

                // put our details in a child node
                element.Add(new XElement("SummaryDetails", string.Format("Gain: {0} -> {1}", matches2[0].Groups[2].Value, matches2[0].Groups[3].Value)));

                int j = 0, k = 0, l = 0; // external incrementers
                for (int i = 0; i < matches4.Count; i++)
                {
                    if (matches3[j].Index < matches4[i].Index)
                    {
                        // make an entry for this hop
                        element.Add(new XElement("Hop"));
                        element = doc.Descendants("Hop").Last();
                        element.Add(new XElement("LoadAt", string.Format("Load @ {0}", matches3[j].Groups[1].Value)));
                        j++;
                    }

                    while (i < matches4.Count && matches4[i].Index < matches5[k].Index)
                    {
                        // process the commodities
                        element.Add(new XElement("Load", string.Format("{0}x {1} @ {2}", matches4[i].Groups[1].Value, matches4[i].Groups[2].Value, matches4[i].Groups[3].Value)));

                        t_Capacity += int.Parse(matches4[i].Groups[1].Value);

                        // intelligently increment to next match index
                        if (i + 1 < matches4.Count && matches4[i + 1].Index < matches5[k].Index)
                            i++;
                        else
                            break;
                    }

                    if (matches6.Count > 0 && l < matches6.Count)
                    {
                        // process the jumps, if they exist
                        element.Add(new XElement("Jumps"));
                        element = doc.Descendants("Jumps").Last();

                        while (l < matches6.Count && matches6[l].Index < matches5[k].Index)
                        {
                            element.Add(new XElement("Jump", string.Format("{0} [{1}]", matches6[l].Groups[2].Value, matches6[l].Groups[1].Value)));

                            l++;
                        }
                    }

                    if (k < matches5.Count && matches5[k].Index > matches4[i].Index)
                    {
                        // make an entry for the destination
                        element = doc.Descendants("Hop").Last();
                        element.Add(new XElement("UnloadAt", string.Format("Unload @ {0}", matches5[k].Groups[1].Value)));

                        if (k + 1 < matches5.Count)
                            k++;
                        else
                            break; // break the loop to avoid OOR
                    }

                    element = doc.Descendants("Route").FirstOrDefault(); // reset the element
                }

                double savedGain = int.Parse(matches2[0].Groups[2].Value.Replace(",", "").Replace(".", "").Replace("cr", ""));
                double savedCapacity = Math.Round(t_Capacity / (double)matches5.Count);
                t_CrTonTally = savedGain / savedCapacity / matches5.Count;
                t_childTitle = string.Format("Est. Profit: {0:n0}cr/t", t_CrTonTally);
            }

            #endregion verbosity1

            #region verbosity2

            else if (runOutputState == 2)
            {
                // for outputVerbosity == -vv
                // Step 1: catch System/Station -> System/Station
                // Reshape lines into:
                // {1.1} -> {1.2}
                string pattern1 = @"(.+?)\s->\s(.+?)\s\(";
                // Gain: {2.1} -> {2.2}
                string pattern2 = @"Finish at.+gaining\s(.+?cr).+est\s(.+?cr)";

                // Step 2: catch Load From
                // [1] System/Station, [2] LS from star, [3] stn details
                string pattern3 = @"Load from (.+?)\s\((.+?),\s(.+?)\)";

                // Step 3: catch as many iterations of commodities as exist
                // [1] Num of items, [2] Name of items, [3] credit cost src, [4] credit cost dest
                // [5] age src, [6] age name, [7] age dest, [8] age name
                string pattern4 = @"(?>(\d+?)\sx\s(.+?)\s\s+(.+?cr)\svs\s+(.+?cr),)(?>\s(.+?)\s(\w+)\svs\s(.+?)\s(\w+)|\s(.+?)\s(\w+))(?!,\stotal:)";

                // Step 4: catch Unload at
                // [1] System/Station, [2] LS from star, [3] stn details, [4] gain/cr, [5] cr/ton
                string pattern5 = @"Unload at (.+?)\s\((.+?ls),\s(.+?)\) => Gain (.+?cr) \((.+?)cr";

                // Step 5: catch jumps between dock/undock
                // xx.xxly -> SYSTEM
                // {1.2} [{1.1}]
                string pattern6 = @"(?:,)?\s(\d*\.\d*ly)\s->\s(.+?)(?:,|\s\s)"; // long form for jumps

                // Compile our patterns
                MatchCollection matches1 = Regex.Matches(input, pattern1, RegexOptions.Compiled);
                MatchCollection matches2 = Regex.Matches(input, pattern2, RegexOptions.Compiled);
                MatchCollection matches3 = Regex.Matches(input, pattern3, RegexOptions.Compiled);
                MatchCollection matches4 = Regex.Matches(input, pattern4, RegexOptions.Compiled);
                MatchCollection matches5 = Regex.Matches(input, pattern5, RegexOptions.Compiled);
                MatchCollection matches6 = Regex.Matches(input, pattern6, RegexOptions.Compiled);

                // Now we compile the output into XML
                doc = new XDocument(new XElement("Route", new XElement("Summary", string.Format("{0} -> {1}", matches1[0].Groups[1].Value, matches1[0].Groups[2].Value))));

                XElement element = doc.Descendants("Route").FirstOrDefault();

                element.Add(new XElement("SummaryDetails", string.Format("Gain: {0} -> est. {1}", matches2[0].Groups[1].Value, matches2[0].Groups[2].Value)));

                int j = 0, k = 0, l = 0;
                for (int i = 0; i < matches4.Count; i++)
                {
                    if (matches3[j].Index < matches4[i].Index)
                    {
                        element.Add(new XElement("Hop"));
                        element = doc.Descendants("Hop").Last();
                        element.Add(new XElement("LoadAt", string.Format("Load @ {0} [{1}]", matches3[j].Groups[1].Value, matches3[j].Groups[2].Value)));
                        element.Add(new XElement("LoadDetails", string.Format("{0}", matches3[j].Groups[3].Value)));
                        j++;
                    }

                    while (i < matches4.Count && matches4[i].Index < matches5[k].Index)
                    {
                        // collect the working capacity of the run
                        t_Capacity += int.Parse(matches4[i].Groups[1].Value);

                        if (!string.IsNullOrEmpty(matches4[i].Groups[9].Value))
                        {
                            // we're missing the dest. time, let's match based on src. time
                            element.Add(new XElement("Load", string.Format("{0}x {1} @ {2}/{3} [{4}{5}]", matches4[i].Groups[1].Value, matches4[i].Groups[2].Value, matches4[i].Groups[3].Value, matches4[i].Groups[4].Value, matches4[i].Groups[9].Value, matches4[i].Groups[10].Value)));

                            // intelligently increment to next match index
                            if (i + 1 < matches4.Count && matches4[i + 1].Index < matches5[k].Index)
                                i++;
                            else
                                break;
                        }
                        else
                        {
                            element.Add(new XElement("Load", string.Format("{0}x {1} @ {2}/{3} [{4}{5}|{6}{7}]", matches4[i].Groups[1].Value, matches4[i].Groups[2].Value, matches4[i].Groups[3].Value, matches4[i].Groups[4].Value, matches4[i].Groups[5].Value, matches4[i].Groups[6].Value, matches4[i].Groups[7].Value, matches4[i].Groups[8].Value)));

                            if (i + 1 < matches4.Count && matches4[i + 1].Index < matches5[k].Index)
                                i++;
                            else
                                break;
                        }
                    }

                    if (matches6.Count > 0 && l < matches6.Count)
                    {
                        // process the jumps, if they exist
                        element.Add(new XElement("Jumps"));
                        element = doc.Descendants("Jumps").Last();

                        while (l < matches6.Count && matches6[l].Index < matches5[k].Index)
                        {
                            element.Add(new XElement("Jump", string.Format("{0} [{1}]", matches6[l].Groups[2].Value, matches6[l].Groups[1].Value)));

                            l++;
                        }
                    }

                    if (k < matches5.Count && matches5[k].Index > matches4[i].Index)
                    {
                        element = doc.Descendants("Hop").Last();
                        element.Add(new XElement("UnloadAt", string.Format("Unload @ {0} [{1}] [G:{2}cr/t|{3}]", matches5[k].Groups[1].Value, matches5[k].Groups[2].Value, matches5[k].Groups[5].Value, matches5[k].Groups[4].Value)));

                        // account for Kls and ls, make sure values are whole numbers
                        if (matches5[k].Groups[2].Value.IndexOf("Kls") >= 0)
                        {
                            t_meanDist += Math.Round(double.Parse(matches5[k].Groups[2].Value.Replace("Kls", ""), CultureInfo.InvariantCulture) * 1000, 0);
                        }
                        else if (matches5[k].Groups[2].Value.IndexOf("Kls") < 0
                            && matches5[k].Groups[2].Value.IndexOf("ls") >= 0)
                        {
                            t_meanDist += int.Parse(matches5[k].Groups[2].Value.Replace("ls", ""));
                        }

                        if (k == matches5.Count - 1)
                        {
                            // if this is our last station
                            element.Add(new XElement("UnloadDetails", string.Format("{0}", matches5[k].Groups[3].Value)));
                        }

                        k++;
                    }
                    element = doc.Descendants("Route").FirstOrDefault(); // reset the element
                }

                /*
                 * The multiplier here is a function of:
                 * Time to travel for unloading ~5.28s/ls
                 * Time to dock ~45s
                 * Time to undock ~60s
                 * Time to jump at least once ~30s
                 * Summed minimum wasted time per hop = 135s
                 *
                 * The formula for cr/t/hr is as follows:
                 * (profit / capacity) * (3600 / ((s/hop * hops) + (meanDist / s/ls))) / 2)
                 */

                double savedGain = int.Parse(matches2[0].Groups[1].Value.Replace(",", "").Replace(".", "").Replace("cr", ""));
                t_meanDist = t_meanDist / matches5.Count;
                double multiplier = 3600 / ((135 * matches5.Count) + (t_meanDist / 5.28)) / 2;
                double savedCapacity = Math.Round(t_Capacity / (double)matches5.Count);
                t_CrTonTally = Math.Round(savedGain / savedCapacity * multiplier); // should be approx. cr/t/hr.
                t_childTitle = string.Format("Est. Profit: {0:n0}cr/t/hr", t_CrTonTally);
            }

            #endregion verbosity2

            #region verbosity3

            else if (runOutputState == 3)
            {
                // for outputVerbosity == -vvv
                // Step 1: catch System/Station -> System/Station
                // {1.1} -> {1.2}
                // Gain: {2.1} -> {2.2}
                string pattern1 = @"(.+?)\s->\s(.+?)\s\(";
                string pattern2 = @"Finish at.+gaining\s(.+?cr).+est\s(.+?cr)";

                // Step 2: catch Load From
                // [1] System/Station, [2] LS from star, [3] stn details
                string pattern3 = @"Load from (.+?)\s\((.+?),\s(.+?)\)";

                // Step 3: catch as many iterations of commodities as exist
                // [1] Num of items, [2] Name of items, [3] credit cost src, [4] credit cost dest
                // [5] age src, [6] age name, [7] age dest, [8] age name, [9] total cost, [10] alt for lacking dest. time
                string pattern4 = @"(?>(\d+)\sx\s(.+?)\s\s+(.+?)cr\svs\s+(.+?)cr,\s(.+?)\s(\w+))(?>\svs\s(.+?)\s(\w+),\stotal:\s+(.+)|,\stotal:\s+(.+))";

                // Step 4: catch Unload at
                // [1] System/Station, [2] LS from star, [3] stn details, [4] gain/cr, [5] cr/ton
                string pattern5 = @"Unload at (.+?)\s\((.+?ls),\s(.+?)\) => Gain (.+?cr) \((.+?)cr";

                // Step 5: catch jumps between dock/undock
                // xx.xxly -> SYSTEM
                // {1.2} [{1.1}]
                string pattern6 = @"(?:,)?\s(\d*\.\d*ly)\s->\s(.+?)(?:,|\s\s)"; // long form for jumps

                // Compile our patterns
                MatchCollection matches1 = Regex.Matches(input, pattern1, RegexOptions.Compiled);
                MatchCollection matches2 = Regex.Matches(input, pattern2, RegexOptions.Compiled);
                MatchCollection matches3 = Regex.Matches(input, pattern3, RegexOptions.Compiled);
                MatchCollection matches4 = Regex.Matches(input, pattern4, RegexOptions.Compiled);
                MatchCollection matches5 = Regex.Matches(input, pattern5, RegexOptions.Compiled);
                MatchCollection matches6 = Regex.Matches(input, pattern6, RegexOptions.Compiled);

                // Now we compile the output into XML
                doc = new XDocument(new XElement("Route", new XElement("Summary", string.Format("{0} -> {1}", matches1[0].Groups[1].Value, matches1[0].Groups[2].Value))));

                XElement element = doc.Descendants("Route").FirstOrDefault();

                element.Add(new XElement("SummaryDetails", string.Format("Gain: {0} -> est. {1}", matches2[0].Groups[1].Value, matches2[0].Groups[2].Value)));

                int j = 0, k = 0, l = 0;
                for (int i = 0; i < matches4.Count; i++)
                {
                    if (matches3[j].Index < matches4[i].Index)
                    {
                        element.Add(new XElement("Hop"));
                        element = doc.Descendants("Hop").Last();
                        element.Add(new XElement("LoadAt", string.Format("Load @ {0} [{1}]", matches3[j].Groups[1].Value, matches3[j].Groups[2].Value)));
                        element.Add(new XElement("LoadDetails", string.Format("{0}", matches3[j].Groups[3].Value)));
                        j++;
                    }

                    while (i < matches4.Count && matches4[i].Index < matches5[k].Index)
                    {
                        // collect the working capacity of the run
                        t_Capacity += int.Parse(matches4[i].Groups[1].Value);

                        if (!string.IsNullOrEmpty(matches4[i].Groups[10].Value))
                        {
                            // we're missing the dest. time, let's match based on src. time
                            element.Add(new XElement("Load", string.Format("{0}x {1} @ {2}/{3} [{4}{5}] [{6}]", matches4[i].Groups[1].Value, matches4[i].Groups[2].Value, matches4[i].Groups[3].Value, matches4[i].Groups[4].Value, matches4[i].Groups[5].Value, matches4[i].Groups[6].Value, matches4[i].Groups[10].Value)));

                            // intelligently increment to next match index
                            if (i + 1 < matches4.Count && matches4[i + 1].Index < matches5[k].Index)
                                i++;
                            else
                                break;
                        }
                        else
                        {
                            element.Add(new XElement("Load", string.Format("{0}x {1} @ {2}/{3} [{4}{5}|{6}{7}] [{8}]", matches4[i].Groups[1].Value, matches4[i].Groups[2].Value, matches4[i].Groups[3].Value, matches4[i].Groups[4].Value, matches4[i].Groups[5].Value, matches4[i].Groups[6].Value, matches4[i].Groups[7].Value, matches4[i].Groups[8].Value, matches4[i].Groups[9].Value)));

                            if (i + 1 < matches4.Count && matches4[i + 1].Index < matches5[k].Index)
                                i++;
                            else
                                break;
                        }
                    }

                    if (matches6.Count > 0 && l < matches6.Count)
                    {
                        // process the jumps, if they exist
                        element.Add(new XElement("Jumps"));
                        element = doc.Descendants("Jumps").Last();

                        while (l < matches6.Count && matches6[l].Index < matches5[k].Index)
                        {
                            element.Add(new XElement("Jump", string.Format("{0} [{1}]", matches6[l].Groups[2].Value, matches6[l].Groups[1].Value)));

                            l++;
                        }
                    }

                    if (k < matches5.Count && matches5[k].Index > matches4[i].Index)
                    {
                        element = doc.Descendants("Hop").Last();
                        element.Add(new XElement("UnloadAt", string.Format("Unload @ {0} [{1}] [G:{2}cr/t|{3}]", matches5[k].Groups[1].Value, matches5[k].Groups[2].Value, matches5[k].Groups[5].Value, matches5[k].Groups[4].Value)));

                        // account for Kls and ls, make sure values are whole numbers
                        if (matches5[k].Groups[2].Value.IndexOf("Kls") >= 0)
                        {
                            t_meanDist += Math.Round(double.Parse(matches5[k].Groups[2].Value.Replace("Kls", ""), CultureInfo.InvariantCulture) * 1000, 0);
                        }
                        else if (matches5[k].Groups[2].Value.IndexOf("Kls") < 0
                            && matches5[k].Groups[2].Value.IndexOf("ls") >= 0)
                        {
                            t_meanDist += int.Parse(matches5[k].Groups[2].Value.Replace("ls", ""));
                        }

                        if (k == matches5.Count - 1)
                        {
                            // if this is our last station
                            element.Add(new XElement("UnloadDetails", string.Format("{0}", matches5[k].Groups[3].Value)));
                        }

                        k++;
                    }

                    element = doc.Descendants("Route").FirstOrDefault(); // reset the element
                }

                double savedGain = int.Parse(matches2[0].Groups[1].Value.Replace(",", "").Replace(".", "").Replace("cr", ""));
                t_meanDist = t_meanDist / matches5.Count;
                double multiplier = 3600 / ((135 * matches5.Count) + (t_meanDist / 5.28)) / 2;
                double savedCapacity = Math.Round(t_Capacity / (double)matches5.Count);
                t_CrTonTally = Math.Round(savedGain / savedCapacity * multiplier); // should be approx. cr/t/hr.
                t_childTitle = string.Format("Est. Profit: {0:n0}cr/t/hr", t_CrTonTally);
            }

            #endregion verbosity3

            if (doc.Descendants("Route").FirstOrDefault() != null)
                PopulateTreeView(doc, tvOutput); // pass to the next step
        }

        private void PopulateTreeView(XDocument xmlInput, TreeView tvBox)
        {
            // this takes an XDocument and parses to a TreeView
            XElement el = xmlInput.Descendants("Route").FirstOrDefault();

            if (el != null)
            {
                TreeNode node = new TreeNode()
                {
                    Text = el.Element("Summary").Value
                };

                // put details in a childnode
                if (el.Element("SummaryDetails") != null)
                {
                    node.Nodes.Add(new TreeNode(el.Element("SummaryDetails").Value));
                }

                foreach (XElement l in el.Descendants("Hop"))
                {
                    TreeNode tnGroupStart = new TreeNode(l.Element("LoadAt").Value);
                    node.Nodes.Add(tnGroupStart);

                    if (l.Element("LoadDetails") != null)
                    {
                        tnGroupStart.Nodes.Add(new TreeNode(l.Element("LoadDetails").Value));
                    }

                    foreach (XElement j in l.Elements("Load"))
                    {
                        tnGroupStart.Nodes.Add(new TreeNode(j.Value));
                    }

                    if (l.Descendants("Jumps") != null && l.Descendants("Jumps").Count() > 0)
                    {
                        TreeNode tnJumpGroup = new TreeNode("Jumps");

                        foreach (XElement k in l.Element("Jumps").Elements())
                        {
                            tnJumpGroup.Nodes.Add(new TreeNode(k.Value));
                        }

                        tnGroupStart.Nodes.Add(tnJumpGroup);
                    }

                    TreeNode tnGroupEnd = new TreeNode(l.Element("UnloadAt").Value);
                    tnGroupStart.Nodes.Add(tnGroupEnd);

                    if (l.Element("UnloadDetails") != null)
                    {
                        tnGroupEnd.Nodes.Add(new TreeNode(l.Element("UnloadDetails").Value));
                    }
                }

                tvBox.Nodes.Clear();
                tvBox.BeginUpdate();
                tvBox.Nodes.Add(node);
                tvBox.EndUpdate();
                tvBox.Refresh();
                tvBox.ExpandAll(); // make sure we start expanded
            }
            else
                throw new Exception("Failure to parse the input XML to the TreeView");
        }

        /// <summary>
        /// Read the Cmdr Profile file.
        /// </summary>
        private string RetrieveCommanderProfile()
        {
            string fileName = Path.Combine(MainForm.settingsRef.EdcePath, "last.json");
            string json = string.Empty;

            // If the file exists read it into the return string.
            if (File.Exists(fileName))
            {
                // Create a StreamReader and open the file
                using (TextReader reader = new StreamReader(fileName, Encoding.Default))
                {
                    // Read all the contents of the file in a string
                    json = reader.ReadToEnd();

                    // Close the StreamReader
                    reader.Close();
                }
            }

            return json;
        }

        /// <summary>
        /// Sets the ship translations.
        /// </summary>
        private void SetupShipTranslations()
        {
            ShipTranslation.Clear();

            ShipTranslation.Add("Adder", "Adder");
            ShipTranslation.Add("Anaconda", "Anaconda");
            ShipTranslation.Add("Asp", "Asp Explorer");
            ShipTranslation.Add("Asp_Scout", "Asp Scout");
            ShipTranslation.Add("BelugaLiner", "Beluga");
            ShipTranslation.Add("CobraMkIII", "Cobra MkIII");
            ShipTranslation.Add("CobraMkIV", "Cobra MkIV");
            ShipTranslation.Add("Cutter", "Imperial Cutter");
            ShipTranslation.Add("DiamondBack", "DiamondBack Scout");
            ShipTranslation.Add("DiamondBackXL", "DiamondBack Explorer");
            ShipTranslation.Add("Dolphin", "Dolphin");
            ShipTranslation.Add("Eagle", "Eagle");
            ShipTranslation.Add("Empire_Courier", "Imperial Courier");
            ShipTranslation.Add("Empire_Eagle", "Imperial Eagle");
            ShipTranslation.Add("Empire_Trader", "Imperial Clipper");
            ShipTranslation.Add("Federation_Corvette", "Federal Corvette");
            ShipTranslation.Add("Federation_Dropship", "Federal Dropship");
            ShipTranslation.Add("Federation_Dropship_MkII", "Federal Assault Ship");
            ShipTranslation.Add("Federation_Gunship", "Federal Gunship");
            ShipTranslation.Add("FerDeLance", "Fer-de-Lance");
            ShipTranslation.Add("Hauler", "Hauler");
            ShipTranslation.Add("Independant_Trader", "Keelback");
            ShipTranslation.Add("Krait_MkII", "Krait MkII");
            ShipTranslation.Add("Orca", "Orca");
            ShipTranslation.Add("Python", "Python");
            ShipTranslation.Add("SideWinder", "Sidewinder");
            ShipTranslation.Add("Type6", "Type 6");
            ShipTranslation.Add("Type7", "Type 7");
            ShipTranslation.Add("Type9", "Type 9");
            ShipTranslation.Add("Type9_Military", "Type 10");
            ShipTranslation.Add("TypeX", "Alliance Chieftain");
            ShipTranslation.Add("TypeX_3", "Alliance Challenger");
            ShipTranslation.Add("Viper", "Viper MkIII");
            ShipTranslation.Add("Viper_MkIV", "Viper MkIV");
            ShipTranslation.Add("Vulture", "Vulture");
        }

        /// <summary>
        /// Get the padsizes upon which the ship may land.
        /// </summary>
        /// <param name="shipType">The internal ship type.</param>
        /// <returns>The padsizes upon which the ship may land.</returns>
        private string TranslateShipType(string shipType)
        {
            string translation = shipType.Trim();

            if (ShipTranslation.Count == 0)
            {
                SetupShipTranslations();
            }

            if (ShipTranslation.ContainsKey(shipType))
            {
                translation = ShipTranslation[shipType];
            }

            return translation;
        }

        /// <summary>
        /// Read the Cmdr Profile file and update the credit balance.
        /// </summary>
        private void UpdateCommanderAndShipDetails()
        {
            string json = RetrieveCommanderProfile();

            JObject cmdrProfile = JObject.Parse(json);

            // Set the commander name and credit balance.
            settingsRef.CmdrName = (string)cmdrProfile["profile"]["commander"]["name"];
            numCommandersCredits.Value = (decimal)cmdrProfile["profile"]["commander"]["credits"];

            // Determine the insurance of the current ship.
            decimal hullValue = (decimal)cmdrProfile["profile"]["ship"]["value"]["hull"];
            decimal modulesValue = (decimal)cmdrProfile["profile"]["ship"]["value"]["modules"];
            decimal rebuyPercentage = settingsRef.RebuyPercentage;

            this.numShipInsurance.Value = (hullValue + modulesValue) * rebuyPercentage / 100;

            // Determine the cargo capacity of the current ship.
            decimal capacity = 0;
            int stringLength = "Int_CargoRack_Size".Length;

            foreach (JToken slot in cmdrProfile["profile"]["ship"]["modules"])
            {
                string module = (string)slot.First["module"]["name"];

                if (module.Length > stringLength && module.Substring(0, stringLength) == "Int_CargoRack_Size")
                {
                    if (Int32.TryParse(module.Substring(stringLength, 1), out int size))
                    {
                        capacity += (decimal)Math.Pow(2, size);
                    }
                }
            }

            if (capacity > 0)
            {
                numRouteOptionsShipCapacity.Value = capacity;
            }

            // Set this ship as the currently selected ship.
            string currentlySelected = GetShipName(cmdrProfile["profile"]["ship"]);

            settingsRef.LastUsedConfig = currentlySelected;

            // Get the details of all the commander's ships and set up the available ships.
            Configuration config = Configuration.LoadFromFile(configFile);

            string availableShips = string.Empty;

            foreach (JToken ship in cmdrProfile["profile"]["ships"])
            {
                string sectionName = GetShipName(ship.First);

                availableShips += ",{0}".With(sectionName);

                decimal shipCapacity
                    = sectionName == currentlySelected
                    ? capacity
                    : 0;

                hullValue = (decimal)ship.First["value"]["hull"];
                modulesValue = (decimal)ship.First["value"]["modules"];

                Setting capacitySetting = config[sectionName]["Capacity"];

                if (capacitySetting == null)
                {
                    // This is a new ship then set the capacity to the current ship capacity or zero as required.
                    config[sectionName]["capacity"].DecimalValue = shipCapacity;
                }
                else if (capacitySetting.IsEmpty)
                {
                    capacitySetting.DecimalValue = shipCapacity;
                }
                else if (shipCapacity > 0 && capacitySetting.DecimalValue != shipCapacity)
                {
                    capacitySetting.DecimalValue = shipCapacity;
                }

                config[sectionName]["LadenLY"].DecimalValue
                    = decimal.TryParse(config[sectionName]["LadenLY"].StringValue, out decimal ladenLy)
                    ? ladenLy
                    : 1;

                config[sectionName]["unladenLY"].DecimalValue
                    = decimal.TryParse(config[sectionName]["UnladenLY"].StringValue, out decimal unladenLy)
                    ? unladenLy
                    : 1;

                config[sectionName]["hullValue"].DecimalValue = hullValue;
                config[sectionName]["modulesValue"].DecimalValue = modulesValue;
                config[sectionName]["Insurance"].DecimalValue = (hullValue + modulesValue) * rebuyPercentage / 100;
                config[sectionName]["Padsizes"].StringValue = this.GetPadSizes((string)ship.First["name"]);
            }

            string currentShips = MainForm.settingsRef.AvailableShips;
            MainForm.settingsRef.AvailableShips = availableShips.Substring(1);
            config["System"]["AvailableShips"].StringValue = MainForm.settingsRef.AvailableShips;

            // Determine if any ships have been sold and remove if found.
            IList<string> missingShips = DeterminMissingShips(currentShips, availableShips);

            foreach (string ship in missingShips)
            {
                config.RemoveAllNamed(ship);
            }

            config.SaveToFile(configFile);

            SetFormTitle(this);
            SetShipList(true);
        }
    }
}