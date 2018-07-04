using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Dynamic;
using System.Text;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using SharpConfig;

namespace TDHelper
{
    #region TDSettings
    public class TDSettings
    {// standard lazy singleton pattern
        /*
         * This class is intended to serve as a central place to store/retrieve settings
         */

        private static readonly Lazy<TDSettings> _inst = new Lazy<TDSettings>(() => new TDSettings());
        private TDSettings() { } // prevent instancing
        public static TDSettings Instance { get { return _inst.Value; } } // return our reference

        public void Reset(TDSettings instance)
        {// go through and reset all accessors in instance
            instance.LocationParent = "";
            instance.LocationChild = "";
            instance.SizeParent = "";
            instance.SizeChild = "";
            instance.TDPath = "";
            instance.EdcePath = "";
            instance.NetLogPath = "";
            instance.PythonPath = "";
            instance.ImportPath = "";
            instance.UploadPath = "";
            instance.LastUsedConfig = "";
            instance.MarkedStations = "";
            instance.CmdrName = "";
            instance.Padsizes = "";
            instance.Avoid = "";
            instance.Via = "";
            instance.ExtraRunParams = "";
            instance.TreeViewFont = "";

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
        }


        // initialize below
        public string LocationParent { get; set; }
        public string LocationChild { get; set; }
        public string SizeParent { get; set; }
        public string SizeChild { get; set; }
        public string TDPath { get; set; }
        public string EdcePath { get; set; }
        public string NetLogPath { get; set; }
        public string PythonPath { get; set; }
        public string ImportPath { get; set; }
        public string UploadPath { get; set; }
        public string LastUsedConfig { get; set; }
        public string MarkedStations { get; set; }
        public string CmdrName { get; set; }
        public string Padsizes { get; set; }
        public string Avoid { get; set; }
        public string Via { get; set; }
        public string TreeViewFont { get; set; }

        private string _ExtraRunParams;
        public string ExtraRunParams
        {
            get { return this._ExtraRunParams ?? string.Empty; }
            set { this._ExtraRunParams = value; }
        }

        public decimal Hops { get; set; }
        public decimal Jumps { get; set; }
        public decimal Credits { get; set; }
        public decimal Insurance { get; set; }
        public decimal Capacity { get; set; }
        public decimal PruneHops { get; set; }
        public decimal PruneScore { get; set; }
        public decimal LSPenalty { get; set; }
        public decimal MaxLSDistance { get; set; }
        public decimal LoopInt { get; set; }
        public decimal Limit { get; set; }
        public decimal AbovePrice { get; set; }
        public decimal BelowPrice { get; set; }
        public decimal Age { get; set; }
        public decimal GPT { get; set; }
        public decimal MaxGPT { get; set; }
        public decimal Stock { get; set; }
        public decimal Demand { get; set; }
        public decimal Verbosity { get; set; }
        public decimal CSVSelect { get; set; }

        private decimal unladenLY;
        public decimal UnladenLY
        {
            get { return decimal.Truncate(unladenLY * 100) / 100; }
            set { unladenLY = decimal.Truncate(value * 100) / 100; }
        }

        private decimal ladenLY;
        public decimal LadenLY
        {
            get { return decimal.Truncate(ladenLY * 100) / 100; }
            set { ladenLY = decimal.Truncate(value * 100) / 100; }
        }

        private decimal margin;
        public decimal Margin
        {
            get { return decimal.Truncate(margin * 100) / 100; }
            set { margin = decimal.Truncate(value * 100) / 100; }
        }

        public bool Towards { get; set; }
        public bool Loop { get; set; }
        public bool Unique { get; set; }
        public bool ShowJumps { get; set; }
        public bool TestSystems { get; set; }
        public bool Corrections { get; set; }
        public bool MiniModeOnTop { get; set; }
        public bool DisableNetLogs { get; set; }
        public bool DoNotUpdate { get; set; }
        public bool HasUpdated { get; set; }

        public bool CopySystemToClipboard { get; set; }

