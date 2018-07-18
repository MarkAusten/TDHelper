using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using SharpConfig;

namespace TDHelper
{
    #region TDSettings

    public static class TopMostMessageBox
    {
        /*
         * This class is taken from: http://goo.gl/EnPqrF
         */

        static public DialogResult Show(bool onTop, bool playAlert, string message, string title, MessageBoxButtons buttons)
        {
            System.Drawing.Rectangle rect = SystemInformation.VirtualScreen;

            // Create a host form that is a TopMost window which will be the
            // parent of the MessageBox.
            // We do not want anyone to see this window so position it off the
            // visible screen and make it as small as possible
            Form topmostForm = new Form()
            {
                Size = new System.Drawing.Size(1, 1),
                Icon = TDHelper.Properties.Resources.TDH_Icon,
                ShowIcon = true,
                Text = title,
                StartPosition = FormStartPosition.Manual,
                Location = new System.Drawing.Point(rect.Bottom + 10, rect.Right + 10),
            };

            topmostForm.Show();

            if (onTop)
            {
                // force the form to the top of the stack
                topmostForm.Focus();
                topmostForm.BringToFront();
                topmostForm.TopMost = true;
            }
            else
            {
                // avoid grabbing focus
                topmostForm.TopMost = false;
            }

            if (playAlert)
            {
                TDHelper.MainForm.PlayAlert(); // make noise to alert the user
            }

            DialogResult result = MessageBox.Show(topmostForm, message, title, buttons);

            topmostForm.Dispose(); // clean it up all the way

            return result;
        }
    }

    public partial class MainForm : Form
    {
        public static string AssemblyGuid
        {
            get
            {
                string assemblyGuid;

                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false);

                assemblyGuid
                    = attributes.Length == 0
                    ? string.Empty
                    : ((System.Runtime.InteropServices.GuidAttribute)attributes[0]).Value.ToUpper();

                return assemblyGuid;
            }
        }

        public static void ForceFormOnScreen(Form form)
        {
            if (form.Left < SystemInformation.VirtualScreen.Left)
            {
                form.Left = SystemInformation.VirtualScreen.Left;
            }

            if (form.Right > SystemInformation.VirtualScreen.Right)
            {
                form.Left = SystemInformation.VirtualScreen.Right - form.Width;
            }

            if (form.Top < SystemInformation.VirtualScreen.Top)
            {
                form.Top = SystemInformation.VirtualScreen.Top;
            }

            if (form.Bottom > SystemInformation.VirtualScreen.Bottom)
            {
                form.Top = SystemInformation.VirtualScreen.Bottom - form.Height;
            }
        }

