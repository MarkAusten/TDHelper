using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Data.SQLite;
using System.Data;
using System.Timers;
using Newtonsoft.Json;
using System.Text;

namespace TDHelper
{
    public partial class Form1 : Form
    {
        #region FormProps
        string appVersion = "v" + Application.ProductVersion;

        private string tv_outputBox = string.Empty;

        private string notesFile = Path.GetDirectoryName(Application.ExecutablePath) + @"\saved_notes.txt";
        private string savedFile1 = Path.GetDirectoryName(Application.ExecutablePath) + @"\saved_1.txt";
        private string savedFile2 = Path.GetDirectoryName(Application.ExecutablePath) + @"\saved_2.txt";
        private string savedFile3 = Path.GetDirectoryName(Application.ExecutablePath) + @"\saved_3.txt";

        private System.Timers.Timer testSystemsTimer = new System.Timers.Timer();

        int buttonCaller, methodIndex, exportIndex, stationIndex, methodFromIndex = -1, hasUpdated, procCode = -1;
        
        List<List<string>> validConfigs = new List<List<string>>();
        CultureInfo userCulture = CultureInfo.CurrentCulture;
        #endregion

        #region Snap-To-Edge
        private const int SnapDist = 10;
        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            Rectangle workingArea = Screen.FromControl(this).WorkingArea;

            if (Math.Abs(workingArea.Left - this.Left) < SnapDist)
                this.Left = workingArea.Left;
            else if (Math.Abs(this.Left + this.Width - workingArea.Left - workingArea.Width) < SnapDist)
                this.Left = workingArea.Left + workingArea.Width - this.Width;

            if (Math.Abs(workingArea.Top - this.Top) < SnapDist)
                this.Top = workingArea.Top;
            else if (Math.Abs(this.Top + this.Height - workingArea.Top - workingArea.Height) < SnapDist)
                this.Top = workingArea.Top + workingArea.Height - this.Height;
        }
        #endregion

