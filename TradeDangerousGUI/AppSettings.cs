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

            instance.EDAPIUser = "";
            instance.EDAPIPass = "";

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
        }


        // initialize below
        public string LocationParent { get; set; }
        public string LocationChild { get; set; }
        public string SizeParent { get; set; }
        public string SizeChild { get; set; }
        public string TDPath { get; set; }
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

        public string EDAPIUser { get; set; }
        public string EDAPIPass { get; set; }

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
                TDHelper.Form1.playAlert(); // make noise to alert the user

            DialogResult result = MessageBox.Show(topmostForm, message, title,
                buttons);
            topmostForm.Dispose(); // clean it up all the way

            return result;
        }
    }
    #endregion

    public partial class Form1 : Form
    {
        public static string AssemblyGuid
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false);
                if (attributes.Length == 0) { return String.Empty; }
                return ((System.Runtime.InteropServices.GuidAttribute)attributes[0]).Value.ToUpper();
            }
        }

        public static void setVerboseLogging(string path)
        {// we should save our new XML to AppConfigLocal.xml
            String savePath = Path.GetDirectoryName(path) + "\\AppConfigLocal.xml";

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
            latestLogPaths = collectLogPaths(settingsRef.NetLogPath, "netLog*.log");
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
            String path = t_AppConfigPath, foundPath = "";
            String altPath = Directory.GetParent(path) + "\\AppConfigLocal.xml";

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
                    latestLogPaths = collectLogPaths(settingsRef.NetLogPath, "netLog*.log");
                    if (latestLogPaths != null && latestLogPaths.Count > 0)
                        recentLogPath = latestLogPaths[0];
                    else
                        recentLogPath = "";
                }
            }
            else
                setVerboseLogging(foundPath); // try to create a valid file
        }

        public static void Serialize(string path, object objectRef, string objectName)
        {// pull data from an object, push to XML
            if (validateConfigFile(path))
            {
                XDocument doc = XDocument.Load(path);
                XElement root = doc.Descendants("TDSettings").FirstOrDefault();
                XElement element = root.Elements(objectName).FirstOrDefault();

                if (element != null)
                {// it exists, let's change it
                    // prevent errors by correcting booleans
                    if (objectRef.GetType() == typeof(bool))
                        element.Value = objectRef.ToString().ToLower();
                    else
                        element.Value = objectRef.ToString();

                    doc.Save(path);
                }
                else
                {// it doesn't exist, let's add it
                    root.Add(new XElement(objectName, objectRef.ToString()));
                    doc.Save(path);
                }
            }
            else
            {// retry after making a valid file
                Serialize(path);
                Serialize(path, objectRef, objectName);
            }
        }

        public static void Serialize(string path)
        {// pull data from the class struct, push to XML
            XDocument doc = new XDocument();

            using (var writer = doc.CreateWriter())
            {
                XmlSerializer x = new XmlSerializer(typeof(TDSettings));
                XmlSerializerNamespaces nsi = new XmlSerializerNamespaces();
                nsi.Add("", ""); // clear the namespace to be tidy
                x.Serialize(writer, settingsRef, nsi);
            }

            XElement element = doc.Root;
            element.Save(path);
        }

        private void Deserialize(string path)
        {// pull data from XML, push to the class struct
            if (validateConfigFile(path))
            {
                XmlSerializer x = new XmlSerializer(typeof(TDSettings));
                StreamReader reader = new StreamReader(path);
                settingsRef = (TDSettings)x.Deserialize(reader);
                reader.Close();
                reader.Dispose();

                // push the filename (no ext) to the Name tag
                this.Name = Path.GetFileNameWithoutExtension(path);
            }
            else
                throw new Exception("Deserializer cannot open the given config file: " + path);
        }

        private void Deserialize(string path, object objectRef, string objectName)
        {// pull object from XML, push to a class object
            if (validateConfigFile(path))
            {
                XDocument doc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
                XElement element = doc.Descendants("TDSettings").Elements(objectName).FirstOrDefault();
                if (element != null)
                {
                    objectRef = element;
                }
                else
                    throw new Exception("Cannot find the referenced object [" + objectName + "] in the given config file: " + path);
            }
            else
                throw new Exception("Deserializer cannot open the given config file: " + path);
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

            return String.Format("{0},{1}", modWidth, modHeight);
        }

        public static int[] loadWinSize(string objRef)
        {// load window size
            int[] winSize = new int[] { };

            if (!String.IsNullOrEmpty(objRef))
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
            return String.Format("{0},{1}", x.Location.X, x.Location.Y);
        }

        public static int[] loadWinLoc(string objRef)
        {// load winLoc from a given variable object
            int[] winLoc = new int[] { };

            if (!String.IsNullOrEmpty(objRef))
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

        private List<List<string>> parseValidConfigs()
        {
            /*
             * Check our local directory for valid files and return a nested list
             * list [0] for the file path, list [1] for the config name
             */

            XDocument tempDoc = new XDocument();

            // make a nested list so we can have 2 dimensions
            List<List<string>> varMatrix = new List<List<string>>();
            varMatrix.Add(new List<string>());
            varMatrix.Add(new List<string>());

            // check for a list of possible config xmls
            string[] t_filePaths = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.xml");

            // populate our table after checking the files
            foreach (String s in t_filePaths)
            {// first the paths need names
                if (checkIfFileOpens(s))
                {
                    if (validateConfigFile(s))
                    {// first we check to make sure the config is valid
                        varMatrix[0].Add(s); // filepath = [0]
                        // then we pull the name from the filename of the config
                        varMatrix[1].Add(Path.GetFileNameWithoutExtension(s)); // config name = [1]
                    }
                }
            }

            if (varMatrix[1].Contains("Default") && varMatrix[1].IndexOf("Default") != 0)
            {// move the Default, if necessary, to the top of the stack
                int i = varMatrix[1].IndexOf("Default");
                string s = varMatrix[0][i]; // path
                string t = varMatrix[1][i]; // name
                varMatrix[0].RemoveAt(i);
                varMatrix[1].RemoveAt(i);
                varMatrix[0].Insert(0, s);
                varMatrix[1].Insert(0, t);
            }

            return varMatrix;
        }

        public static bool validateConfigFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                // try to open the xml file
                XDocument configFile = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);

                if (configFile.Elements("TDSettings").FirstOrDefault() != null)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private bool containsPadSizes(string text)
        {
            // we only want one of each from the key
            if (!String.IsNullOrEmpty(text))
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
            if (!String.IsNullOrEmpty(input.ToUpper()))
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
            if (stringInList(input, parentList))
                return true;
            else
                return false;
        }

        private void addMarkedStation(string input, List<string> parentList)
        {
            if (!isMarkedStation(input, parentList) && stringInList(input, outputSysStnNames) 
                && !stringInList(input, parentList))
            {// insert at the top of the list
                parentList.Insert(0, input);
                settingsRef.MarkedStations = serializeMarkedStations(parentList);
            }
        }

        private void removeMarkedStation(string input, List<string> parentList)
        {
            int index = indexInList(input, parentList);

            // it's valid, grab the index
            if (isMarkedStation(input, parentList) && index >= 0)
            {
                parentList.RemoveAt(index);
                settingsRef.MarkedStations = serializeMarkedStations(parentList);
            }
        }

        private static List<string> parseMarkedStations()
        {
            if (!String.IsNullOrEmpty(settingsRef.MarkedStations))
                return removeExtraWhitespace(settingsRef.MarkedStations).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            else
                return new List<string>();
        }

        private string serializeMarkedStations(List<string> parentList)
        {
            if (parentList.Count > 0)
            {
                string builtString = String.Join(",", parentList);
                return builtString;
            }
            else
                return "";
        }
    }
}