        /// <summary>
        /// Load the setting from the inin file.
        /// </summary>
        public static void LoadSettingsFromIniFile()
        {
            if (CheckIfFileOpens(configFile))
            {
                Configuration config = Configuration.LoadFromFile(configFile);
                TDSettings settings = MainForm.settingsRef;

                Section configSection = config["App"];

                settings.AbovePrice = SectionHasKey(configSection, "AbovePrice") ? configSection["AbovePrice"].DecimalValue : 0;
                settings.Age = SectionHasKey(configSection, "Age") ? configSection["Age"].DecimalValue : 0;
                settings.Avoid = SectionHasKey(configSection, "Avoid") ? configSection["Avoid"].StringValue : string.Empty;
                settings.BelowPrice = SectionHasKey(configSection, "BelowPrice") ? configSection["BelowPrice"].DecimalValue : 0;
                settings.Corrections = SectionHasKey(configSection, "Corrections") ? configSection["Corrections"].BoolValue : false;
                settings.CSVSelect = SectionHasKey(configSection, "CSVSelect") ? configSection["CSVSelect"].DecimalValue : 0;
                settings.Demand = SectionHasKey(configSection, "Demand") ? configSection["Demand"].DecimalValue : 0;
                settings.ExtraRunParams = SectionHasKey(configSection, "ExtraRunParams") ? configSection["ExtraRunParams"].StringValue : string.Empty;
                settings.GPT = SectionHasKey(configSection, "GPT") ? configSection["GPT"].DecimalValue : 0;
                settings.Hops = SectionHasKey(configSection, "Hops") ? configSection["Hops"].DecimalValue : 0;
                settings.Jumps = SectionHasKey(configSection, "Jumps") ? configSection["Jumps"].DecimalValue : 0;
                settings.Limit = SectionHasKey(configSection, "Limit") ? configSection["Limit"].DecimalValue : 0;
                settings.Loop = SectionHasKey(configSection, "Loop") ? configSection["Loop"].BoolValue : false;
                settings.LoopInt = SectionHasKey(configSection, "LoopInt") ? configSection["LoopInt"].DecimalValue : 0;
                settings.LSPenalty = SectionHasKey(configSection, "LSPenalty") ? configSection["LSPenalty"].DecimalValue : 0;
                settings.Margin = SectionHasKey(configSection, "Margin") ? configSection["Margin"].DecimalValue : 0;
                settings.MarkedStations = SectionHasKey(configSection, "MarkedStations") ? configSection["MarkedStations"].StringValue : string.Empty;
                settings.MaxGPT = SectionHasKey(configSection, "MaxGPT") ? configSection["MaxGPT"].DecimalValue : 0;
                settings.MaxLSDistance = SectionHasKey(configSection, "MaxLSDistance") ? configSection["MaxLSDistance"].DecimalValue : 0;
                settings.PruneHops = SectionHasKey(configSection, "PruneHops") ? configSection["PruneHops"].DecimalValue : 0;
                settings.PruneScore = SectionHasKey(configSection, "PruneScore") ? configSection["PruneScore"].DecimalValue : 0;
                settings.ShowJumps = SectionHasKey(configSection, "ShowJumps") ? configSection["ShowJumps"].BoolValue : false;
                settings.Stock = SectionHasKey(configSection, "Stock") ? configSection["Stock"].DecimalValue : 0;
                settings.Towards = SectionHasKey(configSection, "Towards") ? configSection["Towards"].BoolValue : false;
                settings.Unique = SectionHasKey(configSection, "Unique") ? configSection["Unique"].BoolValue : false;
                settings.Verbosity = SectionHasKey(configSection, "Verbosity") ? configSection["Verbosity"].DecimalValue : 0;
                settings.Via = SectionHasKey(configSection, "Via") ? configSection["Via"].StringValue : string.Empty;
                settings.Planetary = SectionHasKey(configSection, "Planetary") ? configSection["Planetary"].StringValue : string.Empty;

                settings.RouteNoPlanet = SectionHasKey(configSection, "RouteNoPlanet") ? configSection["RouteNoPlanet"].BoolValue : false;
                settings.LocalNoPlanet = SectionHasKey(configSection, "LocalNoPlanet") ? configSection["LocalNoPlanet"].BoolValue : false;
                settings.RouteStations = SectionHasKey(configSection, "RouteStations") ? configSection["RouteStations"].BoolValue : false;
                settings.ShowProgress = SectionHasKey(configSection, "ShowProgress") ? configSection["ShowProgress"].BoolValue : false;
                settings.Summary = SectionHasKey(configSection, "Summary") ? configSection["Summary"].BoolValue : false;

                // Commander settings
                configSection = config["Commander"];

                settings.CmdrName = SectionHasKey(configSection, "CmdrName") ? configSection["CmdrName"].StringValue : string.Empty;
                settings.Credits = SectionHasKey(configSection, "Credits") ? configSection["Credits"].DecimalValue : 0;
                settings.RebuyPercentage = SectionHasKey(configSection, "RebuyPercentage") ? configSection["RebuyPercentage"].DecimalValue : 0;

                // TD Helper system settings.
                configSection = config["System"];

                settings.CopySystemToClipboard = SectionHasKey(configSection, "CopySystemToClipboard") ? configSection["CopySystemToClipboard"].BoolValue : false;
                settings.DisableNetLogs = SectionHasKey(configSection, "DisableNetLogs") ? configSection["DisableNetLogs"].BoolValue : false;
                settings.DoNotUpdate = SectionHasKey(configSection, "DoNotUpdate") ? configSection["DoNotUpdate"].BoolValue : false;
                settings.EdcePath = SectionHasKey(configSection, "EdcePath") ? configSection["EdcePath"].StringValue : string.Empty;
                settings.HasUpdated = SectionHasKey(configSection, "HasUpdated") ? configSection["HasUpdated"].BoolValue : false;
                settings.ImportPath = SectionHasKey(configSection, "ImportPath") ? configSection["ImportPath"].StringValue : string.Empty;
                settings.LastUsedConfig = SectionHasKey(configSection, "LastUsedConfig") ? configSection["LastUsedConfig"].StringValue : string.Empty;
                settings.LocationChild = SectionHasKey(configSection, "LocationChild") ? configSection["LocationChild"].StringValue : string.Empty;
                settings.LocationParent = SectionHasKey(configSection, "LocationParent") ? configSection["LocationParent"].StringValue : string.Empty;
                settings.MiniModeOnTop = SectionHasKey(configSection, "MiniModeOnTop") ? configSection["MiniModeOnTop"].BoolValue : false;
                settings.NetLogPath = SectionHasKey(configSection, "NetLogPath") ? configSection["NetLogPath"].StringValue : string.Empty;
                settings.PythonPath = SectionHasKey(configSection, "PythonPath") ? configSection["PythonPath"].StringValue : string.Empty;
                settings.SizeChild = SectionHasKey(configSection, "SizeChild") ? configSection["SizeChild"].StringValue : string.Empty;
                settings.SizeParent = SectionHasKey(configSection, "SizeParent") ? configSection["SizeParent"].StringValue : string.Empty;
                settings.TDPath = SectionHasKey(configSection, "TDPath") ? configSection["TDPath"].StringValue : string.Empty;
                settings.TestSystems = SectionHasKey(configSection, "TestSystems") ? configSection["TestSystems"].BoolValue : false;
                settings.TreeViewFont = SectionHasKey(configSection, "TreeViewFont") ? configSection["TreeViewFont"].StringValue : string.Empty;
                settings.UploadPath = SectionHasKey(configSection, "UploadPath") ? configSection["UploadPath"].StringValue : string.Empty;
                settings.AvailableShips = SectionHasKey(configSection, "AvailableShips") ? configSection["AvailableShips"].StringValue : string.Empty;
                settings.Quiet = SectionHasKey(configSection, "Quiet") ? configSection["Quiet"].BoolValue : false;

                if (string.IsNullOrEmpty(settings.AvailableShips))
                {
                    settings.AvailableShips = "Default";
                }
            }
        }

