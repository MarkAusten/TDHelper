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

        static public DialogResult Show(
            bool onTop, 
            bool playAlert, 
            string message, 
            string title, 
            MessageBoxButtons buttons)
        {
            Rectangle rect = SystemInformation.VirtualScreen;

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
                Location = new Point(rect.Bottom + 10, rect.Right + 10),
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
                MainForm.PlayAlert(); // make noise to alert the user
            }

            DialogResult result = MessageBox.Show(topmostForm, message, title, buttons);

            topmostForm.Dispose(); // clean it up all the way

            return result;
        }
    }

    public partial class MainForm : Form
    {
        private static bool verboseLoggingChecked;

        public static string AssemblyGuid
        {
            get
            {
                string assemblyGuid;

                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false);

                assemblyGuid
                    = attributes.Length == 0
                    ? string.Empty
                    : ((GuidAttribute)attributes[0]).Value.ToUpper();

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
        /// Retrieve a boolean setting from the config file.
        /// </summary>
        /// <param name="section">The section object</param>
        /// <param name="key">The value key</param>
        /// <returns>The required value.</returns>
        public static bool GetBooleanSetting(
            Section section, 
            string key)
        {
            return SectionHasKey(section, key) ? section[key].BoolValue : false;
        }

        /// <summary>
        /// Retrieve a decimal setting from the config file.
        /// </summary>
        /// <param name="section">The section object</param>
        /// <param name="key">The value key</param>
        /// <returns>The required value.</returns>
        public static decimal GetDecimalSetting(
            Section section, 
            string key)
        {
            return SectionHasKey(section, key) ? section[key].DecimalValue : 0M;
        }

        /// <summary>
        /// Retrieve a string setting from the config file.
        /// </summary>
        /// <param name="section">The section object</param>
        /// <param name="key">The value key</param>
        /// <returns><The required value./returns>
        public static string GetStringSetting(
            Section section, 
            string key)
        {
            return SectionHasKey(section, key) ? section[key].StringValue : string.Empty;
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

                settings.Age = GetDecimalSetting(configSection, "Age");
                settings.Avoid = GetStringSetting(configSection, "Avoid");
                settings.Demand = GetDecimalSetting(configSection, "Demand");
                settings.EndJumps = GetDecimalSetting(configSection, "EndJumps");
                settings.ExtraRunParams = GetStringSetting(configSection, "ExtraRunParams");
                settings.GPT = GetDecimalSetting(configSection, "GPT");
                settings.Hops = GetDecimalSetting(configSection, "Hops");
                settings.Jumps = GetDecimalSetting(configSection, "Jumps");
                settings.Limit = GetDecimalSetting(configSection, "Limit");
                settings.Loop = GetBooleanSetting(configSection, "Loop");
                settings.LoopInt = GetDecimalSetting(configSection, "LoopInt");
                settings.LSPenalty = GetDecimalSetting(configSection, "LSPenalty");
                settings.Margin = GetDecimalSetting(configSection, "Margin");
                settings.MarkedStations = GetStringSetting(configSection, "MarkedStations");
                settings.MaxGPT = GetDecimalSetting(configSection, "MaxGPT");
                settings.MaxLSDistance = GetDecimalSetting(configSection, "MaxLSDistance");
                settings.Planetary = GetStringSetting(configSection, "Planetary");
                settings.PruneHops = GetDecimalSetting(configSection, "PruneHops");
                settings.PruneScore = GetDecimalSetting(configSection, "PruneScore");
                settings.NumberOfRoutes = GetDecimalSetting(configSection, "NumberOfRoutes");
                settings.ShowJumps = GetBooleanSetting(configSection, "ShowJumps");
                settings.ShowProgress = GetBooleanSetting(configSection, "ShowProgress");
                settings.StartJumps = GetDecimalSetting(configSection, "startJumps");
                settings.Stock = GetDecimalSetting(configSection, "Stock");
                settings.Summary = GetBooleanSetting(configSection, "Summary");
                settings.Towards = GetBooleanSetting(configSection, "Towards");
                settings.Unique = GetBooleanSetting(configSection, "Unique");
                settings.Verbosity = GetDecimalSetting(configSection, "Verbosity");
                settings.Via = GetStringSetting(configSection, "Via");

                // Commander settings
                configSection = config["Commander"];

                settings.CmdrName = GetStringSetting(configSection, "CmdrName");
                settings.Credits = GetDecimalSetting(configSection, "Credits");
                settings.RebuyPercentage = GetDecimalSetting(configSection, "RebuyPercentage");

                // TD Helper system settings.
                configSection = config["System"];

                settings.AvailableShips = GetStringSetting(configSection, "AvailableShips");
                settings.CopySystemToClipboard = GetBooleanSetting(configSection, "CopySystemToClipboard");
                settings.DisableNetLogs = GetBooleanSetting(configSection, "DisableNetLogs");
                settings.DoNotUpdate = GetBooleanSetting(configSection, "DoNotUpdate");
                settings.EdcePath = GetStringSetting(configSection, "EdcePath");
                settings.HasUpdated = GetBooleanSetting(configSection, "HasUpdated");
                settings.ImportPath = GetStringSetting(configSection, "ImportPath");
                settings.LastUsedConfig = GetStringSetting(configSection, "LastUsedConfig");
                settings.LocationChild = GetStringSetting(configSection, "LocationChild");
                settings.LocationParent = GetStringSetting(configSection, "LocationParent");
                settings.MiniModeOnTop = GetBooleanSetting(configSection, "MiniModeOnTop");
                settings.NetLogPath = GetStringSetting(configSection, "NetLogPath");
                settings.PythonPath = GetStringSetting(configSection, "PythonPath");
                settings.Quiet = GetBooleanSetting(configSection, "Quiet");
                settings.SizeChild = GetStringSetting(configSection, "SizeChild");
                settings.SizeParent = GetStringSetting(configSection, "SizeParent");
                settings.TDPath = GetStringSetting(configSection, "TDPath");
                settings.TestSystems = GetBooleanSetting(configSection, "TestSystems");
                settings.TreeViewFont = GetStringSetting(configSection, "TreeViewFont");
                settings.UploadPath = GetStringSetting(configSection, "UploadPath");

                // EDDBlink settings
                configSection = config["EDDBlink"];

                settings.SkipVend = GetBooleanSetting(configSection, "SkipVend");
                settings.Solo = GetBooleanSetting(configSection, "Solo");

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
            configSection["Age"].DecimalValue = settings.Age;
            configSection["Avoid"].StringValue = settings.Avoid ?? string.Empty;
            configSection["BlackMarket"].BoolValue = settings.BlackMarket;
            configSection["Direct"].BoolValue = settings.Direct;
            configSection["Demand"].DecimalValue = settings.Demand;
            configSection["EndJumps"].DecimalValue = settings.EndJumps;
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
            configSection["NumberOfRoutes"].DecimalValue = settings.NumberOfRoutes;
            configSection["Planetary"].StringValue = settings.Planetary ?? string.Empty;
            configSection["PruneHops"].DecimalValue = settings.PruneHops;
            configSection["PruneScore"].DecimalValue = settings.PruneScore;
            configSection["Shorten"].BoolValue = settings.Shorten;
            configSection["ShowJumps"].BoolValue = settings.ShowJumps;
            configSection["ShowProgress"].BoolValue = settings.ShowProgress;
            configSection["StartJumps"].DecimalValue = settings.StartJumps;
            configSection["Stock"].DecimalValue = settings.Stock;
            configSection["Summary"].BoolValue = settings.Summary;
            configSection["Towards"].BoolValue = settings.Towards;
            configSection["Unique"].BoolValue = settings.Unique;
            configSection["Verbosity"].DecimalValue = settings.Verbosity;
            configSection["Via"].StringValue = settings.Via ?? string.Empty;

            // Commander settings
            configSection = config["Commander"];

            configSection["CmdrName"].StringValue = settings.CmdrName ?? string.Empty;
            configSection["Credits"].DecimalValue = settings.Credits;
            configSection["RebuyPercentage"].DecimalValue = settings.RebuyPercentage;

            // TD Helper system settings.
            configSection = config["System"];

            configSection["AvailableShips"].StringValue = settings.AvailableShips ?? string.Empty;
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
            configSection["Quiet"].BoolValue = settings.Quiet;
            configSection["SizeChild"].StringValue = settings.SizeChild ?? string.Empty;
            configSection["SizeParent"].StringValue = settings.SizeParent ?? string.Empty;
            configSection["TDPath"].StringValue = settings.TDPath ?? string.Empty;
            configSection["TestSystems"].BoolValue = settings.TestSystems;
            configSection["TreeViewFont"].StringValue = settings.TreeViewFont ?? string.Empty;
            configSection["UploadPath"].StringValue = settings.UploadPath ?? string.Empty;

            configSection = config["EDDBlink"];

            configSection["SkipVend"].BoolValue = settings.SkipVend;
            configSection["Solo"].BoolValue = settings.Solo;

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
            return "{0},{1}".With(x.Location.X, x.Location.Y);
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

            return "{0},{1}".With(modWidth, modHeight);
        }

        public static bool SectionHasKey(
            Section section, 
            string key)
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

            RefreshNetLogFileList();

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

        /// <summary>
        /// Refresh the list of net log files.
        /// </summary>
        private static void RefreshNetLogFileList()
        {
            // refresh our path to the latest netlog
            latestLogPaths = CollectLogPaths(settingsRef.NetLogPath, "netLog*.log");

            recentLogPath
                = latestLogPaths != null && latestLogPaths.Count > 0
                ? latestLogPaths[0]
                : string.Empty;
        }

        /// <summary>
        /// Some basic config fie validation.
        /// </summary>
        /// <param name="filePath">The path to the config fie.</param>
        /// <returns></returns>
        public static bool ValidateConfigFile(string filePath)
        {
            bool fileIsValid = false;

            // Check if the file opens.
            if (CheckIfFileOpens(filePath))
            {
                Configuration config = Configuration.LoadFromFile(filePath);

                // The app section is a required section.
                fileIsValid = config.GetSectionsNamed("App").Count() == 1;
            }

            return fileIsValid;
        }

        public static void ValidateVerboseLogging()
        {
            if (!verboseLoggingChecked)
            {
                // Open the AppConfig file and check to see if the setting is found.
                string path = Path.Combine(Path.GetDirectoryName(settingsRef.NetLogPath), "AppConfig.xml"); 

                XDocument file = XDocument.Load(path, LoadOptions.PreserveWhitespace);
                XElement parentElement = file.Element("AppConfig");
                XElement element = parentElement.Element("Network");
                bool elementFound = !(element.Attribute("VerboseLogging") == null);

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

                verboseLoggingChecked = true;
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

        private void AddMarkedStation(
            string input, 
            List<string> parentList)
        {
            if (!IsMarkedStation(input, parentList) && 
                StringInList(input, outputSysStnNames) &&
                !StringInList(input, parentList))
            {
                // insert at the top of the list
                parentList.Insert(0, input);
                settingsRef.MarkedStations = SerializeMarkedStations(parentList);
            }
        }

        private string ContainsPadSizes(string text)
        {
            return ToggleAndSort(text, "SML?");
        }

        private string ContainsPlanetary(string text)
        {
            return ToggleAndSort(text, "YN?");
        }

        private bool IsMarkedStation(
            string input, 
            List<string> parentList)
        {
            return StringInList(input, parentList);
        }

        private void RemoveMarkedStation(
            string input, 
            List<string> parentList)
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

        /// <summary>
        /// Search the text for characters in match and return in the same order as match.
        /// </summary>
        /// <param name="text">The text to be searched.</param>
        /// <param name="match">The required matching charcters.</param>
        /// <returns></returns>
        private string ToggleAndSort(
            string text,
            string match)
        {
            string result = text;

            if (!string.IsNullOrEmpty(result))
            {
                char[] sortedArray = text.ToUpper()
                    .Distinct()
                    .ToArray();

                result = string.Empty;

                foreach (char letter in match.Select(x => x))
                {
                    if (sortedArray.Contains(letter))
                    {
                        result += letter.ToString();
                    }
                }
            }

            return result;
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

        private TDSettings()
        {
        } // prevent instancing

        public static TDSettings Instance { get { return _inst.Value; } } // return our reference
        public decimal Age { get; set; }
        public string AvailableShips { get; set; }
        public string Avoid { get; set; }
        public bool BlackMarket { get; set; }
        public decimal Capacity { get; set; }
        public string CmdrName { get; set; }
        public bool CopySystemToClipboard { get; set; }
        public decimal Credits { get; set; }
        public decimal Demand { get; set; }
        public bool Direct { get; set; }
        public bool DisableNetLogs { get; set; }
        public bool DoNotUpdate { get; set; }
        public string EdcePath { get; set; }
        public decimal EndJumps { get; set; }
        public string ExtraRunParams { get; set; }
        public decimal GPT { get; set; }
        public bool HasUpdated { get; set; }
        public decimal Hops { get; set; }
        public string ImportPath { get; set; }
        public decimal Insurance { get; set; }
        public decimal Jumps { get; set; }
        public decimal LadenLY { get; set; }
        public string LastUsedConfig { get; set; }
        public decimal Limit { get; set; }
        public string LocationChild { get; set; }
        public string LocationParent { get; set; }
        public bool Loop { get; set; }
        public decimal LoopInt { get; set; }
        public decimal LSPenalty { get; set; }
        public decimal Margin { get; set; }
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
        public decimal NumberOfRoutes { get; set; }
        public bool Shorten { get; set; }
        public bool ShowJumps { get; set; }
        public bool ShowProgress { get; set; }
        public string SizeChild { get; set; }
        public string SizeParent { get; set; }
        public bool SkipVend { get; set; }
        public bool Solo { get; set; }
        public decimal StartJumps { get; set; }
        public decimal Stock { get; set; }
        public bool Summary { get; set; }
        public string TDPath { get; set; }
        public bool TestSystems { get; set; }
        public bool Towards { get; set; }
        public string TreeViewFont { get; set; }
        public bool Unique { get; set; }
        public decimal UnladenLY { get; set; }
        public string UploadPath { get; set; }
        public decimal Verbosity { get; set; }
        public string Via { get; set; }

        public Font ConvertFromMemberFont()
        {
            FontConverter conv = new FontConverter();

            return 
                TreeViewFont == null
                ? null
                : conv.ConvertFromInvariantString(TreeViewFont) as Font;
        }

        public string ConvertToFontString(Font fontObject)
        {
            FontConverter conv = new FontConverter();

            return
                fontObject == null
                ? null
                : conv.ConvertToInvariantString(fontObject);
        }

        #endregion Props

        public void Reset(TDSettings instance)
        {
            // go through and reset all accessors in instance
            instance.Age = 0;
            instance.AvailableShips = string.Empty;
            instance.Avoid = string.Empty;
            instance.BlackMarket = false;
            instance.Capacity = 0;
            instance.CmdrName = string.Empty;
            instance.CopySystemToClipboard = false;
            instance.Credits = 0;
            instance.Demand = 0;
            instance.Direct = false;
            instance.DisableNetLogs = false;
            instance.DoNotUpdate = false;
            instance.EdcePath = string.Empty;
            instance.EndJumps = 0;
            instance.ExtraRunParams = string.Empty;
            instance.GPT = 0;
            instance.HasUpdated = false;
            instance.Hops = 0;
            instance.ImportPath = string.Empty;
            instance.Insurance = 0;
            instance.Jumps = 0;
            instance.LadenLY = 0;
            instance.LastUsedConfig = string.Empty;
            instance.Limit = 0;
            instance.LocationChild = string.Empty;
            instance.LocationParent = string.Empty;
            instance.Loop = false;
            instance.LoopInt = 0;
            instance.LSPenalty = 0;
            instance.Margin = 0;
            instance.MarkedStations = string.Empty;
            instance.MaxGPT = 0;
            instance.MaxLSDistance = 0;
            instance.MiniModeOnTop = false;
            instance.NetLogPath = string.Empty;
            instance.Padsizes = string.Empty;
            instance.Planetary = string.Empty;
            instance.PruneHops = 0;
            instance.PruneScore = 0;
            instance.PythonPath = string.Empty;
            instance.Quiet = false;
            instance.RebuyPercentage = 5;
            instance.Shorten = false;
            instance.ShowJumps = false;
            instance.ShowProgress = false;
            instance.SizeChild = string.Empty;
            instance.SizeParent = string.Empty;
            instance.SkipVend = false;
            instance.Solo = false;
            instance.StartJumps = 0;
            instance.Stock = 0;
            instance.Summary = false;
            instance.TDPath = string.Empty;
            instance.TestSystems = false;
            instance.Towards = false;
            instance.TreeViewFont = string.Empty;
            instance.Unique = false;
            instance.UnladenLY = 0;
            instance.UploadPath = string.Empty;
            instance.Verbosity = 0;
            instance.Via = string.Empty;
        }
    }

    #endregion TDSettings
}