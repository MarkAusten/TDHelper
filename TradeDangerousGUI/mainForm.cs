using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace TDHelper
{
    public partial class MainForm : Form
    {
        #region FormProps

        private const int SnapDist = 10;
        private string appVersion = "v" + Application.ProductVersion;

        private int buttonCaller, methodIndex, stationIndex, methodFromIndex = -1, hasUpdated, procCode = -1;
        private string notesFile = Path.GetDirectoryName(Application.ExecutablePath) + @"\saved_notes.txt";
        private string savedFile1 = Path.GetDirectoryName(Application.ExecutablePath) + @"\saved_1.txt";
        private string savedFile2 = Path.GetDirectoryName(Application.ExecutablePath) + @"\saved_2.txt";
        private string savedFile3 = Path.GetDirectoryName(Application.ExecutablePath) + @"\saved_3.txt";
        private System.Timers.Timer testSystemsTimer = new System.Timers.Timer();
        private string tv_outputBox = string.Empty;
        private CultureInfo userCulture = CultureInfo.CurrentCulture;
        private IList<string> validConfigs = new List<string>();

        #endregion FormProps

        public MainForm()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                    | ControlStyles.UserPaint
                    | ControlStyles.DoubleBuffer, true);

            ValidateSettings();

            // Build variables from config
            BuildSettings();

            // Copy the setting to the form controls
            CopySettingsFromConfig();

            // And validate the settings to get rid of any nonsense.
            ValidateSettings();

            // Let's change the title
            SetFormTitle(this);

            testSystemsTimer.AutoReset = false;
            testSystemsTimer.Interval = 10000;
            testSystemsTimer.Elapsed += this.TestSystemsTimer_Delegate;

            this.creditsBox.Maximum = Decimal.MaxValue;
        }

        private void AltConfigBox_DropDown(object sender, EventArgs e)
        {
            // refresh our index
            //validConfigs = parseValidConfigs();
            //altConfigBox.DataSource = null;
            //altConfigBox.DataSource = validConfigs[1];
        }

        private void AltConfigBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            settingsRef.LastUsedConfig = altConfigBox.Text;
            SetShipList();
        }

        private void AvoidBox_TextChanged(object sender, EventArgs e)
        {
            // account for startJumpsBox
            if (startJumpsBox.Value > 0)
                settingsRef.Avoid = avoidBox.Text;
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
             * This worker delegate updates the commodities and recent systems lists
             */

            // let the refresh methods decide what to refresh
            this.Invoke(new Action(() =>
            {
                BuildOutput(buttonCaller == 16);
            }));
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableRunButtons();

            // we were called by getSystemButton
            if (buttonCaller == 2 && output_unclean.Count > 0)
            {
                // skip our favorites to get our most recent system/station
                if (output_unclean.Count > 0 && currentMarkedStations.Count > 0)
                    srcSystemComboBox.SelectedIndex = 1 + currentMarkedStations.Count;
                else if (output_unclean.Count > 0)
                    srcSystemComboBox.SelectedIndex = 1; // if the favorites are empty

                srcSystemComboBox.Focus();
            }

            hasUpdated = -1; // reset the updated semaphore
            buttonCaller = 0; // reset caller semaphore
        }

        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
             * This worker delegate is the main work horse for the application--
             * it controls the logic for all the primary commands.
             */

            // avoid issues by enforcing InvariantCulture
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            bool okayToRun = true;

            td_proc = new Process();
            td_proc.StartInfo.FileName = settingsRef.PythonPath;

            if (settingsRef.PythonPath.EndsWith("trade.exe", StringComparison.OrdinalIgnoreCase))
            {
                t_path = string.Empty; // go in blank so we don't pass silliness to trade.exe
            }
            else
            {
                t_path = "-u \"" + Path.Combine(settingsRef.TDPath, "trade.py") + "\" ";
            }

            /* Indexes are as follows:
             *
             * Index = 0 is the Run command
             * Index = 1 is the Buy command
             * Index = 2 is the Sell command
             * Index = 3 is the Rare command
             * Index = 4 is the Trade command
             * Index = 5 is the Market command
             * Index = 6 is the Station command
             * Index = 7 is the ShipVendor command
             * Index = 8 is the Navigation command
             * Index = 9 is the OldData command
             *
             * buttonCaller = 1 is a semaphore for the Run button
             * buttonCaller = 2 is a semaphore for the "C" button
             * buttonCaller = 4 is a semaphore for the Run delegate
             * buttonCaller = 5 is a semaphore for the Cancel button
             *
             * buttonCaller = 16 forces a full database update
             * buttonCaller = 17 forces a call to populateStationPanel
             * buttonCaller = 18 is an override for the Local box
             *
             * buttonCaller = 5 is the "Update DB" button
             * buttonCaller = 10 is the Station Commodities Editor button
             * buttonCaller = 11 is the Station Editor button (Ctrl+Click)
             * buttonCaller = 12 is the Import button
             * buttonCaller = 14 is the Shift+Import button
             * buttonCaller = 13 is the Upload button
             *
             * buttonCaller = 20 is an explicit semaphore for playAlert()
             * buttonCaller = 21 is the config file selection box
             *
             * buttonCaller = 22 is the Cmdr Profile button
             *
             * Buttons should always be placed above normal indexes!
             */

            if (buttonCaller == 14)
            {
                // we're coming from shift+import button
                if (!string.IsNullOrEmpty(settingsRef.ImportPath))
                    t_path += "import \"" + settingsRef.ImportPath + "\"";
                else
                    okayToRun = false;
            }
            else if (buttonCaller == 12)
            {
                // we're coming from import button
                t_path += "import -P edapi -O eddn";
            }
            else if (buttonCaller == 11)
            {
                // we're coming from getUpdatedPricesFile()
                if (!string.IsNullOrEmpty(temp_src))
                    t_path += "update -A -D --editor=\"nul\" \"" + temp_src + "\"";
                else
                {
                    okayToRun = false;
                    PlayAlert();
                }
            }
            else if (buttonCaller == 10)
            {
                // we're coming from station commodities editor
                if (!string.IsNullOrEmpty(temp_src))
                    t_path += "update -A -D -G \"" + temp_src + "\"";
                else
                {
                    okayToRun = false;
                    PlayAlert();
                }
            }
            else if (t_localNavEnabled)
            {
                // local is a global override, let's catch it first
                if (!string.IsNullOrEmpty(temp_src))
                {
                    t_path += "local --ly " + t_ladenLY;

                    if (rearmBoxChecked == 1) { t_path += " --rearm"; }
                    if (refuelBoxChecked == 1) { t_path += " --refuel"; }
                    if (repairBoxChecked == 1) { t_path += " --repair"; }
                    if (marketBoxChecked == 1) { t_path += " --trading"; }
                    if (blackmarketBoxChecked == 1) { t_path += " --bm"; }
                    if (shipyardBoxChecked == 1) { t_path += " --shipyard"; }
                    if (outfitBoxChecked == 1) { t_path += " --outfitting"; }
                    if (stationsFilterChecked) { t_path += " --stations"; }

                    if (!string.IsNullOrEmpty(settingsRef.Padsizes)) { t_path += " --pad=" + settingsRef.Padsizes; }
                    if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }
                    t_path += " \"" + temp_src + "\"";
                }
                else
                {
                    okayToRun = false;
                    PlayAlert();
                }
            }
            else if (methodIndex == 8)
            {
                // mark us as coming from the olddata command
                if (!string.IsNullOrEmpty(temp_src))
                {
                    t_path += "olddata";

                    if (oldDataRouteChecked) { t_path += " --route"; }
                    if (settingsRef.LadenLY > 0) { t_path += " --ly=" + settingsRef.LadenLY; }
                    if (minAgeUpDown.Value > minAgeUpDown.Minimum
                        && minAgeUpDown.Value < minAgeUpDown.Maximum)
                        t_path += " --min-age=" + minAgeUpDown.Value;

                    t_path += " --lim=50 ";
                    if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }
                    t_path += " --near=\"" + temp_src + "\"";
                }
                else
                {
                    okayToRun = false;
                    PlayAlert();
                }
            }
            else if (methodIndex == 7)
            {
                // mark us as coming from the nav command
                if (!string.IsNullOrEmpty(temp_src) && !string.IsNullOrEmpty(temp_dest))
                {
                    t_path += "nav";

                    if (!string.IsNullOrEmpty(settingsRef.Via)) { t_path += " --via=\"" + settingsRef.Via + "\""; }
                    if (!string.IsNullOrEmpty(settingsRef.Avoid)) { t_path += " --avoid=\"" + settingsRef.Avoid + "\""; }
                    if (settingsRef.LadenLY > 0) { t_path += " --ly=" + settingsRef.LadenLY; }
                    if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }
                    t_path += " \"" + temp_src + "\" " + "\"" + temp_dest + "\"";
                }
                else
                {
                    okayToRun = false;
                    PlayAlert();
                }
            }
            else if (methodIndex == 6)
            {
                // mark us as coming from the shipvendor editor
                if (!string.IsNullOrEmpty(temp_src))
                {
                    if (!string.IsNullOrEmpty(temp_shipsSold) && stationIndex != 2)
                    {
                        // clean our ship input before passing
                        string sanitizedInput = CleanShipVendorInput(temp_shipsSold);

                        if (stationIndex == 0)
                        {
                            // we're adding/updating (default)
                            t_path += "shipvendor -a \"" + temp_src + "\" " + "\"" + sanitizedInput + "\"";
                        }
                        else if (stationIndex == 1)
                        {
                            // removing
                            t_path += "shipvendor -rm \"" + temp_src + "\" " + "\"" + sanitizedInput + "\"";
                        }

                        // force a station/shipvendor panel update
                        buttonCaller = 17;
                    }
                    else
                    {
                        t_path += "shipvendor \"" + temp_src + "\"";
                    }
                }
                else
                {
                    okayToRun = false;
                    PlayAlert();
                }
            }
            else if (methodIndex == 5)
            { // Market command
                if (!string.IsNullOrEmpty(temp_src))
                {
                    t_path += "market";

                    if (bmktCheckBox.Checked) { t_path += " -B"; }
                    if (directCheckBox.Checked) { t_path += " -S"; }
                    if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }
                    t_path += " \"" + temp_src + "\"";
                }
                else
                {
                    okayToRun = false;
                    PlayAlert();
                }
            }
            else if (methodIndex == 4)
            {
                // mark us as coming from the trade command
                if (!string.IsNullOrEmpty(temp_src) && !string.IsNullOrEmpty(temp_dest))
                {
                    t_path += "trade";

                    t_path += " \"" + temp_src + "\" " + "\"" + temp_dest + "\" " + t_outputVerbosity;
                }
                else
                {
                    okayToRun = false;
                    PlayAlert();
                }
            }
            else if (methodIndex == 3)
            { // Rares command
                if (!string.IsNullOrEmpty(temp_src))
                {
                    t_path += "rares";

                    if (r_ladenLY > 0) { t_path += " --ly=" + r_ladenLY; }

                    if (stationIndex == 1) { t_path += " --legal"; }
                    else if (stationIndex == 2) { t_path += " --illegal"; }

                    if (!string.IsNullOrEmpty(settingsRef.Padsizes)) { t_path += " --pad=" + settingsRef.Padsizes; }
                    if (r_unladenLY > 0 && !string.IsNullOrEmpty(r_fromBox)) { t_path += " --away=" + r_unladenLY; }
                    if (!string.IsNullOrEmpty(r_fromBox) && r_unladenLY > 0)
                    {
                        // break from systems into separate pieces
                        // delimited by commas, removing whitespace
                        foreach (string output in
                            r_fromBox.Split(',')
                            .Select(x => x.Trim())
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .ToArray())
                        {
                            t_path += " --from=\"" + output + "\"";
                        }
                    }
                    if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }
                    t_path += " \"" + temp_src + "\"";
                }
                else
                {
                    okayToRun = false;
                    PlayAlert();
                }
            }
            else if (methodIndex == 2)
            { // Sell command
                t_path += "sell";

                if (!string.IsNullOrEmpty(temp_src)) { t_path += " --near=\"" + temp_src + "\""; }
                if (t2_ladenLY > 0) { t_path += " --ly=" + t2_ladenLY; }
                if (settingsRef.AbovePrice > 0) { t_path += " --gt=" + settingsRef.AbovePrice; }
                if (settingsRef.BelowPrice > 0) { t_path += " --ls=" + settingsRef.BelowPrice; }
                if (demandBox.Value > 0) { t_path += " --demand=" + demandBox.Value; }
                if (bmktCheckBox.Checked) { t_path += " --bm"; }
                if (!string.IsNullOrEmpty(settingsRef.Padsizes)) { t_path += " --pad=" + settingsRef.Padsizes; }
                if (!string.IsNullOrEmpty(settingsRef.Avoid)) { t_path += " --avoid=\"" + settingsRef.Avoid + "\""; }
                if (stationIndex == 1 && !string.IsNullOrEmpty(temp_src)) { t_path += " --price-sort"; }
                if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }
                t_path += " --lim=50";

                if (!string.IsNullOrEmpty(temp_commod))
                {
                    t_path += " \"" + temp_commod + "\"";
                }
            }
            else if (methodIndex == 1)
            { // Buy command
                t_path += "buy";

                if (!string.IsNullOrEmpty(temp_src)) { t_path += " --near=\"" + temp_src + "\""; }
                if (t1_ladenLY > 0) { t_path += " --ly=" + t1_ladenLY; }
                if (stockBox.Value > 0) { t_path += " --supply=" + stockBox.Value; }
                if (settingsRef.AbovePrice > 0) { t_path += " --gt=" + settingsRef.AbovePrice; }
                if (settingsRef.BelowPrice > 0) { t_path += " --lt=" + settingsRef.BelowPrice; }
                if (oneStopCheckBox.Checked) { t_path += " -1"; }
                if (bmktCheckBox.Checked) { t_path += " --bm"; }
                if (!string.IsNullOrEmpty(settingsRef.Padsizes)) { t_path += " --pad=" + settingsRef.Padsizes; }
                if (!string.IsNullOrEmpty(settingsRef.Avoid)) { t_path += " --avoid=\"" + settingsRef.Avoid + "\""; }

                if (stationIndex == 1 && !string.IsNullOrEmpty(temp_src)) { t_path += " --price-sort"; }
                else if (stationIndex == 2) { t_path += " --units-sort"; }

                if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }
                t_path += " --lim=50";

                if (!string.IsNullOrEmpty(temp_commod))
                {
                    t_path += " \"" + temp_commod + "\"";
                }
            }
            else if (methodIndex == 0)
            { // Run command
                t_path += "run";

                if (!string.IsNullOrEmpty(temp_src))
                {
                    t_path += " --fr=\"" + temp_src + "\"";

                    // towards requires a source
                    if (!string.IsNullOrEmpty(temp_dest) && settingsRef.Towards)
                        t_path += " --towards=\"" + temp_dest + "\"";
                }

                if (!string.IsNullOrEmpty(temp_dest) && !settingsRef.Towards && !settingsRef.Loop)
                {
                    // allow a destination without a source for anonymous Runs
                    t_path += " --to=\"" + temp_dest + "\"";
                    if (t_EndJumps > 0) { t_path += " --end=" + t_EndJumps; }
                }

                // normal run command (includes non-specific case)
                // last opportunity to sanity check inputs
                if (settingsRef.Capacity > 0) { t_path += " --cap=" + settingsRef.Capacity; }
                if (settingsRef.Limit > 0) { t_path += " --lim=" + settingsRef.Limit; }
                if (settingsRef.Insurance > 0) { t_path += " --ins=" + settingsRef.Insurance; }
                if (settingsRef.Credits > 0) { t_path += " --cr=" + settingsRef.Credits; }
                if (settingsRef.LadenLY > 0.00m) { t_path += " --ly=" + settingsRef.LadenLY; }
                if (settingsRef.UnladenLY > 0.00m) { t_path += " --empty=" + settingsRef.UnladenLY; }
                if (!string.IsNullOrEmpty(settingsRef.Padsizes)) { t_path += " --pad=" + settingsRef.Padsizes; }
                if (settingsRef.GPT > 0) { t_path += " --gpt=" + settingsRef.GPT; }
                if (settingsRef.MaxGPT > 0) { t_path += " --mgpt=" + settingsRef.MaxGPT; }
                if (settingsRef.Stock > 0) { t_path += " --supply=" + settingsRef.Stock; }
                if (settingsRef.Demand > 0) { t_path += " --demand=" + settingsRef.Demand; }
                if (settingsRef.Margin > 0.00m) { t_path += " --margin=" + settingsRef.Margin; }
                if (settingsRef.Hops > 0) { t_path += " --hops=" + settingsRef.Hops; }
                if (settingsRef.Jumps > 0) { t_path += " --jum=" + settingsRef.Jumps; }
                if (t_StartJumps > 0) { t_path += " --start=" + t_StartJumps; }
                if (settingsRef.LSPenalty > 0) { t_path += " --lsp=" + settingsRef.LSPenalty; }
                if (settingsRef.MaxLSDistance > 0) { t_path += " --ls-max=" + settingsRef.MaxLSDistance; }
                if (settingsRef.PruneHops > 0) { t_path += " --prune-hops=" + settingsRef.PruneHops; }
                if (settingsRef.PruneScore > 0) { t_path += " --prune-score=" + settingsRef.PruneScore; }
                if (settingsRef.Age > 0) { t_path += " --age=" + settingsRef.Age; }
                if (bmktCheckBox.Checked) { t_path += " --bm"; }
                if (directCheckBox.Checked) { t_path += " --direct"; }
                if (shortenCheckBox.Checked) { t_path += " --shorten"; }
                if (settingsRef.Unique) { t_path += " --unique"; }
                if (!string.IsNullOrEmpty(settingsRef.Avoid)) { t_path += " --avoid=\"" + settingsRef.Avoid + "\""; }
                if (!string.IsNullOrEmpty(settingsRef.Via)) { t_path += " --via=\"" + settingsRef.Via + "\""; }
                if (settingsRef.Loop) { t_path += " --loop"; }
                if (settingsRef.LoopInt > 0) { t_path += " --loop-int=" + settingsRef.LoopInt; }
                if (belowPriceBox.Value > 0) { t_path += " --routes=" + belowPriceBox.Value; }
                if (settingsRef.ShowJumps) { t_path += " -J"; }
                if (!string.IsNullOrEmpty(settingsRef.ExtraRunParams)) { t_path += " " + settingsRef.ExtraRunParams; }
                if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }

                buttonCaller = 4; // mark as Run command
            }

            // pass the built command-line to the delegate
            if (okayToRun)
                DoTDProc(t_path);
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!backgroundWorker2.IsBusy)
            {
                stopwatch.Stop(); // stop the timer
                circularBuffer = new System.Text.StringBuilder(2 * circularBufferSize);

                // let's alert the user that the Output pane has changed
                if (!string.IsNullOrEmpty(td_outputBox.Text) && tabControl1.SelectedTab != outputPage)
                    tabControl1.SelectedTab = outputPage;

                // assume we're coming from getUpdatedPricesFile()
                if (!string.IsNullOrEmpty(settingsRef.ImportPath) && buttonCaller == 11)
                    CleanUpdatedPricesFile();
                else if (buttonCaller == 16)
                {
                    // force a db sync if we're marked
                    if (!backgroundWorker1.IsBusy)
                        backgroundWorker1.RunWorkerAsync();
                }
                else if (buttonCaller == 17)
                {
                    // force a station/shipvendor panel update
                    PopulateStationPanel(temp_src);
                }

                // reenable after uncancellable task is done
                EnableRunButtons();
                runButton.Font = new Font(runButton.Font, FontStyle.Regular);
                runButton.Text = "&Run";

                td_proc.Dispose();

                // make a sound when we're done with a long operation (>10s)
                if ((stopwatch.ElapsedMilliseconds > 10000 && buttonCaller != 5) || buttonCaller == 20)
                {
                    PlayAlert(); // when not marked as cancelled, or explicit
                }

                if (buttonCaller != 4) // not if we're coming from Run
                {
                    miniModeButton.Enabled = false;
                }
                else if (buttonCaller == 4)
                {
                    string filteredOutput = FilterOutput(td_outputBox.Text);
                    // validate the run output before we enable the mini-mode button
                    runOutputState = IsValidRunOutput(filteredOutput);

                    if (runOutputState > -1)
                    {
                        hasParsed = false; // reset the semaphore
                        tv_outputBox = filteredOutput; // copy our validated input
                        miniModeButton.Enabled = true;
                    }
                    else
                        miniModeButton.Enabled = false;
                }

                buttonCaller = 0;
            }
        }

        private void BackgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
             * This worker delegate is just a thread for the background timer to run on
             */

            stopwatch.Start(); // start the timer

            while (stopwatch.IsRunning)
            {
                System.Threading.Thread.Sleep(1000);

                this.Invoke(new Action(() =>
                {
                    // do this on the UI thread
                    stopWatchLabel.Text = "Elapsed: " + stopwatch.Elapsed.ToString("hh\\:mm\\:ss");
                }));
            }
        }

        private void BackgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // when we get the !IsRunning signal, write out
            this.Invoke(new Action(() =>
            {
                // do this on the UI thread
                stopWatchLabel.Text = "Elapsed: " + stopwatch.Elapsed.ToString("hh\\:mm\\:ss");
                stopwatch.Reset();
            }));
        }

        private void BackgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
             * This worker delegate is for the update process
             */

            GetDataUpdates(); // update conditionally
            DoTDProc(t_path); // pass this to the worker delegate

            t_path = string.Empty; // reset path for thread safety
        }

        private void BackgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!backgroundWorker4.IsBusy)
            {
                stopwatch.Stop(); // stop the timer
                circularBuffer = new System.Text.StringBuilder(2 * circularBufferSize);

                EnableRunButtons();
                td_proc.Dispose();

                // we have to update the comboboxes now
                if (!backgroundWorker1.IsBusy)
                {
                    buttonCaller = 16; // mark us as needing a full refresh
                    backgroundWorker1.RunWorkerAsync();
                }

                // make a sound when we're done with a long operation (>10s)
                if (stopwatch.ElapsedMilliseconds > 10000)
                {
                    PlayAlert();
                }

                rebuildCache = false;
            }
        }

        private void BackgroundWorker5_DoWork(object sender, DoWorkEventArgs e)
        {
            // this background worker is for the auto-updater
            if (!settingsRef.DoNotUpdate && !Program.updateOverride)
            {
                // check for override before running a poll
                DoHotSwap();
            }
        }

        private void BackgroundWorker5_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (settingsRef.HasUpdated)
            {
                // show the update notification
                updateNotifyLabel.Visible = true;
                updateNotifyIcon.Visible = true;
            }

            // always wipe the temp files
            DoHotSwapCleanup();
        }

        private void BackgroundWorker6_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
             * This worker delegate is intended to update the system list every few seconds,
             * notifying when an unrecognized system is detected.
             */

            BuildOutput(!hasRun);

            if (settingsRef.TestSystems)
            {
                if (string.IsNullOrEmpty(t_lastSysCheck) && !string.IsNullOrEmpty(t_lastSystem)
                    && !StringInList(t_lastSystem, outputSysStnNames))
                {
                    // alert the user if we're starting in an unknown system
                    PlayUnknown();
                    this.Invoke(new Action(() =>
                    {
                        // run this on the UI thread
                        this.td_outputBox.Text += "We're currently in an unrecognized system: " + t_lastSystem + "\r\n";
                        if (settingsRef.CopySystemToClipboard) { Clipboard.SetData(DataFormats.Text, t_lastSystem); }
                    }));
                    t_lastSysCheck = t_lastSystem;
                }
                else if ((!string.IsNullOrEmpty(t_lastSysCheck) && !t_lastSysCheck.Equals(t_lastSystem)
                    && !StringInList(t_lastSystem, outputSysStnNames)) || string.IsNullOrEmpty(t_lastSysCheck)
                    && !StringInList(t_lastSystem, outputSysStnNames))
                {
                    // if we've already checked a recent system, only check the newest entered system once
                    PlayUnknown(); // alert the user
                    // only flash if the window isn't active
                    if (!isActive) { FlashWindow.BlinkStart(this); }

                    this.Invoke(new Action(() =>
                    {
                        // run this on the UI thread
                        this.td_outputBox.Text += "Entering an unrecognized system: " + t_lastSystem + "\r\n";
                        if (settingsRef.CopySystemToClipboard) { Clipboard.SetData(DataFormats.Text, t_lastSystem); }
                    }));

                    t_lastSysCheck = t_lastSystem; // prevent rechecking the same system
                }
            }
        }

        private void BackgroundWorker6_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            testSystemsTimer.Start(); // fire again after ~10s
        }

        private void BackgroundWorker7_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
             * This worker delegate is for the Cmdr Profile process
             */
            bool okayToRun = true;

            if (buttonCaller == 22)
            {
                // Check to see if the EDCE folder and files are valid
                if (this.ValidateEdce())
                {
                    // EDCE is valid so set up the call.
                    t_path = "\"" + Path.Combine(MainForm.settingsRef.EdcePath, "edce_client.py") + "\"";
                }
                else
                {
                    okayToRun = false;
                    PlayAlert();
                }
            }

            // pass the built command-line to the delegate
            if (okayToRun)
            {
                string currentFolder = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(MainForm.settingsRef.EdcePath);

                try
                {
                    td_proc = new Process();
                    td_proc.StartInfo.FileName = settingsRef.PythonPath;

                    DoTDProc(t_path);
                }
                finally
                {
                    Directory.SetCurrentDirectory(currentFolder);
                }
            }

            t_path = string.Empty; // reset path for thread safety
        }

        private void BackgroundWorker7_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!backgroundWorker7.IsBusy)
            {
                stopwatch.Stop(); // stop the timer
                circularBuffer = new System.Text.StringBuilder(2 * circularBufferSize);

                runButton.Text = "&Run";
                EnableRunButtons();
                td_proc.Dispose();

                // make a sound when we're done with a long operation (>10s)
                if (stopwatch.ElapsedMilliseconds > 10000)
                {
                    PlayAlert();
                }

                this.UpdateCommanderAndShipDetails();
            }
        }

        private void BmktCheckBox_Click(object sender, EventArgs e)
        {
            // an exception for the market command
            if (methodIndex == 4 && directCheckBox.Checked)
            {
                // we cannot have both buy and sell enabled
                directCheckBox.Checked = false;
                bmktCheckBox.Checked = true;
            }
        }

        /// <summary>
        /// Handle the Cmdr Profile functionality.
        /// </summary>
        /// <param name="sender">The object sender.</param>
        /// <param name="e">The event args.</param>
        private void BtnCmdrProfile_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker7.IsBusy)
            {
                // Cmdr Profile button.
                buttonCaller = 22;
                DisableRunButtons(); // disable buttons during uncancellable operations

                backgroundWorker7.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Handle the save settings functionality.
        /// </summary>
        /// <param name="sender">The object sender.</param>
        /// <param name="e">The event args.</param>
        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            this.btnSaveSettings.Enabled = false;

            CopySettingsFromForm();
            SaveSettingsToIniFile();

            this.btnSaveSettings.Enabled = true;
        }

        private void ClearSaved1MenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(savedTextBox1.Text))
            {
                savedTextBox1.Text = string.Empty;
                if (File.Exists(savedFile1))
                    File.Delete(savedFile1);
            }
        }

        private void ClearSaved2MenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(savedTextBox2.Text))
            {
                savedTextBox2.Text = string.Empty;
                if (File.Exists(savedFile2))
                    File.Delete(savedFile2);
            }
        }

        private void ClearSaved3MenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(savedTextBox3.Text))
            {
                savedTextBox3.Text = string.Empty;
                if (File.Exists(savedFile3))
                    File.Delete(savedFile3);
            }
        }

        private void ComboBox_DropDown(object sender, EventArgs e)
        {
            dropdownOpened = true;
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            dropdownOpened = false;
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;

            if (clickedControl.Name == notesTextBox.Name && !deleteMenuItem.Enabled)
            {
                cutMenuItem.Enabled = true;
                deleteMenuItem.Enabled = true;
                pasteMenuItem.Enabled = true;
                notesClearMenuItem.Visible = true;
                savePage1MenuItem.Enabled = false;
                savePage2MenuItem.Enabled = false;
                savePage3MenuItem.Enabled = false;
            }
            else if (clickedControl.Name == savedTextBox1.Name
                || clickedControl.Name == savedTextBox2.Name
                || clickedControl.Name == savedTextBox3.Name
                || clickedControl.Name == td_outputBox.Name)
            {
                cutMenuItem.Enabled = false;
                deleteMenuItem.Enabled = false;
                pasteMenuItem.Enabled = false;
                notesClearMenuItem.Visible = false;
                savePage1MenuItem.Enabled = true;
                savePage2MenuItem.Enabled = true;
                savePage3MenuItem.Enabled = true;
            }

            // control clearing
            if (clickedControl.Name == savedTextBox1.Name)
            {
                clearSaved1MenuItem.Visible = true;
                clearSaved2MenuItem.Visible = false;
                clearSaved3MenuItem.Visible = false;
            }
            else if (clickedControl.Name == savedTextBox2.Name)
            {
                clearSaved2MenuItem.Visible = true;
                clearSaved1MenuItem.Visible = false;
                clearSaved3MenuItem.Visible = false;
            }
            else if (clickedControl.Name == savedTextBox3.Name)
            {
                clearSaved3MenuItem.Visible = true;
                clearSaved2MenuItem.Visible = false;
                clearSaved1MenuItem.Visible = false;
            }
            else
            {
                clearSaved1MenuItem.Visible = false;
                clearSaved2MenuItem.Visible = false;
                clearSaved3MenuItem.Visible = false;
            }
        }

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.Copy();
        }

        private void CopySettingsFromConfig()
        {
            //
            // Load controls in the form from variables in memory
            //

            avoidBox.Text = settingsRef.Avoid;
            viaBox.Text = settingsRef.Via;

            capacityBox.Value = settingsRef.Capacity > capacityBox.Minimum && settingsRef.Capacity <= capacityBox.Maximum
                ? settingsRef.Capacity
                : capacityBox.Minimum;

            creditsBox.Value = settingsRef.Credits > creditsBox.Minimum && settingsRef.Credits <= creditsBox.Maximum
                ? settingsRef.Credits
                : creditsBox.Minimum;

            insuranceBox.Value = settingsRef.Insurance > insuranceBox.Minimum && settingsRef.Insurance <= insuranceBox.Maximum
                ? settingsRef.Insurance
                : insuranceBox.Minimum;

            lsPenaltyBox.Value = settingsRef.LSPenalty > lsPenaltyBox.Minimum && settingsRef.LSPenalty <= lsPenaltyBox.Maximum
                ? settingsRef.LSPenalty
                : lsPenaltyBox.Minimum;

            maxLSDistanceBox.Value = settingsRef.MaxLSDistance > maxLSDistanceBox.Minimum && settingsRef.MaxLSDistance <= maxLSDistanceBox.Maximum
                ? settingsRef.MaxLSDistance
                : maxLSDistanceBox.Minimum;

            loopIntBox.Value = settingsRef.LoopInt > loopIntBox.Minimum && settingsRef.LoopInt <= loopIntBox.Maximum
                ? settingsRef.LoopInt
                : loopIntBox.Minimum;

            ageBox.Value = settingsRef.Age > ageBox.Minimum && settingsRef.Age <= ageBox.Maximum
                ? settingsRef.Age
                : ageBox.Minimum;

            pruneHopsBox.Value = settingsRef.PruneHops > pruneHopsBox.Minimum && settingsRef.PruneHops <= pruneHopsBox.Maximum
                ? settingsRef.PruneHops
                : pruneHopsBox.Minimum;

            pruneScoreBox.Value = settingsRef.PruneScore > pruneScoreBox.Minimum && settingsRef.PruneScore <= pruneScoreBox.Maximum
                ? settingsRef.PruneScore
                : pruneScoreBox.Minimum;

            stockBox.Value = settingsRef.Stock > stockBox.Minimum && settingsRef.Stock <= stockBox.Maximum
                ? settingsRef.Stock
                : stockBox.Minimum;

            demandBox.Value = settingsRef.Demand > demandBox.Minimum && settingsRef.Demand <= demandBox.Maximum
                ? settingsRef.Demand
                : demandBox.Minimum;

            gptBox.Value = settingsRef.GPT > gptBox.Minimum && settingsRef.GPT <= gptBox.Maximum
                ? settingsRef.GPT
                : gptBox.Minimum;

            maxGPTBox.Value = settingsRef.MaxGPT > maxGPTBox.Minimum && settingsRef.MaxGPT <= maxGPTBox.Maximum
                ? settingsRef.MaxGPT
                : maxGPTBox.Minimum;

            hopsBox.Value = settingsRef.Hops > hopsBox.Minimum && settingsRef.Hops <= hopsBox.Maximum
                ? settingsRef.Hops
                : hopsBox.Minimum;

            jumpsBox.Value = settingsRef.Jumps > jumpsBox.Minimum && settingsRef.Jumps <= jumpsBox.Maximum
                ? settingsRef.Jumps
                : jumpsBox.Minimum;

            limitBox.Value = settingsRef.Limit > limitBox.Minimum && settingsRef.Limit <= limitBox.Maximum
                ? settingsRef.Limit
                : limitBox.Minimum;

            abovePriceBox.Value = settingsRef.AbovePrice > abovePriceBox.Minimum && settingsRef.AbovePrice <= abovePriceBox.Maximum
                ? settingsRef.AbovePrice
                : abovePriceBox.Minimum;

            belowPriceBox.Value = settingsRef.BelowPrice > belowPriceBox.Minimum && settingsRef.BelowPrice <= belowPriceBox.Maximum
                ? settingsRef.BelowPrice
                : belowPriceBox.Minimum;

            unladenLYBox.Value = settingsRef.UnladenLY > unladenLYBox.Minimum && settingsRef.UnladenLY <= unladenLYBox.Maximum
                ? settingsRef.UnladenLY
                : unladenLYBox.Minimum;

            ladenLYBox.Value = settingsRef.LadenLY > ladenLYBox.Minimum && settingsRef.LadenLY <= ladenLYBox.Maximum
                ? settingsRef.LadenLY
                : ladenLYBox.Minimum;

            marginBox.Value = settingsRef.Margin > marginBox.Minimum && settingsRef.Margin <= marginBox.Maximum
                ? settingsRef.Margin
                : marginBox.Minimum;

            // copy verbosity to string format
            switch (settingsRef.Verbosity)
            {
                case 0:
                    t_outputVerbosity = string.Empty;
                    verbosityComboBox.SelectedIndex = 0;
                    break;

                case 2:
                    t_outputVerbosity = "-vv";
                    verbosityComboBox.SelectedIndex = 2;
                    break;

                case 3:
                    t_outputVerbosity = "-vvv";
                    verbosityComboBox.SelectedIndex = 3;
                    break;

                default:
                    t_outputVerbosity = "-v";
                    verbosityComboBox.SelectedIndex = 1;
                    break;
            }

            // exception for Loop
            if (settingsRef.Loop && settingsRef.Towards)
            {
                // reset them both to prevent issues
                settingsRef.Loop = false;
                loopCheckBox.Checked = false;
                settingsRef.Towards = false;
                towardsCheckBox.Checked = false;
            }
            else if (settingsRef.Loop)
            {
                loopCheckBox.Checked = settingsRef.Loop;
                towardsCheckBox.Checked = false; // one or the other
            }
            else if (settingsRef.Towards)
            {
                towardsCheckBox.Checked = settingsRef.Towards;
                loopCheckBox.Checked = false;
            }

            // exceptions
            settingsRef.ShowJumps = showJumpsCheckBox.Checked;
            settingsRef.Unique = uniqueCheckBox.Checked;

            padSizeBox.Text = ContainsPadSizes(settingsRef.Padsizes)
                ? settingsRef.Padsizes
                : string.Empty;

            testSystemsCheckBox.Checked = settingsRef.TestSystems;
        }

        private void CopySettingsFromForm()
        {
            //
            // Load variables from text boxes in the form
            //

            // make sure we strip the excess whitespace in src/dest
            temp_src = RemoveExtraWhitespace(srcSystemComboBox.Text);
            temp_dest = RemoveExtraWhitespace(destSystemComboBox.Text);

            temp_commod = commodityComboBox.Text;

            // make sure we don't pass the "Ship's Sold" box if its still unchanged
            if (shipsSoldBox.Text.Equals(outputStationShips.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                temp_shipsSold = string.Empty;
            }
            else
            {
                temp_shipsSold = shipsSoldBox.Text;
            }

            if (methodIndex == 3)
            {
                r_fromBox = avoidBox.Text;
            }
            else
            {
                settingsRef.Avoid = avoidBox.Text;
            }

            settingsRef.Via = viaBox.Text;

            /*
             * The following is a set of workarounds to fix a bug in Framework 2.0+
             * involving NumericUpDown controls not updating the Value() property
             * when changed from the keyboard instead of the spinner control. We
             * do this by parsing the unbrowsable Text property and copying that.
             */

            if (decimal.TryParse(capacityBox.Text, out decimal t_Capacity))
            {
                capacityBox.Text = t_Capacity.ToString();
                settingsRef.Capacity = t_Capacity;
            }
            else
            {
                settingsRef.Capacity = capacityBox.Minimum; // this is a requirement
                capacityBox.Text = settingsRef.Capacity.ToString();
            }

            if (decimal.TryParse(creditsBox.Text, out decimal t_Credits))
            {
                creditsBox.Text = t_Credits.ToString();
                settingsRef.Credits = t_Credits;
            }
            else
            {
                settingsRef.Credits = creditsBox.Minimum; // this is a requirement
                creditsBox.Text = settingsRef.Credits.ToString();
            }

            if (decimal.TryParse(insuranceBox.Text, out decimal t_Insurance))
            {
                insuranceBox.Text = t_Insurance.ToString();
                settingsRef.Insurance = t_Insurance;
            }
            else
            {
                settingsRef.Insurance = insuranceBox.Minimum;
                insuranceBox.Text = settingsRef.Insurance.ToString();
            }

            if (decimal.TryParse(lsPenaltyBox.Text, out decimal t_lsPenalty))
            {
                lsPenaltyBox.Text = t_lsPenalty.ToString();
                settingsRef.LSPenalty = t_lsPenalty;
            }
            else
            {
                settingsRef.LSPenalty = lsPenaltyBox.Minimum;
                lsPenaltyBox.Text = settingsRef.LSPenalty.ToString();
            }

            if (decimal.TryParse(maxLSDistanceBox.Text, out decimal t_maxLSDistance))
            {
                maxLSDistanceBox.Text = t_maxLSDistance.ToString();
                settingsRef.MaxLSDistance = t_maxLSDistance;
            }
            else
            {
                settingsRef.MaxLSDistance = maxLSDistanceBox.Minimum;
                maxLSDistanceBox.Text = settingsRef.MaxLSDistance.ToString();
            }

            if (decimal.TryParse(loopIntBox.Text, out decimal t_LoopInt))
            {
                loopIntBox.Text = t_LoopInt.ToString();
                settingsRef.LoopInt = t_LoopInt;
            }
            else
            {
                settingsRef.LoopInt = loopIntBox.Minimum;
                loopIntBox.Text = settingsRef.LoopInt.ToString();
            }

            if (decimal.TryParse(pruneHopsBox.Text, out decimal t_pruneHops))
            {
                pruneHopsBox.Text = t_pruneHops.ToString();
                settingsRef.PruneHops = t_pruneHops;
            }
            else
            {
                settingsRef.PruneHops = pruneHopsBox.Minimum;
                pruneHopsBox.Text = settingsRef.PruneHops.ToString();
            }

            if (decimal.TryParse(pruneScoreBox.Text, out decimal t_pruneScore))
            {
                pruneScoreBox.Text = t_pruneScore.ToString();
                settingsRef.PruneScore = t_pruneScore;
            }
            else
            {
                settingsRef.PruneScore = pruneScoreBox.Minimum;
                pruneScoreBox.Text = settingsRef.PruneScore.ToString();
            }

            if (decimal.TryParse(stockBox.Text, out decimal t_Stock))
            {
                stockBox.Text = t_Stock.ToString();
                settingsRef.Stock = t_Stock;
            }
            else
            {
                settingsRef.Stock = stockBox.Minimum;
                stockBox.Text = settingsRef.Stock.ToString();
            }

            if (decimal.TryParse(demandBox.Text, out decimal t_Demand))
            {
                demandBox.Text = t_Demand.ToString();
                settingsRef.Demand = t_Demand;
            }
            else
            {
                settingsRef.Demand = demandBox.Minimum;
                demandBox.Text = settingsRef.Demand.ToString();
            }

            if (decimal.TryParse(ageBox.Text, out decimal t_Age))
            {
                ageBox.Text = t_Age.ToString();
                settingsRef.Age = t_Age;
            }
            else
            {
                settingsRef.Age = ageBox.Minimum;
                ageBox.Text = settingsRef.Age.ToString();
            }

            if (decimal.TryParse(gptBox.Text, out decimal t_GPT))
            {
                gptBox.Text = t_GPT.ToString();
                settingsRef.GPT = t_GPT;
            }
            else
            {
                settingsRef.GPT = gptBox.Minimum;
                gptBox.Text = settingsRef.GPT.ToString();
            }

            if (decimal.TryParse(maxGPTBox.Text, out decimal t_MaxGPT))
            {
                maxGPTBox.Text = t_MaxGPT.ToString();
                settingsRef.MaxGPT = t_MaxGPT;
            }
            else
            {
                settingsRef.MaxGPT = maxGPTBox.Minimum;
                maxGPTBox.Text = settingsRef.MaxGPT.ToString();
            }

            if (decimal.TryParse(limitBox.Text, out decimal t_Limit))
            {
                limitBox.Text = t_Limit.ToString();
                settingsRef.Limit = t_Limit;
            }
            else
            {
                settingsRef.Limit = limitBox.Minimum;
                limitBox.Text = settingsRef.Limit.ToString();
            }

            if (decimal.TryParse(hopsBox.Text, out decimal t_Hops))
            {
                hopsBox.Text = t_Hops.ToString();
                settingsRef.Hops = t_Hops;
            }
            else
            {
                settingsRef.Hops = hopsBox.Minimum;
                hopsBox.Text = settingsRef.Hops.ToString();
            }

            if (decimal.TryParse(jumpsBox.Text, out decimal t_Jumps))
            {
                jumpsBox.Text = t_Jumps.ToString();
                settingsRef.Jumps = t_Jumps;
            }
            else
            {
                settingsRef.Jumps = jumpsBox.Minimum;
                jumpsBox.Text = settingsRef.Jumps.ToString();
            }

            if (decimal.TryParse(endJumpsBox.Text, out t_EndJumps))
                endJumpsBox.Text = t_EndJumps.ToString();
            else
            {
                t_EndJumps = endJumpsBox.Minimum;
                endJumpsBox.Text = t_EndJumps.ToString();
            }

            if (decimal.TryParse(startJumpsBox.Text, out t_StartJumps))
                startJumpsBox.Text = t_StartJumps.ToString();
            else
            {
                t_StartJumps = startJumpsBox.Minimum;
                startJumpsBox.Text = t_StartJumps.ToString();
            }

            if (decimal.TryParse(abovePriceBox.Text, out decimal t_abovePrice))
            {
                abovePriceBox.Text = t_abovePrice.ToString();
                settingsRef.AbovePrice = t_abovePrice;
            }
            else
            {
                settingsRef.AbovePrice = abovePriceBox.Minimum;
                abovePriceBox.Text = settingsRef.AbovePrice.ToString();
            }

            if (decimal.TryParse(belowPriceBox.Text, out t_belowPrice))
            {
                belowPriceBox.Text = t_belowPrice.ToString();
                settingsRef.BelowPrice = t_belowPrice;
            }
            else
            {
                settingsRef.BelowPrice = belowPriceBox.Minimum;
                belowPriceBox.Text = settingsRef.BelowPrice.ToString();
            }

            if (decimal.TryParse(minAgeUpDown.Text, out decimal t_MinAge))
            {
                minAgeUpDown.Text = t_MinAge.ToString();
            }
            else
            {
                minAgeUpDown.Text = minAgeUpDown.Minimum.ToString();
            }

            settingsRef.Loop = loopCheckBox.Checked;
            settingsRef.Towards = towardsCheckBox.Checked;
            settingsRef.Unique = uniqueCheckBox.Checked;
            settingsRef.ShowJumps = showJumpsCheckBox.Checked;

            t_localNavEnabled = localNavCheckBox.Checked;
            stationsFilterChecked = stationsFilterCheckBox.Checked;
            oldDataRouteChecked = oldRoutesCheckBox.Checked;

            // convert the local checkstates to ints as well
            blackmarketBoxChecked = GetCheckBoxCheckState(bmktFilterCheckBox.CheckState);
            shipyardBoxChecked = GetCheckBoxCheckState(shipyardFilterCheckBox.CheckState);
            marketBoxChecked = GetCheckBoxCheckState(itemsFilterCheckBox.CheckState);
            repairBoxChecked = GetCheckBoxCheckState(repairFilterCheckBox.CheckState);
            rearmBoxChecked = GetCheckBoxCheckState(rearmFilterCheckBox.CheckState);
            refuelBoxChecked = GetCheckBoxCheckState(refuelFilterCheckBox.CheckState);
            outfitBoxChecked = GetCheckBoxCheckState(outfitFilterCheckBox.CheckState);

            //
            // exceptions
            //
            switch (verbosityComboBox.SelectedIndex)
            {
                case 0:
                    settingsRef.Verbosity = 0;
                    break;

                case 2:
                    settingsRef.Verbosity = 2;
                    break;

                case 3:
                    settingsRef.Verbosity = 3;
                    break;

                default:
                    settingsRef.Verbosity = 1;
                    break;
            }

            if (ContainsPadSizes(padSizeBox.Text))
            {
                settingsRef.Padsizes = padSizeBox.Text;
            }
            else
            {
                settingsRef.Padsizes = string.Empty;
                padSizeBox.Text = settingsRef.Padsizes.ToString();
            }

            t_confirmCode = string.Empty;

            // handle floats differently
            if (decimal.TryParse(unladenLYBox.Text, out decimal t_unladenLY))
            {
                if (methodIndex == 3)
                {
                    r_unladenLY = decimal.Truncate(t_unladenLY * 100) / 100;
                }
                else
                {
                    settingsRef.UnladenLY = decimal.Truncate(t_unladenLY * 100) / 100;
                }
            }
            else
            {
                settingsRef.UnladenLY = decimal.Truncate(unladenLYBox.Minimum * 100) / 100;
                unladenLYBox.Text = settingsRef.UnladenLY.ToString();
            }

            // the ladenLY is a bit more complicated, let's handle it
            if (!t_localNavEnabled)
            {
                if (decimal.TryParse(ladenLYBox.Text, out t_ladenLY))
                {
                    if (methodIndex == 3)
                    {
                        r_ladenLY = decimal.Truncate(t_ladenLY * 100) / 100; // an exception for the rare command
                    }
                    else if (methodIndex == 1)
                    {
                        t1_ladenLY = decimal.Truncate(t_ladenLY * 100) / 100; // an exception for the buy command
                    }
                    else if (methodIndex == 2)
                    {
                        t2_ladenLY = decimal.Truncate(t_ladenLY * 100) / 100; // an exception for the sell command
                    }
                    else
                    {
                        settingsRef.LadenLY = decimal.Truncate(t_ladenLY * 100) / 100;
                    }
                }
                else
                {
                    settingsRef.LadenLY = decimal.Truncate(ladenLYBox.Minimum * 100) / 100;
                    ladenLYBox.Text = settingsRef.LadenLY.ToString();
                }
            }
            else
            {
                // exception for local override
                if (decimal.TryParse(ladenLYBox.Text, out t_ladenLY))
                {
                    t_ladenLY = decimal.Truncate(t_ladenLY * 100) / 100;
                }
                else
                {
                    t_ladenLY = decimal.Truncate(ladenLYBox.Minimum * 100) / 100;
                }
            }

            if (decimal.TryParse(marginBox.Text, out decimal t_Margin))
            {
                settingsRef.Margin = decimal.Truncate(t_Margin * 100) / 100;
                marginBox.Text = settingsRef.Margin.ToString();
            }
            else
            {
                settingsRef.Margin = decimal.Truncate(marginBox.Minimum * 100) / 100;
                marginBox.Text = settingsRef.Margin.ToString();
            }
        }

        private void CopySystemToDest_Click(object sender, EventArgs e)
        {
            if (pilotsLogDataGrid.Rows.Count > 0 && !string.IsNullOrEmpty(pilotsLogDataGrid.Rows[dRowIndex].Cells[2].Value.ToString()))
            {
                // grab the system from the system field, if it exists, copy to the src box
                string dbSys = pilotsLogDataGrid.Rows[dRowIndex].Cells[2].Value.ToString();
                destSystemComboBox.Text = dbSys;
            }
        }

        private void CopySystemToSrc_Click(object sender, EventArgs e)
        {
            if (pilotsLogDataGrid.Rows.Count > 0 && !string.IsNullOrEmpty(pilotsLogDataGrid.Rows[dRowIndex].Cells[2].Value.ToString()))
            {
                // grab the system from the system field, if it exists, copy to the src box
                string dbSys = pilotsLogDataGrid.Rows[dRowIndex].Cells[2].Value.ToString();
                srcSystemComboBox.Text = dbSys;
            }
        }

        private void CutMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.Cut();
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;

            if (clickedControl.Name == notesTextBox.Name)
            {
                notesTextBox.SelectedText = string.Empty;
            }
        }

        private void DestSystemComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            string filteredString = RemoveExtraWhitespace(destSystemComboBox.Text);

            if ((e.KeyCode == Keys.Enter & e.Modifiers == Keys.Control)
                && StringInList(filteredString, outputSysStnNames))
            {
                // if ctrl+enter, is a known system/station, and not in our net log, mark it down
                AddMarkedStation(filteredString, currentMarkedStations);
                BuildOutput(true);
                SaveSettingsToIniFile();
                e.Handled = true;
            }
            else if ((e.KeyCode == Keys.Enter & e.Modifiers == Keys.Shift)
                && StringInList(filteredString, currentMarkedStations))
            {
                // if shift+enter, item is in our list, remove it
                RemoveMarkedStation(filteredString, currentMarkedStations);
                int index = IndexInList(filteredString, output_unclean);
                BuildOutput(true);
                SaveSettingsToIniFile();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape
                && !string.IsNullOrEmpty(destSystemComboBox.Text))
            {
                // wipe our box selectively if we hit escape
                //first wipe until the delimiter
                string[] tokens = destSystemComboBox.Text.Split(new string[] { "/" }, StringSplitOptions.None);

                if (tokens != null && tokens.Length == 2)
                {
                    // make sure we have a system/station
                    // delete the front of the string until the system
                    destSystemComboBox.Text = tokens[0];
                }
                else if (!destSystemComboBox.Text.Contains("/"))
                {
                    destSystemComboBox.Text = string.Empty;
                }

                e.Handled = true;
            }
        }

        private void DestSystemComboBox_TextChanged(object sender, EventArgs e)
        {
            // wait for the user to type a few characters
            if (destSystemComboBox.Text.Length > 3)
            {
                string filteredString = RemoveExtraWhitespace(destSystemComboBox.Text);

                ValidateDestForEndJumps();

                towardsCheckBox.Enabled = srcSystemComboBox.Text.Length > 3;
            }
            else
            {
                towardsCheckBox.Enabled = false;
            }
        }

        private void DirectCheckBox_Click(object sender, EventArgs e)
        {
            // an exception for the market command
            if (methodIndex == 4 && bmktCheckBox.Checked)
            {
                // we cannot have both buy and sell enabled
                bmktCheckBox.Checked = false;
                directCheckBox.Checked = true;
            }
        }

        private void DisableRunButtons()
        {
            // disable buttons during uncancellable operation
            updateButton.Enabled = false;
            btnCmdrProfile.Enabled = false;
            getSystemButton.Enabled = false;
            miniModeButton.Enabled = false;

            // an exception for Run commands
            if (buttonCaller != 1)
            {
                runButton.Enabled = false;
            }
        }

        private void EnableRunButtons()
        {
            // reenable other worker callers when done
            updateButton.Enabled = true;
            btnCmdrProfile.Enabled = true;
            getSystemButton.Enabled = true;

            // fix Run button when returning from non-Run commands
            if (buttonCaller == 1 || !runButton.Enabled)
            {
                runButton.Enabled = true;
            }
        }

        private void FaqLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/MarkAusten/TDHelper/wiki/Home");
        }

        private void FilterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.ResetFilterButton.Enabled
                = rearmFilterCheckBox.Checked
                || refuelFilterCheckBox.Checked
                || repairFilterCheckBox.Checked
                || itemsFilterCheckBox.Checked
                || bmktFilterCheckBox.Checked
                || outfitFilterCheckBox.Checked
                || shipyardFilterCheckBox.Checked
                || stationsFilterCheckBox.Checked;
        }

        private void ForceRefreshGridView_Click(object sender, EventArgs e)
        {
            InvalidatedRowUpdate(true, -1); // force an invalidate and reload
        }

        private void ForceResortMenuItem_Click(object sender, EventArgs e)
        {
            pilotsLogDataGrid.Columns["Timestamp"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            pilotsLogDataGrid.Columns["System"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            pilotsLogDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private int GetCheckBoxCheckState(CheckState checkState)
        {
            switch (checkState)
            {
                case CheckState.Unchecked:
                    return 0;

                case CheckState.Checked:
                    return 1;

                case CheckState.Indeterminate:
                    return 2;

                default:
                    throw new ArgumentException("Can't get the checkState from the checkBox: " + checkState.ToString());
            }
        }

        private void GetSystemButton_Click(object sender, EventArgs e)
        {
            buttonCaller = 2;

            if (Control.ModifierKeys == Keys.Control)
            {
                buttonCaller = 16; // mark us as needing a full refresh
            }

            ValidateSettings();
            DisableRunButtons();

            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void InsertAtGridRow_Click(object sender, EventArgs e)
        {
            if (pilotsLogDataGrid.Rows.Count > 0)
            {
                // add a row with the timestamp of the selected row
                // basically an insert-below-index when we use select(*)
                string timestamp = pilotsLogDataGrid.Rows[dRowIndex].Cells["Timestamp"].Value.ToString();
                AddAtTimestampDBRow(tdhDBConn, GenerateRecentTimestamp(timestamp));
                InvalidatedRowUpdate(true, -1);
            }
            else
            {
                // special case for an empty gridview
                AddAtTimestampDBRow(tdhDBConn, CurrentTimestamp());
                InvalidatedRowUpdate(true, -1);
            }
        }

        private void LocalNavBox_CheckedChanged(object sender, EventArgs e)
        {
            t_localNavEnabled = localNavCheckBox.Checked;

            if (localNavCheckBox.Checked)
            {
                // switching to Local override
                panRunOptions.Visible = true;
                panShipVendor.Visible = false;
                panLocalFilter.Visible = true;
                // force enable
                panLocalFilter.Enabled = true;
                destSystemComboBox.Enabled = true;
                panRunOptions.Enabled = true;
                methodDropDown.Enabled = false;
                padSizeBox.Enabled = true;
                padSizeLabel.Enabled = true;

                // pull to the front
                panLocalFilter.BringToFront();

                // disable most of the run options
                this.SetPanelEnabledState(panRunOptions);

                ladenLYLabel.Font = new Font(ladenLYLabel.Font, FontStyle.Italic);
                ladenLYLabel.Text = "  Near LY:";
                ladenLYLabel.Enabled = true;

                l0_ladenLY = ladenLYBox.Value; // save our last used ladenLY

                ladenLYBox.Value = l1_ladenLY > 0
                    ? l1_ladenLY // restore local ladenLY
                    : 1.00m; // default to 1.00 LY in local

                toolTip1.SetToolTip(ladenLYLabel, "Distance to search for local system/station info.");
                ladenLYBox.Enabled = true;
                localNavCheckBox.Enabled = true;
            }
            else
            {
                // we're unchecked
                l1_ladenLY = ladenLYBox.Value; // save our last used local ladenLY

                if (l0_ladenLY > 0)
                {
                    ladenLYBox.Value = l0_ladenLY; // restore last used ladenLY
                }

                if (methodIndex >= 4)
                {
                    // disable the padsize box when applicable
                    padSizeBox.Enabled = false;
                    padSizeLabel.Enabled = false;
                }

                buttonCaller = 18; // mark us as local override
                MethodSelectState(); // reset state
            }
        }

        private void LoopCheckBox_Click(object sender, EventArgs e)
        {
            if (shortenCheckBox.Checked)
            {
                shortenCheckBox.Checked = false;
                towardsCheckBox.Checked = false;
                loopCheckBox.Checked = true;
            }
            else if (settingsRef.Towards || towardsCheckBox.Checked)
            {
                settingsRef.Towards = false;
                towardsCheckBox.Checked = false;
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            isActive = true;

            FlashWindow.Stop(this); // stop flashing when we activate the window
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            isActive = false;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // forcefully shutdown the process handler first
            td_proc.Close();
            td_proc.Dispose();
            Application.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CheckIfFileOpens(configFileDefault))
            {
                // serialize window data to the default config file
                CopySettingsFromForm();
                settingsRef.LocationParent = SaveWinLoc(this);
                settingsRef.SizeParent = SaveWinSize(this);

                SaveSettingsToIniFile();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Screen screen = Screen.FromControl(this);
            Rectangle workingArea = screen.WorkingArea;
            int[] winLoc = LoadWinLoc(settingsRef.LocationParent);
            int[] winSize = LoadWinSize(settingsRef.SizeParent);

            // restore window size from config
            if (winSize.Length != 0 && winSize != null)
            {
                this.Size = new Size(winSize[0], winSize[1]);
            }
            else
            {
                // save our default size to the config
                settingsRef.SizeParent = SaveWinSize(this);
            }

            // try to remember and restore the window location
            if (winLoc.Length != 0 && winLoc != null)
            {
                this.Location = new Point(winLoc[0], winLoc[1]);
                // if we're restoring the location, let's force it to be visible on screen
                MainForm.ForceFormOnScreen(this);
            }
            else
            {
                this.Location = new Point()
                {
                    X = Math.Max(workingArea.X, workingArea.X + (workingArea.Width - this.Width) / 2),
                    Y = Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - this.Height) / 2)
                };
            }

            // bind our alternate config files
            SetShipList(true);

            if (!CheckIfFileOpens(configFile))
            {
                SaveSettingsToIniFile();
            }

            if (settingsRef.HasUpdated)
            {
                // display the changelog for the user
                settingsRef.HasUpdated = false; // we've updated
                SaveSettingsToIniFile();
                DoHotSwapCleanup(); // call cleanup to remove unnecessary files if they exist

                // show the user the changelog after an update
                if (File.Exists(Path.Combine(assemblyPath, "Changelog.txt")))
                {
                    ChangeLogForm changelogForm = new ChangeLogForm();
                    changelogForm.ShowDialog(this); // modal
                    changelogForm.Dispose();
                }
            }
            else
            {
                backgroundWorker5.RunWorkerAsync(); // start the auto-updater delegate
            }

            // load our last saved config
            if (!string.IsNullOrEmpty(settingsRef.LastUsedConfig)
                && settingsRef.LastUsedConfig != configFile
                && ValidateConfigFile(settingsRef.LastUsedConfig))
            {
                LoadSettings(settingsRef.LastUsedConfig);
            }
        }

        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            Rectangle workingArea = Screen.FromControl(this).WorkingArea;

            if (Math.Abs(workingArea.Left - this.Left) < SnapDist)
            {
                this.Left = workingArea.Left;
            }
            else if (Math.Abs(this.Left + this.Width - workingArea.Left - workingArea.Width) < SnapDist)
            {
                this.Left = workingArea.Left + workingArea.Width - this.Width;
            }

            if (Math.Abs(workingArea.Top - this.Top) < SnapDist)
            {
                this.Top = workingArea.Top;
            }
            else if (Math.Abs(this.Top + this.Height - workingArea.Top - workingArea.Height) < SnapDist)
            {
                this.Top = workingArea.Top + workingArea.Height - this.Height;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // start the database uploader
            backgroundWorker6.RunWorkerAsync();
        }

        private void MethodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            methodIndex = methodDropDown.SelectedIndex;
            MethodSelectState();

            methodDropDown.Enabled = true;
            localNavCheckBox.Enabled = true;
        }

        private void MethodSelectState()
        {
            /*
             * We do our command state stuff here
             */

            // make an exception for from/to local override
            if (buttonCaller == 18)
            {
                methodFromIndex = -1; // reset our previous index
            }

            // don't do anything if we're reselecting the same mode from the box
            if (methodFromIndex != methodIndex)
            {
                // handle index changes
                if (methodIndex == 1 || methodIndex == 2)
                {
                    // buy/sell command
                    StationEditorChangeState();

                    // show the secondary selection box
                    stationDropDown.Visible = true;
                    stationDropDown.Enabled = true;

                    toolTip1.SetToolTip(stationDropDown, "Sort results by context");

                    if (methodIndex == 1)
                    {
                        toolTip1.SetToolTip(methodDropDown, "Check nearby for stations selling items");
                        List<string> shipVendorDrop = new List<string>(new string[] { "Distance", "Price", "Units" });
                        stationDropDown.DataSource = shipVendorDrop;

                        stockLabel.Enabled = true;
                        stockBox.Enabled = true;
                        oneStopCheckBox.Enabled = true;
                    }
                    else if (methodIndex == 2)
                    {
                        toolTip1.SetToolTip(methodDropDown, "Check nearby for stations buying items");
                        List<string> shipVendorDrop = new List<string>(new string[] { "Distance", "Price" });
                        stationDropDown.DataSource = shipVendorDrop;

                        demandLabel.Enabled = true;
                        demandBox.Enabled = true;
                    }

                    // only reenable the appropriate options
                    ladenLYBox.Enabled = true;
                    ladenLYLabel.Enabled = true;

                    // prepare the commodity box
                    commodityComboBox.Enabled = true;

                    // point the user to the proper labels
                    ladenLYLabel.Font = new Font(ladenLYLabel.Font, FontStyle.Italic);
                    ladenLYLabel.Text = "  Near LY:";
                    toolTip1.SetToolTip(ladenLYBox, "Distance to search near the source system");
                    toolTip1.SetToolTip(ladenLYLabel, "Distance to search near the source system");

                    // enable the appropriate controls
                    bmktCheckBox.Enabled = true;
                    avoidBox.Enabled = true;
                    avoidLabel.Enabled = true;
                    commodityLabel.Enabled = true;
                    commodityComboBox.Enabled = true;
                    abovePriceBox.Enabled = true;
                    abovePriceLabel.Enabled = true;
                    belowPriceBox.Enabled = true;
                    belowPriceLabel.Enabled = true;

                    // use the correct index
                    if (methodIndex == 1)
                    {
                        methodFromIndex = 1;
                    }
                    else if (methodIndex == 2)
                    {
                        methodFromIndex = 2;
                    }
                }
                else if (methodIndex == 3)
                {
                    // rares command
                    StationEditorChangeState(); // more minimal default state

                    // show the secondary selection box
                    stationDropDown.Visible = true;
                    stationDropDown.Enabled = true;

                    List<string> shipVendorDrop = new List<string>(new string[] { string.Empty, "Legal", "Illegal" });
                    stationDropDown.DataSource = shipVendorDrop;

                    // point the user to the proper labels
                    ladenLYLabel.Font = new Font(ladenLYLabel.Font, FontStyle.Italic);
                    ladenLYLabel.Text = "  Near LY:";
                    unladenLYLabel.Font = new Font(unladenLYLabel.Font, FontStyle.Italic);
                    unladenLYLabel.Text = "    Away LY:";
                    avoidLabel.Font = new Font(avoidLabel.Font, FontStyle.Italic);
                    avoidLabel.Text = " From:";
                    avoidBox.TabStop = true;

                    // only reenable the appropriate options
                    ladenLYBox.Enabled = true;
                    ladenLYLabel.Enabled = true;
                    unladenLYBox.Enabled = true;
                    unladenLYLabel.Enabled = true;
                    avoidLabel.Enabled = true;
                    avoidBox.Enabled = true;

                    // fix tooltips
                    toolTip1.SetToolTip(stationDropDown, "Filter rares by context");
                    toolTip1.SetToolTip(methodDropDown, "List all rares in the vicinity of a system");
                    toolTip1.SetToolTip(avoidLabel, "Calculate LY \"Away\" from these systems, delimited by comma");
                    toolTip1.SetToolTip(ladenLYBox, "Distance to search near the source system");
                    toolTip1.SetToolTip(ladenLYLabel, "Distance to search near the source system");
                    toolTip1.SetToolTip(unladenLYLabel, "Distance to calculate away from \"From\" systems");
                    toolTip1.SetToolTip(unladenLYBox, "Distance to calculate away from \"From\" systems");

                    methodFromIndex = 3; // mark rare
                }
                else if (methodIndex == 4)
                {
                    // trade command
                    RunMethodResetState();
                    methodFromIndex = 4;
                }
                else if (methodIndex == 5)
                {
                    // market command
                    StationEditorChangeState();

                    // change the bmkt/direct checkboxes
                    bmktCheckBox.Text = "Buy";
                    directCheckBox.Text = "Sell";
                    bmktCheckBox.Checked = false;
                    bmktCheckBox.Enabled = true;
                    directCheckBox.Checked = false;
                    directCheckBox.Enabled = true;
                    localNavCheckBox.Enabled = true;

                    // fix the tooltips
                    toolTip1.SetToolTip(methodDropDown, "List the price/stock index for commodities at a station");
                    toolTip1.SetToolTip(bmktCheckBox, "Show buying column at the source market");
                    toolTip1.SetToolTip(directCheckBox, "Show selling column at the source market");

                    methodFromIndex = 5;
                }
                else if (methodIndex == 6)
                {
                    // shipvendor command
                    StationEditorChangeState();

                    // show the secondary selection box
                    stationDropDown.Visible = true;
                    // set "add" as the default
                    List<string> shipVendorDrop = new List<string>(new string[] { "Add", "Remove", "List" });
                    stationDropDown.DataSource = shipVendorDrop;

                    // hide the run panel
                    panRunOptions.Visible = false;
                    // activate the station panel
                    panShipVendor.Visible = true;

                    // fix tooltips
                    toolTip1.SetToolTip(methodDropDown, "Add/Remove/List ships being sold at a given station");

                    methodFromIndex = 6;
                }
                else if (methodIndex == 7)
                {
                    // nav command
                    // reset our state to the default for this method
                    StationEditorChangeState();

                    // fix the tooltips
                    toolTip1.SetToolTip(methodDropDown, "Attempt to calculate navigation between source/destination systems");
                    toolTip1.SetToolTip(avoidBox, "Exclude a system from the route; if a station is entered the system it's in will be avoided");
                    toolTip1.SetToolTip(avoidLabel, "Exclude a system from the route; if a station is entered the system it's in will be avoided");
                    toolTip1.SetToolTip(ladenLYBox, "Distance to search near the source system");
                    toolTip1.SetToolTip(ladenLYLabel, "Distance to search near the source system");

                    avoidBox.TabStop = true;
                    viaBox.TabStop = true;

                    // only reenable the appropriate options
                    destSysLabel.Enabled = true;
                    destSystemComboBox.Enabled = true;
                    ladenLYBox.Enabled = true;
                    ladenLYLabel.Enabled = true;
                    avoidLabel.Enabled = true;
                    avoidBox.Enabled = true;
                    viaLabel.Enabled = true;
                    viaBox.Enabled = true;

                    methodFromIndex = 7; // mark nav
                }
                else if (methodIndex == 8)
                {
                    // olddata command
                    StationEditorChangeState();

                    // point the user to the proper labels
                    ladenLYLabel.Font = new Font(ladenLYLabel.Font, FontStyle.Italic);
                    ladenLYLabel.Text = "  Near LY:";

                    // only reenable the appropriate options
                    ladenLYBox.Enabled = true;
                    ladenLYLabel.Enabled = true;

                    // fix tooltips
                    toolTip1.SetToolTip(methodDropDown, "Show oldest station data near a system in the DB");
                    toolTip1.SetToolTip(ladenLYBox, "Distance to search near the source system");
                    toolTip1.SetToolTip(ladenLYLabel, "Distance to search near the source system");

                    methodFromIndex = 8; // mark olddata
                }
                else
                {
                    // run command
                    RunMethodResetState();
                    toolTip1.SetToolTip(methodDropDown, "Calculates optimal trading routes from Source (Destination optional)");
                    methodFromIndex = 0;
                }
            }
        }

        private void MiniModeButton_Click(object sender, EventArgs e)
        {
            TdMiniForm childForm = new TdMiniForm(this); // pass a reference from parentForm

            // populate the treeview from the last valid run output
            ParseRunOutput(tv_outputBox, childForm.TreeViewBox);
            childForm.Text = t_childTitle; // set our profit estimate

            // show our minimode
            this.Hide();
            childForm.ShowDialog(); // always modal, never instance

            // save some globals
            SaveSettingsToIniFile();
            this.Show(); // restore when we return
        }

        private void MiscSettingsButton_Click(object sender, EventArgs e)
        {
            bool oldValue = settingsRef.DisableNetLogs;

            SettingsForm SettingsForm = new SettingsForm()
            {
                StartPosition = FormStartPosition.CenterParent
            };

            SettingsForm.ShowDialog(this);

            // Chec to see if the user has unchecked the disable net logs setting
            if (settingsRef.DisableNetLogs != oldValue && oldValue)
            {
                // Check the verbose logging setting.
                ValidateVerboseLogging();
            }

            if (callForReset)
            {
                // wipe our settings and reinitialize
                File.Delete(configFile);
                settingsRef.Reset(settingsRef);
                ValidateSettings();
                SaveSettingsToIniFile();
                CopySettingsFromConfig();
            }
        }

        private void NotesClearMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(notesTextBox.Text))
            {
                notesTextBox.Text = string.Empty;

                if (File.Exists(notesFile))
                {
                    File.Delete(notesFile);
                }
            }
        }

        private void NumericUpDown_Enter(object sender, EventArgs e)
        {
            // fix for text selection upon focusing numericupdown controls
            (sender as NumericUpDown).Select(0, (sender as NumericUpDown).Text.Length);
        }

        private void NumericUpDown_MouseUp(object sender, MouseEventArgs e)
        {
            (sender as NumericUpDown).Select(0, (sender as NumericUpDown).Text.Length);
        }

        private void PadSizeBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // filter for valid chars
            if (e.KeyChar == 'm'
                || e.KeyChar == 'M'
                || e.KeyChar == 'l'
                || e.KeyChar == 'L'
                || e.KeyChar == '?')
            {
                if (padSizeBox.TextLength < 3)
                {
                    if (!padSizeBox.Text.Contains(e.KeyChar.ToString().ToUpper()))
                    {
                        padSizeBox.Text += e.KeyChar.ToString().ToUpper();
                    }
                    else if (padSizeBox.Text.Contains(e.KeyChar.ToString().ToUpper()))
                    {
                        padSizeBox.Text = e.KeyChar.ToString().ToUpper();
                    }
                }
                else
                {
                    padSizeBox.Text = e.KeyChar.ToString().ToUpper();
                }

                e.Handled = true;
            }
            else
            {
                padSizeBox.Text += string.Empty;
                e.Handled = true;
            }
        }

        private CheckState ParseCheckState(string input)
        {
            CheckState state;

            switch (input.ToUpperInvariant())
            {
                case "Y":
                    state = CheckState.Checked;
                    break;

                case "N":
                    state = CheckState.Unchecked;
                    break;

                default:
                    state = CheckState.Indeterminate;
                    break;
            }

            return state;
        }

        private void PasteMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.Paste();
        }

        private void PilotsLogDataGrid_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            // prevent OOR exception
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
            {
                return;
            }

            if (sender != null)
            {
                // save the datasource index, and the datagrid index of the row
                pilotsLogDataGrid.ClearSelection(); // prevent weirdness
                pRowIndex = int.Parse(localTable.Rows[e.RowIndex][0].ToString());
                dRowIndex = e.RowIndex;
                pilotsLogDataGrid.Rows[dRowIndex].Selected = true;
            }
        }

        private void PilotsLogDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < retriever.RowCount && e.ColumnIndex < retriever.RowCount)
            {
                e.Value = memoryCache.RetrieveElement(e.RowIndex, e.ColumnIndex);
            }
        }

        private void PilotsLogDataGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < retriever.RowCount && e.ColumnIndex < retriever.RowCount)
            {
                // update our local table
                localTable.Rows[e.RowIndex][e.ColumnIndex] = e.Value;
                List<DataRow> row = new List<DataRow> { localTable.Rows[e.RowIndex] };

                // update the physical DB and repaint
                UpdateDBRow(tdhDBConn, row);
                InvalidatedRowUpdate(false, e.RowIndex);
            }
        }

        private void PilotsLogDataGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Index < retriever.RowCount && e.Row.Index >= 0
                && pilotsLogDataGrid.SelectedRows.Count > 0)
            {
                int rowIndex = -1;
                int.TryParse(pilotsLogDataGrid.Rows[e.Row.Index].Cells[0].Value.ToString(), out rowIndex);

                if (rowIndex >= 0)
                {
                    if (pilotsLogDataGrid.SelectedRows.Count == 1)
                    {
                        RemoveDBRow(tdhDBConn, rowIndex);
                        UpdateLocalTable(tdhDBConn);
                        memoryCache.RemoveRow(e.Row.Index, rowIndex);
                    }
                    else if (pilotsLogDataGrid.SelectedRows.Count > 1 && dgRowIDIndexer.Count == 0)
                    {
                        // let's batch the commits for performance
                        batchedRowCount = pilotsLogDataGrid.SelectedRows.Count;
                        foreach (DataGridViewRow r in pilotsLogDataGrid.SelectedRows)
                        {
                            int curRowIndex = int.Parse(r.Cells[0].Value.ToString());
                            dgRowIndexer.Add(e.Row.Index);
                            dgRowIDIndexer.Add(curRowIndex);
                        }

                        pilotsLogDataGrid.Visible = false; // prevent retrieval
                    }

                    if (batchedRowCount != -1)
                    {
                        batchedRowCount--; // keep track of our re-entry
                    }

                    if (dgRowIDIndexer.Count > 0 && batchedRowCount == 0)
                    {
                        // we've got queued commits to remove (should trigger on the last removed row)
                        RemoveDBRows(tdhDBConn, dgRowIDIndexer);
                        UpdateLocalTable(tdhDBConn);
                        memoryCache.RemoveRows(dgRowIndexer, dgRowIDIndexer);
                        pilotsLogDataGrid.Visible = true; // re-enable retrieval
                    }
                }
            }
        }

        private void ProcErrorDataHandler(object sender, DataReceivedEventArgs output)
        {
            if (output.Data != null)
            {
                StackCircularBuffer(output.Data + "\n");
            }
        }

        private void ProcOutputDataHandler(object sender, DataReceivedEventArgs output)
        {
            string[] exceptions = new string[] { "NOTE:", "####" };
            string filteredOutput = string.Empty;

            if (output.Data != null)
            {
                // prevent a null reference
                if (output.Data.Contains("\a"))
                {
                    filteredOutput = output.Data.Replace("\a", string.Empty) + "\n";
                }
                else if (output.Data.StartsWith("["))
                {
                    filteredOutput = output.Data + "\r";
                }
                else
                {
                    filteredOutput = output.Data + "\n";
                }

                if (buttonCaller != 5 && buttonCaller != 12 && !exceptions.Any(output.Data.Contains) && !t_csvExportCheckBox || methodIndex != 5 || methodIndex != 6)
                {
                    // hide output if calculating
                    StackCircularBuffer(filteredOutput);
                }
                else if (t_csvExportCheckBox || methodIndex == 5 || buttonCaller == 5 || buttonCaller == 12)
                {
                    // don't hide any output if updating DB or exporting
                    StackCircularBuffer(filteredOutput);
                }
            }
        }

        private void PushNotesMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;
            clickedControl.Focus();

            if (clickedControl.SelectedText.Length > 0)
            {
                Clipboard.SetData(DataFormats.Text, clickedControl.SelectedText);
            }
            else
            {
                clickedControl.SelectAll();
                Clipboard.SetData(DataFormats.Text, clickedControl.SelectedText);
            }

            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                using (FileStream fs = new FileStream(notesFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter stream = new StreamWriter(fs))
                {
                    if (methodIndex == 1)
                    {
                        stream.WriteLine("Buy " + commodityComboBox.Text.ToString()
                            + " (near "
                            + srcSystemComboBox.Text.ToString()
                            + "):\n"
                            + Clipboard.GetData(DataFormats.Text).ToString());
                    }
                    else if (methodIndex == 2)
                    {
                        stream.WriteLine("Sell " + commodityComboBox.Text.ToString()
                            + " (near "
                            + srcSystemComboBox.Text.ToString()
                            + "):\n"
                            + Clipboard.GetData(DataFormats.Text).ToString());
                    }
                    else
                    {
                        stream.WriteLine(Clipboard.GetData(DataFormats.Text).ToString());
                    }
                }
            }
        }

        private void RemoveAtGridRow_Click(object sender, EventArgs e)
        {
            if (pilotsLogDataGrid.Rows.Count > 0)
            {
                RemoveDBRow(tdhDBConn, pRowIndex);
                UpdateLocalTable(tdhDBConn);
                memoryCache.RemoveRow(dRowIndex, pRowIndex);
                pilotsLogDataGrid.InvalidateRow(dRowIndex);
            }
        }

        private void ResetFilterButton_Click(object sender, EventArgs e)
        {
            rearmFilterCheckBox.Checked = false;
            refuelFilterCheckBox.Checked = false;
            repairFilterCheckBox.Checked = false;
            itemsFilterCheckBox.Checked = false;
            bmktFilterCheckBox.Checked = false;
            outfitFilterCheckBox.Checked = false;
            shipyardFilterCheckBox.Checked = false;
            stationsFilterCheckBox.Checked = false;
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            // mark as run button
            buttonCaller = 1;

            DoRunEvent(); // externalized
        }

        private void RunMethodResetState()
        {
            // we should use this method as an elevator for the form state
            panRunOptions.Visible = true;
            panShipVendor.Visible = false;
            stationDropDown.Visible = false;
            panLocalFilter.Visible = false;

            srcSystemComboBox.Enabled = true;
            panRunOptions.Enabled = true;
            panShipVendor.Enabled = false;

            // make an exception for the padsizebox when applicable
            if (methodIndex <= 3 && methodIndex >= 0)
            {
                padSizeBox.Enabled = true;
                padSizeLabel.Enabled = true;
            }
            else
            {
                padSizeBox.Enabled = false;
                padSizeLabel.Enabled = false;
            }

            // save some previous values before overwriting
            if (methodFromIndex == 0 && belowPriceBox.Value > 0)
            {
                // save our routes value
                t_Routes = belowPriceBox.Value;
            }
            else if (methodFromIndex == 1)
            { // from buy
                t1_ladenLY = ladenLYBox.Value;

                // save our previous supply
                if (stockBox.Value > 0)
                {
                    t_Supply = stockBox.Value;
                }

                // save below price
                if (belowPriceBox.Value > 0)
                {
                    t_belowPrice = belowPriceBox.Value;
                }
            }
            else if (methodFromIndex == 2)
            { // from sell
                t2_ladenLY = ladenLYBox.Value;

                // save our previous demand
                if (demandBox.Value > 0)
                {
                    t_Demand = demandBox.Value;
                }

                // save below price
                if (belowPriceBox.Value > 0)
                {
                    t_belowPrice = belowPriceBox.Value;
                }
            }
            else if (methodFromIndex == 3)
            {
                // save our ladenLY
                r_ladenLY = ladenLYBox.Value; // from rares
            }
            else if (hasRun)
            {
                // save our volatile settings when switching
                if (ladenLYBox.Value > 0 && settingsRef.LadenLY != ladenLYBox.Value)
                {
                    settingsRef.LadenLY = ladenLYBox.Value;
                }

                if (stockBox.Value > 0 && settingsRef.Stock != stockBox.Value)
                {
                    settingsRef.Stock = stockBox.Value;
                }

                if (demandBox.Value > 0 && settingsRef.Demand != demandBox.Value)
                {
                    settingsRef.Demand = demandBox.Value;
                }
            }

            // main state check
            if (methodIndex == 0)
            {
                // return to a normal run state
                this.SetPanelEnabledState(panRunOptions, true);

                belowPriceBox.Enabled = true;
                belowPriceLabel.Enabled = true;
                toolTip1.SetToolTip(belowPriceBox, "Generates this many routes for a Run");
                toolTip1.SetToolTip(belowPriceLabel, "Generates this many routes for a Run");
                belowPriceLabel.Text = "Routes:";

                // reset the min/max values for sanity
                belowPriceBox.Value = 0;
                belowPriceBox.Minimum = 0;
                belowPriceBox.Maximum = 10;

                // restore our last used values
                if (t_Routes > 0)
                {
                    belowPriceBox.Value = t_Routes;
                }

                stockBox.Value = settingsRef.Stock > 0
                    ? settingsRef.Stock
                    : stockBox.Minimum;

                demandBox.Value = settingsRef.Demand > 0
                    ? settingsRef.Demand
                    : demandBox.Minimum;

                oneStopCheckBox.Enabled = false;

                towardsCheckBox.Enabled = srcSystemComboBox.Text.Length > 3 && destSystemComboBox.Text.Length > 3;
            }
            else if (methodIndex == 1 || methodIndex == 2)
            {
                // going into buy/sell
                if (methodIndex == 1)
                {
                    oneStopCheckBox.Enabled = true;
                }
                else
                {
                    oneStopCheckBox.Enabled = false;
                    oneStopCheckBox.Checked = false;
                }

                toolTip1.SetToolTip(belowPriceBox, "Commodities below this price are filtered out");
                toolTip1.SetToolTip(belowPriceLabel, "Commodities below this price are filtered out");
                belowPriceLabel.Text = "  Below:";

                belowPriceBox.Value = 0;
                belowPriceBox.Minimum = 0;
                belowPriceBox.Maximum = 50000;

                // restore our last used values
                if (t_belowPrice > 0)
                {
                    belowPriceBox.Value = t_belowPrice;
                }

                if (methodIndex == 1 && t_Supply > 0)
                {
                    stockBox.Value = t_Supply;
                }
                else if (methodIndex == 1)
                {
                    stockBox.Value = stockBox.Minimum;
                }
                else if (methodIndex == 2 && t_Demand > 0)
                {
                    demandBox.Value = t_Demand;
                }
                else if (methodIndex == 2)
                {
                    demandBox.Value = demandBox.Minimum;
                }
            }
            else if (methodFromIndex != 5)
            {
                // catch everything else (except shipvendor)
                this.SetPanelEnabledState(panRunOptions);
            }

            // change the bmkt/direct checkboxes
            bmktCheckBox.Text = "BMkt";
            directCheckBox.Text = "Direct";
            bmktCheckBox.Checked = false;
            directCheckBox.Checked = false;
            methodDropDown.Enabled = true;
            avoidBox.TabStop = false;
            viaBox.TabStop = false;

            minAgeUpDown.Visible = false;
            minAgeLabel.Visible = false;
            oldRoutesCheckBox.Enabled = false;
            oldRoutesCheckBox.Visible = false;

            // always focus the Source box
            srcSystemComboBox.Focus();

            // then change the distance back to the previous value
            if (methodIndex == 1 && t1_ladenLY > 0)
            {
                ladenLYBox.Value = t1_ladenLY;
            }
            else if (methodIndex == 2 && t2_ladenLY > 0)
            {
                ladenLYBox.Value = t2_ladenLY;
            }
            else if (methodIndex == 3 && r_ladenLY > 0)
            {
                ladenLYBox.Value = r_ladenLY;
            }
            else if (methodIndex == 3 && r_ladenLY == 0)
            {
                ladenLYBox.Value = 120.00m;
            }
            else
            {
                ladenLYBox.Value = settingsRef.LadenLY;
            }

            // and tooltips
            toolTip1.SetToolTip(avoidLabel, "Avoids can include system/station and items delimited by comma");
            toolTip1.SetToolTip(avoidBox, "Avoids can include system/station and items delimited by comma");
            toolTip1.SetToolTip(ladenLYLabel, "Distance that can be travelled while laden (including fuel)");
            toolTip1.SetToolTip(ladenLYBox, "Distance that can be travelled while laden (including fuel)");
            toolTip1.SetToolTip(unladenLYLabel, "Distance that can be travelled while unladen (including fuel)");
            toolTip1.SetToolTip(unladenLYBox, "Distance that can be travelled while unladen (including fuel)");
            toolTip1.SetToolTip(bmktCheckBox, "Require stations with a black market (volatile)");
            toolTip1.SetToolTip(directCheckBox, "Require that stations on a route only be visited once");
            toolTip1.SetToolTip(srcSystemComboBox, "Starting point in the form of system or system/station" + Environment.NewLine + "Ctrl+Enter adds a System/Station to the favorites" + Environment.NewLine + "Shift+Enter removes a System/Station from the favorites");
            toolTip1.SetToolTip(destSystemComboBox, "Destination point in the form of system or system/station" + Environment.NewLine + "Ctrl+Enter adds a System/Station to the favorites" + Environment.NewLine + "Shift+Enter removes a System/Station from the favorites");

            // labels
            ladenLYLabel.Font = new Font(ladenLYLabel.Font, FontStyle.Regular);
            ladenLYLabel.Text = "Laden LY:";
            unladenLYLabel.Font = new Font(unladenLYLabel.Font, FontStyle.Regular);
            unladenLYLabel.Text = "Unladen LY:";
            avoidLabel.Font = new Font(avoidLabel.Font, FontStyle.Regular);
            avoidLabel.Text = "Avoid:";

            // controls
            ageBox.Enabled = true;
            ageLabel.Enabled = true;
            commodityComboBox.Enabled = false;
            commodityLabel.Enabled = false;
            commodityLabel.Enabled = false;
            abovePriceBox.Enabled = false;
            abovePriceLabel.Enabled = false;
            localNavCheckBox.Checked = false;

            // an exception for switching from another command with a valid dest
            endJumpsBox.Enabled = methodFromIndex != 0 && !string.IsNullOrEmpty(destSystemComboBox.Text);

            // an exception for the trade command
            if (methodIndex == 4)
            {
                this.SetPanelEnabledState(panRunOptions);

                destSystemComboBox.Enabled = true;
                destSysLabel.Enabled = true;
                ageBox.Enabled = false;
                ageLabel.Enabled = false;
            }

            // fix ladenLY minimum for certain commands
            if (methodIndex == 8)
            {
                ladenLYBox.Minimum = 0; // just for the olddata command
            }
            else
            {
                // default [requirement!]
                if (ladenLYBox.Value < 1)
                {
                    ladenLYBox.Value = 1;
                }

                ladenLYBox.Minimum = 1;
            }

            // (re)store the Avoid/From box
            if (methodFromIndex != 3 && methodIndex == 3)
            {
                // from anywhere (but Rare) to Rare
                settingsRef.Avoid = avoidBox.Text; // save Avoid list

                // restore the contents if it exists
                avoidBox.Text = !string.IsNullOrEmpty(r_fromBox)
                    ? r_fromBox
                    : string.Empty;
            }
            else if (methodFromIndex == 3)
            {
                // coming from Rare switching to anywhere
                r_fromBox = avoidBox.Text; // save From list

                // restore our contents from global
                avoidBox.Text = !string.IsNullOrEmpty(settingsRef.Avoid)
                    ? settingsRef.Avoid
                    : string.Empty;
            }
        }

        private void SavePage1MenuItem_Click(object sender, EventArgs e)
        {
            if (td_outputBox.SelectedText.Length > 0)
            {
                WriteSavedPage(td_outputBox.SelectedText, savedFile1);
            }
            else
            {
                WriteSavedPage(td_outputBox.Text, savedFile1);
            }
        }

        private void SavePage2MenuItem_Click(object sender, EventArgs e)
        {
            if (td_outputBox.SelectedText.Length > 0)
            {
                WriteSavedPage(td_outputBox.SelectedText, savedFile2);
            }
            else
            {
                WriteSavedPage(td_outputBox.Text, savedFile2);
            }
        }

        private void SavePage3MenuItem_Click(object sender, EventArgs e)
        {
            if (td_outputBox.SelectedText.Length > 0)
            {
                WriteSavedPage(td_outputBox.SelectedText, savedFile3);
            }
            else
            {
                WriteSavedPage(td_outputBox.Text, savedFile3);
            }
        }

        private void SelectMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.SelectAll();
        }

        /// <summary>
        /// Set the enabled state of the controls in the specified panel.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <param name="state">The state.</param>
        private void SetPanelEnabledState(
            Panel panel,
            bool state = false)
        {
            foreach (Control ctrl in panel.Controls)
            {
                ctrl.Enabled = state;
            }
        }

        private void ShipsSoldBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                shipsSoldBox.SelectionLength = 0;
            }
        }

        private void ShortenCheckBox_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(destSystemComboBox.Text))
            {
                if (!shortenCheckBox.Checked)
                {
                    loopCheckBox.Checked = false;
                    towardsCheckBox.Checked = false;
                }
                else if (shortenCheckBox.Checked)
                {
                    if (loopCheckBox.Checked)
                    {
                        loopCheckBox.Checked = false;
                    }

                    shortenCheckBox.Checked = true;
                    towardsCheckBox.Checked = false;
                }
            }
            else
            {
                shortenCheckBox.Checked = false;
            }
        }

        private void SrcSystemComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (methodIndex != 10)
            {
                // make sure we filter unwanted characters from the string
                string filteredString = RemoveExtraWhitespace(srcSystemComboBox.Text);

                if ((e.KeyCode == Keys.Enter & e.Modifiers == Keys.Control)
                    && StringInList(filteredString, outputSysStnNames))
                {
                    // if ctrl+enter, is a known system/station, and not in our net log, mark it down
                    AddMarkedStation(filteredString, currentMarkedStations);
                    BuildOutput(true);
                    SaveSettingsToIniFile();
                    e.Handled = true;
                }
                else if ((e.KeyCode == Keys.Enter & e.Modifiers == Keys.Shift)
                    && StringInList(filteredString, currentMarkedStations))
                {
                    // if shift+enter, item is in our list, remove it
                    RemoveMarkedStation(filteredString, currentMarkedStations);
                    int index = IndexInList(filteredString, output_unclean);
                    BuildOutput(true);
                    SaveSettingsToIniFile();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape
                    && !string.IsNullOrEmpty(filteredString))
                {
                    // wipe our box selectively if we hit escape
                    //first wipe until the delimiter
                    string[] tokens = filteredString.Split(new string[] { "/" }, StringSplitOptions.None);

                    if (tokens != null && tokens.Length == 2)
                    {
                        // make sure we have a system/station
                        // delete the front of the string until the system
                        srcSystemComboBox.Text = tokens[0];
                    }
                    else if (!filteredString.Contains("/"))
                    {
                        srcSystemComboBox.Text = string.Empty; // wipe entirely if only a system is left
                    }

                    e.Handled = true;
                }
            }
        }

        private void SrcSystemComboBox_TextChanged(object sender, EventArgs e)
        {
            // wait for the user to type a few characters
            if (srcSystemComboBox.Text.Length > 3 && methodIndex != 10)
            {
                string filteredString = RemoveExtraWhitespace(srcSystemComboBox.Text);
                PopulateStationPanel(filteredString);

                towardsCheckBox.Enabled = destSystemComboBox.Text.Length > 3; // requires "--fr"
            }
            else
            {
                towardsCheckBox.Enabled = false;
            }
        }

        private void StationDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            stationIndex = stationDropDown.SelectedIndex;

            if (methodIndex == 6)
            {
                // if we're in ShipVendor mode
                if (stationIndex == 2)
                {
                    // disable when list'ing
                    shipsSoldBox.Enabled = false;
                    shipsSoldLabel.Enabled = false;
                }
                else
                {
                    shipsSoldBox.Enabled = true;
                    shipsSoldLabel.Enabled = true;
                }
            }
        }

        private void StationEditorChangeState()
        {
            // we should use this method to move to station editor form state
            RunMethodResetState(); // start from a working base

            // exclusions
            if (methodIndex != 0)
            {
                ageBox.Enabled = false;
                ageLabel.Enabled = false;
            }

            // catch station/shipvendor here
            if (methodIndex == 6)
            { // shipvendor
                panRunOptions.Enabled = false;
                panShipVendor.Enabled = true;

                panRunOptions.Visible = false;
                panShipVendor.Visible = true;

                // we don't need most panShipVendor controls
                this.SetPanelEnabledState(panShipVendor);

                shipsSoldLabel.Enabled = true;
                shipsSoldBox.Enabled = true;
                stationDropDown.Enabled = true;
                toolTip1.SetToolTip(stationDropDown, "Select a mode for station editing");
            }
            else if (methodIndex == 8)
            {
                panRunOptions.Enabled = false;
                panShipVendor.Enabled = true;

                // we don't need most panRunOptions controls
                this.SetPanelEnabledState(panShipVendor);

                panRunOptions.Visible = false;
                panShipVendor.Visible = true;
                panShipVendor.BringToFront();

                ageBox.Enabled = false;
                ageLabel.Enabled = false;

                minAgeLabel.Visible = true;
                minAgeUpDown.Visible = true;

                minAgeLabel.Enabled = true;
                minAgeUpDown.Enabled = true;

                oldRoutesCheckBox.Enabled = true;
                oldRoutesCheckBox.Visible = true;
            }
            else
            {
                // we don't need most panRunOptions or run options controls
                this.SetPanelEnabledState(panRunOptions);
            }
        }

        private void SwapButton_Click(object sender, EventArgs e)
        {
            // here we swap the contents of the boxes with some conditions
            if (!panLocalFilter.Visible && destSystemComboBox.Visible)
            {
                // don't swap if the destination box isn't visible (or covered)
                if (!string.IsNullOrEmpty(srcSystemComboBox.Text)
                    && !string.IsNullOrEmpty(destSystemComboBox.Text))
                {
                    string temp = RemoveExtraWhitespace(destSystemComboBox.Text);
                    destSystemComboBox.Text = RemoveExtraWhitespace(srcSystemComboBox.Text);
                    srcSystemComboBox.Text = temp;
                }
                else if (!string.IsNullOrEmpty(srcSystemComboBox.Text)
                    && string.IsNullOrEmpty(destSystemComboBox.Text))
                {
                    // swap from source to destination
                    string temp = RemoveExtraWhitespace(srcSystemComboBox.Text);
                    destSystemComboBox.Text = temp;
                    srcSystemComboBox.Text = string.Empty;
                }
                else if (string.IsNullOrEmpty(srcSystemComboBox.Text)
                    && !string.IsNullOrEmpty(destSystemComboBox.Text))
                {
                    // swap from destination to source
                    string temp = RemoveExtraWhitespace(destSystemComboBox.Text);
                    srcSystemComboBox.Text = temp;
                    destSystemComboBox.Text = string.Empty;
                }
            }
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fromPane == 5) { /* Pilot's Log tab */ }
            else if (fromPane == 4)
            {
                // if we're coming from the notes pane we should save when we switch
                notesTextBox.SaveFile(notesFile, RichTextBoxStreamType.PlainText);
            }

            if (tabControl1.SelectedTab == tabControl1.TabPages["outputPage"] && !string.IsNullOrEmpty(td_outputBox.Text))
            {
                string filteredOutput = FilterOutput(td_outputBox.Text);
                runOutputState = IsValidRunOutput(filteredOutput);

                // check for parsable Run output
                if (runOutputState > -1)
                {
                    hasParsed = false; // reset the semaphore
                    tv_outputBox = filteredOutput; // copy our validated input
                    miniModeButton.Enabled = true;
                }
                else
                {
                    miniModeButton.Enabled = false;
                }

                td_outputBox.Focus(); // always focus our text box

                outputPage.Font = new Font(outputPage.Font, FontStyle.Regular); // reset the font
                fromPane = 0;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["logPage"])
            {
                fromPane = 5;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["notesPage"] && CheckIfFileOpens(notesFile))
            {
                notesTextBox.LoadFile(notesFile, RichTextBoxStreamType.PlainText);

                notesTextBox.Focus();
                fromPane = 4;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["savedPage1"] && CheckIfFileOpens(savedFile1))
            {
                savedTextBox1.Focus();

                if (File.Exists(savedFile1))
                {
                    savedTextBox1.LoadFile(savedFile1, RichTextBoxStreamType.PlainText);
                }

                string filteredOutput = FilterOutput(savedTextBox1.Text);
                runOutputState = IsValidRunOutput(filteredOutput);

                // check for parsable Run output
                if (runOutputState > -1)
                {
                    hasParsed = false; // reset the semaphore
                    tv_outputBox = filteredOutput; // copy our validated input
                    miniModeButton.Enabled = true;
                }
                else
                {
                    miniModeButton.Enabled = false;
                }

                savedTextBox1.Focus();
                fromPane = 1;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["savedPage2"] && CheckIfFileOpens(savedFile2))
            {
                savedTextBox2.Focus();

                if (File.Exists(savedFile2))
                {
                    savedTextBox2.LoadFile(savedFile2, RichTextBoxStreamType.PlainText);
                }

                string filteredOutput = FilterOutput(savedTextBox2.Text);
                runOutputState = IsValidRunOutput(filteredOutput);

                // check for parsable Run output
                if (runOutputState > -1)
                {
                    hasParsed = false; // reset the semaphore
                    tv_outputBox = filteredOutput; // copy our validated input
                    miniModeButton.Enabled = true;
                }
                else
                {
                    miniModeButton.Enabled = false;
                }

                savedTextBox2.Focus();
                fromPane = 2;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["savedPage3"] && CheckIfFileOpens(savedFile3))
            {
                savedTextBox3.Focus();

                if (File.Exists(savedFile3))
                {
                    savedTextBox3.LoadFile(savedFile3, RichTextBoxStreamType.PlainText);
                }

                string filteredOutput = FilterOutput(savedTextBox3.Text);
                runOutputState = IsValidRunOutput(filteredOutput);

                // check for parsable Run output
                if (runOutputState > -1)
                {
                    hasParsed = false; // reset the semaphore
                    tv_outputBox = filteredOutput; // copy our validated input
                    miniModeButton.Enabled = true;
                }
                else
                {
                    miniModeButton.Enabled = false;
                }

                savedTextBox3.Focus();
                fromPane = 3;
            }
        }

        private void Td_outputBox_TextChanged(object sender, EventArgs e)
        {
            if (buttonCaller == 5)
            {
                // catch the database update button, we want to see its output
                td_outputBox.SelectionStart = td_outputBox.Text.Length;
                td_outputBox.ScrollToCaret();
                td_outputBox.Refresh();
            }
        }

        private void TestSystemsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settingsRef.TestSystems = testSystemsCheckBox.Checked;
        }

        private void TestSystemsTimer_Delegate(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine(string.Format("testSystems Firing at: {0}", CurrentTimestamp()));

            if (!backgroundWorker6.IsBusy && !settingsRef.DisableNetLogs && !string.IsNullOrEmpty(settingsRef.NetLogPath))
            {
                backgroundWorker6.RunWorkerAsync();
            }
        }

        private void TowardsCheckBox_Click(object sender, EventArgs e)
        {
            if (settingsRef.Loop || loopCheckBox.Checked)
            {
                settingsRef.Loop = false;
                loopCheckBox.Checked = false;
            }
            else if (!string.IsNullOrEmpty(destSystemComboBox.Text))
            {
                if (!towardsCheckBox.Checked)
                {
                    towardsCheckBox.Checked = false;
                }
                else if (towardsCheckBox.Checked)
                {
                    towardsCheckBox.Checked = true;
                    shortenCheckBox.Checked = false;
                }
            }
            else
            {
                towardsCheckBox.Checked = false;
            }
        }

        private void TrackerLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/MarkAusten/TDHelper/issues/new");
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            ValidateSettings();

            if (!backgroundWorker4.IsBusy)
            {
                // UpdateDB Button
                buttonCaller = 5;
                DisableRunButtons(); // disable buttons during uncancellable operations

                backgroundWorker4.RunWorkerAsync();
            }
        }

        private void ValidateDestForEndJumps()
        {
            if (!string.IsNullOrEmpty(destSystemComboBox.Text) && methodIndex != 4)
            {
                endJumpsBox.Enabled = true;
                endJumpsLabel.Enabled = true;
            }
            else if (methodIndex != 4)
            {
                endJumpsBox.Enabled = false;
                endJumpsLabel.Enabled = false;
            }
        }
    }
}