        public static int[] LoadWinLoc(string objRef)
        {
            // load winLoc from a given variable object
            int[] winLoc = new int[] { };

            if (!string.IsNullOrEmpty(objRef))
            {
                string[] t_winLoc = objRef.Split(',').ToArray();
                winLoc = new int[2];
                winLoc[0] = Convert.ToInt32(t_winLoc.GetValue(0));
                winLoc[1] = Convert.ToInt32(t_winLoc.GetValue(1));
            }

            return winLoc;
        }

        public static int[] LoadWinSize(string objRef)
        {
            // load window size
            int[] winSize = new int[] { };

            if (!string.IsNullOrEmpty(objRef))
            {
                string[] t_winSize = objRef.Split(',').ToArray();
                winSize = new int[2];
                winSize[0] = Convert.ToInt32(t_winSize.GetValue(0));
                winSize[1] = Convert.ToInt32(t_winSize.GetValue(1));

                if (winSize[0] > SystemInformation.VirtualScreen.Right)
                {
                    winSize[0] = SystemInformation.VirtualScreen.Right;
                }

                if (winSize[1] > SystemInformation.VirtualScreen.Bottom)
                {
                    winSize[1] = SystemInformation.VirtualScreen.Bottom;
                }
            }

            return winSize;
        }

        /// <summary>
        /// Save the settings to the ini file.
        /// </summary>
        public static void SaveSettingsToIniFile()
        {
            Configuration config
                = CheckIfFileOpens(configFile)
                ? Configuration.LoadFromFile(configFile)
                : new Configuration();

            TDSettings settings = MainForm.settingsRef;

            Section configSection = config["App"];

            // Settgins used for trade route calculation.
            configSection["AbovePrice"].DecimalValue = settings.AbovePrice;
            configSection["Age"].DecimalValue = settings.Age;
            configSection["Avoid"].StringValue = settings.Avoid ?? string.Empty;
            configSection["BelowPrice"].DecimalValue = settings.BelowPrice;
            configSection["Corrections"].BoolValue = settings.Corrections;
            configSection["CSVSelect"].DecimalValue = settings.CSVSelect;
            configSection["Demand"].DecimalValue = settings.Demand;
            configSection["ExtraRunParams"].StringValue = settings.ExtraRunParams ?? string.Empty;
            configSection["GPT"].DecimalValue = settings.GPT;
            configSection["Hops"].DecimalValue = settings.Hops;
            configSection["Jumps"].DecimalValue = settings.Jumps;
            configSection["Limit"].DecimalValue = settings.Limit;
            configSection["Loop"].BoolValue = settings.Loop;
            configSection["LoopInt"].DecimalValue = settings.LoopInt;
            configSection["LSPenalty"].DecimalValue = settings.LSPenalty;
            configSection["Margin"].DecimalValue = settings.Margin;
            configSection["MarkedStations"].StringValue = settings.MarkedStations ?? string.Empty;
            configSection["MaxGPT"].DecimalValue = settings.MaxGPT;
            configSection["MaxLSDistance"].DecimalValue = settings.MaxLSDistance;
            configSection["PruneHops"].DecimalValue = settings.PruneHops;
            configSection["PruneScore"].DecimalValue = settings.PruneScore;
            configSection["ShowJumps"].BoolValue = settings.ShowJumps;
            configSection["Stock"].DecimalValue = settings.Stock;
            configSection["Towards"].BoolValue = settings.Towards;
            configSection["Unique"].BoolValue = settings.Unique;
            configSection["Verbosity"].DecimalValue = settings.Verbosity;
            configSection["Via"].StringValue = settings.Via ?? string.Empty;
            configSection["RouteNoPlanet"].BoolValue = settings.RouteNoPlanet;
            configSection["LocalNoPlanet"].BoolValue = settings.LocalNoPlanet;
            configSection["RouteStations"].BoolValue = settings.RouteStations;
            configSection["ShowProgress"].BoolValue = settings.ShowProgress;
            configSection["Summary"].BoolValue = settings.Summary;
            configSection["Planetary"].StringValue = settings.Planetary ?? string.Empty;

            // Commander settings
            configSection = config["Commander"];

            configSection["CmdrName"].StringValue = settings.CmdrName ?? string.Empty;
            configSection["Credits"].DecimalValue = settings.Credits;
            configSection["RebuyPercentage"].DecimalValue = settings.RebuyPercentage;

            // TD Helper system settings.
            configSection = config["System"];

            configSection["CopySystemToClipboard"].BoolValue = settings.CopySystemToClipboard;
            configSection["DisableNetLogs"].BoolValue = settings.DisableNetLogs;
            configSection["DoNotUpdate"].BoolValue = settings.DoNotUpdate;
            configSection["EdcePath"].StringValue = settings.EdcePath ?? string.Empty;
            configSection["HasUpdated"].BoolValue = settings.HasUpdated;
            configSection["ImportPath"].StringValue = settings.ImportPath ?? string.Empty;
            configSection["LastUsedConfig"].StringValue = settings.LastUsedConfig ?? string.Empty;
            configSection["LocationChild"].StringValue = settings.LocationChild ?? string.Empty;
            configSection["LocationParent"].StringValue = settings.LocationParent ?? string.Empty;
            configSection["MiniModeOnTop"].BoolValue = settings.MiniModeOnTop;
            configSection["NetLogPath"].StringValue = settings.NetLogPath ?? string.Empty;
            configSection["PythonPath"].StringValue = settings.PythonPath ?? string.Empty;
            configSection["SizeChild"].StringValue = settings.SizeChild ?? string.Empty;
            configSection["SizeParent"].StringValue = settings.SizeParent ?? string.Empty;
            configSection["TDPath"].StringValue = settings.TDPath ?? string.Empty;
            configSection["TestSystems"].BoolValue = settings.TestSystems;
            configSection["TreeViewFont"].StringValue = settings.TreeViewFont ?? string.Empty;
            configSection["UploadPath"].StringValue = settings.UploadPath ?? string.Empty;
            configSection["AvailableShips"].StringValue = settings.AvailableShips ?? string.Empty;
            configSection["Quiet"].BoolValue = settings.Quiet;

            // Update the current ship if required.
            if (!string.IsNullOrEmpty(settings.LastUsedConfig))
            {
                string sectionName = settings.LastUsedConfig;

                bool hasSection = config.FirstOrDefault(x => x.Name == sectionName) != null;

                if (hasSection)
                {
                    if (config[sectionName]["Capacity"].DecimalValue != settings.Capacity)
                    {
                        config[sectionName]["Capacity"].DecimalValue = settings.Capacity;
                    }

                    if (config[sectionName]["LadenLY"].DecimalValue != settings.LadenLY)
                    {
                        config[sectionName]["LadenLY"].DecimalValue = settings.LadenLY;
                    }

                    if (config[sectionName]["Padsizes"].StringValue != settings.Padsizes)
                    {
                        config[sectionName]["Padsizes"].StringValue = settings.Padsizes;
                    }

                    if (config[sectionName]["UnladenLY"].DecimalValue != settings.UnladenLY)
                    {
                        config[sectionName]["UnladenLY"].DecimalValue = settings.UnladenLY;
                    }
                }
                else
                {
                    config[sectionName]["Capacity"].DecimalValue = settings.Capacity;
                    config[sectionName]["LadenLY"].DecimalValue = settings.LadenLY;
                    config[sectionName]["Padsizes"].StringValue = settings.Padsizes;
                    config[sectionName]["UnladenLY"].DecimalValue = settings.UnladenLY;
                }
            }

            config.SaveToFile(configFile);
        }

