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

        private int buttonCaller;
        private List<string> CommoditiesList = new List<string>();
        private List<string> DestinationList = new List<string>();
        private int hasUpdated;
        private int methodFromIndex;
        private int methodIndex;
        private string notesFile = Path.GetDirectoryName(Application.ExecutablePath) + @"\saved_notes.txt";
        private int procCode = -1;
        private string savedFile1 = Path.GetDirectoryName(Application.ExecutablePath) + @"\saved_1.txt";
        private string savedFile2 = Path.GetDirectoryName(Application.ExecutablePath) + @"\saved_2.txt";
        private string savedFile3 = Path.GetDirectoryName(Application.ExecutablePath) + @"\saved_3.txt";
        private List<string> ShipList = new List<string>();
        private List<string> SourceList = new List<string>();
        private string SourceSystem = string.Empty;
        private string TargetSystem = string.Empty;
        private System.Timers.Timer testSystemsTimer = new System.Timers.Timer();
        private string tv_outputBox = string.Empty;
        private CultureInfo userCulture = CultureInfo.CurrentCulture;
        private IList<string> validConfigs = new List<string>();
        private string SelectedCommodity = string.Empty;

        #endregion FormProps

        public MainForm()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                    | ControlStyles.UserPaint
                    | ControlStyles.DoubleBuffer, true);

            this.numCommandersCredits.Maximum = Decimal.MaxValue;

            SplashScreen.SetStatus("Building settings...");
            // Build variables from config
            BuildSettings();

            SplashScreen.SetStatus("Reaading configuration...");
            // Copy the setting to the form controls
            CopySettingsFromConfig();

            // And validate the settings to get rid of any nonsense.
            ValidateSettings(true);

            // Let's change the title
            SetFormTitle(this);

            testSystemsTimer.AutoReset = false;
            testSystemsTimer.Interval = 10000;
            testSystemsTimer.Elapsed += this.TestSystemsTimer_Delegate;

            this.btnCmdrProfile.Enabled = ValidateEdce();
        }

        private string AddCheckedOption(
                    bool toAdd,
                    string option)
        {
            return toAdd 
                ? " {0}".With(option) 
                : string.Empty;
        }

        /// <summary>
        /// Add the limit modifiers if required.
        /// </summary>
        /// <param name="limit">The settings value.</param>
        /// <returns>the planetary modifiers.</returns>
        private string AddLimitOption(decimal limit)
        {
            return limit > 0
                ? string.Empty
                : " --lim={0}".With(limit);
        }

        private string AddNear(string system)
        {
            return string.IsNullOrEmpty(system) 
                ? string.Empty 
                : " --near=\"{0}\"".With(system);
        }

        private string AddNumericOption(
                    decimal value,
                    string option)
        {
            return value == 0 
                ? string.Empty 
                : " {0}={1}".With(option, value);
        }

        /// <summary>
        /// Add the pad modifiers if required.
        /// </summary>
        /// <param name="pads">The settings value.</param>
        /// <returns>the planetary modifiers.</returns>
        private string AddPadOption(string pads)
        {
            return string.IsNullOrEmpty(pads)
                ? string.Empty
                : " --pad={0}".With(pads);
        }

        /// <summary>
        /// Add the planetary modifiers if required.
        /// </summary>
        /// <param name="planetary">The settings value.</param>
        /// <returns>the planetary modifiers.</returns>
        private string AddPlanetaryOption(string planetary)
        {
            return string.IsNullOrEmpty(planetary)
                ? string.Empty
                : " --planetary={0}".With(planetary);
        }

        private string AddAvoidOption(string toAvoid)
        {
            return string.IsNullOrEmpty(toAvoid) 
                ? string.Empty 
                : " --avoid=\"{0}\"".With(toAvoid);
        }

        private string AddQuotedOption(string system)
        {
            return string.IsNullOrEmpty(system) 
                ? string.Empty 
                : " \"{0}\"".With(system);
        }

        private string AddTextOption(
            string value,
            string option)
        {
            return string.IsNullOrEmpty(value) 
                ? string.Empty 
                : " {0}={1}".With(option, value);
        }

        private string AddVerbosity()
        {
            return settingsRef.Verbosity == 0 
                ? string.Empty 
                : " {0}".With(t_outputVerbosity);
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
            settingsRef.LastUsedConfig = cboCommandersShips.Text;
            SetShipList();
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
            EnablebtnStarts();

            // we were called by getSystemButton
            if (buttonCaller == 2 && output_unclean.Count > 0)
            {
                // skip our favorites to get our most recent system/station
                if (output_unclean.Count > 0 && currentMarkedStations.Count > 0)
                    cboSourceSystem.SelectedIndex = 1 + currentMarkedStations.Count;
                else if (output_unclean.Count > 0)
                    cboSourceSystem.SelectedIndex = 1; // if the favorites are empty

                cboSourceSystem.Focus();
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
             * Index = 6 is the Ship Vendor command
             * Index = 7 is the Navigation command
             * Index = 8 is the OldData command
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

            if (methodIndex == 9)
            {
                // local is a global override, let's catch it first
                if (!string.IsNullOrEmpty(SourceSystem))
                {
                    t_path += GetLocalCommand(SourceSystem);
                }
                else
                {
                    t_path = string.Empty;
                }
            }
            else if (methodIndex == 8)
            {
                if (!string.IsNullOrEmpty(SourceSystem))
                {
                    t_path += GetOldDataCommand(SourceSystem);
                }
                else
                {
                    t_path = string.Empty;
                }
            }
            else if (methodIndex == 7)
            {
                // mark us as coming from the nav command
                if (!string.IsNullOrEmpty(SourceSystem) && !string.IsNullOrEmpty(TargetSystem))
                {
                    t_path += GetNavCommand(SourceSystem, TargetSystem);
                }
                else
                {
                    t_path = string.Empty;
                }
            }
            else if (methodIndex == 5)
            { // Market command
                if (!string.IsNullOrEmpty(SourceSystem))
                {
                    t_path += GetMarketCommand(SourceSystem);
                }
                else
                {
                    t_path = string.Empty;
                }
            }
            else if (methodIndex == 4)
            {
                // mark us as coming from the trade command
                if (!string.IsNullOrEmpty(SourceSystem) && !string.IsNullOrEmpty(TargetSystem))
                {
                    t_path += GetTradeCommand(SourceSystem, TargetSystem);
                }
                else
                {
                    t_path = string.Empty;
                }
            }
            else if (methodIndex == 3)
            { // Rares command
                if (!string.IsNullOrEmpty(SourceSystem))
                {
                    t_path += GetRaresCommand(SourceSystem);
                }
                else
                {
                    t_path = string.Empty;
                }
            }
            else if (methodIndex == 2)
            { // Sell command
                t_path += GetSellCommand(SelectedCommodity, SourceSystem);
            }
            else if (methodIndex == 1)
            { // Buy command
                t_path += GetBuyCommand(SelectedCommodity, SourceSystem);
            }
            else if (methodIndex == 0)
            { // Run command
                t_path += GetRunCommand(SourceSystem, TargetSystem);
            }

            // pass the built command-line to the delegate
            if (string.IsNullOrEmpty(t_path))
            {
                PlayAlert();
            }
            else
            {
                DoTDProc(t_path);
            }
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!backgroundWorker2.IsBusy)
            {
                stopwatch.Stop(); // stop the timer
                circularBuffer = new System.Text.StringBuilder(2 * circularBufferSize);

                // let's alert the user that the Output pane has changed
                if (!string.IsNullOrEmpty(rtbOutput.Text) && tabControl1.SelectedTab != pagOutput)
                    tabControl1.SelectedTab = pagOutput;

                // assume we're coming from getUpdatedPricesFile()
                if (!string.IsNullOrEmpty(settingsRef.ImportPath) && buttonCaller == 11)
                    CleanUpdatedPricesFile();
                else if (buttonCaller == 16)
                {
                    // force a db sync if we're marked
                    if (!backgroundWorker1.IsBusy)
                        backgroundWorker1.RunWorkerAsync();
                }
                //else if (buttonCaller == 17)
                //{
                //    // force a station/shipvendor panel update
                //    PopulateStationPanel(temp_src);
                //}

                // reenable after uncancellable task is done
                EnablebtnStarts();
                btnStart.Font = new Font(btnStart.Font, FontStyle.Bold);
                btnStart.Text = "Start";

                td_proc.Dispose();

                // make a sound when we're done with a long operation (>10s)
                if ((stopwatch.ElapsedMilliseconds > 10000 && buttonCaller != 5) || buttonCaller == 20)
                {
                    PlayAlert(); // when not marked as cancelled, or explicit
                }

                if (buttonCaller != 4) // not if we're coming from Run
                {
                    btnMiniMode.Enabled = false;
                }
                else if (buttonCaller == 4)
                {
                    string filteredOutput = FilterOutput(rtbOutput.Text);
                    // validate the run output before we enable the mini-mode button
                    runOutputState = IsValidRunOutput(filteredOutput);

                    if (runOutputState > -1)
                    {
                        hasParsed = false; // reset the semaphore
                        tv_outputBox = filteredOutput; // copy our validated input
                        btnMiniMode.Enabled = true;
                    }
                    else
                        btnMiniMode.Enabled = false;
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
                    lblStopWatch.Text = "Elapsed: " + stopwatch.Elapsed.ToString("hh\\:mm\\:ss");
                }));
            }
        }

        private void BackgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // when we get the !IsRunning signal, write out
            this.Invoke(new Action(() =>
            {
                // do this on the UI thread
                lblStopWatch.Text = "Elapsed: " + stopwatch.Elapsed.ToString("hh\\:mm\\:ss");
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

                EnablebtnStarts();
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
                icoUpdateNotify.Top = this.Height - 60;
                lblUpdateNotify.Top = icoUpdateNotify.Top;

                icoUpdateNotify.Visible = true;
                lblUpdateNotify.Visible = true;
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
                        this.rtbOutput.Text += "We're currently in an unrecognized system: " + t_lastSystem + "\r\n";
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

                    this.Invoke(new Action(() =>
                    {
                        // run this on the UI thread
                        this.rtbOutput.Text += "Entering an unrecognized system: " + t_lastSystem + "\r\n";
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
                if (ValidateEdce())
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

                btnStart.Text = "Start";
                EnablebtnStarts();
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
            if (methodIndex == 4 && chkRunOptionsDirect.Checked)
            {
                // we cannot have both buy and sell enabled
                chkRunOptionsDirect.Checked = false;
                chkRunOptionsBlkMkt.Checked = true;
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
                DisablebtnStarts(); // disable buttons during uncancellable operations

                backgroundWorker7.RunWorkerAsync();
            }
        }

        private void BtnDbUpdate_Click(object sender, EventArgs e)
        {
            ValidateSettings();

            if (!backgroundWorker4.IsBusy)
            {
                // UpdateDB Button
                buttonCaller = 5;
                DisablebtnStarts(); // disable buttons during uncancellable operations

                backgroundWorker4.RunWorkerAsync();
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

        private void BtnSettings_Click(object sender, EventArgs e)
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

        private void BtnStart_Click(object sender, EventArgs e)
        {
            // Prevent double clicks.
            this.btnStart.Enabled = false;

            // mark as run button
            buttonCaller = 1;

            GetSourceAndTarget();

            DoRunEvent(); // externalized
        }

        private void CboShipsSold_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                //                cboShipsSold.SelectionLength = 0;
            }
        }

        private void ChkLoop_Click(object sender, EventArgs e)
        {
            if (chkRunOptionsDirect.Checked)
            {
                chkRunOptionsDirect.Checked = false;
                chkRunOptionsLoop.Checked = true;
            }

            if (chkRunOptionsUnique.Checked)
            {
                chkRunOptionsUnique.Checked = false;
                chkRunOptionsLoop.Checked = true;
            }

            if (chkRunOptionsShorten.Checked)
            {
                chkRunOptionsShorten.Checked = false;
                chkRunOptionsTowards.Checked = false;
                chkRunOptionsLoop.Checked = true;
            }
            else if (settingsRef.Towards || chkRunOptionsTowards.Checked)
            {
                settingsRef.Towards = false;
                chkRunOptionsTowards.Checked = false;
            }
        }

        private void ChkTowards_Click(object sender, EventArgs e)
        {
            if (settingsRef.Loop || chkRunOptionsLoop.Checked)
            {
                settingsRef.Loop = false;
                chkRunOptionsLoop.Checked = false;
            }
            else if (!string.IsNullOrEmpty(cboRunOptionsDestination.Text))
            {
                if (!chkRunOptionsTowards.Checked)
                {
                    chkRunOptionsTowards.Checked = false;
                }
                else if (chkRunOptionsTowards.Checked)
                {
                    chkRunOptionsTowards.Checked = true;
                    chkRunOptionsShorten.Checked = false;
                }
            }
            else
            {
                chkRunOptionsTowards.Checked = false;
            }
        }

        private void ClearSaved1MenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbSaved1.Text))
            {
                rtbSaved1.Text = string.Empty;
                if (File.Exists(savedFile1))
                    File.Delete(savedFile1);
            }
        }

        private void ClearSaved2MenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbSaved2.Text))
            {
                rtbSaved2.Text = string.Empty;
                if (File.Exists(savedFile2))
                    File.Delete(savedFile2);
            }
        }

        private void ClearSaved3MenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbSaved3.Text))
            {
                rtbSaved3.Text = string.Empty;
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
            RichTextBox clickedControl = (RichTextBox)this.mnuStrip1.SourceControl;

            if (clickedControl.Name == txtNotes.Name && !mnuDelete.Enabled)
            {
                mnuCut.Enabled = true;
                mnuDelete.Enabled = true;
                mnuPaste.Enabled = true;
                mnuNotesClear.Visible = true;
                mnuSavePage1.Enabled = false;
                mnuSavePage2.Enabled = false;
                mnuSavePage3.Enabled = false;
            }
            else if (clickedControl.Name == rtbSaved1.Name
                || clickedControl.Name == rtbSaved2.Name
                || clickedControl.Name == rtbSaved3.Name
                || clickedControl.Name == rtbOutput.Name)
            {
                mnuCut.Enabled = false;
                mnuDelete.Enabled = false;
                mnuPaste.Enabled = false;
                mnuNotesClear.Visible = false;
                mnuSavePage1.Enabled = true;
                mnuSavePage2.Enabled = true;
                mnuSavePage3.Enabled = true;
            }

            // control clearing
            if (clickedControl.Name == rtbSaved1.Name)
            {
                nmuClearSaved1.Visible = true;
                mnuClearSaved2.Visible = false;
                mnuClearSaved3.Visible = false;
            }
            else if (clickedControl.Name == rtbSaved2.Name)
            {
                mnuClearSaved2.Visible = true;
                nmuClearSaved1.Visible = false;
                mnuClearSaved3.Visible = false;
            }
            else if (clickedControl.Name == rtbSaved3.Name)
            {
                mnuClearSaved3.Visible = true;
                mnuClearSaved2.Visible = false;
                nmuClearSaved1.Visible = false;
            }
            else
            {
                nmuClearSaved1.Visible = false;
                mnuClearSaved2.Visible = false;
                mnuClearSaved3.Visible = false;
            }
        }

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.mnuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.Copy();
        }

        private void CopySettingsFromConfig()
        {
            //
            // Load controls in the form from variables in memory
            //

            txtAvoid.Text = settingsRef.Avoid;
            txtVia.Text = settingsRef.Via;

            numRouteOptionsShipCapacity.Value = settingsRef.Capacity > numRouteOptionsShipCapacity.Minimum && settingsRef.Capacity <= numRouteOptionsShipCapacity.Maximum
                ? settingsRef.Capacity
                : numRouteOptionsShipCapacity.Minimum;

            numCommandersCredits.Value = settingsRef.Credits > numCommandersCredits.Minimum && settingsRef.Credits <= numCommandersCredits.Maximum
                ? settingsRef.Credits
                : numCommandersCredits.Minimum;

            numShipInsurance.Value = settingsRef.Insurance > numShipInsurance.Minimum && settingsRef.Insurance <= numShipInsurance.Maximum
                ? settingsRef.Insurance
                : numShipInsurance.Minimum;

            numRouteOptionsLsPenalty.Value = settingsRef.LSPenalty > numRouteOptionsLsPenalty.Minimum && settingsRef.LSPenalty <= numRouteOptionsLsPenalty.Maximum
                ? settingsRef.LSPenalty
                : numRouteOptionsLsPenalty.Minimum;

            numRouteOptionsMaxLSDistance.Value = settingsRef.MaxLSDistance > numRouteOptionsMaxLSDistance.Minimum && settingsRef.MaxLSDistance <= numRouteOptionsMaxLSDistance.Maximum
                ? settingsRef.MaxLSDistance
                : numRouteOptionsMaxLSDistance.Minimum;

            numRunOptionsLoopInt.Value = settingsRef.LoopInt > numRunOptionsLoopInt.Minimum && settingsRef.LoopInt <= numRunOptionsLoopInt.Maximum
                ? settingsRef.LoopInt
                : numRunOptionsLoopInt.Minimum;

            numRouteOptionsAge.Value = settingsRef.Age > numRouteOptionsAge.Minimum && settingsRef.Age <= numRouteOptionsAge.Maximum
                ? settingsRef.Age
                : numRouteOptionsAge.Minimum;

            numRouteOptionsPruneHops.Value = settingsRef.PruneHops > numRouteOptionsPruneHops.Minimum && settingsRef.PruneHops <= numRouteOptionsPruneHops.Maximum
                ? settingsRef.PruneHops
                : numRouteOptionsPruneHops.Minimum;

            numRouteOptionsPruneScore.Value = settingsRef.PruneScore > numRouteOptionsPruneScore.Minimum && settingsRef.PruneScore <= numRouteOptionsPruneScore.Maximum
                ? settingsRef.PruneScore
                : numRouteOptionsPruneScore.Minimum;

            numRouteOptionsStock.Value = settingsRef.Stock > numRouteOptionsStock.Minimum && settingsRef.Stock <= numRouteOptionsStock.Maximum
                ? settingsRef.Stock
                : numRouteOptionsStock.Minimum;

            //numSupply.Value = settingsRef.Supply > numSupply.Minimum && settingsRef.Supply <= numSupply.Maximum
            //    ? settingsRef.Supply
            //    : numSupply.Minimum;

            numRouteOptionsDemand.Value = settingsRef.Demand > numRouteOptionsDemand.Minimum && settingsRef.Demand <= numRouteOptionsDemand.Maximum
                ? settingsRef.Demand
                : numRouteOptionsDemand.Minimum;

            numRouteOptionsGpt.Value = settingsRef.GPT > numRouteOptionsGpt.Minimum && settingsRef.GPT <= numRouteOptionsGpt.Maximum
                ? settingsRef.GPT
                : numRouteOptionsGpt.Minimum;

            numRouteOptionsMaxGpt.Value = settingsRef.MaxGPT > numRouteOptionsMaxGpt.Minimum && settingsRef.MaxGPT <= numRouteOptionsMaxGpt.Maximum
                ? settingsRef.MaxGPT
                : numRouteOptionsMaxGpt.Minimum;

            numRouteOptionsHops.Value = settingsRef.Hops > numRouteOptionsHops.Minimum && settingsRef.Hops <= numRouteOptionsHops.Maximum
                ? settingsRef.Hops
                : numRouteOptionsHops.Minimum;

            numRouteOptionsJumps.Value = settingsRef.Jumps > numRouteOptionsJumps.Minimum && settingsRef.Jumps <= numRouteOptionsJumps.Maximum
                ? settingsRef.Jumps
                : numRouteOptionsJumps.Minimum;

            numRouteOptionsLimit.Value = settingsRef.Limit > numRouteOptionsLimit.Minimum && settingsRef.Limit <= numRouteOptionsLimit.Maximum
                ? settingsRef.Limit
                : numRouteOptionsLimit.Minimum;

            //abovePriceBox.Value = settingsRef.AbovePrice > abovePriceBox.Minimum && settingsRef.AbovePrice <= abovePriceBox.Maximum
            //    ? settingsRef.AbovePrice
            //    : abovePriceBox.Minimum;

            numRunOptionsRoutes.Value = settingsRef.BelowPrice > numRunOptionsRoutes.Minimum && settingsRef.BelowPrice <= numRunOptionsRoutes.Maximum
                ? settingsRef.BelowPrice
                : numRunOptionsRoutes.Minimum;

            numUnladenLy.Value = settingsRef.UnladenLY > numUnladenLy.Minimum && settingsRef.UnladenLY <= numUnladenLy.Maximum
                ? settingsRef.UnladenLY
                : numUnladenLy.Minimum;

            numLadenLy.Value = settingsRef.LadenLY > numLadenLy.Minimum && settingsRef.LadenLY <= numLadenLy.Maximum
                ? settingsRef.LadenLY
                : numLadenLy.Minimum;

            numRouteOptionsMargin.Value = settingsRef.Margin > numRouteOptionsMargin.Minimum && settingsRef.Margin <= numRouteOptionsMargin.Maximum
                ? settingsRef.Margin
                : numRouteOptionsMargin.Minimum;

            // copy verbosity to string format
            switch (settingsRef.Verbosity)
            {
                case 0:
                    t_outputVerbosity = string.Empty;
                    break;

                case 2:
                    t_outputVerbosity = "-vv";
                    break;

                case 3:
                    t_outputVerbosity = "-vvv";
                    break;

                default:
                    t_outputVerbosity = "-v";
                    break;
            }

            // exception for Loop
            if (settingsRef.Loop && settingsRef.Towards)
            {
                // reset them both to prevent issues
                settingsRef.Loop = false;
                chkRunOptionsLoop.Checked = false;
                settingsRef.Towards = false;
                chkRunOptionsTowards.Checked = false;
            }
            else if (settingsRef.Loop)
            {
                chkRunOptionsLoop.Checked = settingsRef.Loop;
                chkRunOptionsTowards.Checked = false; // one or the other
            }
            else if (settingsRef.Towards)
            {
                chkRunOptionsTowards.Checked = settingsRef.Towards;
                chkRunOptionsLoop.Checked = false;
            }

            // exceptions
            settingsRef.ShowJumps = chkRunOptionsJumps.Checked;
            settingsRef.Unique = chkRunOptionsUnique.Checked;

            txtPadSize.Text = ContainsPadSizes(settingsRef.Padsizes);

            txtRunOptionsPlanetary.Text = ContainsPlanetary(settingsRef.Planetary);

            //            chkRouteStations.Checked = settingsRef.RouteStations;
            txtRunOptionsPlanetary.Text = settingsRef.Planetary;
        }

        private void CopySettingsFromForm()
        {
            //
            // Load variables from text boxes in the form
            //

            // make sure we strip the excess whitespace in src/dest
            temp_src = RemoveExtraWhitespace(cboSourceSystem.Text);
            temp_dest = RemoveExtraWhitespace(cboRunOptionsDestination.Text);

            //            temp_commod = commodityComboBox.Text;

            // make sure we don't pass the "Ship's Sold" box if its still unchanged
            //if (cboShipsSold.Text.Equals(outputStationShips.ToString(), StringComparison.OrdinalIgnoreCase))
            //{
            //    temp_shipsSold = string.Empty;
            //}
            //else
            //{
            //    temp_shipsSold = cboShipsSold.Text;
            //}

            if (methodIndex == 3)
            {
                r_fromBox = txtAvoid.Text;
            }
            else
            {
                settingsRef.Avoid = txtAvoid.Text;
            }

            settingsRef.Via = txtVia.Text;

            /*
             * The following is a set of workarounds to fix a bug in Framework 2.0+
             * involving NumericUpDown controls not updating the Value() property
             * when changed from the keyboard instead of the spinner control. We
             * do this by parsing the unbrowsable Text property and copying that.
             */

            if (decimal.TryParse(numRouteOptionsShipCapacity.Text, out decimal t_Capacity))
            {
                numRouteOptionsShipCapacity.Text = t_Capacity.ToString();
                settingsRef.Capacity = t_Capacity;
            }
            else
            {
                settingsRef.Capacity = numRouteOptionsShipCapacity.Minimum; // this is a requirement
                numRouteOptionsShipCapacity.Text = settingsRef.Capacity.ToString();
            }

            if (decimal.TryParse(numCommandersCredits.Text, out decimal t_Credits))
            {
                numCommandersCredits.Text = t_Credits.ToString();
                settingsRef.Credits = t_Credits;
            }
            else
            {
                settingsRef.Credits = numCommandersCredits.Minimum; // this is a requirement
                numCommandersCredits.Text = settingsRef.Credits.ToString();
            }

            if (decimal.TryParse(numShipInsurance.Text, out decimal t_Insurance))
            {
                numShipInsurance.Text = t_Insurance.ToString();
                settingsRef.Insurance = t_Insurance;
            }
            else
            {
                settingsRef.Insurance = numShipInsurance.Minimum;
                numShipInsurance.Text = settingsRef.Insurance.ToString();
            }

            if (decimal.TryParse(numRouteOptionsLsPenalty.Text, out decimal t_lsPenalty))
            {
                numRouteOptionsLsPenalty.Text = t_lsPenalty.ToString();
                settingsRef.LSPenalty = t_lsPenalty;
            }
            else
            {
                settingsRef.LSPenalty = numRouteOptionsLsPenalty.Minimum;
                numRouteOptionsLsPenalty.Text = settingsRef.LSPenalty.ToString();
            }

            if (decimal.TryParse(numRouteOptionsMaxLSDistance.Text, out decimal t_maxLSDistance))
            {
                numRouteOptionsMaxLSDistance.Text = t_maxLSDistance.ToString();
                settingsRef.MaxLSDistance = t_maxLSDistance;
            }
            else
            {
                settingsRef.MaxLSDistance = numRouteOptionsMaxLSDistance.Minimum;
                numRouteOptionsMaxLSDistance.Text = settingsRef.MaxLSDistance.ToString();
            }

            if (decimal.TryParse(numRunOptionsLoopInt.Text, out decimal t_LoopInt))
            {
                numRunOptionsLoopInt.Text = t_LoopInt.ToString();
                settingsRef.LoopInt = t_LoopInt;
            }
            else
            {
                settingsRef.LoopInt = numRunOptionsLoopInt.Minimum;
                numRunOptionsLoopInt.Text = settingsRef.LoopInt.ToString();
            }

            if (decimal.TryParse(numRouteOptionsPruneHops.Text, out decimal t_pruneHops))
            {
                numRouteOptionsPruneHops.Text = t_pruneHops.ToString();
                settingsRef.PruneHops = t_pruneHops;
            }
            else
            {
                settingsRef.PruneHops = numRouteOptionsPruneHops.Minimum;
                numRouteOptionsPruneHops.Text = settingsRef.PruneHops.ToString();
            }

            if (decimal.TryParse(numRouteOptionsPruneScore.Text, out decimal t_pruneScore))
            {
                numRouteOptionsPruneScore.Text = t_pruneScore.ToString();
                settingsRef.PruneScore = t_pruneScore;
            }
            else
            {
                settingsRef.PruneScore = numRouteOptionsPruneScore.Minimum;
                numRouteOptionsPruneScore.Text = settingsRef.PruneScore.ToString();
            }

            if (decimal.TryParse(numRouteOptionsStock.Text, out decimal t_Stock))
            {
                numRouteOptionsStock.Text = t_Stock.ToString();
                settingsRef.Stock = t_Stock;
            }
            else
            {
                settingsRef.Stock = numRouteOptionsStock.Minimum;
                numRouteOptionsStock.Text = settingsRef.Stock.ToString();
            }

            //if (decimal.TryParse(numSupply.Text, out decimal t_Supply))
            //{
            //    numSupply.Text = t_Supply.ToString();
            //    settingsRef.Supply = t_Supply;
            //}
            //else
            //{
            //    settingsRef.Supply = numSupply.Minimum;
            //    numSupply.Text = settingsRef.Supply.ToString();
            //}

            if (decimal.TryParse(numRouteOptionsDemand.Text, out decimal t_Demand))
            {
                numRouteOptionsDemand.Text = t_Demand.ToString();
                settingsRef.Demand = t_Demand;
            }
            else
            {
                settingsRef.Demand = numRouteOptionsDemand.Minimum;
                numRouteOptionsDemand.Text = settingsRef.Demand.ToString();
            }

            if (decimal.TryParse(numRouteOptionsAge.Text, out decimal t_Age))
            {
                numRouteOptionsAge.Text = t_Age.ToString();
                settingsRef.Age = t_Age;
            }
            else
            {
                settingsRef.Age = numRouteOptionsAge.Minimum;
                numRouteOptionsAge.Text = settingsRef.Age.ToString();
            }

            if (decimal.TryParse(numRouteOptionsGpt.Text, out decimal t_GPT))
            {
                numRouteOptionsGpt.Text = t_GPT.ToString();
                settingsRef.GPT = t_GPT;
            }
            else
            {
                settingsRef.GPT = numRouteOptionsGpt.Minimum;
                numRouteOptionsGpt.Text = settingsRef.GPT.ToString();
            }

            if (decimal.TryParse(numRouteOptionsMaxGpt.Text, out decimal t_MaxGPT))
            {
                numRouteOptionsMaxGpt.Text = t_MaxGPT.ToString();
                settingsRef.MaxGPT = t_MaxGPT;
            }
            else
            {
                settingsRef.MaxGPT = numRouteOptionsMaxGpt.Minimum;
                numRouteOptionsMaxGpt.Text = settingsRef.MaxGPT.ToString();
            }

            if (decimal.TryParse(numRouteOptionsLimit.Text, out decimal t_Limit))
            {
                numRouteOptionsLimit.Text = t_Limit.ToString();
                settingsRef.Limit = t_Limit;
            }
            else
            {
                settingsRef.Limit = numRouteOptionsLimit.Minimum;
                numRouteOptionsLimit.Text = settingsRef.Limit.ToString();
            }

            if (decimal.TryParse(numRouteOptionsHops.Text, out decimal t_Hops))
            {
                numRouteOptionsHops.Text = t_Hops.ToString();
                settingsRef.Hops = t_Hops;
            }
            else
            {
                settingsRef.Hops = numRouteOptionsHops.Minimum;
                numRouteOptionsHops.Text = settingsRef.Hops.ToString();
            }

            if (decimal.TryParse(numRouteOptionsJumps.Text, out decimal t_Jumps))
            {
                numRouteOptionsJumps.Text = t_Jumps.ToString();
                settingsRef.Jumps = t_Jumps;
            }
            else
            {
                settingsRef.Jumps = numRouteOptionsJumps.Minimum;
                numRouteOptionsJumps.Text = settingsRef.Jumps.ToString();
            }

            if (decimal.TryParse(numRunOptionsEndJumps.Text, out t_EndJumps))
                numRunOptionsEndJumps.Text = t_EndJumps.ToString();
            else
            {
                t_EndJumps = numRunOptionsEndJumps.Minimum;
                numRunOptionsEndJumps.Text = t_EndJumps.ToString();
            }

            if (decimal.TryParse(numRunOptionsStartJumps.Text, out t_StartJumps))
                numRunOptionsStartJumps.Text = t_StartJumps.ToString();
            else
            {
                t_StartJumps = numRunOptionsStartJumps.Minimum;
                numRunOptionsStartJumps.Text = t_StartJumps.ToString();
            }

            //if (decimal.TryParse(abovePriceBox.Text, out decimal t_abovePrice))
            //{
            //    abovePriceBox.Text = t_abovePrice.ToString();
            //    settingsRef.AbovePrice = t_abovePrice;
            //}
            //else
            //{
            //    settingsRef.AbovePrice = abovePriceBox.Minimum;
            //    abovePriceBox.Text = settingsRef.AbovePrice.ToString();
            //}

            if (decimal.TryParse(numRunOptionsRoutes.Text, out t_belowPrice))
            {
                numRunOptionsRoutes.Text = t_belowPrice.ToString();
                settingsRef.BelowPrice = t_belowPrice;
            }
            else
            {
                settingsRef.BelowPrice = numRunOptionsRoutes.Minimum;
                numRunOptionsRoutes.Text = settingsRef.BelowPrice.ToString();
            }

            //if (decimal.TryParse(numMinAge.Text, out decimal t_MinAge))
            //{
            //    numMinAge.Text = t_MinAge.ToString();
            //}
            //else
            //{
            //    numMinAge.Text = numMinAge.Minimum.ToString();
            //}

            settingsRef.Loop = chkRunOptionsLoop.Checked;
            settingsRef.Towards = chkRunOptionsTowards.Checked;
            settingsRef.Unique = chkRunOptionsUnique.Checked;
            settingsRef.ShowJumps = chkRunOptionsJumps.Checked;
            //            settingsRef.RouteStations = chkRouteStations.Checked;

            settingsRef.Planetary = txtRunOptionsPlanetary.Text;

            //            stationsFilterChecked = stationsFilterCheckBox.Checked;
            //            oldDataRouteChecked = chkOldRoutes.Checked;

            // convert the local checkstates to ints as well
            //blackmarketBoxChecked = GetCheckBoxCheckState(bmktFilterCheckBox.CheckState);
            //shipyardBoxChecked = GetCheckBoxCheckState(shipyardFilterCheckBox.CheckState);
            //marketBoxChecked = GetCheckBoxCheckState(itemsFilterCheckBox.CheckState);
            //repairBoxChecked = GetCheckBoxCheckState(repairFilterCheckBox.CheckState);
            //rearmBoxChecked = GetCheckBoxCheckState(rearmFilterCheckBox.CheckState);
            //refuelBoxChecked = GetCheckBoxCheckState(refuelFilterCheckBox.CheckState);
            //outfitBoxChecked = GetCheckBoxCheckState(outfitFilterCheckBox.CheckState);

            //
            // exceptions
            //
            settingsRef.Padsizes = ContainsPadSizes(txtPadSize.Text);
            settingsRef.Planetary = ContainsPlanetary(txtRunOptionsPlanetary.Text);

            t_confirmCode = string.Empty;

            // handle floats differently
            if (decimal.TryParse(numUnladenLy.Text, out decimal t_unladenLY))
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
                settingsRef.UnladenLY = decimal.Truncate(numUnladenLy.Minimum * 100) / 100;
                numUnladenLy.Text = settingsRef.UnladenLY.ToString();
            }

            // the ladenLY is a bit more complicated, let's handle it
            if (decimal.TryParse(numLadenLy.Text, out t_ladenLY))
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
                settingsRef.LadenLY = decimal.Truncate(numLadenLy.Minimum * 100) / 100;
                numLadenLy.Text = settingsRef.LadenLY.ToString();
            }

            if (decimal.TryParse(numRouteOptionsMargin.Text, out decimal t_Margin))
            {
                settingsRef.Margin = decimal.Truncate(t_Margin * 100) / 100;
                numRouteOptionsMargin.Text = settingsRef.Margin.ToString();
            }
            else
            {
                settingsRef.Margin = decimal.Truncate(numRouteOptionsMargin.Minimum * 100) / 100;
                numRouteOptionsMargin.Text = settingsRef.Margin.ToString();
            }
        }

        private void CopySystemToDest_Click(object sender, EventArgs e)
        {
            if (grdPilotsLog.Rows.Count > 0 && !string.IsNullOrEmpty(grdPilotsLog.Rows[dRowIndex].Cells[2].Value.ToString()))
            {
                // grab the system from the system field, if it exists, copy to the src box
                string dbSys = grdPilotsLog.Rows[dRowIndex].Cells[2].Value.ToString();
                cboRunOptionsDestination.Text = dbSys;
            }
        }

        private void CopySystemToSrc_Click(object sender, EventArgs e)
        {
            if (grdPilotsLog.Rows.Count > 0 && !string.IsNullOrEmpty(grdPilotsLog.Rows[dRowIndex].Cells[2].Value.ToString()))
            {
                // grab the system from the system field, if it exists, copy to the src box
                string dbSys = grdPilotsLog.Rows[dRowIndex].Cells[2].Value.ToString();
                cboSourceSystem.Text = dbSys;
            }
        }

        private void CutMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.mnuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.Cut();
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.mnuStrip1.SourceControl;

            if (clickedControl.Name == txtNotes.Name)
            {
                txtNotes.SelectedText = string.Empty;
            }
        }

        private void DestinationChanged(object sender, EventArgs e)
        {
            ComboBox destination = ((ComboBox)sender);
            CheckBox towards = destination.Parent.Controls.OfType<CheckBox>()
                .FirstOrDefault(x => x.Name.Equals("chkRunOptionsTowards"));

            // wait for the user to type a few characters
            if (destination.Text.Length > 3)
            {
                string filteredString = RemoveExtraWhitespace(destination.Text);

                ValidateDestForEndJumps();
            }

            // Deal with the towards check box if one exists on this panel.
            if (towards != null)
            {
                towards.Enabled = destination.Text.Length > 3 && cboSourceSystem.Text.Length > 3;
            }

            CheckBox shorten = destination.Parent.Controls.OfType<CheckBox>()
                .FirstOrDefault(x => x.Name.Equals("chkRunOptionsShorten"));

            // Deal with the shorten check box if one exists on this panel.
            if (shorten != null)
            {
                shorten.Enabled = destination.Text.Length > 0;
            }
        }

        private void DestSystemComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            string filteredString = RemoveExtraWhitespace(cboRunOptionsDestination.Text);

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
                && !string.IsNullOrEmpty(cboRunOptionsDestination.Text))
            {
                // wipe our box selectively if we hit escape
                //first wipe until the delimiter
                string[] tokens = cboRunOptionsDestination.Text.Split(new string[] { "/" }, StringSplitOptions.None);

                if (tokens != null && tokens.Length == 2)
                {
                    // make sure we have a system/station
                    // delete the front of the string until the system
                    cboRunOptionsDestination.Text = tokens[0];
                }
                else if (!cboRunOptionsDestination.Text.Contains("/"))
                {
                    cboRunOptionsDestination.Text = string.Empty;
                }

                e.Handled = true;
            }
        }

        private void DirectCheckBox_Click(object sender, EventArgs e)
        {
            if (chkRunOptionsLoop.Checked)
            {
                chkRunOptionsLoop.Checked = false;
                chkRunOptionsDirect.Checked = true;
            }

            // an exception for the market command
            if (methodIndex == 4 && chkRunOptionsBlkMkt.Checked)
            {
                // we cannot have both buy and sell enabled
                chkRunOptionsBlkMkt.Checked = false;
                chkRunOptionsDirect.Checked = true;
            }
        }

        private void DisablebtnStarts()
        {
            // disable buttons during uncancellable operation
            btnDbUpdate.Enabled = false;
            btnCmdrProfile.Enabled = false;
            btnGetSystem.Enabled = false;
            btnMiniMode.Enabled = false;

            // an exception for Run commands
            if (buttonCaller != 1)
            {
                btnStart.Enabled = false;
            }
        }

        private void EnablebtnStarts()
        {
            // reenable other worker callers when done
            btnDbUpdate.Enabled = true;
            btnCmdrProfile.Enabled = ValidateEdce();
            btnGetSystem.Enabled = true;

            // fix Run button when returning from non-Run commands
            if (buttonCaller == 1 || !btnStart.Enabled)
            {
                btnStart.Enabled = true;
            }
        }

        /// <summary>
        /// Enable or disable the specified options panel.
        /// </summary>
        /// <param name="panel">The panel to be enabled or disabled.</param>
        /// <param name="enable">True to enable the panel.</param>
        private void EnableOptions(
            Panel panel,
            bool enable = true)
        {
            panel.Enabled = enable;
        }

        private void FaqLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/MarkAusten/TDHelper/wiki/Home");
        }

        private void ForceRefreshGridView_Click(object sender, EventArgs e)
        {
            InvalidatedRowUpdate(true, -1); // force an invalidate and reload
        }

        private void ForceResortMenuItem_Click(object sender, EventArgs e)
        {
            grdPilotsLog.Columns["Timestamp"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grdPilotsLog.Columns["System"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grdPilotsLog.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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

        /// <summary>
        /// Get the local command string.
        /// </summary>
        /// <param name="sourceSystem">The name of the source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetLocalCommand(string sourceSystem)
        {
            string cmdPath = "local";

            cmdPath += AddNumericOption(numLocalOptionsLy.Value, "--ly");

            cmdPath += AddCheckedOption(chkLocalOptionsRearm.Checked, "--rearm");
            cmdPath += AddCheckedOption(chkLocalOptionsRefuel.Checked, "--refuel");
            cmdPath += AddCheckedOption(chkLocalOptionsRepair.Checked, "--repair");
            cmdPath += AddCheckedOption(chkLocalOptionsTrading.Checked, "--trading");
            cmdPath += AddCheckedOption(chkLocalOptionsBlkMkt.Checked, "--bm");
            cmdPath += AddCheckedOption(chkLocalOptionsShipyard.Checked, "--shipyard");
            cmdPath += AddCheckedOption(chkLocalOptionsOutfitting.Checked, "--outfitting");
            cmdPath += AddCheckedOption(chkLocalOptionsStations.Checked, "--stations");

            cmdPath += AddPlanetaryOption(txtLocalOptionsPlanetary.Text);
            cmdPath += AddPadOption(txtLocalOptionsPads.Text);

            cmdPath += AddVerbosity();

            cmdPath += AddQuotedOption(sourceSystem);

            return cmdPath;
        }

        /// <summary>
        /// Get the sell command string.
        /// </summary>
        /// <param name="selectedCommodity">The name of the selected commodity.</param>
        /// <param name="source">The select source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetSellCommand(
            string selectedCommodity,
            string source)
        {
            string cmdPath = "sell";

            cmdPath += AddNear(source);

            cmdPath += AddNumericOption(numSellOptionsNearLy.Value, "--ly");
            cmdPath += AddNumericOption(numSellOptionsAbove.Value, "--gt");
            cmdPath += AddNumericOption(numSellOptionsBelow.Value, "--lt");
            cmdPath += AddNumericOption(numSellOptionsDemand.Value, "--demand");

            cmdPath += AddCheckedOption(chkSellOptionsBlkMkt.Checked, "--bm");

            cmdPath += AddPadOption(txtSellOptionsPads.Text);
            cmdPath += AddPlanetaryOption(txtSellOptionsPlanetary.Text);
            cmdPath += AddLimitOption(numSellOptionsLimit.Value);

            cmdPath += AddAvoidOption(txtSellOptionsAvoid.Text);

            cmdPath += AddVerbosity();

            RadioButton option = grpSellOptionsSort.Controls.OfType<RadioButton>()
                .FirstOrDefault(x => x.Checked);

            if (option != null)
            {
                if (option.Text == "Price")
                {
                    cmdPath += " -P";
                }
            }

            cmdPath += AddQuotedOption(selectedCommodity);

            return cmdPath;
        }

        /// <summary>
        /// Get the buy command string.
        /// </summary>
        /// <param name="selectedCommodity">The name of the selected commodity.</param>
        /// <param name="source">The select source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetBuyCommand(
            string selectedCommodity,
            string source)
        {
            string cmdPath = "buy";

            cmdPath += AddNear(source);

            cmdPath += AddNumericOption(numBuyOptionsNearLy.Value, "--ly");
            cmdPath += AddNumericOption(numBuyOptionsAbove.Value, "--gt");
            cmdPath += AddNumericOption(numBuyOptionsBelow.Value, "--lt");
            cmdPath += AddNumericOption(numBuyOptionsSupply.Value, "--supply");

            cmdPath += AddCheckedOption(chkBuyOptionsBlkMkt.Checked, "--bm");
            cmdPath += AddCheckedOption(chkBuyOptionsOneStop.Checked, "-1");

            cmdPath += AddPadOption(txtBuyOptionsPads.Text);
            cmdPath += AddPlanetaryOption(txtBuyOptionsPlanetary.Text);
            cmdPath += AddLimitOption(numBuyOptionsLimit.Value);

            cmdPath += AddAvoidOption(txtBuyOptionsAvoid.Text);

            cmdPath += AddVerbosity();

            RadioButton option = grpBuyOptionsSort.Controls.OfType<RadioButton>()
                .FirstOrDefault(x => x.Checked);

            if (option != null)
            {
                switch (option.Text.ToLower())
                {
                    case "price":
                        cmdPath += " -P";
                        break;

                    case "supply":
                        cmdPath += " -S";
                        break;
                }
            }

            cmdPath += AddQuotedOption(selectedCommodity);

            return cmdPath;
        }

        /// <summary>
        /// Get the maarket command string.
        /// </summary>
        /// <param name="sourceSystem">The name of the source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetMarketCommand(string sourceSystem)
        {
            string cmdPath = "market";

            RadioButton option = grpMarketOptionsType.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);

            if (option != null)
            {
                switch (option.Text.ToLower())
                {
                    case "buy":
                        cmdPath += " --b";
                        break;

                    case "sell":
                        cmdPath += " --s";
                        break;
                }
            }

            cmdPath += AddVerbosity();
            cmdPath += AddQuotedOption(sourceSystem);

            return cmdPath;
        }

        /// <summary>
        /// Get the nav command string.
        /// </summary>
        /// <param name="sourceSystem">The name of the source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetNavCommand(
            string sourceSystem,
            string destinationSystem)
        {
            string cmdPath = "nav";

            cmdPath += AddTextOption(txtNavOptionsVia.Text, "--via");
            cmdPath += AddTextOption(txtNavOptionsAvoid.Text, "--avoid");
            cmdPath += AddPlanetaryOption(txtNavOptionsPlanetary.Text);
            cmdPath += AddPadOption(txtNavOptionsPads.Text);

            cmdPath += AddNumericOption(numNavOptionsLy.Value, "--ly");
            cmdPath += AddNumericOption(numNavOptionsRefuelJumps.Value, "--ref");

            cmdPath += AddCheckedOption(chkNavOptionsStations.Checked, "--stations");

            cmdPath += AddVerbosity();

            cmdPath += AddQuotedOption(sourceSystem);
            cmdPath += AddQuotedOption(destinationSystem);

            return cmdPath;
        }

        /// <summary>
        /// Get the run command string.
        /// </summary>
        /// <param name="sourceSystem">The name of the source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetRunCommand(
            string sourceSystem,
            string destinationSystem)
        {
            string cmdPath = "run";

            if (!string.IsNullOrEmpty(sourceSystem))
            {
                cmdPath += " --fr=\"{0}\"".With(sourceSystem);

                if (!string.IsNullOrEmpty(destinationSystem))
                {
                    if (chkRunOptionsTowards.Checked)
                    {
                        cmdPath += " --towards\"{0}\"".With(destinationSystem);
                    }
                }
            }

            if (!string.IsNullOrEmpty(destinationSystem) &&
                !chkRunOptionsTowards.Checked &&
                !chkRunOptionsLoop.Checked)
            {
                cmdPath += " --to=\"{0}\"".With(destinationSystem);
                cmdPath += AddNumericOption(numRunOptionsEndJumps.Value, "--end");
            }

            cmdPath += AddNumericOption(numRouteOptionsShipCapacity.Value, "--cap");
            cmdPath += AddNumericOption(numRouteOptionsLimit.Value, "--lim");
            cmdPath += AddNumericOption(numShipInsurance.Value, "--ins");
            cmdPath += AddNumericOption(numCommandersCredits.Value, "--cr");
            cmdPath += AddNumericOption(numLadenLy.Value, "--ly");
            cmdPath += AddNumericOption(numUnladenLy.Value, "--empty");
            cmdPath += AddNumericOption(numRouteOptionsGpt.Value, "--gpt");
            cmdPath += AddNumericOption(numRouteOptionsMaxGpt.Value, "--mgpt");
            cmdPath += AddNumericOption(numRouteOptionsStock.Value, "--supply");
            cmdPath += AddNumericOption(numRouteOptionsDemand.Value, "--demand");
            cmdPath += AddNumericOption(numRouteOptionsMargin.Value, "--margin");
            cmdPath += AddNumericOption(numRouteOptionsJumps.Value, "--jum");
            cmdPath += AddNumericOption(numRunOptionsStartJumps.Value, "--start");
            cmdPath += AddNumericOption(numRouteOptionsLsPenalty.Value, "--lsp");
            cmdPath += AddNumericOption(numRouteOptionsMaxLSDistance.Value, "--ls-max");
            cmdPath += AddNumericOption(numRouteOptionsPruneHops.Value, "--prune-hops");
            cmdPath += AddNumericOption(numRouteOptionsPruneScore.Value, "--prune-score");
            cmdPath += AddNumericOption(numRouteOptionsAge.Value, "--age");
            cmdPath += AddNumericOption(numRunOptionsLoopInt.Value, "--loop-int");
            cmdPath += AddNumericOption(numRunOptionsRoutes.Value, "--routes");

            cmdPath += AddCheckedOption(chkRunOptionsBlkMkt.Checked, "--bm");
            cmdPath += AddCheckedOption(chkRunOptionsDirect.Checked, "--direct");
            cmdPath += AddCheckedOption(chkRunOptionsShorten.Checked, "--shorten");
            cmdPath += AddCheckedOption(chkRunOptionsUnique.Checked, "--unique");
            cmdPath += AddCheckedOption(chkRunOptionsLoop.Checked, "--loop");
            cmdPath += AddCheckedOption(chkRunOptionsJumps.Checked, "-J");

            cmdPath += AddTextOption(txtAvoid.Text, "--avoid");
            cmdPath += AddTextOption(txtVia.Text, "--via");

            cmdPath += AddPlanetaryOption(txtRunOptionsPlanetary.Text);
            cmdPath += AddPadOption(txtPadSize.Text);

            cmdPath += AddVerbosity();

            cmdPath += string.IsNullOrEmpty(settingsRef.ExtraRunParams)
                ? string.Empty
                : " {0}".With(settingsRef.ExtraRunParams);

            if (!chkRunOptionsDirect.Checked &&
                numRouteOptionsHops.Value > 2)
            {
                cmdPath += " --hops={0}".With(numRouteOptionsHops.Value);
            }

            cmdPath += AddCheckedOption(settingsRef.ShowProgress, "--progress");
            cmdPath += AddCheckedOption(settingsRef.Summary, "--summary");

            return cmdPath;
        }

        /// <summary>
        /// Get the old data command string.
        /// </summary>
        /// <param name="sourceSystem">The name of the source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetOldDataCommand(string sourceSystem)
        {
            string cmdPath = "olddata";

            cmdPath += AddCheckedOption(chkOldDataOptionsRoute.Checked, "--route");
            cmdPath += AddNumericOption(numOldDataOptionsNearLy.Value, "--ly");
            cmdPath += AddNumericOption(numOldDataOptionsMinAge.Value, "--min-age");
            cmdPath += AddLimitOption(numOldDataOptionsLimit.Value);

            cmdPath += AddVerbosity();
            cmdPath += AddNear(sourceSystem);

            return cmdPath;
        }

        /// <summary>
        /// Get the rares command string.
        /// </summary>
        /// <param name="sourceSystem">The name of the source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetRaresCommand(string sourceSystem)
        {
            string cmdPath = "rares";

            cmdPath += AddNumericOption(numRaresOptionsLy.Value, "--ly");

            cmdPath += AddPadOption(txtRaresOptionsPads.Text);
            cmdPath += AddPlanetaryOption(txtRaresOptionsPlanetary.Text);
            cmdPath += AddLimitOption(numRaresOptionsLimit.Value);

            cmdPath += AddCheckedOption(chkRaresOptionsReverse.Checked, "--reverse");
            cmdPath += AddCheckedOption(chkRaresOptionsQuiet.Checked, "--quiet");



            RadioButton option = grpRaresOptionsType.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);

            if (option != null)
            {
                switch (option.Text.ToLower())
                {
                    case "legal":
                        cmdPath += " --legal";
                        break;

                    case "illegal":
                        cmdPath += " --illegal";
                        break;
                }
            }

            option = grpRaresOptionsSort.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);

            if (option != null)
            {
                switch (option.Text.ToLower())
                {
                    case "price":
                        cmdPath += " --P";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(txtRaresOptionsFrom.Text) && numRaresOptionsAway.Value > 0)
            {
                cmdPath += AddNumericOption(numRaresOptionsAway.Value, "--away");

                foreach (string output in txtRaresOptionsFrom.Text.Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    cmdPath += " --from=\"{0}\"".With(output);
                }
            }

            cmdPath += AddVerbosity();
            cmdPath += AddQuotedOption(sourceSystem);

            return cmdPath;
        }

        /// <summary>
        /// Set up the correct source and targeet system and selected commodity data.
        /// </summary>
        private void GetSourceAndTarget()
        {
            SourceSystem = RemoveExtraWhitespace(cboSourceSystem.Text);
            string destination = string.Empty;
            string commodity = string.Empty;

            switch (methodFromIndex)
            {
                case 0: // Run
                    destination = cboRunOptionsDestination.Text;
                    break;

                case 4: // Trade
                    destination = cboTradeOptionDestination.Text;
                    break;

                case 7: // Navigation
                    destination = cboNavOptionsDestination.Text;
                    break;

                default:
                    TargetSystem = string.Empty;
                    break;
            }

            if (!string.IsNullOrEmpty(destination))
            {
                TargetSystem = RemoveExtraWhitespace(destination);
            }

            switch (methodFromIndex)
            {
                case 1: // Buy
                    commodity = cboBuyOptionsCommodities.Text;
                    break;

                case 2: // Sell
                    commodity = cboSellOptionsCommodities.Text;
                    break;

                default:
                    SelectedCommodity = string.Empty;
                    break;
            }

            if (!string.IsNullOrEmpty(commodity))
            {
                SelectedCommodity = RemoveExtraWhitespace(commodity);
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
            DisablebtnStarts();

            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Get the trade command string.
        /// </summary>
        /// <param name="sourceSystem">The name of the source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetTradeCommand(
            string sourceSystem,
            string destinationSystem)
        {
            string cmdPath = "trade";

            cmdPath += AddVerbosity();

            cmdPath += AddQuotedOption(sourceSystem);
            cmdPath += AddQuotedOption(destinationSystem);

            return cmdPath;
        }

        private void InsertAtGridRow_Click(object sender, EventArgs e)
        {
            if (grdPilotsLog.Rows.Count > 0)
            {
                // add a row with the timestamp of the selected row
                // basically an insert-below-index when we use select(*)
                string timestamp = grdPilotsLog.Rows[dRowIndex].Cells["Timestamp"].Value.ToString();
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

        private void LocalFilterCheckBoxChanged(object sender, EventArgs e)
        {
            btnLocalOptionsReset.Enabled
                = chkLocalOptionsRearm.Checked
                || chkLocalOptionsRefuel.Checked
                || chkLocalOptionsRepair.Checked
                || chkLocalOptionsCommodities.Checked
                || chkLocalOptionsBlkMkt.Checked
                || chkLocalOptionsOutfitting.Checked
                || chkLocalOptionsShipyard.Checked
                || chkLocalOptionsStations.Checked
                || chkLocalOptionsTrading.Checked;

            btnLocalOptionsAll.Enabled
                = !chkLocalOptionsRearm.Checked
                || !chkLocalOptionsRefuel.Checked
                || !chkLocalOptionsRepair.Checked
                || !chkLocalOptionsCommodities.Checked
                || !chkLocalOptionsBlkMkt.Checked
                || !chkLocalOptionsOutfitting.Checked
                || !chkLocalOptionsShipyard.Checked
                || !chkLocalOptionsStations.Checked
                || !chkLocalOptionsTrading.Checked;
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
            // serialize window data to the default config file
            CopySettingsFromForm();
            settingsRef.LocationParent = SaveWinLoc(this);
            settingsRef.SizeParent = SaveWinSize(this);

            SaveSettingsToIniFile();
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
                    SplashScreen.CloseForm();

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
            if (!string.IsNullOrEmpty(settingsRef.LastUsedConfig))
            {
                LoadSettings();
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
            // Call the refresh methof for the run options panel.
            OptionsPanelRefresh(panRunOptions);

            // start the database uploader
            backgroundWorker6.RunWorkerAsync();

            SplashScreen.CloseForm();
        }

        private void MethodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            methodIndex = cboMethod.SelectedIndex;
            MethodSelectState();

            cboMethod.Enabled = true;
        }

        private void MethodSelectState()
        {
            SuspendLayout();

            // Firstly hide/disable the currently enabled options panel.
            HideAllOptionPanels();
            // ShowOrHideOptionsPanel(methodFromIndex, false);

            // Now show/enable the required options panel.
            ShowOrHideOptionsPanel(methodIndex);

            // Save the currently enabled options panel.
            methodFromIndex = methodIndex;

            ResumeLayout();
        }

        private void HideAllOptionPanels()
        {
            bool show = false;
            ShowOptions(panRunOptions, show);
            EnableOptions(panRouteOptions, show);
            ShowOptions(panBuyOptions, show);
            ShowOptions(panSellOptions, show);
            ShowOptions(panRaresOptions, show);
            ShowOptions(panTradeOptions, show);
            ShowOptions(panMarketOptions, show);
            ShowOptions(panShipVendorOptions, show);
            ShowOptions(panNavOptions, show);
            ShowOptions(panOldDataOptions, show);
            ShowOptions(panLocalOptions, show);
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

        private void NotesClearMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNotes.Text))
            {
                txtNotes.Text = string.Empty;

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

        /// <summary>
        /// Check to see if the options panel has a pad size text box and if it is blanks then
        /// set it to the same value as the currently selected ship.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptionsPanel_StateChanged(object sender, EventArgs e)
        {
            OptionsPanelRefresh((Panel)sender);
        }

        /// <summary>
        /// Check to see if the options panel has a pad size text box and if it is blanks then
        /// set it to the same value as the currently selected ship.
        /// </summary>
        /// <param name="panel"></param>
        private void OptionsPanelRefresh(Panel panel)
        {
            SetPadSizes(panel);
            SetDestinations(panel);
            SetCommodities(panel);
            SetAvailableShips(panel);
        }

        /// <summary>
        /// Ensure that the pad sizes are M, L, ? or a combination of these.
        /// </summary>
        /// <param name="sender">The text box.</param>
        /// <param name="e">The current key press.</param>
        private void PadSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox padSizes = ((TextBox)sender);

            string key = e.KeyChar.ToString().ToUpper();
            string selected = padSizes.Text;

            // filter for valid chars
            if ("ML?".Contains(key))
            {
                if (selected.Contains(key))
                {
                    selected = selected.Replace(key, " ");
                }
                else
                {
                    selected += key;
                }

                if (selected.Trim().Length == 0)
                {
                    selected = string.Empty;
                }

                padSizes.Text = ContainsPadSizes(selected);
            }

            e.Handled = true;
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
            RichTextBox clickedControl = (RichTextBox)this.mnuStrip1.SourceControl;

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
                grdPilotsLog.ClearSelection(); // prevent weirdness
                pRowIndex = int.Parse(localTable.Rows[e.RowIndex][0].ToString());
                dRowIndex = e.RowIndex;
                grdPilotsLog.Rows[dRowIndex].Selected = true;
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
                && grdPilotsLog.SelectedRows.Count > 0)
            {
                int rowIndex = -1;
                int.TryParse(grdPilotsLog.Rows[e.Row.Index].Cells[0].Value.ToString(), out rowIndex);

                if (rowIndex >= 0)
                {
                    if (grdPilotsLog.SelectedRows.Count == 1)
                    {
                        RemoveDBRow(tdhDBConn, rowIndex);
                        UpdateLocalTable(tdhDBConn);
                        memoryCache.RemoveRow(e.Row.Index, rowIndex);
                    }
                    else if (grdPilotsLog.SelectedRows.Count > 1 && dgRowIDIndexer.Count == 0)
                    {
                        // let's batch the commits for performance
                        batchedRowCount = grdPilotsLog.SelectedRows.Count;
                        foreach (DataGridViewRow r in grdPilotsLog.SelectedRows)
                        {
                            int curRowIndex = int.Parse(r.Cells[0].Value.ToString());
                            dgRowIndexer.Add(e.Row.Index);
                            dgRowIDIndexer.Add(curRowIndex);
                        }

                        grdPilotsLog.Visible = false; // prevent retrieval
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
                        grdPilotsLog.Visible = true; // re-enable retrieval
                    }
                }
            }
        }

        /// <summary>
        /// Ensure that the planetary options are M, L, ? or a combination of these.
        /// </summary>
        /// <param name="sender">The text box.</param>
        /// <param name="e">The current key press.</param>
        private void Planetary_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox planetary = ((TextBox)sender);

            string key = e.KeyChar.ToString().ToUpper();
            string selected = planetary.Text;

            // filter for valid chars
            if ("YN?".Contains(key))
            {
                if (selected.Contains(key))
                {
                    selected = selected.Replace(key, " ");
                }
                else
                {
                    selected += key;
                }

                if (selected.Trim().Length == 0)
                {
                    selected = string.Empty;
                }

                planetary.Text = ContainsPlanetary(selected);
            }

            e.Handled = true;
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
            RichTextBox clickedControl = (RichTextBox)this.mnuStrip1.SourceControl;
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
                        //stream.WriteLine("Buy " + commodityComboBox.Text.ToString()
                        //    + " (near "
                        //    + srcSystemComboBox.Text.ToString()
                        //    + "):\n"
                        //    + Clipboard.GetData(DataFormats.Text).ToString());
                    }
                    else if (methodIndex == 2)
                    {
                        //stream.WriteLine("Sell " + commodityComboBox.Text.ToString()
                        //    + " (near "
                        //    + srcSystemComboBox.Text.ToString()
                        //    + "):\n"
                        //    + Clipboard.GetData(DataFormats.Text).ToString());
                    }
                    else
                    {
                        stream.WriteLine(Clipboard.GetData(DataFormats.Text).ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Refresh the currently enabled panel.
        /// </summary>
        private void RefreshCurrentOptionsPanel()
        {
            switch (methodFromIndex)
            {
                case 0: // Run
                    OptionsPanelRefresh(panRunOptions);
                    break;

                case 1: // Buy
                    OptionsPanelRefresh(panBuyOptions);
                    break;

                case 2: // Sell
                    OptionsPanelRefresh(panSellOptions);
                    break;

                case 3: // Rares
                    OptionsPanelRefresh(panRaresOptions);
                    break;

                case 4: // Trade
                    OptionsPanelRefresh(panTradeOptions);
                    break;

                case 5: // Market
                    OptionsPanelRefresh(panMarketOptions);
                    break;

                case 6: // Ship vendor
                    OptionsPanelRefresh(panShipVendorOptions);
                    break;

                case 7: // Navigation
                    OptionsPanelRefresh(panNavOptions);
                    break;

                case 8: // Old data
                    OptionsPanelRefresh(panOldDataOptions);
                    break;

                case 9: // Local
                    OptionsPanelRefresh(panLocalOptions);
                    break;

                default:
                    // Do nothing.
                    break;
            }
        }

        private void RemoveAtGridRow_Click(object sender, EventArgs e)
        {
            if (grdPilotsLog.Rows.Count > 0)
            {
                RemoveDBRow(tdhDBConn, pRowIndex);
                UpdateLocalTable(tdhDBConn);
                memoryCache.RemoveRow(dRowIndex, pRowIndex);
                grdPilotsLog.InvalidateRow(dRowIndex);
            }
        }

        private void ResetLocalFilters(object sender, EventArgs e)
        {
            foreach (CheckBox control in panLocalOptions.Controls.OfType<CheckBox>())
            {
                control.Checked = false;
            }

            btnLocalOptionsAll.Enabled = true;
            btnLocalOptionsReset.Enabled = false;
        }

        private void RunMethodResetState()
        {
        }

        private void SavePage1MenuItem_Click(object sender, EventArgs e)
        {
            if (rtbOutput.SelectedText.Length > 0)
            {
                WriteSavedPage(rtbOutput.SelectedText, savedFile1);
            }
            else
            {
                WriteSavedPage(rtbOutput.Text, savedFile1);
            }
        }

        private void SavePage2MenuItem_Click(object sender, EventArgs e)
        {
            if (rtbOutput.SelectedText.Length > 0)
            {
                WriteSavedPage(rtbOutput.SelectedText, savedFile2);
            }
            else
            {
                WriteSavedPage(rtbOutput.Text, savedFile2);
            }
        }

        private void SavePage3MenuItem_Click(object sender, EventArgs e)
        {
            if (rtbOutput.SelectedText.Length > 0)
            {
                WriteSavedPage(rtbOutput.SelectedText, savedFile3);
            }
            else
            {
                WriteSavedPage(rtbOutput.Text, savedFile3);
            }
        }

        private void SelectMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.mnuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.SelectAll();
        }

        /// <summary>
        /// See if the specified panel has a ships combo box and set it.
        /// </summary>
        /// <param name="panel">The panel to be checked.</param>
        private void SetAvailableShips(Panel panel)
        {
            // See if this options panel has a pad sizes text box.
            ComboBox ships = panel.Controls.OfType<ComboBox>()
                .FirstOrDefault(x => x.Name.Contains("Ships"));

            if (ships != null)
            {
                ships.DataSource = null;

                // shipvendor textbox
                if (outputStationShips.Count > 0)
                {
                    ships.DataSource = outputStationShips;
                }
            }
        }

        /// <summary>
        /// Set the commodities.
        /// </summary>
        /// <param name="panel">The panel to be checked.</param>
        private void SetCommodities(Panel panel)
        {
            // See if this options panel has a pad sizes text box.
            ComboBox commodities = panel.Controls.OfType<ComboBox>()
                .FirstOrDefault(x => x.Name.Contains("Commodities"));

            if (commodities != null)
            {
                // Detach and reattach the destinations data source.
                commodities.DataSource = null;

                switch (panel.Name)
                {
                    case "panBuyOptions":
                        // All commodities & ships
                        commodities.DataSource = outputItems;
                        break;

                    case "panSellOptions":
                        // Just commodities.
                        commodities.DataSource = CommoditiesList;
                        break;
                }
            }
        }

        /// <summary>
        /// Set the destinations.
        /// </summary>
        /// <param name="panel">The panel to be checked.</param>
        private void SetDestinations(Panel panel)
        {
            // See if this options panel has a pad sizes text box.
            ComboBox destinations = panel.Controls.OfType<ComboBox>()
                .FirstOrDefault(x => x.Name.Contains("Destination"));

            if (destinations != null)
            {
                // Detach and reattach the destinations data source.
                destinations.DataSource = null;
                destinations.DataSource = DestinationList;

                destinations.AutoCompleteCustomSource.Clear();
                destinations.AutoCompleteCustomSource.AddRange(outputSysStnNames.ToArray());
            }
        }

        private void SetLocalFilters(object sender, EventArgs e)
        {
            foreach (CheckBox control in panLocalOptions.Controls.OfType<CheckBox>())
            {
                control.Checked = true;
            }

            btnLocalOptionsAll.Enabled = false;
            btnLocalOptionsReset.Enabled = true;
        }

        /// <summary>
        /// See if the specified panel has a pad sizes text box and set if blank.
        /// </summary>
        /// <param name="panel">The panel to be checked.</param>
        private void SetPadSizes(Panel panel)
        {
            // See if this options panel has a pad sizes text box.
            TextBox padsizes = panel.Controls.OfType<TextBox>()
                .FirstOrDefault(x => x.Name.Contains("Pads"));

            if (padsizes != null && string.IsNullOrEmpty(padsizes.Text))
            {
                // It has but it is empty so set it to the same value as the currently selected ship.
                padsizes.Text = txtPadSize.Text;
            }
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
            panel.Enabled = true;

            foreach (Control ctrl in panel.Controls)
            {
                ctrl.Enabled = state;
            }
        }

        private void ShortenCheckBox_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboRunOptionsDestination.Text))
            {
                if (!chkRunOptionsShorten.Checked)
                {
                    chkRunOptionsLoop.Checked = false;
                    chkRunOptionsTowards.Checked = false;
                }
                else if (chkRunOptionsShorten.Checked)
                {
                    if (chkRunOptionsLoop.Checked)
                    {
                        chkRunOptionsLoop.Checked = false;
                    }

                    chkRunOptionsShorten.Checked = true;
                    chkRunOptionsTowards.Checked = false;
                }
            }
            else
            {
                chkRunOptionsShorten.Checked = false;
            }
        }

        /// <summary>
        /// Show or hide the specified options panel.
        /// </summary>
        /// <param name="panel">The panel to be shown or hidden.</param>
        /// <param name="show">True to show the panel.</param>
        private void ShowOptions(
            Panel panel,
            bool show = true)
        {
            panel.Visible = show;
        }

        /// <summary>
        /// Show/enable or hide/disable the panel specified by the index.
        /// </summary>
        /// <param name="optionsIndex">The index of the required panel.</param>
        /// <param name="show">True to show/enable.</param>
        private void ShowOrHideOptionsPanel(
            int optionsIndex,
            bool show = true)
        {
            switch (optionsIndex)
            {
                case 0: // Run
                    ShowOptions(panRunOptions, show);
                    EnableOptions(panRouteOptions, show);
                    break;

                case 1: // Buy
                    ShowOptions(panBuyOptions, show);
                    break;

                case 2: // Sell
                    ShowOptions(panSellOptions, show);
                    break;

                case 3: // Rares
                    ShowOptions(panRaresOptions, show);
                    break;

                case 4: // Trade
                    ShowOptions(panTradeOptions, show);
                    break;

                case 5: // Market
                    ShowOptions(panMarketOptions, show);
                    break;

                case 6: // Ship vendor
                    ShowOptions(panShipVendorOptions, show);
                    break;

                case 7: // Navigation
                    ShowOptions(panNavOptions, show);
                    break;

                case 8: // Old data
                    ShowOptions(panOldDataOptions, show);
                    break;

                case 9: // Local
                    ShowOptions(panLocalOptions, show);
                    break;

                default:
                    // Do nothing.
                    break;
            }
        }

        private void SrcSystemComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (methodIndex != 10)
            {
                // make sure we filter unwanted characters from the string
                string filteredString = RemoveExtraWhitespace(cboSourceSystem.Text);

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
                        cboSourceSystem.Text = tokens[0];
                    }
                    else if (!filteredString.Contains("/"))
                    {
                        cboSourceSystem.Text = string.Empty; // wipe entirely if only a system is left
                    }

                    e.Handled = true;
                }
            }
        }

        private void SrcSystemComboBox_TextChanged(object sender, EventArgs e)
        {
            // wait for the user to type a few characters
            if (cboSourceSystem.Text.Length > 3 && methodIndex != 10)
            {
                string filteredString = RemoveExtraWhitespace(cboSourceSystem.Text);
                PopulateStationPanel(filteredString);

                chkRunOptionsTowards.Enabled = cboRunOptionsDestination.Text.Length > 3; // requires "--fr"
            }
            else
            {
                chkRunOptionsTowards.Enabled = false;
            }
        }

        private void StationDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            //stationIndex = stationDropDown.SelectedIndex;

            //if (methodIndex == 6)
            //{
            //    // if we're in ShipVendor mode
            //    if (stationIndex == 2)
            //    {
            //        // disable when list'ing
            //        cboShipsSold.Enabled = false;
            //        lblShipsSold.Enabled = false;
            //    }
            //    else
            //    {
            //        cboShipsSold.Enabled = true;
            //        lblShipsSold.Enabled = true;
            //    }
            //}
        }

        private void SwapSourceAndDestination(object sender, EventArgs e)
        {
            // Locate the destination control for the currently enabled options panel.
            ComboBox destination = ((Button)sender).Parent.Controls.OfType<ComboBox>()
                .FirstOrDefault(x => x.Name.ToLower().Contains("destination"));

            if (destination != null)
            {
                string target = RemoveExtraWhitespace(destination.Text);
                string source = RemoveExtraWhitespace(cboSourceSystem.Text);

                destination.Text = source;
                cboSourceSystem.Text = target;
            }
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fromPane == 5) { /* Pilot's Log tab */ }
            else if (fromPane == 4)
            {
                // if we're coming from the notes pane we should save when we switch
                txtNotes.SaveFile(notesFile, RichTextBoxStreamType.PlainText);
            }

            if (tabControl1.SelectedTab == tabControl1.TabPages["outputPage"] && !string.IsNullOrEmpty(rtbOutput.Text))
            {
                string filteredOutput = FilterOutput(rtbOutput.Text);
                runOutputState = IsValidRunOutput(filteredOutput);

                // check for parsable Run output
                if (runOutputState > -1)
                {
                    hasParsed = false; // reset the semaphore
                    tv_outputBox = filteredOutput; // copy our validated input
                    btnMiniMode.Enabled = true;
                }
                else
                {
                    btnMiniMode.Enabled = false;
                }

                rtbOutput.Focus(); // always focus our text box

                pagOutput.Font = new Font(pagOutput.Font, FontStyle.Regular); // reset the font
                fromPane = 0;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["logPage"])
            {
                fromPane = 5;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["notesPage"] && CheckIfFileOpens(notesFile))
            {
                txtNotes.LoadFile(notesFile, RichTextBoxStreamType.PlainText);

                txtNotes.Focus();
                fromPane = 4;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["savedPage1"] && CheckIfFileOpens(savedFile1))
            {
                rtbSaved1.Focus();

                if (File.Exists(savedFile1))
                {
                    rtbSaved1.LoadFile(savedFile1, RichTextBoxStreamType.PlainText);
                }

                string filteredOutput = FilterOutput(rtbSaved1.Text);
                runOutputState = IsValidRunOutput(filteredOutput);

                // check for parsable Run output
                if (runOutputState > -1)
                {
                    hasParsed = false; // reset the semaphore
                    tv_outputBox = filteredOutput; // copy our validated input
                    btnMiniMode.Enabled = true;
                }
                else
                {
                    btnMiniMode.Enabled = false;
                }

                rtbSaved1.Focus();
                fromPane = 1;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["savedPage2"] && CheckIfFileOpens(savedFile2))
            {
                rtbSaved2.Focus();

                if (File.Exists(savedFile2))
                {
                    rtbSaved2.LoadFile(savedFile2, RichTextBoxStreamType.PlainText);
                }

                string filteredOutput = FilterOutput(rtbSaved2.Text);
                runOutputState = IsValidRunOutput(filteredOutput);

                // check for parsable Run output
                if (runOutputState > -1)
                {
                    hasParsed = false; // reset the semaphore
                    tv_outputBox = filteredOutput; // copy our validated input
                    btnMiniMode.Enabled = true;
                }
                else
                {
                    btnMiniMode.Enabled = false;
                }

                rtbSaved2.Focus();
                fromPane = 2;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["savedPage3"] && CheckIfFileOpens(savedFile3))
            {
                rtbSaved3.Focus();

                if (File.Exists(savedFile3))
                {
                    rtbSaved3.LoadFile(savedFile3, RichTextBoxStreamType.PlainText);
                }

                string filteredOutput = FilterOutput(rtbSaved3.Text);
                runOutputState = IsValidRunOutput(filteredOutput);

                // check for parsable Run output
                if (runOutputState > -1)
                {
                    hasParsed = false; // reset the semaphore
                    tv_outputBox = filteredOutput; // copy our validated input
                    btnMiniMode.Enabled = true;
                }
                else
                {
                    btnMiniMode.Enabled = false;
                }

                rtbSaved3.Focus();
                fromPane = 3;
            }
        }

        private void Td_outputBox_TextChanged(object sender, EventArgs e)
        {
            if (buttonCaller == 5)
            {
                // catch the database update button, we want to see its output
                rtbOutput.SelectionStart = rtbOutput.Text.Length;
                rtbOutput.ScrollToCaret();
                rtbOutput.Refresh();
            }
        }

        private void TestSystemsTimer_Delegate(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine(string.Format("testSystems Firing at: {0}", CurrentTimestamp()));

            if (!backgroundWorker6.IsBusy && !settingsRef.DisableNetLogs && !string.IsNullOrEmpty(settingsRef.NetLogPath))
            {
                backgroundWorker6.RunWorkerAsync();
            }
        }

        private void TrackerLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/MarkAusten/TDHelper/issues/new");
        }

        private void TxtAvoid_TextChanged(object sender, EventArgs e)
        {
            // account for startJumpsBox
            if (numRunOptionsStartJumps.Value > 0)
                settingsRef.Avoid = txtAvoid.Text;
        }

        private void TxtLocalPlanetary_TextChanged(object sender, EventArgs e)
        {
            txtLocalOptionsPlanetary.Text = ContainsPlanetary(txtLocalOptionsPlanetary.Text);
        }

        private void UniqueCheckBox_Click(object sender, EventArgs e)
        {
            if (chkRunOptionsLoop.Checked)
            {
                chkRunOptionsLoop.Checked = false;
                chkRunOptionsUnique.Checked = true;
            }
        }

        private void ValidateDestForEndJumps()
        {
            if (!string.IsNullOrEmpty(cboRunOptionsDestination.Text) && methodIndex != 4)
            {
                numRunOptionsEndJumps.Enabled = true;
                lblRunOptionsEndJumps.Enabled = true;
            }
            else if (methodIndex != 4)
            {
                numRunOptionsEndJumps.Enabled = false;
                lblRunOptionsEndJumps.Enabled = false;
            }
        }

        private void BuyOptionsPrice_CheckedChanged(object sender, EventArgs e)
        {
            if (optBuyOptionsPrice.Checked)
            {
                chkBuyOptionsOneStop.Checked = false;
            }
        }

        private void BuyOptionsOneStop_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBuyOptionsOneStop.Checked)
            {
                optBuyOptionsPrice.Checked = false;
            }
        }

        /// <summary>
        /// Caled on clicking one of the distance menu items. 
        /// </summary>
        /// <param name="sender">The menu item clicked.</param>
        /// <param name="e">The event arguments.</param>
        private void DistanceMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            var sourceControl = ((ContextMenuStrip)(menuItem.GetCurrentParent())).SourceControl;

            if (sourceControl is NumericUpDown)
            {
                SetDecimalControlValue((NumericUpDown)sourceControl, menuItem.Name);
            }
            else if (sourceControl is TextBox)
            {
                SetTextBoxControlValue((TextBox)sourceControl, menuItem.Name);
            }
            else if (sourceControl is ComboBox)
            {
                SetComboBoxControlValue((ComboBox)sourceControl, menuItem.Name);
            }
        }

        /// <summary>
        /// The the value of the specified control to the correct vale.
        /// </summary>
        /// <param name="control">The control to be set.</param>
        /// <param name="requiredSource">The key to the source value.</param>
        private void SetDecimalControlValue(
            NumericUpDown control, 
            string requiredSource)
        {
            // Determine the corect source control.
            decimal source = 0M;

            switch (requiredSource)
            {
                case "mnuLadenLY":
                    source = numLadenLy.Value;
                    break;

                case "mnuUnladenLY":
                    source = numUnladenLy.Value;
                    break;

                case "mnuCapacity":
                    source = numRouteOptionsShipCapacity.Value;
                    break;

                case "mnuReset":
                    source = control.Minimum;
                    break;
            }

            control.Value = source;
        }

        /// <summary>
        /// The the value of the specified control to the correct vale.
        /// </summary>
        /// <param name="control">The control to be set.</param>
        /// <param name="requiredSource">The key to the source value.</param>
        private void SetTextBoxControlValue(
            TextBox control, 
            string requiredSource)
        {
            // Determine the corect source control.
            string source = string.Empty;

            switch (requiredSource)
            {
                case "mnuReset":
                    source = string.Empty;
                    break;
            }

            control.Text = source;
        }

        /// <summary>
        /// The the value of the specified control to the correct vale.
        /// </summary>
        /// <param name="control">The control to be set.</param>
        /// <param name="requiredSource">The key to the source value.</param>
        private void SetComboBoxControlValue(
            ComboBox control, 
            string requiredSource)
        {
            // Determine the corect source control.
            string source = string.Empty;

            switch (requiredSource)
            {
                case "mnuReset":
                    source = string.Empty;
                    break;
            }

            control.Text = source;
        }

        /// <summary>
        /// Set up the context menu state depending on the controls clicked.
        /// </summary>
        /// <param name="sender">The context menu strip.</param>
        /// <param name="e">The event arguments.</param>
        private void SetValuesMenu_Opening(object sender, CancelEventArgs e)
        {
            string clickedControlName = ((ContextMenuStrip)sender).SourceControl.Name;

            // Setup the menu.
            switch (clickedControlName)
            {
                case "numBuyOptionsSupply":
                case "numSellOptionsDemand":
                case "numRouteOptionsDemand":
                case "numRouteOptionsStock":
                    mnuLadenLY.Visible = false;
                    mnuUnladenLY.Visible = false;
                    mnuCapacity.Visible = true;
                    mnuSep3.Visible = true;
                    break;

                case "numBuyOptionsNearLy":
                case "numLocalOptionsLy":
                case "numNavOptionsLy":
                case "numOldDataOptionsNearLy":
                case "numRaresOptionsLy":
                case "numSellOptionsNearLy":
                    mnuLadenLY.Visible = true;
                    mnuUnladenLY.Visible = true;
                    mnuCapacity.Visible = false;
                    mnuSep3.Visible = true;
                    break;

                default:
                    mnuLadenLY.Visible = false;
                    mnuUnladenLY.Visible = false;
                    mnuCapacity.Visible = false;
                    mnuSep3.Visible = false;
                    break;
            }

            mnuReset.Enabled = true;
            mnuSep2.Visible = false;
        }
    }
}