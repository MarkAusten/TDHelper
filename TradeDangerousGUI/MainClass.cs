using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Security;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace TDHelper
{
    public partial class MainForm : Form
    {
        public const int API_TIMEOUT = 60;
        public const string ITEM_CSV_FILE = @"data\Item.csv";
        public const string PAD_SIZE_FILTER = "SML?";
        public const string PLANETARY_FILTER = "YN?";
        public const string SHIP_CSV_FILE = @"data\Ship.csv";

        #region Props

        public static string assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public static bool callForReset = false;
        public static string configFile = Path.Combine(assemblyPath, "tdh.ini");
        public static string configFileDefault = Path.Combine(assemblyPath, "tdh.ini");
        public static string DBUpdateCommandString = string.Empty;
        public static bool hasParsed = false;
        public static List<string> latestLogPaths;
        public static string localManifestPath = Path.Combine(assemblyPath, "TDHelper.manifest.tmp");
        public static string recentLogPath;
        public static string remoteArchiveLocalPath;
        public static TDSettings settingsRef = TDSettings.Instance;
        public static string t_AppConfigPath;
        public static double t_CrTonTally;
        public static string t_itemListPath;
        public static double t_meanDist;
        public static string t_shipListPath;
        public static string updateLogPath = Path.Combine(assemblyPath, "update.log");
        public string commandString;
        public List<string> CommodityAndShipList = new List<string>();
        public List<string> currentMarkedStations;
        public bool dropdownOpened;
        public int fromPane = -1;
        public bool hasRun;
        public bool rebuildCache;
        public string remoteManifestPath = ConfigurationManager.AppSettings["remoteManifestPath"];
        public int runOutputState = -1;
        public Stopwatch stopwatch = new Stopwatch();
        public string t_childTitle;
        public string t_lastSysCheck;
        public string t_lastSystem;
        public Process td_proc = new Process();
        private const int circularBufferSize = 32768;

        /// <summary>
        /// An application sends the WM_SETREDRAW message to a window to allow changes in that
        /// window to be redrawn or to prevent changes in that window from being redrawn.
        /// </summary>
        private const int WM_SETREDRAW = 11;

        private static string tdhPath = string.Empty;
        private static string tdPath = string.Empty;
        private StringBuilder circularBuffer = new StringBuilder(circularBufferSize);
        private List<int> dgRowIDIndexer = new List<int>();
        private List<int> dgRowIndexer = new List<int>();
        private int dRowIndex = 0;
        private bool hasLogLoaded;
        private bool hasRefreshedRecents;
        private int insertPosition = 0;
        private int listLimit = 50;
        private bool loadedFromDB;
        private List<KeyValuePair<string, string>> localSystemList = new List<KeyValuePair<string, string>>();
        private Dictionary<string, string> netLogOutput = new Dictionary<string, string>();
        private List<string> output_unclean = new List<string>();
        private DataTable pilotsSystemLogTable = new DataTable("SystemLog");
        private int pRowIndex = 0;
        private Object readNetLock = new Object();
        private DataTable retrieverCacheTable = new DataTable();

        #endregion Props

        /// <summary>
        /// Suspends painting for the target control. Do NOT forget to call EndControlUpdate!!!
        /// </summary>
        /// <param name="control">visual control</param>
        public static void BeginControlUpdate(Control control)
        {
            Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                  IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        /// <summary>
        /// Check the default paths for the AppConfig.xml file
        /// </summary>
        public static void CheckDefaultNetLogPaths()
        {
            if (string.IsNullOrEmpty(settingsRef.NetLogPath))
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                path = Path.Combine(path, @"Frontier\Products\elite-dangerous-64");

                string file = CheckNetLogPath(path);

                if (string.IsNullOrEmpty(file))
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    path = Path.Combine(path, @"Frontier_Developments\Products\elite-dangerous-64");

                    file = CheckNetLogPath(path);
                }

                if (string.IsNullOrEmpty(file))
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                    path = Path.Combine(path, @"Steam\steamapps\common\Elite Dangerous\Products\elite-dangerous-64");

                    file = CheckNetLogPath(path);
                }

                if (string.IsNullOrEmpty(file))
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                    path = Path.Combine(path, @"Oculus\Software\frontier-developments-plc-elite-dangerous\Products\elite-dangerous-64");

                    file = CheckNetLogPath(path);
                }

#if DEBUG
                if (string.IsNullOrEmpty(file))
                {
                    path = @"C:\Development";

                    file = CheckNetLogPath(path);
                }
#endif
                if (!string.IsNullOrEmpty(file))
                {
                    settingsRef.NetLogPath = file.Replace("AppConfig.xml", "Logs");
                }
            }
        }

        public static bool CheckIfFileOpens(string path)
        {
            bool fileOpens = false;

            try
            {
                if (File.Exists(path))
                {
                    // throw if file can't be opened, hopefully
                    FileStream p = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    p.Close();
                    p.Dispose();

                    fileOpens = true;
                }
            }
            catch
            {
                // Do nothing...
            }

            return fileOpens;
        }

        /// <summary>
        /// Check to se if the AppConfig.xml file exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>The full path to the file if found otherwise a blank string.</returns>
        public static string CheckNetLogPath(string path)
        {
            string file = Path.Combine(path, "AppConfig.xml");

            if (!File.Exists(file))
            {
                file = string.Empty;
            }

            return file;
        }

        public static string Decrypt(string input)
        {
            // return a normalized string version of our decoded input
            var inputBytes = Convert.FromBase64String(input);

            return Encoding.UTF8.GetString(MachineKey.Unprotect(inputBytes));
        }

        public static string Encrypt(string input)
        {
            // return a base64 string version of our encoded input
            var inputBytes = Encoding.UTF8.GetBytes(input);

            return Convert.ToBase64String(MachineKey.Protect(inputBytes));
        }

        /// <summary>
        /// Resumes painting for the target control. Intended to be called following a call to BeginControlUpdate()
        /// </summary>
        /// <param name="control">visual control</param>
        public static void EndControlUpdate(Control control)
        {
            // Create a C "true" boolean as an IntPtr
            IntPtr wparam = new IntPtr(1);
            Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                  IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);
            control.Invalidate();
            control.Refresh();
        }

        /// <summary>
        /// Try to locate the path to the python interpreter from the environment.
        /// </summary>
        /// <param name="target">The target environment.</param>
        /// <returns>The full path to the interpreter or blank if not found.</returns>
        public static string ExtractPythonPathFromEnvironment(EnvironmentVariableTarget target)
        {
            string[] userPath = Environment.GetEnvironmentVariable("Path", target).Split(';');

            // Scan through path variables to find the python path.
            string pythonPath = string.Empty;
            string pythonExe = string.Empty;

            if (!int.TryParse(ConfigurationManager.AppSettings["minimumPythonVersion"], out int minVersion))
            {
                minVersion = 34;
            }

            foreach (string path in userPath)
            {
                if (path.ToLower().Contains("python"))
                {
                    // Find that last array element that starts with python
                    string[] folders = path.ToLower().Split('\\');
                    int offset = -1;

                    for (int i = folders.Length - 1; i >= 0; --i)
                    {
                        if (folders[i].StartsWith("python"))
                        {
                            offset = i;
                            break;
                        }
                    }

                    if (offset > -1)
                    {
                        // Check the version number
                        bool versionOkay = false;

                        if (int.TryParse(folders[offset].Substring(6), out int version))
                        {
                            versionOkay = version >= minVersion;
                        }

                        if (versionOkay)
                        {
                            pythonExe = Path.Combine(string.Join(@"\", folders.Take(1 + offset)), "python.exe");

                            if (!File.Exists(pythonExe))
                            {
                                pythonExe = string.Empty;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return pythonExe;
        }

        public static void PlayAlert()
        {
            PlaySoundFile("notify.wav");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>
        public static void PlaySoundFile(string fileName)
        {
            if (!settingsRef.Quiet)
            {
                // a simple method for playing a custom beep.wav or the default system Beep
                SoundPlayer player = new SoundPlayer();
                Assembly thisExecutable = System.Reflection.Assembly.GetExecutingAssembly();
                string localSound = Path.Combine(assemblyPath, fileName);

                if (CheckIfFileOpens(localSound))
                {
                    player.SoundLocation = localSound;
                    player.LoadAsync();
                    player.Play();
                }
            }
        }

        public static void PlayUnknown()
        {
            PlaySoundFile("unknown.wav");
        }

        /// <summary>
        /// Displays the status message if the splash screen is visible.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public static void SetSplashScreenStatus(string message)
        {
            if (SplashScreen.IsVisible)
            {
                SplashScreen.SetStatus(message);
            }
        }

        public static DialogResult ValidateNetLogPath(
            string altPath,
            bool force = false)
        {
            DialogResult result = DialogResult.None;

            // override to avoid net log logic
            if (!settingsRef.DisableNetLogs)
            {
                CheckDefaultNetLogPaths();

                string appConfigPath = string.Empty;

                if (!string.IsNullOrEmpty(settingsRef.NetLogPath))
                {
                    appConfigPath = Path.Combine(
                        Directory.GetParent(settingsRef.NetLogPath).ToString(),
                        "AppConfig.xml");
                }

                if (force ||
                    string.IsNullOrEmpty(settingsRef.NetLogPath) ||
                    string.IsNullOrEmpty(appConfigPath) ||
                    !CheckIfFileOpens(appConfigPath))
                {
                    // let's just ask the user where to look
                    OpenFileDialog x = new OpenFileDialog()
                    {
                        Title = "TD Helper - Select a valid Elite: Dangerous AppConfig.xml",
                        Filter = "AppConfig.xml|*.xml"
                    };

                    result = x.ShowDialog();

                    if (result != DialogResult.Cancel)
                    {
                        // set the appropriate Logs folder
                        t_AppConfigPath = x.FileName;
                        settingsRef.NetLogPath = Path.Combine(Directory.GetParent(t_AppConfigPath).ToString(), "Logs");

                        SaveSettingsToIniFile();

                        SetSplashScreenStatus("Validating verbose logging");

                        // always validate when verboselogging is enabled
                        verboseLoggingChecked = false;
                        ValidateVerboseLogging();
                    }
                    else
                    {
                        DialogResult dialog2 = TopMostMessageBox.Show(
                            true,
                            true,
                            "Scanning for recent systems has been disabled.",
                            "TD Helper - Error",
                            MessageBoxButtons.OK);

                        settingsRef.DisableNetLogs = true;

                        SaveSettingsToIniFile();
                    }
                }
            }

            return result;
        }

        public static void ValidatePython(string altPath)
        {
            /*
             * This method attempts to find python.exe by using 'where', and
             * if that should fail we then ask the user
             */

            if (string.IsNullOrEmpty(settingsRef.PythonPath))
            {
                string pythonExe = ExtractPythonPathFromEnvironment(EnvironmentVariableTarget.User);

                if (string.IsNullOrEmpty(pythonExe))
                {
                    pythonExe = ExtractPythonPathFromEnvironment(EnvironmentVariableTarget.Process);
                }

                if (string.IsNullOrEmpty(pythonExe))
                {
                    pythonExe = ExtractPythonPathFromEnvironment(EnvironmentVariableTarget.Machine);
                }

                if (!string.IsNullOrEmpty(pythonExe))
                {
                    settingsRef.PythonPath = pythonExe;
                }
            }

            // before we do anything else, check if the current path works
            if (string.IsNullOrEmpty(settingsRef.PythonPath) || !CheckIfFileOpens(settingsRef.PythonPath))
            {
                OpenFileDialog x = new OpenFileDialog()
                {
                    Title = "TD Helper - Select your python.exe",
                    Filter = "Python Interpreter (*.exe)|*.exe"
                };

                if (x.ShowDialog() == DialogResult.OK)
                {
                    altPath = x.FileName;
                }

                if (CheckIfFileOpens(altPath))
                {
                    settingsRef.PythonPath = altPath;
                    SaveSettingsToIniFile();
                }
                else
                {
                    MessageBox.Show(
                        "Fatal Error - python.exe must be located.",
                        "Fatal Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);

                    Application.Exit();
                }
            }
        }

        public static void ValidateTDPath()
        {
            string output = RunProcess("pip", "show tradedangerous");

            if (string.IsNullOrEmpty(output))
            {
                DialogResult result = MessageBox.Show(
                    "Tradedangerous not installed via pip. Install now?",
                    "Tradedangerous",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    output = RunProcess("pip", "install tradedangerous");
                }
                else
                {
                    MessageBox.Show(
                        "Fatal Error - tradedangerous must be installed.",
                        "Fatal Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);

                    Application.Exit();
                }
            }
            else
            {
                // Get the installed version.
                string[] lines = output.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.None);

                string installedVersion = lines[1].Substring(1 + lines[1].IndexOf(":")).Trim();

                using (WebClient client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    output = client.DownloadString("https://pypi.python.org/pypi/tradedangerous/json");

                    JObject jo = JObject.Parse(output);

                    string currentVersion = ((JObject)jo["releases"])
                        .Properties()
                        .Select(p => p.Name)
                        .OrderByDescending(x => x)
                        .First();

                    if (installedVersion != currentVersion)
                    {
                        DialogResult result = MessageBox.Show(
                            "New tradedangerous version available. Upgrade now?",
                            "Tradedangerous",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            output = RunProcess("pip", "install --upgrade tradedangerous");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve the setting from the config file as a decimal value.
        /// </summary>
        /// <param name="config">The configuration object,</param>
        /// <param name="section">The required section.</param>
        /// <param name="key">The required key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The decimal value of the setting or the default value.</returns>
        public decimal GetConfigSetting(
            SharpConfig.Configuration config,
            string section,
            string key,
            decimal defaultValue = 0)
        {
            // Set up the result anad get the raw value.
            decimal result = defaultValue;
            string rawValue = config[section][key].RawValue;

            // Check to see if the raw value is null or empty.
            if (!string.IsNullOrEmpty(rawValue))
            {
                // We have a non-null, non-empty value so try to get the decimal value and
                // set the result to the default value if there is an exception.
                try
                {
                    result = config[section][key].DecimalValue;
                }
                catch
                {
                    result = defaultValue;
                }
            }

            return result;
        }

        /// <summary>
        /// Populate the list of available ships and set the current selection
        /// and set the appropriate values.
        /// </summary>
        /// <param name="refreshList">Set to true to refresh the drop down list.</param>
        public void SetShipList(bool refreshList = false)
        {
            if (refreshList)
            {
                validConfigs = SetAvailableShips();

                cboCommandersShips.Items.Clear();
                cboCommandersShips.ClearSeparators();

                foreach (ComboBoxItem item in validConfigs)
                {
                    cboCommandersShips.Add(item);
                }

                int index = ((ShipSection)ConfigurationManager.GetSection("ships"))
                    .ShipSettings
                    .Cast<ShipConfig>()
                    .Count();

                if (index < cboCommandersShips.Items.Count)
                {
                    cboCommandersShips.SetSeparator(cboCommandersShips.Items.Count - index);
                }
            }

            TDSettings settings = settingsRef;
            SharpConfig.Configuration config = GetConfigurationObject();
            string shipType = string.Empty;

            bool hasSection = config.FirstOrDefault(x => x.Name == settings.LastUsedConfig) != null;

            if (hasSection)
            {
                settings.Capacity = GetConfigSetting(config, settings.LastUsedConfig, "Capacity");
                settings.Insurance = GetConfigSetting(config, settings.LastUsedConfig, "Insurance");
                settings.LadenLY = GetConfigSetting(config, settings.LastUsedConfig, "LadenLY");
                settings.Padsizes = ContainsPadSizes(config[settings.LastUsedConfig]["Padsizes"].StringValue);
                settings.UnladenLY = GetConfigSetting(config, settings.LastUsedConfig, "UnladenLY");
                shipType = config[settings.LastUsedConfig]["ShipType"].StringValue;
            }
            else
            {
                settings.Capacity = 1;
                settings.Insurance = 0;
                settings.LadenLY = 1;
                settings.Padsizes = string.Empty;
                settings.UnladenLY = 1;
            }

            cboCommandersShips.SelectedIndex
               = !string.IsNullOrEmpty(settings.LastUsedConfig)
               ? cboCommandersShips.FindStringExact(GetShipNameFromConfigSection(settings.LastUsedConfig))
               : 0;

            if (!string.IsNullOrEmpty(shipType))
            {
                ShipConfig ship = ((ShipSection)ConfigurationManager.GetSection("ships"))
                .ShipSettings.Cast<ShipConfig>()
                .FirstOrDefault(x => x.ShipType == shipType);

                if (ship != null &&
                    decimal.TryParse(ship.MaxCapacity, out decimal maxCapacity))
                {
                    numRouteOptionsShipCapacity.Maximum = maxCapacity;
                }
            }

            numRouteOptionsShipCapacity.Value = settings.Capacity;
            numShipInsurance.Value = Math.Max(settings.Insurance, numShipInsurance.Minimum);
            numLadenLy.Value = Math.Max(settings.LadenLY, numLadenLy.Minimum);
            txtPadSize.Text = settings.Padsizes;
            numUnladenLy.Value = Math.Max(settings.UnladenLY, numUnladenLy.Minimum);
            numRouteOptionsShipCapacity.Maximum = Decimal.MaxValue;
        }

        /// <summary>
        /// Validate the various path settings.
        /// </summary>
        public void ValidatePaths()
        {
            // check our paths.
            ValidatePython(null);
            ValidateNetLogPath(null);
        }

        public void ValidateSettings()
        {
            SetSplashScreenStatus("Validating the path settings...");

            ValidatePaths();

            SetSplashScreenStatus("Validating net logs...");

            ValidateNetLogPath(null);

            // sanity check our inputs

            SetSplashScreenStatus("Validating settings...");

            ValidateSetting("Age", numRouteOptionsAge);
            ValidateSetting("Capacity", numRouteOptionsShipCapacity);
            ValidateSetting("Credits", numCommandersCredits);
            ValidateSetting("EndJumps", numRunOptionsEndJumps);
            ValidateSetting("GPT", numRouteOptionsGpt);
            ValidateSetting("Hops", numRouteOptionsHops);
            ValidateSetting("Insurance", numShipInsurance);
            ValidateSetting("Jumps", numRouteOptionsJumps);
            ValidateSetting("LadenLY", numLadenLy);
            ValidateSetting("Limit", numRouteOptionsLimit);
            ValidateSetting("LSPenalty", numRouteOptionsLsPenalty);
            ValidateSetting("Margin", numRouteOptionsMargin);
            ValidateSetting("MaxGPT", numRouteOptionsMaxGpt);
            ValidateSetting("MaxLSDistance", numRouteOptionsMaxLSDistance);
            ValidateSetting("PruneHops", numRouteOptionsPruneHops);
            ValidateSetting("PruneScore", numRouteOptionsPruneScore);
            ValidateSetting("StartJumps", numRunOptionsStartJumps);
            ValidateSetting("Stock", numRouteOptionsStock);
            ValidateSetting("UnladenLY", numUnladenLy);

            settingsRef.Padsizes = ContainsPadSizes(settingsRef.Padsizes);
            settingsRef.Planetary = ContainsPlanetary(settingsRef.Planetary);

            // an exception is made for checkboxes, we shouldn't ever get here
            if (settingsRef.Towards && settingsRef.Loop)
            {
                settingsRef.Loop = false;
            }
            else if (settingsRef.Towards &&
                string.IsNullOrEmpty(RemoveExtraWhitespace(cboRunOptionsDestination.Text)))
            {
                settingsRef.Towards = false;
            }

            // default to Run command if unset
            if (methodIndex > -1 && methodIndex < cboMethod.Items.Count)
            {
                cboMethod.SelectedIndex = methodIndex;
            }

            // make sure we pull CSV paths after we validate our inputs
            t_itemListPath = Path.Combine(assemblyPath, ITEM_CSV_FILE);
            t_shipListPath = Path.Combine(assemblyPath, SHIP_CSV_FILE);

            // Set the default rebuy percentage to 5%.
            if (settingsRef.RebuyPercentage == 0)
            {
                settingsRef.RebuyPercentage = 5.00M;
            }
        }

        private static string FilterOutput(string input)
        {
            // make sure we remove junk from the run output
            string[] exceptions = new string[] { "Command line:", "NOTE:", "SORRY:", "####", "Entering" };

            string[] filteredLines = input
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Where(x => !exceptions.Any(y => x.Contains(y)))
                .ToArray();

            string result = string.Join(Environment.NewLine, filteredLines);

            return result;
        }

        private static string RemoveExtraWhitespace(string input)
        {
            // should work with most patterns, and favorite systems/stations
            string pattern = @"^\s*!|^\s*(?=\D)|(?!\S)[ ]+(?=\/)|(?<=\/)[ ]+(?=\S)|(?<=\S)[ ]+(?!\S)";
            string sanitized = Regex.Replace(input, pattern, string.Empty, RegexOptions.Compiled);

            return sanitized;
        }

        private static string RunProcess(string filename, string args)
        {
            // Start the child process.
            Process p = new Process();

            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = filename;
            p.StartInfo.Arguments = args;
            p.StartInfo.CreateNoWindow = true;

            p.Start();

            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.

            string output = p.StandardOutput.ReadToEnd();

            p.WaitForExit();

            return output;
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(
                    IntPtr hWnd,
                    uint wMsg,
                    UIntPtr wParam,
                    IntPtr lParam);

        /// <summary>
        /// Determine if the specified array contains duplicate entries.
        /// </summary>
        /// <param name="inputArray">The array to be checked.</param>
        /// <returns>True if duplicates are found.</returns>
        private bool ArrayContainsDuplicate(string[] inputArray)
        {
            // Groups the array and count the number of groups that have more than one entry.
            return inputArray.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Count() > 0;
        }

        private void BuildSettings()
        {
            // force InvariantCulture to prevent issues
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // snag the newest data from the file if it exists
            if (CheckIfFileOpens(configFile))
            {
                LoadSettingsFromIniFile();
                currentMarkedStations = ParseMarkedStations();
            }

            // reset culture
            Thread.CurrentThread.CurrentCulture = userCulture;
        }

        private string CleanShipVendorInput(string input)
        {
            // just a simple method to clean invalid data from the shipvendor input
            return Regex.Replace(input, @"\s\[.*\]|(?<=\w)\,\s*(?!\w|\s+\w)|(?<=\s)\s+", string.Empty);
        }

        private void ClearCircularBuffer()
        {
            insertPosition = 0;
            circularBuffer.Clear();

            this.rtbOutput.Invoke(new Action(() =>
            {
                this.rtbOutput.Text = circularBuffer.ToString();
            }));
        }

        private string CurrentTimestamp()
        {
            return DateTime.Now.ToString("yy-MM-dd HH:mm:ss");
        }

        private string GenerateRecentTimestamp(string inputStamp)
        {
            // we take an input timestamp string, and try to generate a new timestamp from it by removing a second
            DateTime parsedStamp = new DateTime(), outputStamp = new DateTime();
            string format = "yy-MM-dd HH:mm:ss";

            if (DateTime.TryParseExact(inputStamp, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStamp))
            {
                outputStamp = parsedStamp.AddSeconds(-1);
            }
            else
            {
                outputStamp = DateTime.Now; // fail, but don't explode
            }

            return outputStamp.ToString(format);
        }

        /// <summary>
        /// Get the ship name from the specified config section.
        /// </summary>
        /// <param name="sectionName">The name of the section contining the ship data.</param>
        /// <returns>The ship name.</returns>
        private string GetShipNameFromConfigSection(string sectionName)
        {
            SharpConfig.Configuration config = GetConfigurationObject();

            SharpConfig.Section configSection = config[sectionName];

            return GetStringSetting(configSection, "shipName");
        }

        private int IndexInList(string input, List<string> listToSearch)
        {
            // return only an index of the first partial match (insensitive)
            int index = listToSearch.IndexOf(input);

            return index >= 0 ? index : -1;
        }

        private int IndexInListExact(string input, List<string> listToSearch)
        {
            // return an index of the first exact match
            for (int i = listToSearch.Count - 1; i >= 0; i--)
            {
                // we should hit a match faster in reverse for our particular dataset
                if (listToSearch[i].Equals(input, StringComparison.InvariantCulture))
                {
                    // only return the first match found from the bottom of the list
                    return i;
                }
            }

            return -1;
        }

        private bool ListEqualsExact(List<string> list1, List<string> list2)
        {
            // quick loop to check equality of two string lists
            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ListInListDesc(List<string> list1, List<string> list2)
        {
            // check if list1 exists in list2, descending order
            for (int i = 0; i < list1.Count; i++)
            {
                // compare exact indexes
                if (!list1[i].Equals(list2[i]))
                {
                    return false; // break on the first negative
                }
            }

            return true;
        }

        private void LoadSettings()
        {
            // make sure to load our data as invariant
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // don't populate if switching configs
            if (buttonCaller != 21)
            {
                SplashScreen.SetStatus("Set available systems lists...");

                BuildOutput(true);

                SplashScreen.SetStatus("Set pilot's log...");

                BuildPilotsLog();
            }

            // populate the notes page
            if (File.Exists(notesFile))
            {
                txtNotes.LoadFile(notesFile, RichTextBoxStreamType.PlainText);
            }

            // reset our selected command for safety
            cboMethod.SelectedIndex = 0;

            // reset to our previous culture type
            Thread.CurrentThread.CurrentCulture = userCulture;
        }

        private void PopulateStationPanel(string input)
        {
            // we need to split our system and station names to match with the DB
            if (!string.IsNullOrEmpty(input))
            {
                string[] tokens = input.Split(new string[] { "/" }, StringSplitOptions.None);

                if (tokens != null && tokens.Length == 2)
                {
                    // has both system and station
                    string t_system = tokens[0];
                    string t_station = tokens[1];

                    if (!string.IsNullOrEmpty(t_system) && !string.IsNullOrEmpty(t_station))
                    {
                        GrabStationData(t_system, t_station);
                        RefreshCurrentOptionsPanel();
                    }
                }
            }
        }

        private void ReadCircularBuffer()
        {
            // here we consume our buffer
            Invoke(new Action(() =>
             {
                 // Stop the control updating.
                 BeginControlUpdate(rtbOutput);

                 // COnsume the buffer.
                 if (circularBuffer.Length > 0 && circularBuffer.Length <= circularBuffer.Capacity)
                 {
                     // if our buffer is full, display it
                     rtbOutput.Text = circularBuffer.ToString();
                     rtbOutput.SelectionStart = rtbOutput.TextLength;
                     rtbOutput.ScrollToCaret();
                 }
                 else
                 {
                     // if the buffer overflows, wipe and return empty
                     ClearCircularBuffer(); // circularBuffer = new StringBuilder(circularBufferSize);
                 }

                 // Make the contents of the control scroll down one line.
                 SendMessage(rtbOutput.Handle, (uint)0x00B6, (UIntPtr)0, (IntPtr)(1));

                 // Enable the control updating.
                 EndControlUpdate(rtbOutput);
             }));
        }

        /// <summary>
        /// Set the title of the main form.
        /// </summary>
        /// <param name="mainform"></param>
        private void SetFormTitle(Form mainform)
        {
            // Let's change the title to the current version
            mainform.Text = "TDHelper " + appVersion;

            // Plus the commander name if set.
            if (!string.IsNullOrEmpty(MainForm.settingsRef.CmdrName))
            {
                mainform.Text += " : Commander " + MainForm.settingsRef.CmdrName;
            }
        }

        private void StackCircularBuffer(string input)
        {
            // here we generate our buffer to be consumed
            if (input.Length > circularBuffer.Capacity)
            {
                // if the input is bigger than our buffer size, toss what doesn't fit
                circularBuffer = new StringBuilder(circularBufferSize);
                circularBuffer.Append(input, input.Length - circularBuffer.Capacity, circularBuffer.Capacity);

                insertPosition = circularBuffer.Length;
            }
            else if (input.Length + circularBuffer.Length > circularBuffer.Capacity)
            {
                // in case we can append, but that would put us OOB
                circularBuffer.Remove(0, circularBuffer.Length + input.Length - circularBufferSize);
                circularBuffer.Append(input);

                insertPosition = circularBuffer.Length;
            }
            else
            {
                circularBuffer.Length = insertPosition;
                char lastChar = input[input.Length - 1];

                if (lastChar == '\n')
                {
                    circularBuffer.Append(input);

                    insertPosition = circularBuffer.Length;
                }
                else if (lastChar == '\r')
                {
                    circularBuffer.Append(input.TrimEnd(new char[] { '\n', '\r' }));

                    while (circularBuffer.Length > 1 && insertPosition > 0 && circularBuffer[insertPosition - 1] != '\n')
                    {
                        --insertPosition;
                    }
                }
                else
                {
                    circularBuffer.Append(input);

                    insertPosition = circularBuffer.Length;
                }
            }

            ReadCircularBuffer();
        }

        private bool StringInArray(string input, string[] stringsToSearch)
        {
            // true if partial string (insensitive) exists in a string array
            foreach (string s in stringsToSearch)
            {
                if (s.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool StringInList(string input, List<string> listToSearch)
        {
            // check if a partial string exists inside a list of strings, stop at first match
            for (int i = 0; i < listToSearch.Count; i++)
            {
                if (listToSearch[i].IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool StringInListExact(string input, List<string> listToSearch)
        {
            for (int i = 0; i < listToSearch.Count; i++)
            {
                // go in reverse to hit a match faster with our particular dataset
                if (listToSearch[i].Equals(input, StringComparison.InvariantCulture))
                {
                    return true; // return on the first match
                }
            }

            return false;
        }

        private bool TimestampIsNewer(string inputStamp1, string inputStamp2)
        {
            // true if timeStamp1 newer than timeStamp2, false if equal or older
            if (DateTime.TryParseExact(inputStamp1, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStamp1) && DateTime.TryParseExact(inputStamp2, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStamp2))
            {
                return parsedStamp1.CompareTo(parsedStamp2) > 0;
            }
            else
            {
                throw new ArgumentException("Unable to parse column timestamps for comparison");
            }
        }

        /// <summary>
        /// Validate the control value. Write the updated value back to the settings object.
        /// </summary>
        /// <param name="propertyName">The name of the setting property.</param>
        /// <param name="control">The associated control.</param>
        /// <param name="settings">The settings object.</param>
        private void ValidateControlValue(
            string propertyName,
            NumericUpDown control,
            TDSettings settings)
        {
            decimal value = control.Value;
            decimal newValue = value;

            // Compare with the control limits and set a new value if outside those limits.
            if (value < control.Minimum)
            {
                newValue = control.Minimum;
            }
            else if (value > control.Maximum)
            {
                newValue = control.Maximum;
            }

            // Change the value if required.
            if (newValue != value)
            {
                control.Value = newValue;

                PropertyInfo prop = settings.GetType().GetProperty(propertyName);

                prop.SetValue(settings, newValue);
            }
        }

        /// <summary>
        /// Validate the control value. Write the updated value back to the settings object.
        /// </summary>
        /// <param name="propertyName">The name of the setting property.</param>
        /// <param name="control">The associated control.</param>
        private void ValidateControlValue(
            string propertyName,
            NumericUpDown control)
        {
            ValidateControlValue(propertyName, control, settingsRef);
        }

        private void ValidateImportPath()
        {
            if (string.IsNullOrEmpty(settingsRef.ImportPath) || !CheckIfFileOpens(settingsRef.ImportPath) && buttonCaller == 14)
            {
                // only execute if called from Import button
                OpenFileDialog x = new OpenFileDialog()
                {
                    Title = "TD Helper - Select a .prices file"
                };

                if (Directory.Exists(settingsRef.ImportPath))
                    x.InitialDirectory = settingsRef.ImportPath;

                x.Filter = "Prices files|*.prices;*.updated;*.last|All files|*.*";

                if (x.ShowDialog() == DialogResult.OK)
                {
                    settingsRef.ImportPath = x.FileName;
                    SaveSettingsToIniFile();
                }
            }
        }

        /// <summary>
        /// Validate the setting value.
        /// </summary>
        /// <param name="propertyName">The name of the setting property.</param>
        /// <param name="control">The associated control.</param>
        private void ValidateSetting(
            string propertyName,
            NumericUpDown control,
            decimal defaultValue = -1)
        {
            ValidateSettingValue(propertyName, control, settingsRef, defaultValue);
        }

        /// <summary>
        /// Validate the setting value.
        /// </summary>
        /// <param name="propertyName">The name of the setting property.</param>
        /// <param name="control">The associated control.</param>
        /// <param name="settings">The settings object.</param>
        private void ValidateSettingValue(
            string propertyName,
            NumericUpDown control,
            TDSettings settings,
            decimal defaultValue = -1)
        {
            // retrieve the value from the settings object.
            PropertyInfo prop = settings.GetType().GetProperty(propertyName);

            decimal value = (decimal)prop.GetValue(settings);
            decimal newValue = value;

            // Compare with the control limits and set a new value if outside those limits.
            if (value < control.Minimum)
            {
                newValue
                    = defaultValue == -1
                    ? control.Minimum
                    : defaultValue;
            }
            else if (value > control.Maximum)
            {
                newValue = control.Maximum;
            }

            // Change the value if required.
            if (newValue != value)
            {
                prop.SetValue(settings, newValue);
            }
        }

        private void ValidateUploadPath()
        {
            if (string.IsNullOrEmpty(settingsRef.UploadPath) || !CheckIfFileOpens(settingsRef.UploadPath) && buttonCaller == 13)
            {
                // only execute if called from Upload button
                OpenFileDialog x = new OpenFileDialog()
                {
                    Title = "TD Helper - Select a file to upload"
                };

                if (Directory.Exists(settingsRef.UploadPath))
                {
                    x.InitialDirectory = settingsRef.UploadPath;
                }

                x.Filter = "Prices/CSV files|*.prices;*.csv|All files|*.*";

                if (x.ShowDialog() == DialogResult.OK)
                {
                    settingsRef.UploadPath = x.FileName;
                    SaveSettingsToIniFile();
                }
            }
        }

        private void WriteSavedPage(string text, string file)
        {
            // this takes text as an input, filters it, and outputs to a file
            string filteredOutput = FilterOutput(text);

            if (!string.IsNullOrEmpty(filteredOutput))
            {
                File.WriteAllText(file, filteredOutput, Encoding.UTF8);
            }
        }
    }
}