        public static string SaveWinLoc(Form x)
        {
            // save winLoc to a given variable object
            return string.Format("{0},{1}", x.Location.X, x.Location.Y);
        }

        public static string SaveWinSize(Form x)
        {
            // save window size
            string modWidth, modHeight;

            // make sure we're not out of bounds
            if (x.Size.Width > SystemInformation.VirtualScreen.Right)
            {
                modWidth = SystemInformation.VirtualScreen.Right.ToString();
            }
            else
            {
                modWidth = x.Size.Width.ToString();
            }

            if (x.Size.Height > SystemInformation.VirtualScreen.Bottom)
            {
                modHeight = SystemInformation.VirtualScreen.Bottom.ToString();
            }
            else
            {
                modHeight = x.Size.Height.ToString();
            }

            return string.Format("{0},{1}", modWidth, modHeight);
        }

        public static bool SectionHasKey(Section section, string key)
        {
            bool result = section.FirstOrDefault(x => x.Name == key) != null;

            return result;
        }

        public static void SetVerboseLogging(string path)
        {
            XDocument file;
            XmlWriterSettings settings = new XmlWriterSettings();
            StringBuilder unformat = new StringBuilder();
            StringWriterUTF8 output = new StringWriterUTF8(unformat);
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            // if our file exists, load it, if not create a fresh one
            if (File.Exists(path))
            {
                file = XDocument.Load(path, LoadOptions.PreserveWhitespace);

                XElement parentElement = file.Element("AppConfig");
                XElement element = parentElement.Element("Network");

                // check the attributes to see if VerboseLogging is set
                if (parentElement != null && element != null)
                {
                    // we've got a valid local config, let's modify it
                    int verboseLogging
                        = element.Attribute("VerboseLogging") == null
                        ? -1
                        : int.Parse(element.Attribute("VerboseLogging").Value);

                    if (verboseLogging < 1 && verboseLogging != -1)
                    {
                        // the attribute seems to exist, lets correct it and move on
                        element.Attribute("VerboseLogging").Value = "1";
                    }
                    else if (verboseLogging == -1)
                    {
                        // our attribute doesn't exist, create it
                        element.Add(new XAttribute("VerboseLogging", "1"));
                    }
                }
            }
            else
            {
                // ceate a fresh config file with the correct setting.
                file = new XDocument(
                    new XElement("AppConfig",
                        new XElement("Network", new XAttribute("VerboseLogging", "1"))));
            }

            // refresh our path to the latest netlog
            latestLogPaths = CollectLogPaths(settingsRef.NetLogPath, "netLog*.log");

            if (latestLogPaths != null && latestLogPaths.Count > 0)
            {
                recentLogPath = latestLogPaths[0];
            }
            else
            {
                recentLogPath = string.Empty;
            }

            // always make a backup for safety
            if (File.Exists(path))
            {
                File.Copy(path, path + ".backup", true);
            }

            // must be utf-8 on the output, so we force it with a class override
            using (XmlWriter xmlWriter = XmlWriter.Create(path, settings))
            {
                file.WriteTo(xmlWriter);
            }
        }