        #region FormStuff
        public Form1()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint 
                    | ControlStyles.UserPaint 
                    | ControlStyles.DoubleBuffer, true);

            // Let's change the title to the current version
            this.Text = "TDHelper " + appVersion + "-Beta";

            // Build variables from config
            buildSettings();

            testSystemsTimer.AutoReset = false;
            testSystemsTimer.Interval = 10000;
            testSystemsTimer.Elapsed += this.testSystemsTimer_Delegate;

            this.creditsBox.Maximum = Decimal.MaxValue;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Screen screen = Screen.FromControl(this);
            Rectangle workingArea = screen.WorkingArea;
            int[] winLoc = loadWinLoc(settingsRef.LocationParent);
            int[] winSize = loadWinSize(settingsRef.SizeParent);
            validConfigs = parseValidConfigs();

            // restore window size from config
            if (winSize.Length != 0 && winSize != null)
                this.Size = new Size(winSize[0], winSize[1]);
            else
            {// save our default size to the config
                settingsRef.SizeParent = saveWinSize(this);
            }

            // try to remember and restore the window location
            if (winLoc.Length != 0 && winLoc != null)
            {
                this.Location = new Point(winLoc[0], winLoc[1]);
                // if we're restoring the location, let's force it to be visible on screen
                Form1.forceFormOnScreen(this);
            }
            else
            {
                this.Location = new Point()
                {
                    X = Math.Max(workingArea.X, workingArea.X + (workingArea.Width - this.Width) / 2),
                    Y = Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - this.Height) / 2)
                };
            }

            validateSettings();
            // copy variables to controls
            copySettingsFromConfig();

            // bind our alternate config files
            altConfigBox.DataSource = validConfigs[1]; // only the names, we use this for our index

            if (!checkIfFileOpens(configFile))
                Serialize(configFile); // overwrite the config file for validity

            if (settingsRef.HasUpdated)
            {// display the changelog for the user
                Form3 changelogForm = new Form3();
                settingsRef.HasUpdated = false; // we've updated
                Serialize(configFile); // force a serialize to catch any missing tags
                doHotSwapCleanup(); // call cleanup to remove unnecessary files if they exist

                // show the user the changelog after an update
                if (File.Exists(localDir + "\\Changelog.txt"))
                {
                    changelogForm.ShowDialog(this); // modal
                    changelogForm.Dispose();
                }
            }
            else
                backgroundWorker5.RunWorkerAsync(); // start the auto-updater delegate

            // load our last saved config
            if (!String.IsNullOrEmpty(settingsRef.LastUsedConfig)
                && settingsRef.LastUsedConfig != configFile
                && validateConfigFile(settingsRef.LastUsedConfig))
            {
                loadSettings(settingsRef.LastUsedConfig);
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // start the database uploader
            backgroundWorker6.RunWorkerAsync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkIfFileOpens(configFileDefault))
            {// serialize window data to the default config file
                settingsRef.LocationParent = saveWinLoc(this);
                settingsRef.SizeParent = saveWinSize(this);
                Serialize(configFileDefault, settingsRef.LocationParent, "LocationParent");
                Serialize(configFileDefault, settingsRef.SizeParent, "SizeParent");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // forcefully shutdown the process handler first
            td_proc.Close();
            td_proc.Dispose();
            Application.Exit();
        }
        #endregion

        #region FormSettings
        private void copySettingsFromConfig()
        {
            //
            // Load controls in the form from variables in memory
            //

            avoidBox.Text = settingsRef.Avoid;
            viaBox.Text = settingsRef.Via;
            cmdrNameTextBox.Text = settingsRef.CmdrName;

            if (settingsRef.Capacity > capacityBox.Minimum && settingsRef.Capacity <= capacityBox.Maximum)
                capacityBox.Value = settingsRef.Capacity;
            else
                capacityBox.Value = capacityBox.Minimum;

            if (settingsRef.Credits > creditsBox.Minimum && settingsRef.Credits <= creditsBox.Maximum)
                creditsBox.Value = settingsRef.Credits;
            else
                creditsBox.Value = creditsBox.Minimum;

            if (settingsRef.Insurance > insuranceBox.Minimum && settingsRef.Insurance <= insuranceBox.Maximum)
                insuranceBox.Value = settingsRef.Insurance;
            else
                insuranceBox.Value = insuranceBox.Minimum;

            if (settingsRef.LSPenalty > lsPenaltyBox.Minimum && settingsRef.LSPenalty <= lsPenaltyBox.Maximum)
                lsPenaltyBox.Value = settingsRef.LSPenalty;
            else
                lsPenaltyBox.Value = lsPenaltyBox.Minimum;

            if (settingsRef.MaxLSDistance > maxLSDistanceBox.Minimum && settingsRef.MaxLSDistance <= maxLSDistanceBox.Maximum)
                maxLSDistanceBox.Value = settingsRef.MaxLSDistance;
            else
                maxLSDistanceBox.Value = maxLSDistanceBox.Minimum;

            if (settingsRef.LoopInt > loopIntBox.Minimum && settingsRef.LoopInt <= loopIntBox.Maximum)
                loopIntBox.Value = settingsRef.LoopInt;
            else
                loopIntBox.Value = loopIntBox.Minimum;

            if (settingsRef.Age > ageBox.Minimum && settingsRef.Age <= ageBox.Maximum)
                ageBox.Value = settingsRef.Age;
            else
                ageBox.Value = ageBox.Minimum;

            if (settingsRef.PruneHops > pruneHopsBox.Minimum && settingsRef.PruneHops <= pruneHopsBox.Maximum)
                pruneHopsBox.Value = settingsRef.PruneHops;
            else
                pruneHopsBox.Value = pruneHopsBox.Minimum;

            if (settingsRef.PruneScore > pruneScoreBox.Minimum && settingsRef.PruneScore <= pruneScoreBox.Maximum)
                pruneScoreBox.Value = settingsRef.PruneScore;
            else
                pruneScoreBox.Value = pruneScoreBox.Minimum;

            if (settingsRef.Stock > stockBox.Minimum && settingsRef.Stock <= stockBox.Maximum)
                stockBox.Value = settingsRef.Stock;
            else
                stockBox.Value = stockBox.Minimum;

            if (settingsRef.Demand > demandBox.Minimum && settingsRef.Demand <= demandBox.Maximum)
                demandBox.Value = settingsRef.Demand;
            else
                demandBox.Value = demandBox.Minimum;

            if (settingsRef.GPT > gptBox.Minimum && settingsRef.GPT <= gptBox.Maximum)
                gptBox.Value = settingsRef.GPT;
            else
                gptBox.Value = gptBox.Minimum;

            if (settingsRef.MaxGPT > maxGPTBox.Minimum && settingsRef.MaxGPT <= maxGPTBox.Maximum)
                maxGPTBox.Value = settingsRef.MaxGPT;
            else
                maxGPTBox.Value = maxGPTBox.Minimum;

            if (settingsRef.Hops > hopsBox.Minimum && settingsRef.Hops <= hopsBox.Maximum)
                hopsBox.Value = settingsRef.Hops;
            else
                hopsBox.Value = hopsBox.Minimum;

            if (settingsRef.Jumps > jumpsBox.Minimum && settingsRef.Jumps <= jumpsBox.Maximum)
                jumpsBox.Value = settingsRef.Jumps;
            else
                jumpsBox.Value = jumpsBox.Minimum;

            if (settingsRef.Limit > limitBox.Minimum && settingsRef.Limit <= limitBox.Maximum)
                limitBox.Value = settingsRef.Limit;
            else
                limitBox.Value = limitBox.Minimum;

            if (settingsRef.AbovePrice > abovePriceBox.Minimum && settingsRef.AbovePrice <= abovePriceBox.Maximum)
                abovePriceBox.Value = settingsRef.AbovePrice;
            else
                abovePriceBox.Value = abovePriceBox.Minimum;

            if (settingsRef.BelowPrice > belowPriceBox.Minimum && settingsRef.BelowPrice <= belowPriceBox.Maximum)
                belowPriceBox.Value = settingsRef.BelowPrice;
            else
                belowPriceBox.Value = belowPriceBox.Minimum;

            if (settingsRef.UnladenLY > unladenLYBox.Minimum && settingsRef.UnladenLY <= unladenLYBox.Maximum)
                unladenLYBox.Value = settingsRef.UnladenLY;
            else
                unladenLYBox.Value = unladenLYBox.Minimum;

            if (settingsRef.LadenLY > ladenLYBox.Minimum && settingsRef.LadenLY <= ladenLYBox.Maximum)
                ladenLYBox.Value = settingsRef.LadenLY;
            else
                ladenLYBox.Value = ladenLYBox.Minimum;

            if (settingsRef.Margin > marginBox.Minimum && settingsRef.Margin <= marginBox.Maximum)
                marginBox.Value = settingsRef.Margin;
            else
                marginBox.Value = marginBox.Minimum;

            // copy verbosity to string format
            if (settingsRef.Verbosity == 0)
            {
                t_outputVerbosity = string.Empty;
                verbosityComboBox.SelectedIndex = 0;
            }
            else if (settingsRef.Verbosity == 2)
            {
                t_outputVerbosity = "-vv";
                verbosityComboBox.SelectedIndex = 2;
            }
            else if (settingsRef.Verbosity == 3)
            {
                t_outputVerbosity = "-vvv";
                verbosityComboBox.SelectedIndex = 3;
            }
            else
            {
                t_outputVerbosity = "-v";
                verbosityComboBox.SelectedIndex = 1;
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
            if (settingsRef.ShowJumps)
                showJumpsCheckBox.Checked = true;
            else
                showJumpsCheckBox.Checked = false;

            if (settingsRef.Unique)
                uniqueCheckBox.Checked = true;
            else
                uniqueCheckBox.Checked = false;

            if (containsPadSizes(settingsRef.Padsizes))
                padSizeBox.Text = settingsRef.Padsizes;
            else
                padSizeBox.Text = string.Empty;

            if (settingsRef.TestSystems)
                testSystemsCheckBox.Checked = settingsRef.TestSystems;
            else
                testSystemsCheckBox.Checked = settingsRef.TestSystems;
        }

        private void copySettingsFromForm()
        {
            //
            // Load variables from text boxes in the form
            //

            decimal t_Capacity, t_Credits, t_Insurance, t_lsPenalty, t_maxLSDistance, t_pruneHops, t_pruneScore, t_Margin, t_LoopInt, t_Age, t_GPT, t_MaxGPT, t_Limit, t_Hops, t_Jumps, t_abovePrice, t_belowPrice, t_unladenLY, t_Stock, t_Demand, t_MinAge;

            // make sure we strip the excess whitespace in src/dest
            temp_src = removeExtraWhitespace(srcSystemComboBox.Text);
            temp_dest = removeExtraWhitespace(destSystemComboBox.Text);

            temp_commod = commodityComboBox.Text;
            t_maxPadSize = stn_padSizeBox.Text;

            // make sure we don't pass the "Ship's Sold" box if its still unchanged
            if (shipsSoldBox.Text.Equals(outputStationShips.ToString(), StringComparison.OrdinalIgnoreCase))
                temp_shipsSold = string.Empty;
            else
                temp_shipsSold = shipsSoldBox.Text;

            if (!String.IsNullOrEmpty(cmdrNameTextBox.Text))
                settingsRef.CmdrName = cmdrNameTextBox.Text;

            if (methodIndex == 3)
                r_fromBox = avoidBox.Text;
            else
                settingsRef.Avoid = avoidBox.Text;

            settingsRef.Via = viaBox.Text;

            /*
             * The following is a set of workarounds to fix a bug in Framework 2.0+
             * involving NumericUpDown controls not updating the Value() property
             * when changed from the keyboard instead of the spinner control. We
             * do this by parsing the unbrowsable Text property and copying that.
             */

            if (decimal.TryParse(capacityBox.Text, out t_Capacity))
            {
                capacityBox.Text = t_Capacity.ToString();
                settingsRef.Capacity = t_Capacity;
            }
            else
            {
                settingsRef.Capacity = capacityBox.Minimum; // this is a requirement
                capacityBox.Text = settingsRef.Capacity.ToString();
            }

            if (decimal.TryParse(creditsBox.Text, out t_Credits))
            {
                creditsBox.Text = t_Credits.ToString();
                settingsRef.Credits = t_Credits;
            }
            else
            {
                settingsRef.Credits = creditsBox.Minimum; // this is a requirement
                creditsBox.Text = settingsRef.Credits.ToString();
            }

            if (decimal.TryParse(insuranceBox.Text, out t_Insurance))
            {
                insuranceBox.Text = t_Insurance.ToString();
                settingsRef.Insurance = t_Insurance;
            }
            else
            {
                settingsRef.Insurance = insuranceBox.Minimum;
                insuranceBox.Text = settingsRef.Insurance.ToString();
            }

            if (decimal.TryParse(lsPenaltyBox.Text, out t_lsPenalty))
            {
                lsPenaltyBox.Text = t_lsPenalty.ToString();
                settingsRef.LSPenalty = t_lsPenalty;
            }
            else
            {
                settingsRef.LSPenalty = lsPenaltyBox.Minimum;
                lsPenaltyBox.Text = settingsRef.LSPenalty.ToString();
            }

            if (decimal.TryParse(maxLSDistanceBox.Text, out t_maxLSDistance))
            {
                maxLSDistanceBox.Text = t_maxLSDistance.ToString();
                settingsRef.MaxLSDistance = t_maxLSDistance;
            }
            else
            {
                settingsRef.MaxLSDistance = maxLSDistanceBox.Minimum;
                maxLSDistanceBox.Text = settingsRef.MaxLSDistance.ToString();
            }

            if (decimal.TryParse(loopIntBox.Text, out t_LoopInt))
            {
                loopIntBox.Text = t_LoopInt.ToString();
                settingsRef.LoopInt = t_LoopInt;
            }
            else
            {
                settingsRef.LoopInt = loopIntBox.Minimum;
                loopIntBox.Text = settingsRef.LoopInt.ToString();
            }

            if (decimal.TryParse(pruneHopsBox.Text, out t_pruneHops))
            {
                pruneHopsBox.Text = t_pruneHops.ToString();
                settingsRef.PruneHops = t_pruneHops;
            }
            else
            {
                settingsRef.PruneHops = pruneHopsBox.Minimum;
                pruneHopsBox.Text = settingsRef.PruneHops.ToString();
            }

            if (decimal.TryParse(pruneScoreBox.Text, out t_pruneScore))
            {
                pruneScoreBox.Text = t_pruneScore.ToString();
                settingsRef.PruneScore = t_pruneScore;
            }
            else
            {
                settingsRef.PruneScore = pruneScoreBox.Minimum;
                pruneScoreBox.Text = settingsRef.PruneScore.ToString();
            }

            if (decimal.TryParse(stockBox.Text, out t_Stock))
            {
                stockBox.Text = t_Stock.ToString();
                settingsRef.Stock = t_Stock;
            }
            else
            {
                settingsRef.Stock = stockBox.Minimum;
                stockBox.Text = settingsRef.Stock.ToString();
            }

            if (decimal.TryParse(demandBox.Text, out t_Demand))
            {
                demandBox.Text = t_Demand.ToString();
                settingsRef.Demand = t_Demand;
            }
            else
            {
                settingsRef.Demand = demandBox.Minimum;
                demandBox.Text = settingsRef.Demand.ToString();
            }

            if (decimal.TryParse(ageBox.Text, out t_Age))
            {
                ageBox.Text = t_Age.ToString();
                settingsRef.Age = t_Age;
            }
            else
            {
                settingsRef.Age = ageBox.Minimum;
                ageBox.Text = settingsRef.Age.ToString();
            }

            if (decimal.TryParse(gptBox.Text, out t_GPT))
            {
                gptBox.Text = t_GPT.ToString();
                settingsRef.GPT = t_GPT;
            }
            else
            {
                settingsRef.GPT = gptBox.Minimum;
                gptBox.Text = settingsRef.GPT.ToString();
            }

            if (decimal.TryParse(maxGPTBox.Text, out t_MaxGPT))
            {
                maxGPTBox.Text = t_MaxGPT.ToString();
                settingsRef.MaxGPT = t_MaxGPT;
            }
            else
            {
                settingsRef.MaxGPT = maxGPTBox.Minimum;
                maxGPTBox.Text = settingsRef.MaxGPT.ToString();
            }

            if (decimal.TryParse(limitBox.Text, out t_Limit))
            {
                limitBox.Text = t_Limit.ToString();
                settingsRef.Limit = t_Limit;
            }
            else
            {
                settingsRef.Limit = limitBox.Minimum;
                limitBox.Text = settingsRef.Limit.ToString();
            }

            if (decimal.TryParse(hopsBox.Text, out t_Hops))
            {
                hopsBox.Text = t_Hops.ToString();
                settingsRef.Hops = t_Hops;
            }
            else
            {
                settingsRef.Hops = hopsBox.Minimum;
                hopsBox.Text = settingsRef.Hops.ToString();
            }

            if (decimal.TryParse(jumpsBox.Text, out t_Jumps))
            {
                jumpsBox.Text = t_Jumps.ToString();
                settingsRef.Jumps = t_Jumps;
            }
            else
            {
                settingsRef.Jumps = jumpsBox.Minimum;
                jumpsBox.Text = settingsRef.Jumps.ToString();
            }

            if (decimal.TryParse(lsFromStarBox.Text, out t_lsFromStar))
                lsFromStarBox.Text = t_lsFromStar.ToString();
            else
            {
                t_lsFromStar = lsFromStarBox.Minimum;
                lsFromStarBox.Text = t_lsFromStar.ToString();
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

            if (decimal.TryParse(abovePriceBox.Text, out t_abovePrice))
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

            if (decimal.TryParse(minAgeUpDown.Text, out t_MinAge))
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

            // convert the station checkstates to ints for our delegate
            stn_blackmarketBoxChecked = getCheckBoxCheckState(blackMarketCheckBox.CheckState);
            stn_shipyardBoxChecked = getCheckBoxCheckState(shipyardCheckBox.CheckState);
            stn_marketBoxChecked = getCheckBoxCheckState(marketCheckBox.CheckState);
            stn_repairBoxChecked = getCheckBoxCheckState(repairCheckBox.CheckState);
            stn_rearmBoxChecked = getCheckBoxCheckState(rearmCheckBox.CheckState);
            stn_refuelBoxChecked = getCheckBoxCheckState(refuelCheckBox.CheckState);
            stn_outfitBoxChecked = getCheckBoxCheckState(outfitCheckBox.CheckState);

            // convert the local checkstates to ints as well
            blackmarketBoxChecked = getCheckBoxCheckState(bmktFilterCheckBox.CheckState);
            shipyardBoxChecked = getCheckBoxCheckState(shipyardFilterCheckBox.CheckState);
            marketBoxChecked = getCheckBoxCheckState(itemsFilterCheckBox.CheckState);
            repairBoxChecked = getCheckBoxCheckState(repairFilterCheckBox.CheckState);
            rearmBoxChecked = getCheckBoxCheckState(rearmFilterCheckBox.CheckState);
            refuelBoxChecked = getCheckBoxCheckState(refuelFilterCheckBox.CheckState);
            outfitBoxChecked = getCheckBoxCheckState(outfitFilterCheckBox.CheckState);

            //
            // exceptions
            //
            if (verbosityComboBox.SelectedIndex == 0)
                settingsRef.Verbosity = 0;
            else if (verbosityComboBox.SelectedIndex == 2)
                settingsRef.Verbosity = 2;
            else if (verbosityComboBox.SelectedIndex == 3)
                settingsRef.Verbosity = 3;
            else
                settingsRef.Verbosity = 1;

            if (containsPadSizes(padSizeBox.Text))
                settingsRef.Padsizes = padSizeBox.Text;
            else
            {
                settingsRef.Padsizes = string.Empty;
                padSizeBox.Text = settingsRef.Padsizes.ToString();
            }

            if (containsHexCode(confirmBox.Text))
                t_confirmCode = confirmBox.Text;
            else
            {
                t_confirmCode = string.Empty;
                confirmBox.Text = t_confirmCode.ToString();
            }

            // handle floats differently
            if (decimal.TryParse(unladenLYBox.Text, out t_unladenLY))
            {
                if (methodIndex == 3)
                    r_unladenLY = decimal.Truncate(t_unladenLY * 100) / 100;
                else
                    settingsRef.UnladenLY = decimal.Truncate(t_unladenLY * 100) / 100;
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
                        r_ladenLY = decimal.Truncate(t_ladenLY * 100) / 100; // an exception for the rare command
                    else if (methodIndex == 1)
                        t1_ladenLY = decimal.Truncate(t_ladenLY * 100) / 100; // an exception for the buy command
                    else if (methodIndex == 2)
                        t2_ladenLY = decimal.Truncate(t_ladenLY * 100) / 100; // an exception for the sell command
                    else
                        settingsRef.LadenLY = decimal.Truncate(t_ladenLY * 100) / 100;
                }
                else
                {
                    settingsRef.LadenLY = decimal.Truncate(ladenLYBox.Minimum * 100) / 100;
                    ladenLYBox.Text = settingsRef.LadenLY.ToString();
                }
            }
            else
            {// exception for local override
                if (decimal.TryParse(ladenLYBox.Text, out t_ladenLY))
                {
                    t_ladenLY = decimal.Truncate(t_ladenLY * 100) / 100;
                }
                else
                    t_ladenLY = decimal.Truncate(ladenLYBox.Minimum * 100) / 100;
            }

            if (decimal.TryParse(marginBox.Text, out t_Margin))
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
        #endregion

        #region Delegates
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
             * This worker delegate updates the commodities and recent systems lists
             */

            // let the refresh methods decide what to refresh
            this.Invoke(new Action(() =>
            {
                if (buttonCaller == 16)
                    buildOutput(true); // update everything
                else
                    buildOutput(false);
            }));
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            enableRunButtons();

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

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
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
                t_path = string.Empty; // go in blank so we don't pass silliness to trade.exe
            else
                t_path = "-u \"" + settingsRef.TDPath + "\\trade.py\" ";


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
            {// we're coming from shift+import button
                if (!String.IsNullOrEmpty(settingsRef.ImportPath))
                    t_path += "import \"" + settingsRef.ImportPath + "\"";
                else
                    okayToRun = false;
            }
            else if (buttonCaller == 12)
            {// we're coming from import button
                t_path += "import -P edapi -O eddn";
            }
            else if (buttonCaller == 11)
            {// we're coming from getUpdatedPricesFile()
                if (!String.IsNullOrEmpty(temp_src))
                    t_path += "update -A -D --editor=\"nul\" \"" + temp_src + "\"";
                else
                {
                    okayToRun = false;
                    playAlert();
                }
            }
            else if (buttonCaller == 10)
            {// we're coming from station commodities editor
                if (!String.IsNullOrEmpty(temp_src))
                    t_path += "update -A -D -G \"" + temp_src + "\"";
                else
                {
                    okayToRun = false;
                    playAlert();
                }
            }
            else if (t_localNavEnabled)
            {// local is a global override, let's catch it first
                if (!String.IsNullOrEmpty(temp_src))
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

                    if (!String.IsNullOrEmpty(settingsRef.Padsizes)) { t_path += " --pad=" + settingsRef.Padsizes; }
                    if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }
                    t_path += " \"" + temp_src + "\"";
                }
                else
                {
                    okayToRun = false;
                    playAlert();
                }
            }
            else if (methodIndex == 10)
            {// coming from the EDSC command (Submit)
                // make sure we only use a system name (not system/station), and that it doesn't already exist
                if (!String.IsNullOrEmpty(temp_src) && temp_src.IndexOf("/") >= 0)
                    temp_src = temp_src.Substring(0, temp_src.IndexOf("/")); // remove station

                if (stationIndex == 0 && validateEDSCInput())
                {
                    SubmitSystem(temp_src, refSysTextBox1.Text, t_refDist1, refSysTextBox2.Text, t_refDist2, refSysTextBox3.Text, t_refDist3, refSysTextBox4.Text, t_refDist4, refSysTextBox5.Text, t_refDist5, cmdrNameTextBox.Text);

                    // save our cmdrname to our config for next time
                    Serialize(configFile, settingsRef.CmdrName, "CmdrName");
                }
                else if (stationIndex > 0)
                {// coming from the EDSC command (Lookup/Recent)
                    if (!GetSystems(temp_src, (int)crFilterUpDown.Value))
                        buttonCaller = 20; // mark us for a notification
                }
                else
                    buttonCaller = 20;

                okayToRun = false;
            }
            else if (methodIndex == 9)
            {// mark us as coming from the olddata command
                if (!String.IsNullOrEmpty(temp_src))
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
                    playAlert();
                }
            }
            else if (methodIndex == 8)
            {// mark us as coming from the nav command
                if (!String.IsNullOrEmpty(temp_src) && !String.IsNullOrEmpty(temp_dest))
                {
                    t_path += "nav";

                    if (!String.IsNullOrEmpty(settingsRef.Via)) { t_path += " --via=\"" + settingsRef.Via + "\""; }
                    if (!String.IsNullOrEmpty(settingsRef.Avoid)) { t_path += " --avoid=\"" + settingsRef.Avoid + "\""; }
                    if (settingsRef.LadenLY > 0) { t_path += " --ly=" + settingsRef.LadenLY; }
                    if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }
                    t_path += " \"" + temp_src + "\" " + "\"" + temp_dest + "\"";
                }
                else
                {
                    okayToRun = false;
                    playAlert();
                }
            }
            else if ((methodIndex == 6 || methodIndex == 7) && t_csvExportCheckBox)
            {// we're coming from the csvExport override
                if (exportIndex == 1)
                {// an override for System export
                    t_path += "export --tables System";
                }
                else if (exportIndex == 2)
                {// an override for Station export
                    t_path += "export --tables Station";
                }
                else if (exportIndex == 3)
                {// an override for ShipVendor export
                    t_path += "export --tables ShipVendor";
                }
                else
                {// catch all
                    t_path += "export --tables ShipVendor,System,Station";
                }
            }
            else if (methodIndex == 7)
            {// mark us as coming from the shipvendor editor
                if (!String.IsNullOrEmpty(temp_src))
                {
                    if (!String.IsNullOrEmpty(temp_shipsSold) && stationIndex != 2)
                    {
                        // clean our ship input before passing
                        string sanitizedInput = cleanShipVendorInput(temp_shipsSold);

                        if (stationIndex == 0)
                        {// we're adding/updating (default)
                            t_path += "shipvendor -a \"" + temp_src + "\" " + "\"" + sanitizedInput + "\"";
                        }
                        else if (stationIndex == 1)
                        {// removing
                            t_path += "shipvendor -rm \"" + temp_src + "\" " + "\"" + sanitizedInput + "\"";
                        }

                        // force a station/shipvendor panel update
                        buttonCaller = 17;
                    }
                    else
                        t_path += "shipvendor \"" + temp_src + "\"";
                }
                else
                {
                    okayToRun = false;
                    playAlert();
                }
            }
            else if (methodIndex == 6)
            {// mark us as coming from the station editor
                if (!String.IsNullOrEmpty(temp_src))
                {
                    t_path += "station ";

                    if (stationIndex == 0 || stationIndex == 1)
                    {
                        if (stationIndex == 0)
                            // we're updating (default)
                            t_path += "-u";
                        else if (stationIndex == 1)
                        {
                            t_path += "-a";
                            buttonCaller = 16; // mark us as forcing a db update
                        }

                        if (stn_blackmarketBoxChecked == 2) { t_path += " --bm=" + "\"?\""; }
                        else if (stn_blackmarketBoxChecked == 1) { t_path += " --bm=" + "\"Y\""; }
                        else if (stn_blackmarketBoxChecked == 0) { t_path += " --bm=" + "\"N\""; }

                        if (stn_shipyardBoxChecked == 2) { t_path += " --shipyard=" + "\"?\""; }
                        else if (stn_shipyardBoxChecked == 1) { t_path += " --shipyard=" + "\"Y\""; }
                        else if (stn_shipyardBoxChecked == 0) { t_path += " --shipyard=" + "\"N\""; }

                        if (stn_marketBoxChecked == 2) { t_path += " --market=" + "\"?\""; }
                        else if (stn_marketBoxChecked == 1) { t_path += " --market=" + "\"Y\""; }
                        else if (stn_marketBoxChecked == 0) { t_path += " --market=" + "\"N\""; }

                        if (stn_rearmBoxChecked == 2) { t_path += " --rearm=" + "\"?\""; }
                        else if (stn_rearmBoxChecked == 1) { t_path += " --rearm=" + "\"Y\""; }
                        else if (stn_rearmBoxChecked == 0) { t_path += " --rearm=" + "\"N\""; }

                        if (stn_repairBoxChecked == 2) { t_path += " --repair=" + "\"?\""; }
                        else if (stn_repairBoxChecked == 1) { t_path += " --repair=" + "\"Y\""; }
                        else if (stn_repairBoxChecked == 0) { t_path += " --repair=" + "\"N\""; }

                        if (stn_refuelBoxChecked == 2) { t_path += " --refuel=" + "\"?\""; }
                        else if (stn_refuelBoxChecked == 1) { t_path += " --refuel=" + "\"Y\""; }
                        else if (stn_refuelBoxChecked == 0) { t_path += " --refuel=" + "\"N\""; }

                        if (stn_outfitBoxChecked == 2) { t_path += " --outfitting=" + "\"?\""; }
                        else if (stn_outfitBoxChecked == 1) { t_path += " --outfitting=" + "\"Y\""; }
                        else if (stn_outfitBoxChecked == 0) { t_path += " --outfitting=" + "\"N\""; }

                        if (t_lsFromStar > 0) { t_path += " --ls-from-star=" + t_lsFromStar; }
                        if (!String.IsNullOrEmpty(t_maxPadSize)) { t_path += " --pad-size=\"" + t_maxPadSize + "\""; }
                        if (!String.IsNullOrEmpty(t_confirmCode)) { t_path += " --confirm=" + t_confirmCode; }
                        t_path += " \"" + temp_src + "\"";
                    }
                    else if (stationIndex == 2)
                    {// removing
                        t_path += "-rm";
                        if (!String.IsNullOrEmpty(t_confirmCode)) { t_path += " --confirm=" + t_confirmCode; }
                        t_path += " \"" + temp_src + "\"";
                        buttonCaller = 16; // mark us as forcing a db update
                    }
                }
                else
                {
                    okayToRun = false;
                    playAlert();
                }
            }
            else if (methodIndex == 5)
            { // Market command
                if (!String.IsNullOrEmpty(temp_src))
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
                    playAlert();
                }
            }
            else if (methodIndex == 4)
            {// mark us as coming from the trade command
                if (!String.IsNullOrEmpty(temp_src) && !String.IsNullOrEmpty(temp_dest))
                {
                    t_path += "trade";

                    t_path += " \"" + temp_src + "\" " + "\"" + temp_dest + "\" " + t_outputVerbosity;
                }
                else
                {
                    okayToRun = false;
                    playAlert();
                }
            }
            else if (methodIndex == 3)
            { // Rares command
                if (!String.IsNullOrEmpty(temp_src))
                {
                    t_path += "rares";

                    if (r_ladenLY > 0) { t_path += " --ly=" + r_ladenLY; }

                    if (stationIndex == 1) { t_path += " --legal"; }
                    else if (stationIndex == 2) { t_path += " --illegal"; }

                    if (!String.IsNullOrEmpty(settingsRef.Padsizes)) { t_path += " --pad=" + settingsRef.Padsizes; }
                    if (r_unladenLY > 0 && !String.IsNullOrEmpty(r_fromBox)) { t_path += " --away=" + r_unladenLY; }
                    if (!String.IsNullOrEmpty(r_fromBox) && r_unladenLY > 0)
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
                    playAlert();
                }
            }
            else if (methodIndex == 2)
            { // Sell command
                t_path += "sell";

                if (!String.IsNullOrEmpty(temp_src)) { t_path += " --near=\"" + temp_src + "\""; }
                if (t2_ladenLY > 0) { t_path += " --ly=" + t2_ladenLY; }
                if (settingsRef.AbovePrice > 0) { t_path += " --gt=" + settingsRef.AbovePrice; }
                if (settingsRef.BelowPrice > 0) { t_path += " --ls=" + settingsRef.BelowPrice; }
                if (demandBox.Value > 0) { t_path += " --demand=" + demandBox.Value; }
                if (bmktCheckBox.Checked) { t_path += " --bm"; }
                if (!String.IsNullOrEmpty(settingsRef.Padsizes)) { t_path += " --pad=" + settingsRef.Padsizes; }
                if (!String.IsNullOrEmpty(settingsRef.Avoid)) { t_path += " --avoid=\"" + settingsRef.Avoid + "\""; }
                if (stationIndex == 1 && !String.IsNullOrEmpty(temp_src)) { t_path += " --price-sort"; }
                if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }
                t_path += " --lim=50";

                if (!String.IsNullOrEmpty(temp_commod))
                {
                    t_path += " \"" + temp_commod + "\"";
                }
            }
            else if (methodIndex == 1)
            { // Buy command
                t_path += "buy";

                if (!String.IsNullOrEmpty(temp_src)) { t_path += " --near=\"" + temp_src + "\""; }
                if (t1_ladenLY > 0) { t_path += " --ly=" + t1_ladenLY; }
                if (stockBox.Value > 0) { t_path += " --supply=" + stockBox.Value; }
                if (settingsRef.AbovePrice > 0) { t_path += " --gt=" + settingsRef.AbovePrice; }
                if (settingsRef.BelowPrice > 0) { t_path += " --lt=" + settingsRef.BelowPrice; }
                if (oneStopCheckBox.Checked) { t_path += " -1"; }
                if (bmktCheckBox.Checked) { t_path += " --bm"; }
                if (!String.IsNullOrEmpty(settingsRef.Padsizes)) { t_path += " --pad=" + settingsRef.Padsizes; }
                if (!String.IsNullOrEmpty(settingsRef.Avoid)) { t_path += " --avoid=\"" + settingsRef.Avoid + "\""; }

                if (stationIndex == 1 && !String.IsNullOrEmpty(temp_src)) { t_path += " --price-sort"; }
                else if (stationIndex == 2) { t_path += " --units-sort"; }

                if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }
                t_path += " --lim=50";

                if (!String.IsNullOrEmpty(temp_commod))
                {
                    t_path += " \"" + temp_commod + "\"";
                }
            }
            else if (methodIndex == 0)
            { // Run command
                t_path += "run";

                if (!String.IsNullOrEmpty(temp_src))
                {
                    t_path += " --fr=\"" + temp_src + "\"";

                    // towards requires a source
                    if (!String.IsNullOrEmpty(temp_dest) && settingsRef.Towards)
                        t_path += " --towards=\"" + temp_dest + "\"";
                }

                if (!String.IsNullOrEmpty(temp_dest) && !settingsRef.Towards && !settingsRef.Loop)
                {// allow a destination without a source for anonymous Runs
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
                if (!String.IsNullOrEmpty(settingsRef.Padsizes)) { t_path += " --pad=" + settingsRef.Padsizes; }
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
                if (!String.IsNullOrEmpty(settingsRef.Avoid)) { t_path += " --avoid=\"" + settingsRef.Avoid + "\""; }
                if (!String.IsNullOrEmpty(settingsRef.Via)) { t_path += " --via=\"" + settingsRef.Via + "\""; }
                if (settingsRef.Loop) { t_path += " --loop"; }
                if (settingsRef.LoopInt > 0) { t_path += " --loop-int=" + settingsRef.LoopInt; }
                if (belowPriceBox.Value > 0) { t_path += " --routes=" + belowPriceBox.Value; }
                if (settingsRef.ShowJumps) { t_path += " -J"; }
                if (!String.IsNullOrEmpty(settingsRef.ExtraRunParams)) { t_path += " " + settingsRef.ExtraRunParams; }
                if (settingsRef.Verbosity > 0) { t_path += " " + t_outputVerbosity; }

                buttonCaller = 4; // mark as Run command
            }

            // pass the built command-line to the delegate
            if (okayToRun)
                doTDProc(t_path);
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!backgroundWorker2.IsBusy)
            {
                stopwatch.Stop(); // stop the timer
                circularBuffer = new System.Text.StringBuilder(2 * circularBufferSize);

                // turn off checkboxes where sensible
                if (t_csvExportCheckBox) { csvExportCheckBox.Checked = false; }
                if (!String.IsNullOrEmpty(confirmBox.Text)) { confirmBox.Text = string.Empty; }

                // let's alert the user that the Output pane has changed
                if (!String.IsNullOrEmpty(td_outputBox.Text) && tabControl1.SelectedTab != outputPage)
                    tabControl1.SelectedTab = outputPage;

                // assume we're coming from getUpdatedPricesFile()
                if (!String.IsNullOrEmpty(settingsRef.ImportPath) && buttonCaller == 11)
                    cleanUpdatedPricesFile();
                else if (buttonCaller == 16)
                {// force a db sync if we're marked
                    if (!backgroundWorker1.IsBusy)
                        backgroundWorker1.RunWorkerAsync();
                }
                else if (buttonCaller == 17)
                {// force a station/shipvendor panel update
                    populateStationPanel(temp_src);
                }

                // reenable after uncancellable task is done
                enableRunButtons();
                runButton.Font = new Font(runButton.Font, FontStyle.Regular);
                runButton.Text = "&Run";

                td_proc.Dispose();

                // make a sound when we're done with a long operation (>10s)
                if ((stopwatch.ElapsedMilliseconds > 10000 && buttonCaller != 5) || buttonCaller == 20)
                    playAlert(); // when not marked as cancelled, or explicit

                if (buttonCaller != 4) // not if we're coming from Run
                {
                    miniModeButton.Enabled = false;
                }
                else if (buttonCaller == 4)
                {
                    string filteredOutput = filterOutput(td_outputBox.Text);
                    // validate the run output before we enable the mini-mode button
                    runOutputState = isValidRunOutput(filteredOutput);

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

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
             * This worker delegate is just a thread for the background timer to run on
             */

            stopwatch.Start(); // start the timer

            while (stopwatch.IsRunning)
            {
                System.Threading.Thread.Sleep(100);
                this.Invoke(new Action(() =>
                {// do this on the UI thread
                    stopWatchLabel.Text = "Elapsed: " + stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.fff");
                }));
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // when we get the !IsRunning signal, write out
            this.Invoke(new Action(() =>
            {// do this on the UI thread
                stopWatchLabel.Text = "Elapsed: " + stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.fff");
                stopwatch.Reset();
            }));
        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
             * This worker delegate is for the update process
             */

            getDataUpdates(); // update conditionally
            doTDProc(t_path); // pass this to the worker delegate

            t_path = string.Empty; // reset path for thread safety
        }

        private void backgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!backgroundWorker4.IsBusy)
            {
                stopwatch.Stop(); // stop the timer
                circularBuffer = new System.Text.StringBuilder(2 * circularBufferSize);

                enableRunButtons();
                td_proc.Dispose();

                // we have to update the comboboxes now
                if (!backgroundWorker1.IsBusy)
                {
                    buttonCaller = 16; // mark us as needing a full refresh
                    backgroundWorker1.RunWorkerAsync();
                }

                // make a sound when we're done with a long operation (>10s)
                if (stopwatch.ElapsedMilliseconds > 10000)
                    playAlert();

                rebuildCache = false;
            }
        }

        private void backgroundWorker5_DoWork(object sender, DoWorkEventArgs e)
        {// this background worker is for the auto-updater
            if (!settingsRef.DoNotUpdate && !Program.updateOverride)
            {// check for override before running a poll
                doHotSwap();
            }
        }

        private void backgroundWorker5_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (settingsRef.HasUpdated)
            {// show the update notification
                updateNotifyLabel.Visible = true;
                updateNotifyIcon.Visible = true;
            }

            // always wipe the temp files
            doHotSwapCleanup();
        }

        private void backgroundWorker6_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
             * This worker delegate is intended to update the system list every few seconds,
             * notifying when an unrecognized system is detected.
             */

            if (!hasRun)
                buildOutput(true);
            else if (hasRun)
                buildOutput(false); // update with only the most recent

            if (settingsRef.TestSystems)
            {
                if (String.IsNullOrEmpty(t_lastSysCheck) && !String.IsNullOrEmpty(t_lastSystem)
                    && !stringInList(t_lastSystem, outputSysStnNames))
                {// alert the user if we're starting in an unknown system
                    playUnknown();
                    this.Invoke(new Action(() =>
                    {// run this on the UI thread
                        this.td_outputBox.Text += "We're currently in an unrecognized system: " + t_lastSystem + "\r\n";
                        if (settingsRef.CopySystemToClipboard) { Clipboard.SetData(DataFormats.Text, t_lastSystem); }
                    }));
                    t_lastSysCheck = t_lastSystem;
                }
                else if ((!String.IsNullOrEmpty(t_lastSysCheck) && !t_lastSysCheck.Equals(t_lastSystem)
                    && !stringInList(t_lastSystem, outputSysStnNames)) || String.IsNullOrEmpty(t_lastSysCheck)
                    && !stringInList(t_lastSystem, outputSysStnNames))
                {// if we've already checked a recent system, only check the newest entered system once
                    playUnknown(); // alert the user
                    // only flash if the window isn't active
                    if (!isActive) { FlashWindow.BlinkStart(this); }

                    this.Invoke(new Action(() =>
                    {// run this on the UI thread
                        this.td_outputBox.Text += "Entering an unrecognized system: " + t_lastSystem + "\r\n";
                        if (settingsRef.CopySystemToClipboard) { Clipboard.SetData(DataFormats.Text, t_lastSystem); }
                    }));

                    t_lastSysCheck = t_lastSystem; // prevent rechecking the same system
                }
            }
        }

        private void backgroundWorker6_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            testSystemsTimer.Start(); // fire again after ~10s
        }

        private void backgroundWorker7_DoWork(object sender, DoWorkEventArgs e)
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
                    t_path = Path.Combine(Form1.settingsRef.EdcePath, "edce_client.py");
                }
                else
                {
                    okayToRun = false;
                    playAlert();
                }
            }

            // pass the built command-line to the delegate
            if (okayToRun)
            {
                string currentFolder = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(Form1.settingsRef.EdcePath);

                try
                {
                    td_proc = new Process();
                    td_proc.StartInfo.FileName = settingsRef.PythonPath;

                    doTDProc(t_path);
                }
                finally
                {
                    Directory.SetCurrentDirectory(currentFolder);
                }
            }

            t_path = string.Empty; // reset path for thread safety
        }

        private void backgroundWorker7_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!backgroundWorker7.IsBusy)
            {
                stopwatch.Stop(); // stop the timer
                circularBuffer = new System.Text.StringBuilder(2 * circularBufferSize);

                runButton.Text = "&Run";
                enableRunButtons();
                td_proc.Dispose();

                // make a sound when we're done with a long operation (>10s)
                if (stopwatch.ElapsedMilliseconds > 10000)
                {
                    playAlert();
                }

                this.UpdateCreditBalance();
            }
        }

        #endregion

        #region FormHelpers
        private int getCheckBoxCheckState(CheckState checkState)
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

        private void methodSelectState()
        {
            /*
             * We do our command state stuff here
             */

            // make an exception for from/to local override
            if (buttonCaller == 18)
                methodFromIndex = -1; // reset our previous index

            // don't do anything if we're reselecting the same mode from the box
            if (methodFromIndex != methodIndex)
            {
                // handle index changes
                if (methodIndex == 1 || methodIndex == 2)
                {// buy/sell command
                    stationEditorChangeState();

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
                        methodFromIndex = 1;
                    else if (methodIndex == 2)
                        methodFromIndex = 2;
                }
                else if (methodIndex == 3)
                {// rares command
                    stationEditorChangeState(); // more minimal default state

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
                {// trade command
                    runMethodResetState();
                    methodFromIndex = 4;
                }
                else if (methodIndex == 5)
                {// market command
                    stationEditorChangeState();

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
                {// station command
                    stationEditorChangeState();

                    // show the secondary selection box
                    stationDropDown.Visible = true;
                    // set "Update" as the default
                    List<string> shipVendorDrop = new List<string>(new string[] { "Update", "Add", "Remove" });
                    stationDropDown.DataSource = shipVendorDrop;

                    // activate the shipsSoldBox
                    shipsSoldBox.Enabled = false;
                    shipsSoldLabel.Enabled = false;

                    // fix tooltips
                    toolTip1.SetToolTip(methodDropDown, "Add/Remove/List detailed station data for a given station");

                    methodFromIndex = 6;
                }
                else if (methodIndex == 7)
                {// shipvendor command
                    stationEditorChangeState();

                    // show the secondary selection box
                    stationDropDown.Visible = true;
                    // set "add" as the default
                    List<string> shipVendorDrop = new List<string>(new string[] { "Add", "Remove", "List" });
                    stationDropDown.DataSource = shipVendorDrop;

                    // hide the run panel
                    panel1.Visible = false;
                    // activate the station panel
                    panel2.Visible = true;

                    // fix tooltips
                    toolTip1.SetToolTip(methodDropDown, "Add/Remove/List ships being sold at a given station");

                    methodFromIndex = 7;
                }
                else if (methodIndex == 8)
                {// nav command
                    // reset our state to the default for this method
                    stationEditorChangeState();

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

                    methodFromIndex = 8; // mark nav
                }
                else if (methodIndex == 9)
                {// olddata command
                    stationEditorChangeState();

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

                    methodFromIndex = 9; // mark olddata
                }
                else if (methodIndex == 10)
                {// edsc command
                    stationEditorChangeState();

                    // set "submit" as the default
                    List<string> shipVendorDrop = new List<string>(new string[] { "Submit", "Lookup", "Recent" });
                    stationDropDown.DataSource = shipVendorDrop;

                    // fix tooltips
                    toolTip1.SetToolTip(methodDropDown, "Submit/Check systems against EDSC database");
                    toolTip1.SetToolTip(srcSystemComboBox, "System name must be exact and cannot exist in our database for proper submission to EDSC");

                    methodFromIndex = 10; // mark edsc
                }
                else
                {// run command
                    runMethodResetState();
                    toolTip1.SetToolTip(methodDropDown, "Calculates optimal trading routes from Source (Destination optional)");
                    methodFromIndex = 0;
                }
            }
        }

        private void runMethodResetState()
        {// we should use this method as an elevator for the form state
            panel1.Visible = true;
            panel2.Visible = false;
            stationDropDown.Visible = false;
            localFilterParentPanel.Visible = false;
            edscPanel.Visible = false;

            srcSystemComboBox.Enabled = true;
            panel1.Enabled = true;
            panel2.Enabled = false;
            edscPanel.Enabled = false;

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

            // disable the push context menu option
            pushEDSCToCSV.Visible = false;

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
                    t_Supply = stockBox.Value;

                // save below price
                if (belowPriceBox.Value > 0)
                    t_belowPrice = belowPriceBox.Value;
            }
            else if (methodFromIndex == 2)
            { // from sell
                t2_ladenLY = ladenLYBox.Value;

                // save our previous demand
                if (demandBox.Value > 0)
                    t_Demand = demandBox.Value;

                // save below price
                if (belowPriceBox.Value > 0)
                    t_belowPrice = belowPriceBox.Value;
            }
            else if (methodFromIndex == 3)
            {
                // save our ladenLY
                r_ladenLY = ladenLYBox.Value; // from rares
            }
            else if (hasRun)
            {
                // save our volatile settings when switching
                if (ladenLYBox.Value > 0 && settingsRef.LadenLY != ladenLYBox.Value) { settingsRef.LadenLY = ladenLYBox.Value; }
                if (stockBox.Value > 0 && settingsRef.Stock != stockBox.Value) { settingsRef.Stock = stockBox.Value; }
                if (demandBox.Value > 0 && settingsRef.Demand != demandBox.Value) { settingsRef.Demand = demandBox.Value; }
            }

            // main state check
            if (methodIndex == 0)
            {// return to a normal run state
                foreach (Control ctrl in runOptionsPanel.Controls)
                    ctrl.Enabled = true;
                foreach (Control ctrl in panel1.Controls)
                    ctrl.Enabled = true;

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
                    belowPriceBox.Value = t_Routes;

                if (settingsRef.Stock > 0)
                    stockBox.Value = settingsRef.Stock;
                else
                    stockBox.Value = stockBox.Minimum;

                if (settingsRef.Demand > 0)
                    demandBox.Value = settingsRef.Demand;
                else
                    demandBox.Value = demandBox.Minimum;

                oneStopCheckBox.Enabled = false;

                if (srcSystemComboBox.Text.Length > 3 && destSystemComboBox.Text.Length > 3)
                    towardsCheckBox.Enabled = true;
                else
                    towardsCheckBox.Enabled = false;
            }
            else if (methodIndex == 1 || methodIndex == 2)
            {// going into buy/sell
                if (methodIndex == 1)
                    oneStopCheckBox.Enabled = true;
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
                    belowPriceBox.Value = t_belowPrice;

                if (methodIndex == 1 && t_Supply > 0)
                    stockBox.Value = t_Supply;
                else if (methodIndex == 1)
                    stockBox.Value = stockBox.Minimum;
                else if (methodIndex == 2 && t_Demand > 0)
                    demandBox.Value = t_Demand;
                else if (methodIndex == 2)
                    demandBox.Value = demandBox.Minimum;
            }
            else if (methodFromIndex != 5 || methodFromIndex != 6)
            {// catch everything else (except station/shipvendor)
                foreach (Control ctrl in runOptionsPanel.Controls)
                    ctrl.Enabled = false;
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
                ladenLYBox.Value = t1_ladenLY;
            else if (methodIndex == 2 && t2_ladenLY > 0)
                ladenLYBox.Value = t2_ladenLY;
            else if (methodIndex == 3 && r_ladenLY > 0)
                ladenLYBox.Value = r_ladenLY;
            else if (methodIndex == 3 && r_ladenLY == 0)
                ladenLYBox.Value = 120.00m;
            else
                ladenLYBox.Value = settingsRef.LadenLY;

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
            csvExportCheckBox.Checked = false;
            localNavCheckBox.Checked = false;

            // an exception for switching from another command with a valid dest
            if (methodFromIndex != 0 && !String.IsNullOrEmpty(destSystemComboBox.Text))
                endJumpsBox.Enabled = true;
            else
                endJumpsBox.Enabled = false;

            // an exception for the trade command
            if (methodIndex == 4)
            {
                foreach (Control ctrl in panel1.Controls)
                    ctrl.Enabled = false;

                destSystemComboBox.Enabled = true;
                destSysLabel.Enabled = true;
                ageBox.Enabled = false;
                ageLabel.Enabled = false;
            }

            // fix ladenLY minimum for certain commands
            if (methodIndex == 9)
            {
                ladenLYBox.Minimum = 0; // just for the olddata command
            }
            else
            {// default [requirement!]
                if (ladenLYBox.Value < 1) { ladenLYBox.Value = 1; }
                ladenLYBox.Minimum = 1;
            }

            // (re)store the Avoid/From box
            if (methodFromIndex != 3 && methodIndex == 3)
            {// from anywhere (but Rare) to Rare
                settingsRef.Avoid = avoidBox.Text; // save Avoid list

                // restore the contents if it exists
                if (!String.IsNullOrEmpty(r_fromBox))
                    avoidBox.Text = r_fromBox;
                else
                    avoidBox.Text = string.Empty;
            }
            else if (methodFromIndex == 3)
            {// coming from Rare switching to anywhere
                r_fromBox = avoidBox.Text; // save From list

                // restore our contents from global
                if (!String.IsNullOrEmpty(settingsRef.Avoid))
                    avoidBox.Text = settingsRef.Avoid;
                else
                    avoidBox.Text = string.Empty;
            }
        }

        private void stationEditorChangeState()
        {// we should use this method to move to station editor form state
            runMethodResetState(); // start from a working base

            // exclusions
            if (methodIndex != 0)
            {
                ageBox.Enabled = false;
                ageLabel.Enabled = false;
            }

            // catch station/shipvendor here
            if (methodIndex == 7)
            { // shipvendor
                panel1.Enabled = false;
                panel2.Enabled = true;
                edscPanel.Enabled = false;

                panel1.Visible = false;
                panel2.Visible = true;

                // we don't need most panel2 controls
                foreach (Control ctrl in panel2.Controls)
                    ctrl.Enabled = false;

                shipsSoldLabel.Enabled = true;
                shipsSoldBox.Enabled = true;
                csvExportComboBox.Visible = false;
                stationDropDown.Enabled = true;
                toolTip1.SetToolTip(stationDropDown, "Select a mode for station editing");
            }
            else if (methodIndex == 6)
            { // station
                panel1.Enabled = false;
                panel2.Enabled = true;
                edscPanel.Enabled = false;

                panel1.Visible = false;
                panel2.Visible = true;

                foreach (Control ctrl in panel2.Controls)
                    ctrl.Enabled = true;

                shipsSoldLabel.Enabled = false;
                shipsSoldBox.Enabled = false;
                csvExportComboBox.Visible = false;
                stationDropDown.Enabled = true;
                toolTip1.SetToolTip(stationDropDown, "Select a mode for station editing");
            }
            else if (methodIndex == 9)
            {
                panel1.Enabled = false;
                panel2.Enabled = true;
                edscPanel.Enabled = false;

                // we don't need most panel1 controls
                foreach (Control ctrl in panel2.Controls)
                    ctrl.Enabled = false;

                panel1.Visible = false;
                panel2.Visible = true;
                panel2.BringToFront();

                ageBox.Enabled = false;
                ageLabel.Enabled = false;

                minAgeLabel.Visible = true;
                minAgeUpDown.Visible = true;

                minAgeLabel.Enabled = true;
                minAgeUpDown.Enabled = true;

                oldRoutesCheckBox.Enabled = true;
                oldRoutesCheckBox.Visible = true;
            }
            else if (methodIndex == 10)
            {// edsc panel
                panel1.Enabled = false;
                panel2.Enabled = false;
                edscPanel.Enabled = true;

                // disable the push context menu option
                pushEDSCToCSV.Visible = true;

                toolTip1.SetToolTip(stationDropDown, "Select an EDSC command mode");

                if (stationIndex == 0)
                {
                    // disable most controls in this panel
                    foreach (Control ctrl in edscPanel.Controls)
                        ctrl.Enabled = true;

                    crFilterUpDown.Enabled = false;
                    crFilterLabel.Enabled = false;
                    edscPanel.Visible = true;
                    edscPanel.BringToFront();
                    stationDropDown.Visible = true;
                    stationDropDown.Enabled = true;
                }
                else if (stationIndex > 0)
                {
                    if (stationIndex == 2) { srcSystemComboBox.Enabled = false; }

                    // disable most controls in this panel
                    foreach (Control ctrl in edscPanel.Controls)
                        ctrl.Enabled = false;

                    crFilterUpDown.Enabled = true;
                    crFilterLabel.Enabled = true;
                    edscPanel.Visible = true;
                    edscPanel.BringToFront();
                    stationDropDown.Visible = true;
                    stationDropDown.Enabled = true;
                }
            }
            else
            {
                // we don't need most panel1 or run options controls
                foreach (Control ctrl in panel1.Controls)
                    ctrl.Enabled = false;

                foreach (Control ctrl in runOptionsPanel.Controls)
                    ctrl.Enabled = false;
            }
        }

        private void disableRunButtons()
        {
            // disable buttons during uncancellable operation
            updateButton.Enabled = false;
            getSystemButton.Enabled = false;
            miniModeButton.Enabled = false;

            // an exception for Run commands
            if (buttonCaller != 1)
                runButton.Enabled = false;
        }

        private void enableRunButtons()
        {
            // reenable other worker callers when done
            updateButton.Enabled = true;
            getSystemButton.Enabled = true;

            // fix Run button when returning from non-Run commands
            if (buttonCaller == 1 || !runButton.Enabled)
                runButton.Enabled = true;
        }

        private CheckState parseCheckState(string input)
        {
            if (input == "?")
                return CheckState.Indeterminate;
            else if (input == "Y")
                return CheckState.Checked;
            else if (input == "N")
                return CheckState.Unchecked;
            else
                return CheckState.Indeterminate;
        }

        private void validateDestForEndJumps()
        {
            if (!String.IsNullOrEmpty(destSystemComboBox.Text) && methodIndex != 4)
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

        private bool validateDistance(string input, string output)
        {// this is our generic validator for a distance (in LY) in the EDSC panel
            double t_Distance = 0.00f;

            // we require our distances to be 2 decimal places at all times
            if (double.TryParse(input, out t_Distance) && t_Distance > 0.00f && t_Distance <= 500.00f)
            {
                output = String.Format("{0:0.00}", t_Distance);
                return true;
            }
            else
            {
                output = "0.00";
                return false;
            }
        }
        #endregion

        #region FormMembers
        private void testSystemsTimer_Delegate(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine(String.Format("testSystems Firing at: {0}", currentTimestamp()));
            if (!backgroundWorker6.IsBusy && !settingsRef.DisableNetLogs && !String.IsNullOrEmpty(settingsRef.NetLogPath))
            {
                backgroundWorker6.RunWorkerAsync();
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            isActive = true;

            FlashWindow.Stop(this); // stop flashing when we activate the window
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            isActive = false;
        }

        private void miscSettingsButton_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.StartPosition = FormStartPosition.CenterParent;
            form4.ShowDialog(this);

            if (callForReset)
            {// wipe our settings and reinitialize
                File.Delete(configFile);
                settingsRef.Reset(settingsRef);
                validateSettings();
                Serialize(configFile);
                copySettingsFromConfig();
            }
        }

        private void getSystemButton_Click(object sender, EventArgs e)
        {
            buttonCaller = 2;

            if (Control.ModifierKeys == Keys.Control)
                buttonCaller = 16; // mark us as needing a full refresh

            validateSettings();
            disableRunButtons();

            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void loadSettingsButton_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {// let's ask the user the path they'd like
                OpenFileDialog x = new OpenFileDialog();
                x.Title = "Select an XML configuration file to load";
                x.Filter = "TDHelper config files (*.xml)|*.xml";
                x.InitialDirectory = Application.StartupPath;
                x.RestoreDirectory = true;
                if (x.ShowDialog() == DialogResult.OK)
                    configFile = x.FileName;
            }

            loadSettings(configFile);
        }

        private void saveSettingsButton_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {// let's ask the user the path they'd like to save to
                SaveFileDialog x = new SaveFileDialog();
                x.Title = "Select a new filename to save to";
                x.Filter = "TDHelper config files (*.xml)|*.xml";
                x.InitialDirectory = Application.StartupPath;
                x.RestoreDirectory = true;
                if (x.ShowDialog() == DialogResult.OK)
                    configFile = x.FileName; // we've got a valid path
            }

            writeSettings();
        }

        private void localNavBox_CheckedChanged(object sender, EventArgs e)
        {
            t_localNavEnabled = localNavCheckBox.Checked;

            if (localNavCheckBox.Checked)
            {
                // switching to Local override
                panel1.Visible = true;
                panel2.Visible = false;
                localFilterParentPanel.Visible = true;
                // force enable
                localFilterParentPanel.Enabled = true;
                destSystemComboBox.Enabled = true;
                panel1.Enabled = true;
                methodDropDown.Enabled = false;
                padSizeBox.Enabled = true;
                padSizeLabel.Enabled = true;

                // pull to the front
                localFilterParentPanel.BringToFront();

                // disable most of the run options
                foreach (Control ctrl in runOptionsPanel.Controls)
                    ctrl.Enabled = false;

                ladenLYLabel.Font = new Font(ladenLYLabel.Font, FontStyle.Italic);
                ladenLYLabel.Text = "  Near LY:";
                ladenLYLabel.Enabled = true;

                l0_ladenLY = ladenLYBox.Value; // save our last used ladenLY
                if (l1_ladenLY > 0)
                    ladenLYBox.Value = l1_ladenLY; // restore local ladenLY
                else
                    ladenLYBox.Value = 1.00m; // default to 1.00 LY in local

                toolTip1.SetToolTip(ladenLYLabel, "Distance to search for local system/station info.");
                ladenLYBox.Enabled = true;
            }
            else
            {// we're unchecked
                l1_ladenLY = ladenLYBox.Value; // save our last used local ladenLY
                if (l0_ladenLY > 0)
                    ladenLYBox.Value = l0_ladenLY; // restore last used ladenLY

                if (methodIndex >= 4)
                {// disable the padsize box when applicable
                    padSizeBox.Enabled = false;
                    padSizeLabel.Enabled = false;
                }

                buttonCaller = 18; // mark us as local override
                methodSelectState(); // reset state
            }
        }

        private void avoidBox_TextChanged(object sender, EventArgs e)
        {
            // account for startJumpsBox
            if (startJumpsBox.Value > 0)
                settingsRef.Avoid = avoidBox.Text;
        }

        private void methodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            methodIndex = methodDropDown.SelectedIndex;
            methodSelectState();
        }

        private void td_outputBox_TextChanged(object sender, EventArgs e)
        {
            if (buttonCaller == 5)
            {// catch the database update button, we want to see its output
                td_outputBox.SelectionStart = td_outputBox.Text.Length;
                td_outputBox.ScrollToCaret();
                td_outputBox.Refresh();
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            validateSettings();

            if (!backgroundWorker4.IsBusy)
            { // UpdateDB Button
                buttonCaller = 5;
                disableRunButtons(); // disable buttons during uncancellable operations

                backgroundWorker4.RunWorkerAsync();
            }
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            // mark as run button
            buttonCaller = 1;

            doRunEvent(); // externalized
        }

        private void clearSaved1MenuItem_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(savedTextBox1.Text))
            {
                savedTextBox1.Text = string.Empty;
                if (File.Exists(savedFile1))
                    File.Delete(savedFile1);
            }
        }

        private void clearSaved2MenuItem_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(savedTextBox2.Text))
            {
                savedTextBox2.Text = string.Empty;
                if (File.Exists(savedFile2))
                    File.Delete(savedFile2);
            }
        }

        private void clearSaved3MenuItem_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(savedTextBox3.Text))
            {
                savedTextBox3.Text = string.Empty;
                if (File.Exists(savedFile3))
                    File.Delete(savedFile3);
            }
        }

        private void stn_padSizeBox_KeyPress(object sender, KeyPressEventArgs e)
        {// filter for valid chars
            if (e.KeyChar == 's'
                || e.KeyChar == 'S'
                || e.KeyChar == 'm'
                || e.KeyChar == 'M'
                || e.KeyChar == 'l'
                || e.KeyChar == 'L'
                || e.KeyChar == '?')
            {
                stn_padSizeBox.Text = e.KeyChar.ToString().ToUpper();
                e.Handled = true;
            }
        }

        private void padSizeBox_KeyPress(object sender, KeyPressEventArgs e)
        {// filter for valid chars
            if (e.KeyChar == 'm'
                || e.KeyChar == 'M'
                || e.KeyChar == 'l'
                || e.KeyChar == 'L'
                || e.KeyChar == '?')
            {
                if (padSizeBox.TextLength < 3)
                {
                    if (!padSizeBox.Text.Contains(e.KeyChar.ToString().ToUpper()))
                        padSizeBox.Text += e.KeyChar.ToString().ToUpper();
                    else if (padSizeBox.Text.Contains(e.KeyChar.ToString().ToUpper()))
                        padSizeBox.Text = e.KeyChar.ToString().ToUpper();
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

        private void stationDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            stationIndex = stationDropDown.SelectedIndex;

            if (methodIndex == 10)
            {// if we're in EDSC mode
                stationEditorChangeState(); // call our state change
            }
            else if (methodIndex == 7)
            {// if we're in ShipVendor mode
                if (stationIndex == 2)
                {// disable when list'ing
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

        private void csvExportCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (csvExportCheckBox.Checked)
            {
                t_csvExportCheckBox = true;
                csvExportComboBox.Visible = true;
                csvExportComboBox.SelectedIndex = 0; // always reset the box
            }
            else
            {
                t_csvExportCheckBox = false;
                csvExportComboBox.Visible = false;
                csvExportComboBox.SelectedIndex = 0;
            }
        }

        private void bmktCheckBox_Click(object sender, EventArgs e)
        {
            // an exception for the market command
            if (methodIndex == 4 && directCheckBox.Checked)
            {// we cannot have both buy and sell enabled
                directCheckBox.Checked = false;
                bmktCheckBox.Checked = true;
            }
        }

        private void directCheckBox_Click(object sender, EventArgs e)
        {
            // an exception for the market command
            if (methodIndex == 4 && bmktCheckBox.Checked)
            {// we cannot have both buy and sell enabled
                bmktCheckBox.Checked = false;
                directCheckBox.Checked = true;
            }
        }

        private void csvExportComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (csvExportComboBox.SelectedIndex == 1)
                exportIndex = 1;
            else if (csvExportComboBox.SelectedIndex == 2)
                exportIndex = 2;
            else if (csvExportComboBox.SelectedIndex == 3)
                exportIndex = 3;
            else if (csvExportComboBox.SelectedIndex == 4)
                exportIndex = 4;
            else if (csvExportComboBox.SelectedIndex == 5)
                exportIndex = 5;
            else
                exportIndex = 0;
        }

        private void resetFilterButton_Click(object sender, EventArgs e)
        {
            rearmFilterCheckBox.Checked = false;
            refuelFilterCheckBox.Checked = false;
            repairFilterCheckBox.Checked = false;
            itemsFilterCheckBox.Checked = false;
            bmktFilterCheckBox.Checked = false;
            outfitFilterCheckBox.Checked = false;
            shipyardFilterCheckBox.Checked = false;
        }

        private void resetStationButton_Click(object sender, EventArgs e)
        {
            rearmCheckBox.CheckState = CheckState.Indeterminate;
            refuelCheckBox.CheckState = CheckState.Indeterminate;
            repairCheckBox.CheckState = CheckState.Indeterminate;
            outfitCheckBox.CheckState = CheckState.Indeterminate;
            shipyardCheckBox.CheckState = CheckState.Indeterminate;
            marketCheckBox.CheckState = CheckState.Indeterminate;
            blackMarketCheckBox.CheckState = CheckState.Indeterminate;
            csvExportCheckBox.Checked = false;

            lsFromStarBox.Value = 0;
            stn_padSizeBox.Text = "?";
        }

        private void swapButton_Click(object sender, EventArgs e)
        {// here we swap the contents of the boxes with some conditions
            if (!localFilterParentPanel.Visible && destSystemComboBox.Visible)
            {// don't swap if the destination box isn't visible (or covered)
                if (!String.IsNullOrEmpty(srcSystemComboBox.Text)
                    && !String.IsNullOrEmpty(destSystemComboBox.Text))
                {
                    string temp = removeExtraWhitespace(destSystemComboBox.Text);
                    destSystemComboBox.Text = removeExtraWhitespace(srcSystemComboBox.Text);
                    srcSystemComboBox.Text = temp;
                }
                else if (!String.IsNullOrEmpty(srcSystemComboBox.Text)
                    && String.IsNullOrEmpty(destSystemComboBox.Text))
                {// swap from source to destination
                    string temp = removeExtraWhitespace(srcSystemComboBox.Text);
                    destSystemComboBox.Text = temp;
                    srcSystemComboBox.Text = string.Empty;
                }
                else if (String.IsNullOrEmpty(srcSystemComboBox.Text)
                    && !String.IsNullOrEmpty(destSystemComboBox.Text))
                {// swap from destination to source
                    string temp = removeExtraWhitespace(destSystemComboBox.Text);
                    srcSystemComboBox.Text = temp;
                    destSystemComboBox.Text = string.Empty;
                }
            }
        }

        private void miniModeButton_Click(object sender, EventArgs e)
        {
            Form2 childForm = new Form2(this); // pass a reference from parentForm

            // populate the treeview from the last valid run output
            parseRunOutput(tv_outputBox, childForm.treeViewBox);
            childForm.Text = t_childTitle; // set our profit estimate

            // show our minimode
            this.Hide();
            childForm.ShowDialog(); // always modal, never instance
            // save some globals
            Serialize(configFile, settingsRef.LocationChild, "LocationChild");
            Serialize(configFile, settingsRef.SizeChild, "SizeChild");
            Serialize(configFile, settingsRef.MiniModeOnTop, "MiniModeOnTop");
            this.Show(); // restore when we return
        }

        private void loopCheckBox_Click(object sender, EventArgs e)
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

        private void selectMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.SelectAll();
        }

        private void cutMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.Cut();
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.Copy();
        }

        private void pasteMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;

            clickedControl.Focus();
            clickedControl.Paste();
        }

        private void savePage1MenuItem_Click(object sender, EventArgs e)
        {
            if (td_outputBox.SelectedText.Length > 0)
                writeSavedPage(td_outputBox.SelectedText, savedFile1);
            else
                writeSavedPage(td_outputBox.Text, savedFile1);
        }

        private void savePage2MenuItem_Click(object sender, EventArgs e)
        {
            if (td_outputBox.SelectedText.Length > 0)
                writeSavedPage(td_outputBox.SelectedText, savedFile2);
            else
                writeSavedPage(td_outputBox.Text, savedFile2);
        }

        private void savePage3MenuItem_Click(object sender, EventArgs e)
        {
            if (td_outputBox.SelectedText.Length > 0)
                writeSavedPage(td_outputBox.SelectedText, savedFile3);
            else
                writeSavedPage(td_outputBox.Text, savedFile3);
        }

        private void pushNotesMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;
            clickedControl.Focus();
            if (clickedControl.SelectedText.Length > 0)
                Clipboard.SetData(DataFormats.Text, clickedControl.SelectedText);
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

        private void notesClearMenuItem_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(notesTextBox.Text))
            {
                notesTextBox.Text = string.Empty;
                if (File.Exists(notesFile))
                    File.Delete(notesFile);
            }
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox clickedControl = (RichTextBox)this.contextMenuStrip1.SourceControl;

            if (clickedControl.Name == notesTextBox.Name)
                notesTextBox.SelectedText = string.Empty;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fromPane == 5) { /* Pilot's Log tab */ }
            else if (fromPane == 4)
            {// if we're coming from the notes pane we should save when we switch
                notesTextBox.SaveFile(notesFile, RichTextBoxStreamType.PlainText);
            }

            if (tabControl1.SelectedTab == tabControl1.TabPages["outputPage"] && !String.IsNullOrEmpty(td_outputBox.Text))
            {
                string filteredOutput = filterOutput(td_outputBox.Text);
                runOutputState = isValidRunOutput(filteredOutput);

                // check for parsable Run output
                if (runOutputState > -1)
                {
                    hasParsed = false; // reset the semaphore
                    tv_outputBox = filteredOutput; // copy our validated input
                    miniModeButton.Enabled = true;
                }
                else
                    miniModeButton.Enabled = false;

                td_outputBox.Focus(); // always focus our text box

                outputPage.Font = new Font(outputPage.Font, FontStyle.Regular); // reset the font
                fromPane = 0;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["logPage"])
            {
                fromPane = 5;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["notesPage"] && checkIfFileOpens(notesFile))
            {
                notesTextBox.LoadFile(notesFile, RichTextBoxStreamType.PlainText);

                notesTextBox.Focus();
                fromPane = 4;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["savedPage1"] && checkIfFileOpens(savedFile1))
            {
                savedTextBox1.Focus();
                if (File.Exists(savedFile1))
                    savedTextBox1.LoadFile(savedFile1, RichTextBoxStreamType.PlainText);

                string filteredOutput = filterOutput(savedTextBox1.Text);
                runOutputState = isValidRunOutput(filteredOutput);

                // check for parsable Run output
                if (runOutputState > -1)
                {
                    hasParsed = false; // reset the semaphore
                    tv_outputBox = filteredOutput; // copy our validated input
                    miniModeButton.Enabled = true;
                }
                else
                    miniModeButton.Enabled = false;

                savedTextBox1.Focus();
                fromPane = 1;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["savedPage2"] && checkIfFileOpens(savedFile2))
            {
                savedTextBox2.Focus();
                if (File.Exists(savedFile2))
                    savedTextBox2.LoadFile(savedFile2, RichTextBoxStreamType.PlainText);

                string filteredOutput = filterOutput(savedTextBox2.Text);
                runOutputState = isValidRunOutput(filteredOutput);

                // check for parsable Run output
                if (runOutputState > -1)
                {
                    hasParsed = false; // reset the semaphore
                    tv_outputBox = filteredOutput; // copy our validated input
                    miniModeButton.Enabled = true;
                }
                else
                    miniModeButton.Enabled = false;

                savedTextBox2.Focus();
                fromPane = 2;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["savedPage3"] && checkIfFileOpens(savedFile3))
            {
                savedTextBox3.Focus();
                if (File.Exists(savedFile3))
                    savedTextBox3.LoadFile(savedFile3, RichTextBoxStreamType.PlainText);

                string filteredOutput = filterOutput(savedTextBox3.Text);
                runOutputState = isValidRunOutput(filteredOutput);

                // check for parsable Run output
                if (runOutputState > -1)
                {
                    hasParsed = false; // reset the semaphore
                    tv_outputBox = filteredOutput; // copy our validated input
                    miniModeButton.Enabled = true;
                }
                else
                    miniModeButton.Enabled = false;

                savedTextBox3.Focus();
                fromPane = 3;
            }
        }

        private void procOutputDataHandler(object sender, DataReceivedEventArgs output)
        {
            string[] exceptions = new string[] { "NOTE:", "####" };
            string filteredOutput = string.Empty;

            if (output.Data != null)
            {
                // prevent a null reference
                if (output.Data.Contains("\a"))
                    filteredOutput = output.Data.Replace("\a", string.Empty) + "\n";
                else
                    filteredOutput = output.Data + "\n";

                if (buttonCaller != 5 && buttonCaller != 12 && !exceptions.Any(output.Data.Contains) && !t_csvExportCheckBox || methodIndex != 5 || methodIndex != 6)
                {// hide output if calculating
                    stackCircularBuffer(filteredOutput);
                }
                else if (t_csvExportCheckBox || methodIndex == 5 || methodIndex == 6 || buttonCaller == 5 || buttonCaller == 12)
                {// don't hide any output if updating DB or exporting
                    stackCircularBuffer(filteredOutput);
                }
            }
        }

        private void procErrorDataHandler(object sender, DataReceivedEventArgs output)
        {
            if (output.Data != null)
            {
                stackCircularBuffer(output.Data + "\n");
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
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

        private void altConfigBox_SelectionChangeCommitted(object sender, EventArgs e)
        {// let's switch configs based on their name in the index
            // something other than default (by name)
            int index = validConfigs[1].IndexOf(altConfigBox.Text);
            string indexPath = validConfigs[0][index];
            if (!String.IsNullOrEmpty(indexPath) && validateConfigFile(indexPath))
            {
                buttonCaller = 21; // mark us as coming from the config selector
                loadSettings(indexPath);
            }
        }

        private void altConfigBox_DropDown(object sender, EventArgs e)
        {
            // refresh our index
            validConfigs = parseValidConfigs();
            altConfigBox.DataSource = null;
            altConfigBox.DataSource = validConfigs[1];
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {// fix for text selection upon focusing numericupdown controls
            (sender as NumericUpDown).Select(0, (sender as NumericUpDown).Text.Length);
        }

        private void numericUpDown_MouseUp(object sender, MouseEventArgs e)
        {
            (sender as NumericUpDown).Select(0, (sender as NumericUpDown).Text.Length);
        }

        private void srcSystemComboBox_TextChanged(object sender, EventArgs e)
        {
            // wait for the user to type a few characters
            if (srcSystemComboBox.Text.Length > 3 && methodIndex != 10)
            {
                string filteredString = removeExtraWhitespace(srcSystemComboBox.Text);
                populateStationPanel(filteredString);

                if (destSystemComboBox.Text.Length > 3)
                    towardsCheckBox.Enabled = true; // requires "--fr"
                else
                    towardsCheckBox.Enabled = false;
            }
            else
            {
                towardsCheckBox.Enabled = false;
            }
        }

        private void srcSystemComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (methodIndex != 10)
            {
                // make sure we filter unwanted characters from the string
                string filteredString = removeExtraWhitespace(srcSystemComboBox.Text);

                if ((e.KeyCode == Keys.Enter & e.Modifiers == Keys.Control)
                    && stringInList(filteredString, outputSysStnNames))
                {// if ctrl+enter, is a known system/station, and not in our net log, mark it down
                    addMarkedStation(filteredString, currentMarkedStations);
                    buildOutput(true);
                    Serialize(configFile, settingsRef.MarkedStations, "MarkedStations");
                    e.Handled = true;
                }
                else if ((e.KeyCode == Keys.Enter & e.Modifiers == Keys.Shift)
                    && stringInList(filteredString, currentMarkedStations))
                {// if shift+enter, item is in our list, remove it
                    removeMarkedStation(filteredString, currentMarkedStations);
                    int index = indexInList(filteredString, output_unclean);
                    buildOutput(true);
                    Serialize(configFile, settingsRef.MarkedStations, "MarkedStations");
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape
                    && !String.IsNullOrEmpty(filteredString))
                {// wipe our box selectively if we hit escape
                    //first wipe until the delimiter
                    string[] tokens = filteredString.Split(new string[] { "/" }, StringSplitOptions.None);
                    if (tokens != null && tokens.Length == 2)
                    {// make sure we have a system/station
                        // delete the front of the string until the system
                        srcSystemComboBox.Text = tokens[0];
                    }
                    else if (!filteredString.Contains("/"))
                        srcSystemComboBox.Text = string.Empty; // wipe entirely if only a system is left

                    e.Handled = true;
                }
            }
        }

        private void destSystemComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            string filteredString = removeExtraWhitespace(destSystemComboBox.Text);

            if ((e.KeyCode == Keys.Enter & e.Modifiers == Keys.Control)
                && stringInList(filteredString, outputSysStnNames))
            {// if ctrl+enter, is a known system/station, and not in our net log, mark it down
                addMarkedStation(filteredString, currentMarkedStations);
                buildOutput(true);
                Serialize(configFile, settingsRef.MarkedStations, "MarkedStations");
                e.Handled = true;
            }
            else if ((e.KeyCode == Keys.Enter & e.Modifiers == Keys.Shift)
                && stringInList(filteredString, currentMarkedStations))
            {// if shift+enter, item is in our list, remove it
                removeMarkedStation(filteredString, currentMarkedStations);
                int index = indexInList(filteredString, output_unclean);
                buildOutput(true);
                Serialize(configFile, settingsRef.MarkedStations, "MarkedStations");
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape
                && !String.IsNullOrEmpty(destSystemComboBox.Text))
            {// wipe our box selectively if we hit escape
                //first wipe until the delimiter
                string[] tokens = destSystemComboBox.Text.Split(new string[] { "/" }, StringSplitOptions.None);
                if (tokens != null && tokens.Length == 2)
                {// make sure we have a system/station
                    // delete the front of the string until the system
                    destSystemComboBox.Text = tokens[0];
                }
                else if (!destSystemComboBox.Text.Contains("/"))
                    destSystemComboBox.Text = string.Empty;

                e.Handled = true;
            }
        }

        private void destSystemComboBox_TextChanged(object sender, EventArgs e)
        {
            // wait for the user to type a few characters
            if (destSystemComboBox.Text.Length > 3)
            {
                string filteredString = removeExtraWhitespace(destSystemComboBox.Text);
                validateDestForEndJumps();

                if (srcSystemComboBox.Text.Length > 3)
                    towardsCheckBox.Enabled = true;
                else
                    towardsCheckBox.Enabled = false;
            }
            else
            {
                towardsCheckBox.Enabled = false;
            }
        }

        private void shipsSoldBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                shipsSoldBox.SelectionLength = 0;
            }
        }

        private void testSystemsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settingsRef.TestSystems = testSystemsCheckBox.Checked;
        }

        private void towardsCheckBox_Click(object sender, EventArgs e)
        {
            if (settingsRef.Loop || loopCheckBox.Checked)
            {
                settingsRef.Loop = false;
                loopCheckBox.Checked = false;
            }
            else if (!String.IsNullOrEmpty(destSystemComboBox.Text))
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
                towardsCheckBox.Checked = false;
        }

        private void shortenCheckBox_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(destSystemComboBox.Text))
            {
                if (!shortenCheckBox.Checked)
                {
                    loopCheckBox.Checked = false;
                    towardsCheckBox.Checked = false;
                }
                else if (shortenCheckBox.Checked)
                {
                    if (loopCheckBox.Checked)
                        loopCheckBox.Checked = false;

                    shortenCheckBox.Checked = true;
                    towardsCheckBox.Checked = false;
                }
            }
            else
            {
                shortenCheckBox.Checked = false;
            }
        }

        private void trackerLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/MarkAusten/TDHelper/issues/new");
        }

        private void faqLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/MarkAusten/TDHelper/wiki/Home");
        }

        private void comboBox_DropDown(object sender, EventArgs e)
        {
            dropdownOpened = true;
        }

        private void comboBox_DropDownClosed(object sender, EventArgs e)
        {
            dropdownOpened = false;
        }

        private void altConfigBox_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control && !configFile.Contains("Default.xml"))
            {
                File.Delete(configFile);
                if (!String.IsNullOrEmpty(altConfigBox.Items[0].ToString()))
                {// refresh our dropdown
                    (sender as ComboBox).DroppedDown = false;
                    altConfigBox.SelectedIndex = 0;
                }
            }
        }

        private void pilotsLogDataGrid_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            // prevent OOR exception
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            DataGridView pDataGrid = sender as DataGridView;
            if (pDataGrid != null)
            {// save the datasource index, and the datagrid index of the row
                pilotsLogDataGrid.ClearSelection(); // prevent weirdness
                pRowIndex = int.Parse(localTable.Rows[e.RowIndex][0].ToString());
                dRowIndex = e.RowIndex;
                pilotsLogDataGrid.Rows[dRowIndex].Selected = true;
            }
        }

        private void insertAtGridRow_Click(object sender, EventArgs e)
        {
            if (pilotsLogDataGrid.Rows.Count > 0)
            {
                // add a row with the timestamp of the selected row
                // basically an insert-below-index when we use select(*)
                string timestamp = pilotsLogDataGrid.Rows[dRowIndex].Cells["Timestamp"].Value.ToString();
                addAtTimestampDBRow(tdhDBConn, generateRecentTimestamp(timestamp));
                invalidatedRowUpdate(true, -1);
            }
            else
            {// special case for an empty gridview
                addAtTimestampDBRow(tdhDBConn, currentTimestamp());
                invalidatedRowUpdate(true, -1);
            }
        }

        private void forceRefreshGridView_Click(object sender, EventArgs e)
        {
            invalidatedRowUpdate(true, -1); // force an invalidate and reload
        }

        private void copySystemToSrc_Click(object sender, EventArgs e)
        {
            if (pilotsLogDataGrid.Rows.Count > 0 && !String.IsNullOrEmpty(pilotsLogDataGrid.Rows[dRowIndex].Cells[2].Value.ToString()))
            {// grab the system from the system field, if it exists, copy to the src box
                string dbSys = pilotsLogDataGrid.Rows[dRowIndex].Cells[2].Value.ToString();
                srcSystemComboBox.Text = dbSys;
            }
        }

        private void copySystemToDest_Click(object sender, EventArgs e)
        {
            if (pilotsLogDataGrid.Rows.Count > 0 && !String.IsNullOrEmpty(pilotsLogDataGrid.Rows[dRowIndex].Cells[2].Value.ToString()))
            {// grab the system from the system field, if it exists, copy to the src box
                string dbSys = pilotsLogDataGrid.Rows[dRowIndex].Cells[2].Value.ToString();
                destSystemComboBox.Text = dbSys;
            }
        }

        private void btnCmdrProfile_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker7.IsBusy)
            {
                // Cmdr Profile button.
                buttonCaller = 22;
                disableRunButtons(); // disable buttons during uncancellable operations

                backgroundWorker7.RunWorkerAsync();
            }
        }

        private void copySystemToEDSC_Click(object sender, EventArgs e)
        {
            // forcefully switch our panel to the EDSC method
            methodDropDown.SelectedIndex = 10;

            if (pilotsLogDataGrid.Rows.Count > 0 && !String.IsNullOrEmpty(pilotsLogDataGrid.Rows[dRowIndex].Cells[2].Value.ToString()))
            {// grab the system from the system field, if it exists, copy to the src box
                string dbSys = pilotsLogDataGrid.Rows[dRowIndex].Cells[2].Value.ToString();
                srcSystemComboBox.Text = dbSys;
            }
        }

        private void pilotsLogDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < retriever.RowCount && e.ColumnIndex < retriever.RowCount)
                e.Value = memoryCache.RetrieveElement(e.RowIndex, e.ColumnIndex);
        }

        private void pilotsLogDataGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < retriever.RowCount && e.ColumnIndex < retriever.RowCount)
            {
                // update our local table
                localTable.Rows[e.RowIndex][e.ColumnIndex] = e.Value;
                List<DataRow> row = new List<DataRow> { localTable.Rows[e.RowIndex] };

                // update the physical DB and repaint
                updateDBRow(tdhDBConn, row);
                invalidatedRowUpdate(false, e.RowIndex);
            }
        }

        private void removeAtGridRow_Click(object sender, EventArgs e)
        {
            if (pilotsLogDataGrid.Rows.Count > 0)
            {
                removeDBRow(tdhDBConn, pRowIndex);
                updateLocalTable(tdhDBConn);
                memoryCache.RemoveRow(dRowIndex, pRowIndex);
                pilotsLogDataGrid.InvalidateRow(dRowIndex);
            }
        }

        private void forceResortMenuItem_Click(object sender, EventArgs e)
        {
            pilotsLogDataGrid.Columns["Timestamp"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            pilotsLogDataGrid.Columns["System"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            pilotsLogDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void pilotsLogDataGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
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
                        removeDBRow(tdhDBConn, rowIndex);
                        updateLocalTable(tdhDBConn);
                        memoryCache.RemoveRow(e.Row.Index, rowIndex);
                    }
                    else if (pilotsLogDataGrid.SelectedRows.Count > 1 && dgRowIDIndexer.Count == 0)
                    {// let's batch the commits for performance
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
                        batchedRowCount--; // keep track of our re-entry

                    if (dgRowIDIndexer.Count > 0 && batchedRowCount == 0)
                    {// we've got queued commits to remove (should trigger on the last removed row)
                        removeDBRows(tdhDBConn, dgRowIDIndexer);
                        updateLocalTable(tdhDBConn);
                        memoryCache.RemoveRows(dgRowIndexer, dgRowIDIndexer);
                        pilotsLogDataGrid.Visible = true; // re-enable retrieval
                    }
                }
            }
        }
        #endregion
    }
}
