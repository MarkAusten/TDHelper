using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

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

namespace TDHelper
{
    public partial class MainForm : Form
    {
        #region FormProps

        private const int SnapDist = 10;
        private readonly List<string> DestinationList = new List<string>();
        private readonly IList<Panel> optionPanels = new List<Panel>();
        private readonly IDictionary<string, string> ShipTranslation = new Dictionary<string, string>();
        private readonly List<string> SourceList = new List<string>();
        private string appVersion = "v{0}".With(Application.ProductVersion);

        private int buttonCaller;
        private List<string> CommoditiesList = new List<string>();
        private int hasUpdated;
        private int methodFromIndex;
        private int methodIndex;
        private string notesFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "saved_notes.txt");
        private int procCode = -1;
        private string savedFile1 = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "saved_1.txt");
        private string savedFile2 = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "saved_2.txt");
        private string savedFile3 = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "saved_3.txt");
        private string SelectedCommodity = string.Empty;
        private List<string> ShipList = new List<string>();
        private string SourceSystem = string.Empty;
        private string TargetSystem = string.Empty;
        private System.Timers.Timer testSystemsTimer = new System.Timers.Timer();
        private string tv_outputBox = string.Empty;
        private CultureInfo userCulture = CultureInfo.CurrentCulture;
        private IList<string> validConfigs = new List<string>();

        private bool RefreshingDestinations = false;

        #endregion FormProps

        public MainForm()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint
                    | ControlStyles.UserPaint
                    | ControlStyles.DoubleBuffer, true);

            numCommandersCredits.Maximum = Decimal.MaxValue;

            SplashScreen.SetStatus("Building settings...");
            BuildSettings();

            SplashScreen.SetStatus("Reading configuration...");
            CopySettingsFromConfig();

            // And validate the settings to get rid of any nonsense.
            ValidateSettings();

            SplashScreen.SetStatus("Set connections...");
            SetConnections();

            // Let's change the title
            SplashScreen.SetStatus("Set form title...");
            SetFormTitle(this);

            testSystemsTimer.AutoReset = false;
            testSystemsTimer.Interval = 10000;
            testSystemsTimer.Elapsed += this.TestSystemsTimer_Delegate;

            btnCmdrProfile.Enabled = ValidateEdce();

            SplashScreen.SetStatus("Set ititial state...");
            SetOptionPanelList();
            ShowOrHideOptionsPanel(0);
        }

        /// <summary>
        /// Add the avoid option.
        /// </summary>
        /// <param name="toAvoid">The option to populate the avoid parameter.</param>
        /// <returns>The avoid parameter.</returns>
        private string AddAvoidOption(string toAvoid)
        {
            return string.IsNullOrEmpty(toAvoid)
                ? string.Empty
                : " --avoid=\"{0}\"".With(toAvoid);
        }

        /// <summary>
        /// Add a checked option.
        /// </summary>
        /// <param name="toAdd">True to add the parameter.</param>
        /// <param name="option">The parameter to add.</param>
        /// <returns>The parameter or a blank string.</returns>
        private string AddCheckedOption(
            bool toAdd,
            string option)
        {
            return toAdd
                ? " --{0}".With(option)
                : string.Empty;
        }

        /// <summary>
        /// Add the limit parameter if required.
        /// </summary>
        /// <param name="limit">The settings value.</param>
        /// <returns>The limit parameter.</returns>
        private string AddLimitOption(decimal limit)
        {
            return AddNumericOption((limit == 0 ? 42 : limit), "limit");
        }

        /// <summary>
        /// Add the near parameter if required.
        /// </summary>
        /// <param name="system">The near system to add.</param>
        /// <returns>The near parameter.</returns>
        private string AddNear(string system)
        {
            return AddTextOption(system, "near", true);
        }

        /// <summary>
        /// Add a numeric parameter.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="option">The parameter option.</param>
        /// <returns>The parameter string.</returns>
        private string AddNumericOption(
            decimal value,
            string option)
        {
            return value == 0
                ? string.Empty
                : " --{0}={1}".With(option, value);
        }

        /// <summary>
        /// Add the pad modifiers if required.
        /// </summary>
        /// <param name="pads">The settings value.</param>
        /// <returns>the planetary modifiers.</returns>
        private string AddPadOption(string pads)
        {
            return AddTextOption(pads, "pad");
        }

        /// <summary>
        /// Add the planetary modifiers if required.
        /// </summary>
        /// <param name="planetary">The settings value.</param>
        /// <returns>the planetary modifiers.</returns>
        private string AddPlanetaryOption(string planetary)
        {
            return AddTextOption(planetary, "planetary");
        }

        /// <summary>
        /// Add a quoted parameter.
        /// </summary>
        /// <param name="value">The value to be added in quotes.</param>
        /// <returns>The quoted parameter.</returns>
        private string AddQuotedOption(string value)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : " \"{0}\"".With(value);
        }

        /// <summary>
        /// Add a text parameter.
        /// </summary>
        /// <param name="value">the value to be added.</param>
        /// <param name="option">The parameter option.</param>
        /// <returns>The parameter string.</returns>
        private string AddTextOption(
            string value,
            string option,
            bool quoted = false)
        {
            if (quoted)
            {
                value = "\"{0}\"".With(value);
            }

            return string.IsNullOrEmpty(value)
                ? string.Empty
                : " --{0}={1}".With(option, value);
        }

        /// <summary>
        /// Add the verbosity parameter.
        /// </summary>
        /// <returns>The parameter string.</returns>
        private string AddVerbosity()
        {
            string verb = string.Empty;

            switch (settingsRef.Verbosity)
            {

                case 1:
                    verb = "-v";
                    break;

                case 2:
                    verb = "-vv";
                    break;

                case 3:
                    verb = "-vvv";
                    break;

                default:
                    verb = string.Empty;
                    break;
            }

            return string.IsNullOrEmpty(verb)
                ? string.Empty
                : " {0}".With(verb);
        }

        private void AltConfigBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            settingsRef.LastUsedConfig = cboCommandersShips.Text;
            SetShipList();
        }

        /// <summary>
        /// This worker delegate updates the commodities and recent systems lists
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // let the refresh methods decide what to refresh
            this.Invoke(new Action(() =>
            {
                BuildOutput(buttonCaller == 16);
            }));
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableBtnStarts();

            // we were called by getSystemButton
            if (buttonCaller == 2 && output_unclean.Count > 0)
            {
                // skip our favorites to get our most recent system/station
                if (output_unclean.Count > 0 && currentMarkedStations.Count > 0)
                {
                    cboSourceSystem.SelectedIndex = 1 + currentMarkedStations.Count;
                }
                else if (output_unclean.Count > 0)
                {
                    cboSourceSystem.SelectedIndex = 1; // if the favorites are empty
                }

                cboSourceSystem.Focus();
            }

            hasUpdated = -1; // reset the updated semaphore
            buttonCaller = 0; // reset caller semaphore
        }

        /// <summary>
        /// This worker delegate is the main work horse for the application.
        /// it controls the logic for all the primary commands.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            commandString = GetMethodCommandString(methodIndex);

            // pass the built command-line to the delegate
            if (string.IsNullOrEmpty(commandString))
            {
                PlayAlert();
            }
            else
            {
                // Avoid issues by enforcing InvariantCulture
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                td_proc = new Process();
                td_proc.StartInfo.FileName = settingsRef.PythonPath;

                if (!settingsRef.PythonPath.EndsWith("trade.exe", StringComparison.OrdinalIgnoreCase))
                {
                    commandString = "-u \"" + Path.Combine(settingsRef.TDPath, "trade.py") + "\" " + commandString;
                }

                DoTDProc(commandString);
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
                {
                    tabControl1.SelectedTab = pagOutput;
                }

                // assume we're coming from getUpdatedPricesFile()
                if (!string.IsNullOrEmpty(settingsRef.ImportPath) && buttonCaller == 11)
                {
                    CleanUpdatedPricesFile();
                }
                else if (!backgroundWorker1.IsBusy && buttonCaller == 16)
                {
                    // force a db sync if we're marked
                    backgroundWorker1.RunWorkerAsync();
                }

                // reenable after uncancellable task is done
                EnableBtnStarts();

                td_proc.Dispose();

                // make a sound when we're done with a long operation (>10s)
                if ((stopwatch.ElapsedMilliseconds > 10000 && buttonCaller != 5) || buttonCaller == 20)
                {
                    PlayAlert(); // when not marked as cancelled, or explicit
                }

                methodIndex = cboMethod.SelectedIndex;

                if (buttonCaller == 4 || buttonCaller == 1)
                {
                    CheckForParsableOutput(rtbOutput);
                }
                else
                {
                    btnMiniMode.Enabled = false;
                }

                buttonCaller = 0;
            }
        }

        /// <summary>
        /// This worker delegate is just a thread on which the background timer runs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            stopwatch.Start(); // start the timer

            while (stopwatch.IsRunning)
            {
               Thread.Sleep(1000);

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

        /// <summary>
        /// This worker delegate is for the update process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            if (DBUpdateCommandString == "ANALYZE")
            {
                // only start the stopwatch for callers that run in the background
                if (!backgroundWorker3.IsBusy)
                {
                    backgroundWorker3.RunWorkerAsync();
                }
                else
                {
                    stopwatch.Start();
                }

                circularBuffer.Clear();

                StackCircularBuffer("Analysing database...\n");

                AnalyseAllDatabases();
            }
            else
            {
                GetDataUpdates(); // update conditionally
                DoTDProc(commandString); // pass this to the worker delegate
            }

            commandString = string.Empty; // reset path for thread safety
        }

        private void BackgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!backgroundWorker4.IsBusy)
            {
                stopwatch.Stop(); // stop the timer
                circularBuffer = new System.Text.StringBuilder(2 * circularBufferSize);

                EnableBtnStarts();
                td_proc.Dispose();

                // we have to update the comboboxes now
                if (!backgroundWorker1.IsBusy)
                {
                    buttonCaller = 16; // mark us as needing a full refresh
                    backgroundWorker1.RunWorkerAsync();
                }

                OptimiseAllDatabases();

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
                        this.rtbOutput.Text += "We're currently in an unrecognized system: {0}\r\n".With(t_lastSystem);

                        if (settingsRef.CopySystemToClipboard)
                        {
                            Clipboard.SetData(DataFormats.Text, t_lastSystem);
                        }
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

                        if (settingsRef.CopySystemToClipboard)
                        {
                            Clipboard.SetData(DataFormats.Text, t_lastSystem);
                        }
                    }));

                    t_lastSysCheck = t_lastSystem; // prevent rechecking the same system
                }
            }
        }

        private void BackgroundWorker6_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            testSystemsTimer.Start(); // fire again after ~10s
        }


        /// <summary>
        /// This worker delegate is for the Cmdr Profile process
        /// </summary>
        /// <param name="sender">The calling object.</param>
        /// <param name="e">The event parameters.</param>
        private void BackgroundWorker7_DoWork(object sender, DoWorkEventArgs e)
        {
            if (buttonCaller == 22)
            {
                // Check to see if the EDCE folder and files are valid
                if (ValidateEdce())
                {
                    // EDCE is valid so set up the call.
                    commandString = "\"" + Path.Combine(MainForm.settingsRef.EdcePath, "edce_client.py") + "\"";
                    string currentFolder = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(MainForm.settingsRef.EdcePath);

                    try
                    {
                        td_proc = new Process();
                        td_proc.StartInfo.FileName = settingsRef.PythonPath;

                        DoTDProc(commandString);
                    }
                    finally
                    {
                        Directory.SetCurrentDirectory(currentFolder);
                    }
                }
                else
                {
                    PlayAlert();
                }
            }

            commandString = string.Empty; // reset path for thread safety
        }

        private void BackgroundWorker7_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!backgroundWorker7.IsBusy)
            {
                stopwatch.Stop(); // stop the timer
                circularBuffer = new System.Text.StringBuilder(2 * circularBufferSize);

                EnableBtnStarts();
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
            EddbLinkDbUpdateForm eddblinkForm = new EddbLinkDbUpdateForm()
            {
                StartPosition = FormStartPosition.CenterParent
            };

            eddblinkForm.ShowDialog(this);

            Application.DoEvents();

            if (!string.IsNullOrEmpty(DBUpdateCommandString))
            {
                ValidatePaths();

                if (!backgroundWorker4.IsBusy)
                {
                    // UpdateDB Button
                    buttonCaller = 5;
                    DisablebtnStarts(); // disable buttons during uncancellable operations

                    backgroundWorker4.RunWorkerAsync();
                }
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
            btnStart.Enabled = false;
            btnStationInfo.Enabled = false;

            ProcessCommand();
        }

        private void BuyOptionsOneStop_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBuyOptionsOneStop.Checked)
            {
                optBuyOptionsPrice.Checked = false;
            }
        }

        private void BuyOptionsPrice_CheckedChanged(object sender, EventArgs e)
        {
            if (optBuyOptionsPrice.Checked)
            {
                chkBuyOptionsOneStop.Checked = false;
            }
        }

        private void CboShipsSold_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                //                cboShipsSold.SelectionLength = 0;
            }
        }

        /// <summary>
        /// check for parsable Run output.
        /// </summary>
        /// <param name="page">A reference to the control.</param>
        private void CheckForParsableOutput(RichTextBox page)
        {
            string filteredOutput = FilterOutput(page.Text);
            runOutputState = IsValidRunOutput(filteredOutput);

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
                {
                    File.Delete(savedFile1);
                }
            }
        }

        private void ClearSaved2MenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbSaved2.Text))
            {
                rtbSaved2.Text = string.Empty;

                if (File.Exists(savedFile2))
                {
                    File.Delete(savedFile2);
                }
            }
        }

        private void ClearSaved3MenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbSaved3.Text))
            {
                rtbSaved3.Text = string.Empty;

                if (File.Exists(savedFile3))
                {
                    File.Delete(savedFile3);
                }
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

        /// <summary>
        /// The event handler for the mnuCopy1 click event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void Copy1_Click(object sender, EventArgs e)
        {
            CopyTextToClipboard(sender);
        }

        /// <summary>
        /// Check the value of the specified control.
        /// </summary>
        /// <param name="control">The decomal control.</param>
        /// <param name="propertyName">The name of the property holding the value.</param>
        private void CopyDecimalFormValue(
            string propertyName,
            NumericUpDown control)
        {
            CopyDecimalFormValue(propertyName, control, settingsRef);
        }

        /// <summary>
        /// Check the value of the specified control.
        /// </summary>
        /// <param name="control">The decomal control.</param>
        /// <param name="propertyName">The name of the property holding the value.</param>
        /// <param name="settings">The TDSettings object.</param>
        private void CopyDecimalFormValue(
            string propertyName,
            NumericUpDown control,
            TDSettings settings)
        {
            /*
             * The following is a set of workarounds to fix a bug in Framework 2.0+
             * involving NumericUpDown controls not updating the Value() property
             * when changed from the keyboard instead of the spinner control. We
             * do this by parsing the unbrowsable Text property and copying that.
             */

            PropertyInfo prop = settings.GetType().GetProperty(propertyName);

            if (decimal.TryParse(control.Text, out decimal value))
            {
                // Ensure that the value is withing the limits.
                if (value < control.Minimum)
                {
                    value = control.Minimum;
                }
                else if (value > control.Maximum)
                {
                    value = control.Maximum;
                }

                control.Text = value.ToString();
                prop.SetValue(settings, value);
            }
            else
            {
                prop.SetValue(settings, control.Minimum);
                control.Text = control.Minimum.ToString();
            }
        }

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.mnuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.Copy();
        }

        /// <summary>
        /// Load controls in the form from variables in memory
        /// </summary>
        private void CopySettingsFromConfig()
        {
            CopyDecimalSettingValue("Age", numRouteOptionsAge);
            CopyDecimalSettingValue("Capacity", numRouteOptionsShipCapacity);
            CopyDecimalSettingValue("Credits", numCommandersCredits);
            CopyDecimalSettingValue("Demand", numRouteOptionsDemand);
            CopyDecimalSettingValue("EndJumps", numRunOptionsEndJumps);
            CopyDecimalSettingValue("GPT", numRouteOptionsGpt);
            CopyDecimalSettingValue("Hops", numRouteOptionsHops);
            CopyDecimalSettingValue("Insurance", numShipInsurance);
            CopyDecimalSettingValue("Jumps", numRouteOptionsJumps);
            CopyDecimalSettingValue("LadenLY", numLadenLy);
            CopyDecimalSettingValue("Limit", numRouteOptionsLimit);
            CopyDecimalSettingValue("LoopInt", numRunOptionsLoopInt);
            CopyDecimalSettingValue("LSPenalty", numRouteOptionsLsPenalty);
            CopyDecimalSettingValue("Margin", numRouteOptionsMargin);
            CopyDecimalSettingValue("MaxGPT", numRouteOptionsMaxGpt);
            CopyDecimalSettingValue("MaxLSDistance", numRouteOptionsMaxLSDistance);
            CopyDecimalSettingValue("PruneHops", numRouteOptionsPruneHops);
            CopyDecimalSettingValue("PruneScore", numRouteOptionsPruneScore);
            CopyDecimalSettingValue("NumberOfRoutes", numRunOptionsRoutes);
            CopyDecimalSettingValue("StartJumps", numRunOptionsStartJumps);
            CopyDecimalSettingValue("Stock", numRouteOptionsStock);
            CopyDecimalSettingValue("UnladenLY", numUnladenLy);

            chkRunOptionsBlkMkt.Checked = settingsRef.BlackMarket;
            chkRunOptionsDirect.Checked = settingsRef.Direct;
            chkRunOptionsLoop.Checked = settingsRef.Loop;
            chkRunOptionsShorten.Checked = settingsRef.Shorten;
            chkRunOptionsJumps.Checked = settingsRef.ShowJumps;
            chkRunOptionsTowards.Checked = settingsRef.Towards;
            chkRunOptionsUnique.Checked = settingsRef.Unique;

            txtAvoid.Text = settingsRef.Avoid;
            txtVia.Text = settingsRef.Via;

            txtPadSize.Text = ContainsPadSizes(settingsRef.Padsizes);
            txtRunOptionsPlanetary.Text = ContainsPlanetary(settingsRef.Planetary);

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
        }

        /// <summary>
        ///  Load variables from text boxes in the form
        /// </summary>
        private void CopySettingsFromForm()
        {
            CopyDecimalFormValue("Age", numRouteOptionsAge);
            CopyDecimalFormValue("Capacity", numRouteOptionsShipCapacity);
            CopyDecimalFormValue("Credits", numCommandersCredits);
            CopyDecimalFormValue("Demand", numRouteOptionsDemand);
            CopyDecimalFormValue("EndJumps", numRunOptionsEndJumps);
            CopyDecimalFormValue("GPT", numRouteOptionsGpt);
            CopyDecimalFormValue("Hops", numRouteOptionsHops);
            CopyDecimalFormValue("Insurance", numShipInsurance);
            CopyDecimalFormValue("Jumps", numRouteOptionsJumps);
            CopyDecimalFormValue("LadenLY", numLadenLy);
            CopyDecimalFormValue("Limit", numRouteOptionsLimit);
            CopyDecimalFormValue("LoopInt", numRunOptionsLoopInt);
            CopyDecimalFormValue("LSPenalty", numRouteOptionsLsPenalty);
            CopyDecimalFormValue("Margin", numRouteOptionsMargin);
            CopyDecimalFormValue("MaxGPT", numRouteOptionsMaxGpt);
            CopyDecimalFormValue("MaxLSDistance", numRouteOptionsMaxLSDistance);
            CopyDecimalFormValue("NumberOfRoutes", numRunOptionsRoutes);
            CopyDecimalFormValue("PruneHops", numRouteOptionsPruneHops);
            CopyDecimalFormValue("PruneScore", numRouteOptionsPruneScore);
            CopyDecimalFormValue("StartJumps", numRunOptionsStartJumps);
            CopyDecimalFormValue("Stock", numRouteOptionsStock);
            CopyDecimalFormValue("UnladenLY", numUnladenLy);

            settingsRef.BlackMarket = chkRunOptionsBlkMkt.Checked;
            settingsRef.Direct = chkRunOptionsDirect.Checked;
            settingsRef.Loop = chkRunOptionsLoop.Checked;
            settingsRef.Shorten = chkRunOptionsShorten.Checked;
            settingsRef.ShowJumps = chkRunOptionsJumps.Checked;
            settingsRef.Towards = chkRunOptionsTowards.Checked;
            settingsRef.Unique = chkRunOptionsUnique.Checked;

            settingsRef.Avoid = txtAvoid.Text;
            settingsRef.Via = txtVia.Text;

            settingsRef.Padsizes = ContainsPadSizes(txtPadSize.Text);
            settingsRef.Planetary = ContainsPlanetary(txtRunOptionsPlanetary.Text);
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

        /// <summary>
        /// Copy the text from the control to the clipboard.
        /// </summary>
        /// <param name="control">The clicked control.</param>
        private void CopyTextToClipboard(TextBox control)
        {
            control.Focus();
            control.Copy();
        }

        /// <summary>
        /// Copy the text from the control to the clipboard.
        /// </summary>
        /// <param name="sender">The clicked control.</param>
        private void CopyTextToClipboard(object sender)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            var control = ((ContextMenuStrip)(menuItem.GetCurrentParent())).SourceControl;

            if (control is TextBox)
            {
                ((TextBox)control).Copy();
            }
            else if (control is ComboBox)
            {
                Clipboard.SetText(((ComboBox)control).SelectedText);
            }
        }

        /// <summary>
        /// The event handler for the mnuCut1 click event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void Cut1_Click(object sender, EventArgs e)
        {
            CutTextToClipboard(sender);
        }

        private void CutMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.mnuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.Cut();
        }

        /// <summary>
        /// Cut the text from the control to the clipboard.
        /// </summary>
        /// <param name="control">The clicked control.</param>
        private void CutTextToClipboard(TextBox control)
        {
            control.Focus();
            control.Cut();
        }

        /// <summary>
        /// Cut the text from the control to the clipboard.
        /// </summary>
        /// <param name="control">The clicked control.</param>
        private void CutTextToClipboard(object sender)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            var control = ((ContextMenuStrip)(menuItem.GetCurrentParent())).SourceControl;

            if (control is TextBox)
            {
                ((TextBox)control).Cut();
            }
            else if (control is ComboBox cbo)
            {
                if (cbo.SelectedText != string.Empty)
                {
                    Clipboard.SetText(cbo.SelectedText);

                    int sPos = cbo.SelectionStart;
                    cbo.SelectedText = cbo.SelectedText.Replace(cbo.SelectedText, string.Empty);
                    cbo.SelectionStart = sPos;
                }
            }
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.mnuStrip1.SourceControl;

            if (clickedControl.Name == txtNotes.Name)
            {
                txtNotes.SelectedText = string.Empty;
            }
        }

        /// <summary>
        /// Ensure that all the destination combo box controls are in sync.
        /// </summary>
        /// <param name="destination">The destination text.</param>
        private void SetAllDestinations(string destination)
        {
            foreach (ComboBox control in this.GetAllChildren().OfType<ComboBox>()
                .Where(x=>x.Name.EndsWith("Destination")))
            {
                var name = control.Name;

                if (control.Text != destination)
                {
                    control.Text = destination;
                }
            }
        }

        /// <summary>
        /// The event handler for destination changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void DestinationChanged(object sender, EventArgs e)
        {
            if (!RefreshingDestinations)
            {
                RefreshingDestinations = true;

                ComboBox destination = ((ComboBox)sender);

                SetAllDestinations(destination.Text);

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

                RefreshingDestinations = false;
            }
        }

        private void DestSystemComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox control = (ComboBox)sender;

            string filteredString = RemoveExtraWhitespace(control.Text);

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
                && !string.IsNullOrEmpty(control.Text))
            {
                control.Text = RemoveSystemOrStation(filteredString);

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

        /// <summary>
        /// Disable all the option panels.
        /// </summary>
        private void DisableAllOptionPanels()
        {
            foreach (Panel panel in optionPanels)
            {
                panel.Enabled = false;
            }

            panRouteOptions.Enabled = false;
        }

        private void DisablebtnStarts()
        {
            // disable buttons during uncancellable operation
            btnDbUpdate.Enabled = false;
            btnCmdrProfile.Enabled = false;
            btnGetSystem.Enabled = false;
            btnMiniMode.Enabled = false;
            btnStationInfo.Enabled = false;

            // an exception for Run commands
            if (buttonCaller != 1)
            {
                btnStart.Enabled = false;
            }
        }

        /// <summary>
        /// Display the ships available at the current station in the output window.
        /// </summary>
        private void DisplayAvailableShipsAtcurrentstation()
        {
            if (methodIndex == 6 &&
                cboSourceSystem.Text.Length > 0 &&
                btnStart.Enabled &&
                btnStart.Text.Contains("Start"))
            {
                circularBuffer.Clear();

                StackCircularBuffer("Ships currently available at {0}:{1}{2}".With(
                    cboSourceSystem.Text,
                    Environment.NewLine,
                    Environment.NewLine));

                if (outputStationShips.Count == 0)
                {
                    StackCircularBuffer("None");
                }
                else
                {
                    int maxNameLength = outputStationShips
                        .Select(x => x.Substring(0, x.IndexOf("[")).Length)
                        .Max();

                    int maxCostLength = outputStationShips
                        .Select(x => x.Substring(x.IndexOf("[")).Length)
                        .Max();

                    foreach (string ship in outputStationShips)
                    {
                        string[] data = ship.Split(new string[] { "[" }, StringSplitOptions.RemoveEmptyEntries);

                        StackCircularBuffer("{0}[{1}{2}".With(
                            data[0].PadRight(maxNameLength),
                            data[1].PadLeft(maxCostLength),
                            Environment.NewLine));
                    }
                }
            }
        }

        /// <summary>
        /// Called on clicking one of the distance menu items.
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
        /// Disable all the option panels.
        /// </summary>
        private void EnableAllOptionPanels()
        {
            foreach (Panel panel in optionPanels)
            {
                panel.Enabled = true;
            }

            panRouteOptions.Enabled = true;
        }

        private void EnableBtnStarts()
        {
            //ShowOrHideOptionsPanel(methodFromIndex);

            // reenable other controls when done
            btnDbUpdate.Enabled = true;
            btnCmdrProfile.Enabled = ValidateEdce();
            btnGetSystem.Enabled = true;

            // fix Run button when returning from non-Run commands
            if (buttonCaller == 1 || !btnStart.Enabled)
            {
                btnStart.Enabled = true;
            }

            btnStart.Font = new Font(btnStart.Font, FontStyle.Bold);
            btnStart.Text = "Start";

            SetStationInfoButtonState();
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
            Process.Start("https://github.com/MarkAusten/TDHelper/wiki/Home");
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

        /// <summary>
        /// Get the buy command string.
        /// </summary>
        /// <param name="selectedCommodity">The name of the selected commodity.</param>
        /// <param name="nearSystem">The select source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetBuyCommand(
            string selectedCommodity,
            string nearSystem)
        {
            string cmdPath = string.Empty;

            if (!string.IsNullOrEmpty(selectedCommodity))
            {
                cmdPath += "buy";

                cmdPath += AddNear(nearSystem);

                if (!string.IsNullOrEmpty(nearSystem))
                {
                    cmdPath += AddNumericOption(numBuyOptionsNearLy.Value, "ly");
                }

                cmdPath += AddNumericOption(numBuyOptionsAbove.Value, "gt");
                cmdPath += AddNumericOption(numBuyOptionsBelow.Value, "lt");
                cmdPath += AddNumericOption(numBuyOptionsSupply.Value, "supply");

                cmdPath += AddCheckedOption(chkBuyOptionsBlkMkt.Checked, "bm");
                cmdPath += AddCheckedOption(chkBuyOptionsOneStop.Checked, "one-stop");

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
            }

            return cmdPath;
        }

        /// <summary>
        /// Show/enable or hide/disable the panel specified by the index.
        /// </summary>
        /// <param name="optionsIndex">The index of the required panel.</param>
        private Panel GetCurrentOptionsPanel(int optionsIndex)
        {
            Panel panel = null;

            if (optionPanels.Count > 0 &&
                optionsIndex >= 0 &&
                optionsIndex < optionPanels.Count)
            {
                panel = optionPanels[optionsIndex];
            }

            return panel;
        }

        /// <summary>
        /// populate the control with the specified setting.
        /// </summary>
        /// <param name="propertyName">The name of the settings property.</param>
        /// <param name="control">The control to be populated.</param>
        private void CopyDecimalSettingValue(
            string propertyName,
            NumericUpDown control)
        {
            CopyDecimalSettingValue(propertyName, control, settingsRef);
        }

        /// <summary>
        /// populate the control with the specified setting.
        /// </summary>
        /// <param name="propertyName">The name of the settings property.</param>
        /// <param name="control">The control to be populated.</param>
        /// <param name="settings">The Settings object.</param>
        private void CopyDecimalSettingValue(
            string propertyName,
            NumericUpDown control,
            TDSettings settings)
        {
            ValidateSettingValue(propertyName, control, settings);

            PropertyInfo prop = settings.GetType().GetProperty(propertyName);
            control.Value = (decimal)prop.GetValue(settings);
        }

        /// <summary>
        /// Get the local command string.
        /// </summary>
        /// <param name="sourceSystem">The name of the source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetLocalCommand(string sourceSystem)
        {
            string cmdPath = string.Empty;

            if (!string.IsNullOrEmpty(sourceSystem))
            {
                cmdPath += "local";
                cmdPath += AddNumericOption(numLocalOptionsLy.Value, "ly");

                cmdPath += AddCheckedOption(chkLocalOptionsRearm.Checked, "rearm");
                cmdPath += AddCheckedOption(chkLocalOptionsRefuel.Checked, "refuel");
                cmdPath += AddCheckedOption(chkLocalOptionsRepair.Checked, "repair");
                cmdPath += AddCheckedOption(chkLocalOptionsTrading.Checked, "trading");
                cmdPath += AddCheckedOption(chkLocalOptionsBlkMkt.Checked, "bm");
                cmdPath += AddCheckedOption(chkLocalOptionsShipyard.Checked, "shipyard");
                cmdPath += AddCheckedOption(chkLocalOptionsOutfitting.Checked, "outfitting");
                cmdPath += AddCheckedOption(chkLocalOptionsStations.Checked, "stations");

                cmdPath += AddPlanetaryOption(txtLocalOptionsPlanetary.Text);
                cmdPath += AddPadOption(txtLocalOptionsPads.Text);

                cmdPath += AddVerbosity();

                cmdPath += AddQuotedOption(sourceSystem);
            }

            return cmdPath;
        }

        /// <summary>
        /// Get the maarket command string.
        /// </summary>
        /// <param name="sourceSystem">The name of the source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetMarketCommand(string sourceSystem)
        {
            string cmdPath = string.Empty;

            if (!string.IsNullOrEmpty(sourceSystem))
            {
                cmdPath = "market";

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
            }

            return cmdPath;
        }

        /// <summary>
        /// This worker delegate is the main work horse for the application.
        /// it controls the logic for all the primary commands.
        /// </summary>
        /// <param name="index">The method index.</param>
        private string GetMethodCommandString(int index)
        {
            string command = string.Empty;

            switch (index)
            {
                // Station command
                case 10:
                    command = GetStationCommand(SourceSystem);
                    break;

                // Local command
                case 9:
                    command = GetLocalCommand(SourceSystem);
                    break;

                // OldData command
                case 8:
                    command = GetOldDataCommand(SourceSystem);
                    break;

                // Nav command
                case 7:
                    command = GetNavCommand(SourceSystem, TargetSystem);
                    break;

                // Market command
                case 5:
                    command = GetMarketCommand(SourceSystem);
                    break;

                // Trade command
                case 4:
                    command = GetTradeCommand(SourceSystem, TargetSystem);
                    break;

                // Rares command
                case 3:
                    command = GetRaresCommand(SourceSystem);
                    break;

                // Sell command
                case 2:
                    command = GetSellCommand(SelectedCommodity, SourceSystem);
                    break;

                // Buy command
                case 1:
                    command = GetBuyCommand(SelectedCommodity, SourceSystem);
                    break;

                // Run command
                case 0:
                    command = GetRunCommand(SourceSystem, TargetSystem);
                    break;
            }

            return command;
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
            string cmdPath = string.Empty;

            if (!string.IsNullOrEmpty(sourceSystem) &&
                !string.IsNullOrEmpty(destinationSystem))
            {
                cmdPath += "nav";

                cmdPath += AddTextOption(txtNavOptionsVia.Text, "via");
                cmdPath += AddTextOption(txtNavOptionsAvoid.Text, "avoid");
                cmdPath += AddPlanetaryOption(txtNavOptionsPlanetary.Text);
                cmdPath += AddPadOption(txtNavOptionsPads.Text);

                cmdPath += AddNumericOption(numNavOptionsLy.Value, "ly");
                cmdPath += AddNumericOption(numNavOptionsRefuelJumps.Value, "ref");

                cmdPath += AddCheckedOption(chkNavOptionsStations.Checked, "stations");

                cmdPath += AddVerbosity();

                cmdPath += AddQuotedOption(sourceSystem);
                cmdPath += AddQuotedOption(destinationSystem);
            }

            return cmdPath;
        }

        /// <summary>
        /// Get the old data command string.
        /// </summary>
        /// <param name="sourceSystem">The name of the source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetOldDataCommand(string sourceSystem)
        {
            string cmdPath = string.Empty;

            if (!string.IsNullOrEmpty(sourceSystem))
            {
                cmdPath += "olddata";

                cmdPath += AddCheckedOption(chkOldDataOptionsRoute.Checked, "route");
                cmdPath += AddNumericOption(numOldDataOptionsNearLy.Value, "ly");
                cmdPath += AddNumericOption(numOldDataOptionsMinAge.Value, "min-age");
                cmdPath += AddLimitOption(numOldDataOptionsLimit.Value);

                cmdPath += AddVerbosity();
                cmdPath += AddNear(sourceSystem);
            }

            return cmdPath;
        }

        /// <summary>
        /// Get the rares command string.
        /// </summary>
        /// <param name="sourceSystem">The name of the source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetRaresCommand(string sourceSystem)
        {
            string cmdPath = string.Empty;

            if (!string.IsNullOrEmpty(sourceSystem))
            {
                cmdPath = "rares";

                cmdPath += AddNumericOption(numRaresOptionsLy.Value, "ly");

                cmdPath += AddPadOption(txtRaresOptionsPads.Text);
                cmdPath += AddPlanetaryOption(txtRaresOptionsPlanetary.Text);
                cmdPath += AddLimitOption(numRaresOptionsLimit.Value);

                cmdPath += AddCheckedOption(chkRaresOptionsReverse.Checked, "reverse");
                cmdPath += AddCheckedOption(chkRaresOptionsQuiet.Checked, "quiet");

                RadioButton option = grpRaresOptionsType.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);

                if (option != null &&
                    option.Text != "All")
                {
                    cmdPath += " --{0}".With(option.Text);
                }

                option = grpRaresOptionsSort.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);

                if (option != null &&
                    option.Text == "Price")
                {
                    cmdPath += " --P";
                }

                if (!string.IsNullOrEmpty(txtRaresOptionsFrom.Text) &&
                    numRaresOptionsAway.Value > 0)
                {
                    cmdPath += AddNumericOption(numRaresOptionsAway.Value, "away");

                    foreach (string output in txtRaresOptionsFrom.Text.Split(',')
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        cmdPath += " --from=\"{0}\"".With(output);
                    }
                }

                cmdPath += AddVerbosity();
                cmdPath += AddQuotedOption(sourceSystem);
            }

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

                if (!string.IsNullOrEmpty(destinationSystem) &&
                    chkRunOptionsTowards.Checked)
                {
                    cmdPath += " --towards=\"{0}\"".With(destinationSystem);
                }
            }

            if (!string.IsNullOrEmpty(destinationSystem) &&
                !chkRunOptionsTowards.Checked &&
                !chkRunOptionsLoop.Checked)
            {
                cmdPath += " --to=\"{0}\"".With(destinationSystem);
                cmdPath += AddNumericOption(numRunOptionsEndJumps.Value, "end");
            }

            cmdPath += AddNumericOption(numRouteOptionsShipCapacity.Value, "cap");
            cmdPath += AddNumericOption(numRouteOptionsLimit.Value, "lim");
            cmdPath += AddNumericOption(numShipInsurance.Value, "ins");
            cmdPath += AddNumericOption(numCommandersCredits.Value, "cr");
            cmdPath += AddNumericOption(numLadenLy.Value, "ly");
            cmdPath += AddNumericOption(numUnladenLy.Value, "empty");
            cmdPath += AddNumericOption(numRouteOptionsGpt.Value, "gpt");
            cmdPath += AddNumericOption(numRouteOptionsMaxGpt.Value, "mgpt");
            cmdPath += AddNumericOption(numRouteOptionsStock.Value, "supply");
            cmdPath += AddNumericOption(numRouteOptionsDemand.Value, "demand");
            cmdPath += AddNumericOption(numRouteOptionsMargin.Value, "margin");
            cmdPath += AddNumericOption(numRouteOptionsJumps.Value, "jum");
            cmdPath += AddNumericOption(numRunOptionsStartJumps.Value, "start");
            cmdPath += AddNumericOption(numRouteOptionsLsPenalty.Value, "lsp");
            cmdPath += AddNumericOption(numRouteOptionsMaxLSDistance.Value, "ls-max");
            cmdPath += AddNumericOption(numRouteOptionsPruneHops.Value, "prune-hops");
            cmdPath += AddNumericOption(numRouteOptionsPruneScore.Value, "prune-score");
            cmdPath += AddNumericOption(numRouteOptionsAge.Value, "age");
            cmdPath += AddNumericOption(numRunOptionsLoopInt.Value, "loop-int");
            cmdPath += AddNumericOption(numRunOptionsRoutes.Value, "routes");

            cmdPath += AddCheckedOption(chkRunOptionsBlkMkt.Checked, "bm");
            cmdPath += AddCheckedOption(chkRunOptionsDirect.Checked, "direct");
            cmdPath += AddCheckedOption(chkRunOptionsShorten.Checked, "shorten");
            cmdPath += AddCheckedOption(chkRunOptionsUnique.Checked, "unique");
            cmdPath += AddCheckedOption(chkRunOptionsLoop.Checked, "loop");
            cmdPath += AddCheckedOption(chkRunOptionsJumps.Checked, "show-jumps");

            cmdPath += AddTextOption(txtAvoid.Text, "avoid");
            cmdPath += AddTextOption(txtVia.Text, "via");

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

            cmdPath += AddCheckedOption(settingsRef.ShowProgress, "progress");
            cmdPath += AddCheckedOption(settingsRef.Summary, "summary");

            return cmdPath;
        }

        /// <summary>
        /// Set up the selected commodity data.
        /// </summary>
        private void GetSelectedCommodity()
        {
            string commodity = string.Empty;

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

        /// <summary>
        /// Get the sell command string.
        /// </summary>
        /// <param name="selectedCommodity">The name of the selected commodity.</param>
        /// <param name="nearSystem">The select source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetSellCommand(
            string selectedCommodity,
            string nearSystem)
        {
            string cmdPath = string.Empty;

            if (!string.IsNullOrEmpty(selectedCommodity))
            {
                cmdPath += "sell";

                cmdPath += AddNear(nearSystem);

                if (!string.IsNullOrEmpty(nearSystem))
                {
                    cmdPath += AddNumericOption(numSellOptionsNearLy.Value, "ly");
                }

                cmdPath += AddNumericOption(numSellOptionsAbove.Value, "gt");
                cmdPath += AddNumericOption(numSellOptionsBelow.Value, "lt");
                cmdPath += AddNumericOption(numSellOptionsDemand.Value, "demand");

                cmdPath += AddCheckedOption(chkSellOptionsBlkMkt.Checked, "bm");

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
            }

            return cmdPath;
        }

        /// <summary>
        /// Set up the correct source and targeet system.
        /// </summary>
        private void GetSourceAndDestinationSystems()
        {
            SourceSystem = RemoveExtraWhitespace(cboSourceSystem.Text);
            string destination = string.Empty;

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
        }

        /// <summary>
        /// Set up the correct source and targeet system and selected commodity data.
        /// </summary>
        private void GetSourceTargetAndCommodity()
        {
            GetSourceAndDestinationSystems();
            GetSelectedCommodity();
        }

        /// <summary>
        /// Get the station command string.
        /// </summary>
        /// <param name="sourceSystem">The name of the source system.</param>
        /// <returns>A correctly formatted command string.</returns>
        private string GetStationCommand(string sourceSystem)
        {
            string cmdPath = string.Empty;

            if (!string.IsNullOrEmpty(sourceSystem))
            {
                cmdPath += "station -v";
                cmdPath += AddQuotedOption(sourceSystem);
            }

            return cmdPath;
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
            string cmdPath = string.Empty;

            if (!string.IsNullOrEmpty(sourceSystem) &&
                !string.IsNullOrEmpty(destinationSystem))
            {
                cmdPath = "trade";

                cmdPath += AddVerbosity();

                cmdPath += AddQuotedOption(sourceSystem);
                cmdPath += AddQuotedOption(destinationSystem);
            }

            return cmdPath;
        }

        private void HideAllOptionPanels()
        {
            foreach (Panel panel in optionPanels)
            {
                ShowOptions(panel, false);
            }

            EnableOptions(panRouteOptions, false);
        }

        private void InsertAtGridRow_Click(object sender, EventArgs e)
        {
            if (grdPilotsLog.Rows.Count > 0)
            {
                // add a row with the timestamp of the selected row
                // basically an insert-below-index when we use select(*)
                string timestamp = grdPilotsLog.Rows[dRowIndex].Cells["Timestamp"].Value.ToString();

                AddAtTimestampDBRow(GenerateRecentTimestamp(timestamp));
                InvalidatedRowUpdate(true, -1);
            }
            else
            {
                // special case for an empty gridview
                AddAtTimestampDBRow(CurrentTimestamp());
                InvalidatedRowUpdate(true, -1);
            }
        }

        /// <summary>
        /// Load the speicifed file into the specified control.
        /// </summary>
        /// <param name="fileName">The name of the file to load.</param>
        /// <param name="page">A reference to the control.</param>
        private void LoadFileIntoPage(
            string fileName,
            RichTextBox page)
        {
            if (CheckIfFileOpens(fileName))
            {
                page.Focus();

                if (File.Exists(fileName))
                {
                    page.LoadFile(savedFile1, RichTextBoxStreamType.PlainText);
                }

                CheckForParsableOutput(page);

                page.Focus();
            }
        }

        private void LocalFilterCheckBoxChanged(object sender, EventArgs e)
        {
            IEnumerable<CheckBox> checkboxes = panLocalOptions.Controls.OfType<CheckBox>();

            btnLocalOptionsReset.Enabled = checkboxes.Count(x => x.Checked) > 0;
            btnLocalOptionsAll.Enabled = checkboxes.Count(x => x.Checked) > 0;
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

            OptimiseAllDatabases();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SplashScreen.SetStatus("Set form position...");

            Screen screen = Screen.FromControl(this);
            Rectangle workingArea = screen.WorkingArea;
            int[] winLoc = LoadWinLoc(settingsRef.LocationParent);
            int[] winSize = LoadWinSize(settingsRef.SizeParent);

            // restore window size from config
            if (winSize.Length != 0 &&
                winSize != null)
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
                ForceFormOnScreen(this);
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
            SplashScreen.SetStatus("Set ship list...");

            SetShipList(true);

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
                SplashScreen.SetStatus("Load ship settings...");

                LoadSettings();
            }

            SplashScreen.SetStatus("Completed.");
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
            // Call the refresh method for the run options panel.
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

            // Firstly hide all the options panel.
            HideAllOptionPanels();

            // Now show/enable the required options panel.
            ShowOrHideOptionsPanel(methodIndex);

            // Save the currently enabled options panel.
            methodFromIndex = methodIndex;

            ResumeLayout();
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
            if ("SML?".Contains(key))
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

        /// <summary>
        /// The event handler for the mnuPaste1 click event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void Paste1_Click(object sender, EventArgs e)
        {
            PasteTextToControl(sender);
        }

        private void PasteMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.mnuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.Paste();
        }

        /// <summary>
        /// Paste the text from the clipboard to the control.
        /// </summary>
        /// <param name="control">The clicked control.</param>
        private void PasteTextToControl(TextBox control)
        {
            control.Focus();
            control.Paste();
        }

        ///// <summary>
        ///// Select all the text on the control.
        ///// </summary>
        ///// <param name="control">The clicked control.</param>
        //private void SelectAllControlText(TextBox control)
        //{
        //    control.Focus();
        //    control.SelectAll();
        //}
        /// <summary>
        /// Paste the text from the clipboard to the control.
        /// </summary>
        /// <param name="sender">The clicked control.</param>
        private void PasteTextToControl(object sender)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            var control = ((ContextMenuStrip)(menuItem.GetCurrentParent())).SourceControl;

            if (control is TextBox)
            {
                ((TextBox)control).Paste();
            }
            else if (control is ComboBox cbo)
            {
                string txtInClip = Clipboard.GetText();

                int sPos = cbo.SelectionStart;

                if (cbo.SelectedText != string.Empty)
                {
                    cbo.SelectedText = cbo.SelectedText.Replace(cbo.SelectedText, txtInClip);
                }
                else
                {
                    cbo.Text = cbo.Text.Insert(cbo.SelectionStart, txtInClip);
                    cbo.SelectionStart = sPos + txtInClip.Length;
                }
            }
        }

        private void PilotsLogDataGrid_CellContextMenuStripNeeded(
                    object sender,
                    DataGridViewCellContextMenuStripNeededEventArgs e)
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

        private void PilotsLogDataGrid_CellValueNeeded(
                    object sender,
                    DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < retriever.RowCount && e.ColumnIndex < retriever.RowCount)
            {
                e.Value = memoryCache.RetrieveElement(e.RowIndex, e.ColumnIndex);
            }
        }

        private void PilotsLogDataGrid_CellValuePushed(
                    object sender,
                    DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < retriever.RowCount && e.ColumnIndex < retriever.RowCount)
            {
                // update our local table
                localTable.Rows[e.RowIndex][e.ColumnIndex] = e.Value;
                List<DataRow> row = new List<DataRow> { localTable.Rows[e.RowIndex] };

                // update the physical DB and repaint
                UpdateDBRow(row);
                InvalidatedRowUpdate(false, e.RowIndex);
            }
        }

        private void PilotsLogDataGrid_UserDeletingRow(
                    object sender,
                    DataGridViewRowCancelEventArgs e)
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
                        RemoveDBRow(rowIndex);
                        UpdateLocalTable();
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
                        RemoveDBRows(dgRowIDIndexer);
                        UpdateLocalTable();
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
        private void Planetary_KeyPress(
            object sender,
            KeyPressEventArgs e)
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

                planetary.Text = ContainsPlanetary(selected);
            }

            e.Handled = true;
        }

        private void ProcErrorDataHandler(
                    object sender,
                    DataReceivedEventArgs output)
        {
            if (output.Data != null)
            {
                StackCircularBuffer(output.Data + "\n");
            }
        }

        /// <summary>
        /// Process the currently set up command.
        /// </summary>
        private void ProcessCommand()
        {
            // mark as run button
            buttonCaller = 1;

            GetSourceTargetAndCommodity();

            DoRunEvent(); // externalized
        }

        private void ProcOutputDataHandler(
                    object sender,
                    DataReceivedEventArgs output)
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

                if (buttonCaller != 5 && buttonCaller != 12 && !exceptions.Any(output.Data.Contains) && methodIndex != 5 || methodIndex != 6)
                {
                    // hide output if calculating
                    StackCircularBuffer(filteredOutput);
                }
                else if (methodIndex == 5 || buttonCaller == 5 || buttonCaller == 12)
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
                        stream.WriteLine("Buy " + cboBuyOptionsCommodities.Text.ToString()
                            + " (near "
                            + cboSourceSystem.Text.ToString()
                            + "):\n"
                            + Clipboard.GetData(DataFormats.Text).ToString());
                    }
                    else if (methodIndex == 2)
                    {
                        stream.WriteLine("Sell " + cboSellOptionsCommodities.Text.ToString()
                            + " (near "
                            + cboSourceSystem.Text.ToString()
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

        /// <summary>
        /// Refresh the currently enabled panel.
        /// </summary>
        private void RefreshCurrentOptionsPanel()
        {
            Panel panel = GetCurrentOptionsPanel(methodFromIndex);

            if (panel != null)
            {
                OptionsPanelRefresh(panel);
                ShowOptions(panel);
            }

            EnableOptions(panRouteOptions, methodFromIndex == 0);
        }

        private void RemoveAtGridRow_Click(object sender, EventArgs e)
        {
            if (grdPilotsLog.Rows.Count > 0)
            {
                RemoveDBRow(pRowIndex);
                UpdateLocalTable();
                memoryCache.RemoveRow(dRowIndex, pRowIndex);
                grdPilotsLog.InvalidateRow(dRowIndex);
            }
        }

        /// <summary>
        /// Remove the station from the supplied string. If only the system is passed then
        /// remove that.
        /// </summary>
        /// <param name="filteredString">The string upon which to filter.</param>
        /// <returns>The filtered string.</returns>
        private string RemoveSystemOrStation(string filteredString)
        {
            string[] tokens = filteredString.Split(new string[] { "/" }, StringSplitOptions.None);

            return tokens != null && tokens.Length == 2
                ? tokens[0]
                : string.Empty;
        }

        /// <summary>
        /// Reset the local filters.
        /// </summary>
        private void ResetAllLocalFilters()
        {
            foreach (CheckBox control in panLocalOptions.Controls.OfType<CheckBox>())
            {
                control.Checked = false;
            }

            btnLocalOptionsAll.Enabled = true;
            btnLocalOptionsReset.Enabled = false;
        }

        /// <summary>
        /// Reset the local filters.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void ResetLocalFilters(object sender, EventArgs e)
        {
            ResetAllLocalFilters();
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

        /// <summary>
        /// The event handler for the mnuSelectAll1 click event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void SelectAll1_Click(object sender, EventArgs e)
        {
            SelectAllControlText(sender);
        }

        /// <summary>
        /// Select all the text on the control.
        /// </summary>
        /// <param name="sender">The clicked control.</param>
        private void SelectAllControlText(object sender)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            var control = ((ContextMenuStrip)(menuItem.GetCurrentParent())).SourceControl;

            control.Focus();

            if (control is TextBox)
            {
                ((TextBox)control).SelectAll();
            }
            else if (control is ComboBox)
            {
                ((ComboBox)control).SelectAll();
            }
        }

        private void SelectMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.mnuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.SelectAll();
        }

        /// <summary>
        /// Set the local filters.
        /// </summary>
        private void SetAllLocalFilters()
        {
            foreach (CheckBox control in panLocalOptions.Controls.OfType<CheckBox>())
            {
                control.Checked = true;
            }

            btnLocalOptionsAll.Enabled = false;
            btnLocalOptionsReset.Enabled = true;
        }

        /// <summary>
        /// See if the specified panel has a ships combo box and set it.
        /// </summary>
        /// <param name="panel">The panel to be checked.</param>
        private void SetAvailableShips(Panel panel)
        {
            // See if this options panel has a pad sizes text box.
            ListView ships = panel.Controls.OfType<ListView>()
                .FirstOrDefault(x => x.Name.Contains("Ships"));

            if (ships != null)
            {
                // shipvendor textbox
                if (outputStationShips.Count > 0)
                {
                    ships.Items.Clear();

                    foreach (string shipCost in outputStationShips)
                    {
                        int index = shipCost.IndexOf("[") - 1;

                        ListViewItem item = new ListViewItem(new[]
                        {
                            shipCost.Substring(0, index),
                            shipCost.Substring(index + 2).Replace("]", string.Empty)
                        });

                        ships.Items.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// The the value of the specified control to blank.
        /// </summary>
        /// <param name="control">The control to be set.</param>
        /// <param name="requiredSource">The key to the source value.</param>
        private void SetComboBoxControlValue(
            ComboBox control,
            string requiredSource)
        {
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
        /// Set the commodities.
        /// </summary>
        /// <param name="panel">The panel to be checked.</param>
        private void SetCommodities(Panel panel)
        {
            // Only refresh the panelo if it is visible.
            if (panel.Visible)
            {
                // See if this options panel has a commodity combo box.
                ComboBox commodities = panel.Controls.OfType<ComboBox>()
                    .FirstOrDefault(x => x.Name.Contains("Commodities"));

                if (commodities != null)
                {
                    // Detach and reattach the destinations data source.
                    commodities.DataSource = null;

                    int index
                        = methodFromIndex == 0
                        ? methodIndex
                        : methodFromIndex;

                    switch (index)
                    {
                        case 1: // Buy
                                // All commodities & ships
                            commodities.DataSource = CommodityAndShipList;
                            break;

                        case 2: // Sell
                                // Just commodities.
                            commodities.DataSource = CommoditiesList;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// The the value of the specified control to the correct value.
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
        /// Set the destinations.
        /// </summary>
        /// <param name="panel">The panel to be checked.</param>
        private void SetDestinations(Panel panel)
        {
            // See if this options panel has a destination combobox.
            ComboBox destinations = panel.Controls.OfType<ComboBox>()
                .FirstOrDefault(x => x.Name.Contains("Destination"));

            if (destinations != null)
            {
                // save the current value as resetting the data source sets this to blank.
                string text = destinations.Text;

                // Detach and reattach the destinations data source.
                destinations.DataSource = null;
                destinations.DataSource = DestinationList;

                destinations.AutoCompleteCustomSource.Clear();
                destinations.AutoCompleteCustomSource.AddRange(outputSysStnNames.ToArray());

                // Reset the current value.
                destinations.Text = text;
            }
        }

        /// <summary>
        /// Set the local filters.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void SetLocalFilters(object sender, EventArgs e)
        {
            SetAllLocalFilters();
        }

        /// <summary>
        /// Set up the option panels list.
        /// </summary>
        private void SetOptionPanelList()
        {
            optionPanels.Add(panRunOptions);
            optionPanels.Add(panBuyOptions);
            optionPanels.Add(panSellOptions);
            optionPanels.Add(panRaresOptions);
            optionPanels.Add(panTradeOptions);
            optionPanels.Add(panMarketOptions);
            optionPanels.Add(panShipVendorOptions);
            optionPanels.Add(panNavOptions);
            optionPanels.Add(panOldDataOptions);
            optionPanels.Add(panLocalOptions);
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

        /// <summary>
        /// Set the state of the Station Info Button.
        /// </summary>
        private void SetStationInfoButtonState()
        {
            // If a command is not already running...
            if (btnStart.Enabled && btnStart.Text.Contains("Start"))
            {
                // ...then the button is enable if a source system/station is selected.
                string[] data = cboSourceSystem.Text.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                btnStationInfo.Enabled = data.Length == 2 && data[0].Length > 3 & data[1].Length > 3;
            }
            else
            {
                // ...otherwise it should be disabled.
                btnStationInfo.Enabled = false;
            }
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
                    mnuLadenLY.Enabled = false;
                    mnuUnladenLY.Enabled = false;
                    mnuCapacity.Enabled = true;
                    break;

                case "numBuyOptionsNearLy":
                case "numLocalOptionsLy":
                case "numNavOptionsLy":
                case "numOldDataOptionsNearLy":
                case "numRaresOptionsLy":
                case "numSellOptionsNearLy":
                    mnuLadenLY.Enabled = true;
                    mnuUnladenLY.Enabled = true;
                    mnuCapacity.Enabled = false;
                    break;

                default:
                    mnuLadenLY.Enabled = false;
                    mnuUnladenLY.Enabled = false;
                    mnuCapacity.Enabled = false;
                    break;
            }

            var control = ((ContextMenuStrip)sender).SourceControl;

            if (control is TextBox ||
                control is ComboBox)
            {
                string text = string.Empty;
                string selectedText = string.Empty;

                if (control is TextBox)
                {
                    text = ((TextBox)control).Text;
                    selectedText = ((TextBox)control).SelectedText;
                }
                else if (control is ComboBox)
                {
                    text = ((ComboBox)control).Text;
                    selectedText = ((ComboBox)control).SelectedText;
                }

                if (text.Length > 0)
                {
                    if (selectedText.Length > 0)
                    {
                        mnuCut1.Enabled = true;
                        mnuCopy1.Enabled = true;
                    }
                    else
                    {
                        mnuCut1.Enabled = false;
                        mnuCopy1.Enabled = false;
                    }

                    mnuSelectAll1.Enabled = true;
                }
                else
                {
                    mnuCut1.Enabled = false;
                    mnuCopy1.Enabled = false;
                }

                mnuPaste1.Enabled = true;
            }
            else
            {
                mnuCut1.Enabled = false;
                mnuCopy1.Enabled = false;
                mnuPaste1.Enabled = false;
                mnuSelectAll1.Enabled = false;
            }

            mnuReset.Enabled = true;
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
            Panel panel = GetCurrentOptionsPanel(optionsIndex);

            if (panel != null)
            {
                ShowOptions(panel, show);

                if (optionsIndex == 0)
                {
                    EnableOptions(panRouteOptions, show);
                }
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
                    cboSourceSystem.Text = RemoveSystemOrStation(filteredString);

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

            SetStationInfoButtonState();
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

        private void StationInfo_Click(object sender, EventArgs e)
        {
            // Prevent double clicks
            btnStationInfo.Enabled = false;
            btnStart.Enabled = false;

            methodIndex = 10;

            ProcessCommand();
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
            if (fromPane == 4)
            {
                // if we're coming from the notes pane we should save when we switch
                txtNotes.SaveFile(notesFile, RichTextBoxStreamType.PlainText);
            }

            if (tabControl1.SelectedIndex == 0 &&
                !string.IsNullOrEmpty(rtbOutput.Text))
            {
                CheckForParsableOutput(rtbOutput);

                rtbOutput.Focus(); // always focus our text box

                pagOutput.Font = new Font(pagOutput.Font, FontStyle.Regular); // reset the font
            }
            else if (tabControl1.SelectedIndex == 4 &&
                CheckIfFileOpens(notesFile))
            {
                txtNotes.LoadFile(notesFile, RichTextBoxStreamType.PlainText);

                txtNotes.Focus();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                LoadFileIntoPage(savedFile1, rtbSaved1);
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                LoadFileIntoPage(savedFile2, rtbSaved2);
            }
            else if (tabControl1.SelectedIndex == 3)
            {
                LoadFileIntoPage(savedFile3, rtbSaved3);
            }

            fromPane = tabControl1.SelectedIndex;
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
    }
}