        public static bool ValidateConfigFile(string filePath)
        {
            bool fileIsValid = false;

            if (CheckIfFileOpens(filePath))
            {
                Configuration config = Configuration.LoadFromFile(filePath);

                fileIsValid = config.GetSectionsNamed("App").Count() == 1;
            }

            return fileIsValid;
        }

        public static void ValidateVerboseLogging()
        {
            // Open the AppConfig file and check to see if the setting is found.
            string path = t_AppConfigPath;

            XDocument file = XDocument.Load(path, LoadOptions.PreserveWhitespace);
            XElement parentElement = file.Element("AppConfig");
            XElement element = parentElement.Element("Network");
            bool elementFound = !(element.Attribute("VerboseLogging") == null); ;

            if (!elementFound)
            {
                // Not found in AppConfig.xml so check to see if there is an AppConfigLocal.xml file and check that.
                path = Path.Combine(Path.GetDirectoryName(path), "AppConfigLocal.xml");

                if (File.Exists(path))
                {
                    file = XDocument.Load(path, LoadOptions.PreserveWhitespace);
                    parentElement = file.Element("AppConfig");
                    element = parentElement.Element("Network");
                    elementFound = !(element.Attribute("VerboseLogging") == null);
                }
            }

            if (elementFound)
            {
                // The VerboseLogging element has been found in the file pointed to by path so check for the correct value.
                elementFound = int.Parse(element.Attribute("VerboseLogging").Value) == 1;
            }

            if (!elementFound)
            {
                // If elementFound is false at this point, then VerboseLogging was either not found or it was found but not set.
                // Ask the user if we can set it.
                DialogResult dialog = TopMostMessageBox.Show(
                    true,
                    true, 
                    "VerboseLogging isn't set, it must be corrected so we can grab recent systems.\r\n\nMay we fix it?",
                    "TD Helper - Error",
                    MessageBoxButtons.YesNo);

                if (dialog == DialogResult.Yes)
                {
                    SetVerboseLogging(path); // so fix it
                }
                else
                {
                    DialogResult dialog2 = TopMostMessageBox.Show(
                        true,
                        true,
                        "We will set the DisableNetLogs override in our config file to prevent prompts.\r\n",
                        "TD Helper - Notice",
                        MessageBoxButtons.OK);

                    settingsRef.DisableNetLogs = true;
                }
            }

            if (!settingsRef.DisableNetLogs)
            {
                // refresh our path to the first netlog
                latestLogPaths = CollectLogPaths(settingsRef.NetLogPath, "netLog*.log");

                recentLogPath
                    = latestLogPaths != null && latestLogPaths.Count > 0
                    ? latestLogPaths[0]
                    : string.Empty;
            }
        }