        public Font convertFromMemberFont()
        {
            FontConverter conv = new FontConverter();
            if (this.TreeViewFont != null)
                return conv.ConvertFromInvariantString(this.TreeViewFont) as Font;
            else
                return null;
        }

        public string convertToFontString(Font fontObject)
        {
            FontConverter conv = new FontConverter();
            if (fontObject != null)
                return conv.ConvertToInvariantString(fontObject);
            else
                return "";
        }

        public decimal RebuyPercentage { get; set; }
        public string AvailableShips { get; set; }
    }
    #endregion

    #region Helpers
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

    public static class TopMostMessageBox
    {
        /*
         * This class is taken from: http://goo.gl/EnPqrF
         */

        static public DialogResult Show(bool onTop, bool playAlert, string message, string title, MessageBoxButtons buttons)
        {
            // Create a host form that is a TopMost window which will be the 
            // parent of the MessageBox.
            Form topmostForm = new Form();
            // We do not want anyone to see this window so position it off the 
            // visible screen and make it as small as possible
            topmostForm.Size = new System.Drawing.Size(1, 1);
            topmostForm.Icon = TDHelper.Properties.Resources.TDH_Icon;
            topmostForm.ShowIcon = true;
            topmostForm.Text = title;
            topmostForm.StartPosition = FormStartPosition.Manual;
            System.Drawing.Rectangle rect = SystemInformation.VirtualScreen;
            topmostForm.Location = new System.Drawing.Point(rect.Bottom + 10,
                rect.Right + 10);
            topmostForm.Show();

            if (onTop)
            {// force the form to the top of the stack
                topmostForm.Focus();
                topmostForm.BringToFront();
                topmostForm.TopMost = true;
            }
            else
            {// avoid grabbing focus
                topmostForm.TopMost = false;
            }

            if (playAlert)
                TDHelper.MainForm.PlayAlert(); // make noise to alert the user

            DialogResult result = MessageBox.Show(topmostForm, message, title,
                buttons);
            topmostForm.Dispose(); // clean it up all the way

            return result;
        }
    }
    #endregion