        /// <summary>
        /// Get a list of available ships from the settings.
        /// </summary>
        /// <returns>A list of available ships.</returns>
        public IList<string> SetAvailableShips()
        {
            if (string.IsNullOrEmpty(MainForm.settingsRef.AvailableShips))
            {
                MainForm.settingsRef.AvailableShips = "default";
            }

            return MainForm
                .settingsRef
                .AvailableShips
                .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .OrderBy(x => x)
                .ToList();
        }

        private static List<string> ParseMarkedStations()
        {
            List<string> result
                = string.IsNullOrEmpty(settingsRef.MarkedStations)
                ? new List<string>()
                : RemoveExtraWhitespace(settingsRef.MarkedStations)
                    .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                
            return result;
        }

        private void AddMarkedStation(string input, List<string> parentList)
        {
            if (!IsMarkedStation(input, parentList) && StringInList(input, outputSysStnNames)
                && !StringInList(input, parentList))
            {
                // insert at the top of the list
                parentList.Insert(0, input);
                settingsRef.MarkedStations = SerializeMarkedStations(parentList);
            }
        }

        private bool ContainsPadSizes(string text)
        {
            bool containsPadSizes = false;

            // we only want one of each from the key
            if (!string.IsNullOrEmpty(text))
            {
                char[] c = new char[] { 'M', 'L', '?' };
                char[] z = text.ToUpperInvariant().ToCharArray();

                // count how many we found
                var intersect = z.Intersect(c).ToList();

                // check for only M/L/? and no more than that
                containsPadSizes = !(intersect.Count > 3 || intersect.Count < 1);
            }

            return containsPadSizes;
        }

        private bool ContainsPlanetary(string text)
        {
            bool containsPlanetary = false;

            // we only want one of each from the key
            if (!string.IsNullOrEmpty(text))
            {
                char[] c = new char[] { 'Y', 'N', '?' };
                char[] z = text.ToUpperInvariant().ToCharArray();

                // count how many we found
                var intersect = z.Intersect(c).ToList();

                // check for only M/L/? and no more than that
                containsPlanetary = !(intersect.Count > 3 || intersect.Count < 1);
            }

            return containsPlanetary;
        }

        private bool IsMarkedStation(string input, List<string> parentList)
        {
            return StringInList(input, parentList);
        }

        private void RemoveMarkedStation(string input, List<string> parentList)
        {
            int index = IndexInList(input, parentList);

            // it's valid, grab the index
            if (IsMarkedStation(input, parentList) && index >= 0)
            {
                parentList.RemoveAt(index);
                settingsRef.MarkedStations = SerializeMarkedStations(parentList);
            }
        }

        private string SerializeMarkedStations(List<string> parentList)
        {
            return parentList.Count > 0
                ? string.Join(",", parentList)
                : string.Empty;
        }
    }

    public class StringWriterUTF8 : StringWriter
    {
        /*
         * Taken from: http://goo.gl/2iFFkj
         */

        public StringWriterUTF8(StringBuilder sb) : base(sb)
        {
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }

    public class TDSettings
    {
        // standard lazy singleton pattern
        /*
         * This class is intended to serve as a central place to store/retrieve settings
         */

        #region Props

        private static readonly Lazy<TDSettings> _inst = new Lazy<TDSettings>(() => new TDSettings());
        private string _ExtraRunParams;
        private decimal ladenLY;
        private decimal margin;
        private decimal unladenLY;

        private TDSettings()
        {
        } // prevent instancing

        public static TDSettings Instance { get { return _inst.Value; } } // return our reference
        public decimal AbovePrice { get; set; }
        public decimal Age { get; set; }
        public string AvailableShips { get; set; }
        public string Avoid { get; set; }
        public decimal BelowPrice { get; set; }
        public decimal Capacity { get; set; }
        public string CmdrName { get; set; }
        public bool CopySystemToClipboard { get; set; }
        public bool Corrections { get; set; }
        public decimal Credits { get; set; }
        public decimal CSVSelect { get; set; }
        public decimal Demand { get; set; }
        public bool DisableNetLogs { get; set; }
        public bool DoNotUpdate { get; set; }
        public string EdcePath { get; set; }

        public string ExtraRunParams
        {
            get { return this._ExtraRunParams ?? string.Empty; }
            set { this._ExtraRunParams = value; }
        }

        public decimal GPT { get; set; }
        public bool HasUpdated { get; set; }
        public decimal Hops { get; set; }
        public string ImportPath { get; set; }
        public decimal Insurance { get; set; }
        public decimal Jumps { get; set; }

        public decimal LadenLY
        {
            get { return decimal.Truncate(ladenLY * 100) / 100; }
            set { ladenLY = decimal.Truncate(value * 100) / 100; }
        }

        public string LastUsedConfig { get; set; }
        public decimal Limit { get; set; }
        public bool LocalNoPlanet { get; set; }
        public string LocationChild { get; set; }
        public string LocationParent { get; set; }
        public bool Loop { get; set; }
        public decimal LoopInt { get; set; }
        public decimal LSPenalty { get; set; }

        public decimal Margin
        {
            get { return decimal.Truncate(margin * 100) / 100; }
            set { margin = decimal.Truncate(value * 100) / 100; }
        }

        public string MarkedStations { get; set; }
        public decimal MaxGPT { get; set; }
        public decimal MaxLSDistance { get; set; }
        public bool MiniModeOnTop { get; set; }
        public string NetLogPath { get; set; }
        public string Padsizes { get; set; }
        public string Planetary { get; set; }
        public decimal PruneHops { get; set; }
        public decimal PruneScore { get; set; }
        public string PythonPath { get; set; }
        public bool Quiet { get; set; }
        public decimal RebuyPercentage { get; set; }
        public bool RouteNoPlanet { get; set; }
        public bool RouteStations { get; set; }
        public bool ShowJumps { get; set; }
        public bool ShowProgress { get; set; }
        public bool Summary { get; set; }
        public string SizeChild { get; set; }
        public string SizeParent { get; set; }
        public decimal Stock { get; set; }
        public string TDPath { get; set; }
        public bool TestSystems { get; set; }
        public bool Towards { get; set; }
        public string TreeViewFont { get; set; }
        public bool Unique { get; set; }

        public decimal UnladenLY
        {
            get { return decimal.Truncate(unladenLY * 100) / 100; }
            set { unladenLY = decimal.Truncate(value * 100) / 100; }
        }

        public string UploadPath { get; set; }
        public decimal Verbosity { get; set; }
        public string Via { get; set; }

        public Font ConvertFromMemberFont()
        {
            FontConverter conv = new FontConverter();

            if (this.TreeViewFont != null)
            {
                return conv.ConvertFromInvariantString(this.TreeViewFont) as Font;
            }
            else
            {
                return null;
            }
        }

        public string ConvertToFontString(Font fontObject)
        {
            FontConverter conv = new FontConverter();

            if (fontObject != null)
            {
                return conv.ConvertToInvariantString(fontObject);
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion Props

        public void Reset(TDSettings instance)
        {
            // go through and reset all accessors in instance
            instance.LocationParent = string.Empty;
            instance.LocationChild = string.Empty;
            instance.SizeParent = string.Empty;
            instance.SizeChild = string.Empty;
            instance.TDPath = string.Empty;
            instance.EdcePath = string.Empty;
            instance.NetLogPath = string.Empty;
            instance.PythonPath = string.Empty;
            instance.ImportPath = string.Empty;
            instance.UploadPath = string.Empty;
            instance.LastUsedConfig = string.Empty;
            instance.MarkedStations = string.Empty;
            instance.CmdrName = string.Empty;
            instance.Padsizes = string.Empty;
            instance.Avoid = string.Empty;
            instance.Via = string.Empty;
            instance.ExtraRunParams = string.Empty;
            instance.TreeViewFont = string.Empty;

            instance.Hops = 0;
            instance.Jumps = 0;
            instance.Credits = 0;
            instance.Insurance = 0;
            instance.Capacity = 0;
            instance.PruneHops = 0;
            instance.PruneScore = 0;
            instance.LSPenalty = 0;
            instance.MaxLSDistance = 0;
            instance.LoopInt = 0;
            instance.Limit = 0;
            instance.AbovePrice = 0;
            instance.BelowPrice = 0;
            instance.Age = 0;
            instance.GPT = 0;
            instance.MaxGPT = 0;
            instance.Stock = 0;
            instance.Demand = 0;
            instance.Verbosity = 0;
            instance.CSVSelect = 0;

            instance.UnladenLY = 0;
            instance.LadenLY = 0;
            instance.Margin = 0;

            instance.Towards = false;
            instance.Loop = false;
            instance.Unique = false;
            instance.ShowJumps = false;
            instance.TestSystems = false;
            instance.Corrections = false;
            instance.MiniModeOnTop = false;
            instance.DisableNetLogs = false;
            instance.DoNotUpdate = false;
            instance.HasUpdated = false;
            instance.CopySystemToClipboard = false;

            instance.RebuyPercentage = 5;
            instance.AvailableShips = string.Empty;

            instance.RouteNoPlanet = false;
            instance.LocalNoPlanet = false;
            instance.ShowProgress = false;
            instance.Planetary = string.Empty;
            instance.Quiet = false;
            instance.Summary = false;
        }
    }

    #endregion TDSettings
}