    public partial class MainForm : Form
    {
        public static string AssemblyGuid
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false);
                if (attributes.Length == 0) { return string.Empty; }
                return ((System.Runtime.InteropServices.GuidAttribute)attributes[0]).Value.ToUpper();
            }
        }

        public static void setVerboseLogging(string path)
        {// we should save our new XML to AppConfigLocal.xml
            string savePath = Path.GetDirectoryName(path) + "\\AppConfigLocal.xml";

            XDocument file;
            // if our file exists, load it, if not create a fresh one
            if (File.Exists(savePath))
                file = XDocument.Load(savePath, LoadOptions.PreserveWhitespace);
            else
                file = new XDocument(new XElement("AppConfig",
                        new XElement("Network", new XAttribute("VerboseLogging", "1"))));

            XmlWriterSettings settings = new XmlWriterSettings();
            StringBuilder unformat = new StringBuilder();
            StringWriterUTF8 output = new StringWriterUTF8(unformat);
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            XElement parentElement = file.Element("AppConfig");
            XElement element = parentElement.Element("Network");

            // check the attributes to see if VerboseLogging is set
            if (parentElement != null && element != null)
            {// we've got a valid local config, let's modify it
                int verboseLogging = element.Attribute("VerboseLogging") == null ? -1 : int.Parse(element.Attribute("VerboseLogging").Value);

                if (verboseLogging < 1 && verboseLogging != -1)
                {// the attribute seems to exist, lets correct it and move on
                    element.Attribute("VerboseLogging").Value = "1";
                }
                else if (verboseLogging == -1)
                {// our attribute doesn't exist, create it
                    element.Add(new XAttribute("VerboseLogging", "1"));
                }
            }

            // refresh our path to the latest netlog
            latestLogPaths = CollectLogPaths(settingsRef.NetLogPath, "netLog*.log");
            if (latestLogPaths != null && latestLogPaths.Count > 0)
                recentLogPath = latestLogPaths[0];
            else
                recentLogPath = "";

            // always make a backup for safety
            if (File.Exists(path))
                File.Copy(path, path + ".backup", true);

            // must be utf-8 on the output, so we force it with a class override
            using (XmlWriter xmlWriter = XmlWriter.Create(savePath, settings))
            {
                file.WriteTo(xmlWriter);
            }
        }

        public static void validateVerboseLogging()
        {
            string path = t_AppConfigPath, foundPath = "";
            string altPath = Directory.GetParent(path) + "\\AppConfigLocal.xml";

            // test AppConfigLocal.xml first, then AppConfigLocal.xml
            if (File.Exists(altPath))
                foundPath = altPath;
            else if (File.Exists(path))
                foundPath = path;
            else
                throw new ArgumentException("Cannot find an AppConfigLocal file based on the given path: " + t_AppConfigPath);

            XDocument file = XDocument.Load(foundPath, LoadOptions.PreserveWhitespace);
            XElement parentElement = file.Element("AppConfig");
            XElement element = parentElement.Element("Network");

            if (parentElement != null && element != null)
            {// we've got a valid config
                if (element.Attribute("VerboseLogging") == null || int.Parse(element.Attribute("VerboseLogging").Value) != 1)
                {// it can be set
                    // ask the user first
                    DialogResult dialog = TopMostMessageBox.Show(true, true, "VerboseLogging isn't set, it must be corrected so we can grab recent systems.\r\n\nMay we fix it?", "Error", MessageBoxButtons.YesNo);
                    if (dialog == DialogResult.Yes)
                    {
                        setVerboseLogging(foundPath); // so fix it
                    }
                    else
                    {
                        DialogResult dialog2 = TopMostMessageBox.Show(true, true, "We will set the DisableNetLogs override in our config file to prevent prompts.\r\n", "Notice", MessageBoxButtons.OK);
                        settingsRef.DisableNetLogs = true;
                    }
                }
                else
                {// it's already set, let's continue
                    // refresh our path to the first netlog
                    latestLogPaths = CollectLogPaths(settingsRef.NetLogPath, "netLog*.log");
                    if (latestLogPaths != null && latestLogPaths.Count > 0)
                        recentLogPath = latestLogPaths[0];
                    else
                        recentLogPath = "";
                }
            }
            else
                setVerboseLogging(foundPath); // try to create a valid file
        }

        public static string saveWinSize(Form x)
        {// save window size
            string modWidth, modHeight;

            // make sure we're not out of bounds
            if (x.Size.Width > SystemInformation.VirtualScreen.Right)
                modWidth = SystemInformation.VirtualScreen.Right.ToString();
            else
                modWidth = x.Size.Width.ToString();

            if (x.Size.Height > SystemInformation.VirtualScreen.Bottom)
                modHeight = SystemInformation.VirtualScreen.Bottom.ToString();
            else
                modHeight = x.Size.Height.ToString();

            return string.Format("{0},{1}", modWidth, modHeight);
        }

        public static int[] loadWinSize(string objRef)
        {// load window size
            int[] winSize = new int[] { };

            if (!string.IsNullOrEmpty(objRef))
            {
                string[] t_winSize = objRef.Split(',').ToArray();
                winSize = new int[2];
                winSize[0] = Convert.ToInt32(t_winSize.GetValue(0));
                winSize[1] = Convert.ToInt32(t_winSize.GetValue(1));

                if (winSize[0] > SystemInformation.VirtualScreen.Right)
                    winSize[0] = SystemInformation.VirtualScreen.Right;

                if (winSize[1] > SystemInformation.VirtualScreen.Bottom)
                    winSize[1] = SystemInformation.VirtualScreen.Bottom;
            }

            return winSize;
        }

        public static string saveWinLoc(Form x)
        {// save winLoc to a given variable object
            return string.Format("{0},{1}", x.Location.X, x.Location.Y);
        }

        public static int[] loadWinLoc(string objRef)
        {// load winLoc from a given variable object
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

        public static void forceFormOnScreen(Form form)
        {
            if (form.Left < SystemInformation.VirtualScreen.Left)
                form.Left = SystemInformation.VirtualScreen.Left;

            if (form.Right > SystemInformation.VirtualScreen.Right)
                form.Left = SystemInformation.VirtualScreen.Right - form.Width;

            if (form.Top < SystemInformation.VirtualScreen.Top)
                form.Top = SystemInformation.VirtualScreen.Top;

            if (form.Bottom > SystemInformation.VirtualScreen.Bottom)
                form.Top = SystemInformation.VirtualScreen.Bottom - form.Height;
        }

        public static bool validateConfigFile(string filePath)
        {
            bool fileIsValid = false;

            if (CheckIfFileOpens(filePath))
            {
                Configuration config = Configuration.LoadFromFile(filePath);

               fileIsValid = config.GetSectionsNamed("App").Count() == 1;
            }

            return fileIsValid;
        }

        private bool containsPadSizes(string text)
        {
            // we only want one of each from the key
            if (!string.IsNullOrEmpty(text))
            {
                char[] c = new char[] { 'M', 'L', '?' };
                char[] z = text.ToUpperInvariant().ToCharArray();

                // count how many we found
                var intersect = z.Intersect(c).ToList();

                // check for only M/L/? and no more than that
                if (intersect.Count > 3 || intersect.Count < 1)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        private bool containsHexCode(string input)
        {
            /*
             * Taken from: http://goo.gl/gBLkGs with adjustments
             */

            // to be safe we force uppercase for the comparison
            if (!string.IsNullOrEmpty(input.ToUpper()))
            {// must be only 4 characters
                char[] chars = input.ToUpper().ToCharArray();
                if (chars.Length < 5 && chars.Length > 0)
                {
                    bool isHex;

                    foreach (var c in chars)
                    {
                        isHex = ((c >= '0' && c <= '9') ||
                                 (c >= 'A' && c <= 'F'));

                        if (!isHex)
                            return false;
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        private bool isMarkedStation(string input, List<string> parentList)
        {
            return StringInList(input, parentList);
        }

        private void addMarkedStation(string input, List<string> parentList)
        {
            if (!isMarkedStation(input, parentList) && StringInList(input, outputSysStnNames) 
                && !StringInList(input, parentList))
            {// insert at the top of the list
                parentList.Insert(0, input);
                settingsRef.MarkedStations = serializeMarkedStations(parentList);
            }
        }

        private void removeMarkedStation(string input, List<string> parentList)
        {
            int index = IndexInList(input, parentList);

            // it's valid, grab the index
            if (isMarkedStation(input, parentList) && index >= 0)
            {
                parentList.RemoveAt(index);
                settingsRef.MarkedStations = serializeMarkedStations(parentList);
            }
        }

        private static List<string> parseMarkedStations()
        {
            if (!string.IsNullOrEmpty(settingsRef.MarkedStations))
                return RemoveExtraWhitespace(settingsRef.MarkedStations).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            else
                return new List<string>();
        }

        private string serializeMarkedStations(List<string> parentList)
        {
            if (parentList.Count > 0)
            {
                string builtString = string.Join(",", parentList);
                return builtString;
            }
            else
                return "";
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

            // Settgins used for trade route calculation.
            config["App"]["AbovePrice"].DecimalValue = settings.AbovePrice;
            config["App"]["Age"].DecimalValue = settings.Age;
            config["App"]["Avoid"].StringValue = settings.Avoid ?? string.Empty;
            config["App"]["BelowPrice"].DecimalValue = settings.BelowPrice;
            config["App"]["Corrections"].BoolValue = settings.Corrections;
            config["App"]["CSVSelect"].DecimalValue = settings.CSVSelect;
            config["App"]["Demand"].DecimalValue = settings.Demand;
            config["App"]["ExtraRunParams"].StringValue = settings.ExtraRunParams ?? string.Empty;
            config["App"]["GPT"].DecimalValue = settings.GPT;
            config["App"]["Hops"].DecimalValue = settings.Hops;
            config["App"]["Jumps"].DecimalValue = settings.Jumps;
            config["App"]["Limit"].DecimalValue = settings.Limit;
            config["App"]["Loop"].BoolValue = settings.Loop;
            config["App"]["LoopInt"].DecimalValue = settings.LoopInt;
            config["App"]["LSPenalty"].DecimalValue = settings.LSPenalty;
            config["App"]["Margin"].DecimalValue = settings.Margin;
            config["App"]["MarkedStations"].StringValue = settings.MarkedStations ?? string.Empty;
            config["App"]["MaxGPT"].DecimalValue = settings.MaxGPT;
            config["App"]["MaxLSDistance"].DecimalValue = settings.MaxLSDistance;
            config["App"]["PruneHops"].DecimalValue = settings.PruneHops;
            config["App"]["PruneScore"].DecimalValue = settings.PruneScore;
            config["App"]["ShowJumps"].BoolValue = settings.ShowJumps;
            config["App"]["Stock"].DecimalValue = settings.Stock;
            config["App"]["Towards"].BoolValue = settings.Towards;
            config["App"]["Unique"].BoolValue = settings.Unique;
            config["App"]["Verbosity"].DecimalValue = settings.Verbosity;
            config["App"]["Via"].StringValue = settings.Via ?? string.Empty;

            // Commander settings
            config["Commander"]["CmdrName"].StringValue = settings.CmdrName ?? string.Empty;
            config["Commander"]["Credits"].DecimalValue = settings.Credits;
            config["Commander"]["RebuyPercentage"].DecimalValue = settings.RebuyPercentage;

            // TD Helper system settings.
            config["System"]["CopySystemToClipboard"].BoolValue = settings.CopySystemToClipboard;
            config["System"]["DisableNetLogs"].BoolValue = settings.DisableNetLogs;
            config["System"]["DoNotUpdate"].BoolValue = settings.DoNotUpdate;
            config["System"]["EdcePath"].StringValue = settings.EdcePath ?? string.Empty;
            config["System"]["HasUpdated"].BoolValue = settings.HasUpdated;
            config["System"]["ImportPath"].StringValue = settings.ImportPath ?? string.Empty;
            config["System"]["LastUsedConfig"].StringValue = settings.LastUsedConfig ?? string.Empty;
            config["System"]["LocationChild"].StringValue = settings.LocationChild ?? string.Empty;
            config["System"]["LocationParent"].StringValue = settings.LocationParent ?? string.Empty;
            config["System"]["MiniModeOnTop"].BoolValue = settings.MiniModeOnTop;
            config["System"]["NetLogPath"].StringValue = settings.NetLogPath ?? string.Empty;
            config["System"]["PythonPath"].StringValue = settings.PythonPath ?? string.Empty;
            config["System"]["SizeChild"].StringValue = settings.SizeChild ?? string.Empty;
            config["System"]["SizeParent"].StringValue = settings.SizeParent ?? string.Empty;
            config["System"]["TDPath"].StringValue = settings.TDPath ?? string.Empty;
            config["System"]["TestSystems"].BoolValue = settings.TestSystems;
            config["System"]["TreeViewFont"].StringValue = settings.TreeViewFont ?? string.Empty;
            config["System"]["UploadPath"].StringValue = settings.UploadPath ?? string.Empty;
            config["System"]["AvailableShips"].StringValue = settings.AvailableShips ?? string.Empty ;

            // Update the current ship if required.
            if (! string.IsNullOrEmpty(settings.LastUsedConfig))
            {
                string sectionName = settings.LastUsedConfig;

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

            config.SaveToFile(configFile);
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

                settings.AbovePrice = config["App"]["AbovePrice"].DecimalValue;
                settings.Age = config["App"]["Age"].DecimalValue;
                settings.Avoid = config["App"]["Avoid"].StringValue;
                settings.BelowPrice = config["App"]["BelowPrice"].DecimalValue;
                settings.Corrections = config["App"]["Corrections"].BoolValue;
                settings.CSVSelect = config["App"]["CSVSelect"].DecimalValue;
                settings.Demand = config["App"]["Demand"].DecimalValue;
                settings.ExtraRunParams = config["App"]["ExtraRunParams"].StringValue;
                settings.GPT = config["App"]["GPT"].DecimalValue;
                settings.Hops = config["App"]["Hops"].DecimalValue;
                settings.Jumps = config["App"]["Jumps"].DecimalValue;
                settings.Limit = config["App"]["Limit"].DecimalValue;
                settings.Loop = config["App"]["Loop"].BoolValue;
                settings.LoopInt = config["App"]["LoopInt"].DecimalValue;
                settings.LSPenalty = config["App"]["LSPenalty"].DecimalValue;
                settings.Margin = config["App"]["Margin"].DecimalValue;
                settings.MarkedStations = config["App"]["MarkedStations"].StringValue;
                settings.MaxGPT = config["App"]["MaxGPT"].DecimalValue;
                settings.MaxLSDistance = config["App"]["MaxLSDistance"].DecimalValue;
                settings.PruneHops = config["App"]["PruneHops"].DecimalValue;
                settings.PruneScore = config["App"]["PruneScore"].DecimalValue;
                settings.ShowJumps = config["App"]["ShowJumps"].BoolValue;
                settings.Stock = config["App"]["Stock"].DecimalValue;
                settings.Towards = config["App"]["Towards"].BoolValue;
                settings.Unique = config["App"]["Unique"].BoolValue;
                settings.Verbosity = config["App"]["Verbosity"].DecimalValue;
                settings.Via = config["App"]["Via"].StringValue;

                // Commander settings
                settings.CmdrName = config["Commander"]["CmdrName"].StringValue;
                settings.Credits = config["Commander"]["Credits"].DecimalValue;
                settings.RebuyPercentage = config["Commander"]["RebuyPercentage"].DecimalValue;

                // TD Helper system settings.
                settings.CopySystemToClipboard = config["System"]["CopySystemToClipboard"].BoolValue;
                settings.DisableNetLogs = config["System"]["DisableNetLogs"].BoolValue;
                settings.DoNotUpdate = config["System"]["DoNotUpdate"].BoolValue;
                settings.EdcePath = config["System"]["EdcePath"].StringValue;
                settings.HasUpdated = config["System"]["HasUpdated"].BoolValue;
                settings.ImportPath = config["System"]["ImportPath"].StringValue;
                settings.LastUsedConfig = config["System"]["LastUsedConfig"].StringValue;
                settings.LocationChild = config["System"]["LocationChild"].StringValue;
                settings.LocationParent = config["System"]["LocationParent"].StringValue;
                settings.MiniModeOnTop = config["System"]["MiniModeOnTop"].BoolValue;
                settings.NetLogPath = config["System"]["NetLogPath"].StringValue;
                settings.PythonPath = config["System"]["PythonPath"].StringValue;
                settings.SizeChild = config["System"]["SizeChild"].StringValue;
                settings.SizeParent = config["System"]["SizeParent"].StringValue;
                settings.TDPath = config["System"]["TDPath"].StringValue;
                settings.TestSystems = config["System"]["TestSystems"].BoolValue;
                settings.TreeViewFont = config["System"]["TreeViewFont"].StringValue;
                settings.UploadPath = config["System"]["UploadPath"].StringValue;
                settings.AvailableShips = config["System"]["AvailableShips"].StringValue;

                if (string.IsNullOrEmpty(settings.AvailableShips))
                {
                    settings.AvailableShips = "Default";
                }
            }
        }

        /// <summary>
        /// Get a list of available ships from the settings.
        /// </summary>
        /// <returns>A list of available ships.</returns>
        public IList<string> SetAvailableShips()
        {
            string[] ships = MainForm.settingsRef.AvailableShips.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            return new List<string>(ships).OrderBy(x => x).ToList();
        }
    }
}