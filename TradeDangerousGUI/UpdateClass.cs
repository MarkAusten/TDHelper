using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TDHelper
{
    public class Downloader : WebClient
    {
        public Downloader() : this(10000)
        {
        }

        public Downloader(int timeout)
        {
            this.Timeout = timeout;
        }

        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);

            if (request != null)
            {
                request.Timeout = this.Timeout;
            }

            return request;
        }
    }

    public class UpdateClass
    {
        public static string CalculateMD5(string filePath)
        {
            // calculate an md5 string
            using (var md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    // output a valid lowercase md5sum
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }

        public static bool CompareAssemblyToManifest(string manifest, string path)
        {
            /*
             * This method compares the md5sum of an assembly in a manifest to
             * a local assembly residing inside the path.
             */

            if (File.Exists(manifest))
            {
                XDocument doc = XDocument.Load(manifest);
                XElement root = doc.Element("Manifest").Element("Assembly");
                string manifestAssemblyName = root.Attribute("Name").Value;
                string localAssemblyPath = Path.Combine(path, manifestAssemblyName);

                if (!string.IsNullOrEmpty(manifestAssemblyName) && File.Exists(localAssemblyPath))
                {
                    // resolve the local path to the assembly mentioned in the manifest
                    string manifestAssemblyHash = root.Element("MD5").Value;
                    string localAssemblyHash = CalculateMD5(localAssemblyPath);

                    return (!string.IsNullOrEmpty(manifestAssemblyHash) && localAssemblyHash.Equals(manifestAssemblyHash));
                }
                else
                {
                    WriteToLog(MainForm.updateLogPath, "The assembly mentioned in the manifest cannot be found: " + manifestAssemblyName);

                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool CompareFileHashes(string path1, string path2)
        {
            bool matchFound = false;

            // take two file paths, spit out true/false if their hashes match
            if (File.Exists(path1) && File.Exists(path2))
            {
                string firstHash = CalculateMD5(path1);
                string secondHash = CalculateMD5(path2);

                matchFound = firstHash.Equals(secondHash);
            }

            return matchFound;
        }

        /// <summary>
        /// Compare the version number found in the manifest with that of the assembly.
        /// </summary>
        /// <param name="manifest"></param>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public static bool CompareVersionNumbers(
            string manifest, 
            string assemblyPath)
        {
            bool manifestVersionIsGreater = false;

            if (File.Exists(manifest))
            {
                XDocument doc = XDocument.Load(manifest);
                XElement root = doc.Element("Manifest").Element("Assembly");
                string manifestAssemblyName = root.Attribute("Name").Value;
                string localAssemblyPath = Path.Combine(assemblyPath, manifestAssemblyName);

                if (!string.IsNullOrEmpty(manifestAssemblyName) && File.Exists(localAssemblyPath))
                {
                    // resolve the local path to the assembly mentioned in the manifest
                    string manifestAssemblyVersion = root.Element("Version").Value;
                    string localAssemblyVersion = GetFileVersion(localAssemblyPath);

                    int manifestVersion = ConvertVersion(manifestAssemblyVersion);
                    int assemblyVersion = ConvertVersion(localAssemblyVersion);

                    manifestVersionIsGreater = manifestVersion > assemblyVersion;
                }
                else
                {
                    WriteToLog(MainForm.updateLogPath, "The assembly mentioned in the manifest cannot be found: " + manifestAssemblyName);
                }
            }

            return manifestVersionIsGreater;
        }

        /// <summary>
        /// Convert the string version number to an int suitable for copmaring.
        /// </summary>
        /// <param name="version">The version to convert.</param>
        /// <returns>The version number as an int.</returns>
        public static int ConvertVersion(string version)
        {
            string newVersion = string.Empty;

            string[] data = version.Split(new string[] { "." }, StringSplitOptions.None);

            for (int i = 0; i < 4; ++i)
            {
                string part = data.Length >= i + 1 ? data[i] : "0";

                newVersion += part.PadLeft(3).Replace(" ", "0");
            }

            if (!int.TryParse(newVersion, out int versionNumber))
            {
                versionNumber = 0;
            }

            return versionNumber;
        }

        public static void DecompressFile(string zipFile, string fileInZip, string outputFile)
        {
            // decompress a file in a zip to a directory, overwriting it if it exists
            try
            {
                if (File.Exists(zipFile))
                {
                    byte[] zipEntry = ReadFileInZip(zipFile, fileInZip);

                    File.WriteAllBytes(outputFile, zipEntry);
                }
            }
            catch (Exception e)
            {
                WriteToLog(MainForm.updateLogPath, "Exception: " + e.Message);
            }
        }

        public static void DecompressZip(string inputFile, string outputPath)
        {
            // decompress all files in a zip to a directory
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(inputFile))
                {
                    // overwrite all destination files to avoid errors
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        entry.ExtractToFile(Path.Combine(outputPath, entry.FullName), true);
                    }
                }
            }
            catch (IOException e)
            {
                WriteToLog(MainForm.updateLogPath, "IOException: " + e.Message);
            }
            catch (Exception e)
            {
                WriteToLog(MainForm.updateLogPath, "Exception: " + e.Message);
            }
        }

        public static bool DownloadFile(string url, string outputPath)
        {
            // generic file downloader with a timeout
            bool result = false;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                using (Downloader client = new Downloader())
                {
                    client.DownloadFile(url, outputPath);
                }

                result = true;
            }
            catch (TimeoutException)
            {
                DialogResult d = TopMostMessageBox.Show(
                    true,
                    true,
                    "HTTP download request timed out, retry?",
                    "TD Helper - Error",
                    MessageBoxButtons.YesNo);

                if (d == DialogResult.Yes)
                {
                    result = DownloadFile(url, outputPath); // retry
                }
            }
            catch (Exception e)
            {
                WriteToLog(MainForm.updateLogPath, e.Message + " [URL: " + url + "] " + (!string.IsNullOrEmpty(e.InnerException.Message) ? e.InnerException.Message : string.Empty));
            }

            return result;
        }

        public static void GenerateManifest(string workingPath, string manifest, string URL)
        {
            /*
             * Take a set of files, calculate data for each (assembly first),
             * print the list to a manifest in XML in the working directory.
             */

            XDocument doc = new XDocument(new XElement("Manifest"));
            XElement root = doc.Element("Manifest");

            try
            {
                // let's make a proper manifest xml from the list of files
                if (Directory.Exists(workingPath) && Directory.GetFiles(workingPath).Length > 0)
                {
                    // only the non-debugging assembly
                    string[] fileList = {
                        "TDHelper.exe",
                        "System.Data.SQLite.dll",
                        "Newtonsoft.Json.dll",
                        "SharpConfig.dll" };

                    // put the assembly info first
                    if (!string.IsNullOrEmpty(fileList[0]))
                    {
                        string assemblyVersion = GetFileVersion(fileList[0]);
                        string assemblyMD5 = CalculateMD5(fileList[0]);
                        string assemblyExeName = Path.GetFileName(fileList[0]);

                        root.Add(new XElement("Assembly", new XAttribute("Name", assemblyExeName), new XElement("Version", assemblyVersion), new XElement("MD5", assemblyMD5)));

                        if (!string.IsNullOrEmpty(URL))
                        {
                            XElement el = root.Element("Assembly");
                            el.Add(new XElement("URL", URL));
                        }
                        else
                        {
                            WriteToLog(MainForm.updateLogPath, "Possibly invalid manifest file, can't find URL tag in Assembly");
                        }

                        // Now the remaining files.
                        for (int i = 1; i < fileList.Length; ++i)
                        {
                            string fileMD5 = CalculateMD5(fileList[i]);
                            string fileName = Path.GetFileName(fileList[i]);

                            root.Add(new XElement("Name", new XAttribute("Value", fileName), new XElement("MD5", fileMD5)));
                        }

                        doc.Save(manifest);
                    }
                    else
                    {
                        WriteToLog(MainForm.updateLogPath, "Cannot find an assembly in the working path: " + workingPath);
                    }
                }
                else
                {
                    DialogResult d = TopMostMessageBox.Show(
                        true,
                        true,
                        "The manifest input directory does not contain any files, or cannot be created.\r\nPlease create the following directory and then confirm: " + workingPath,
                        "TD Helper - Error",
                        MessageBoxButtons.OKCancel);

                    if (d == DialogResult.OK)
                    {
                        GenerateManifest(workingPath, manifest, URL);
                    }
                }
            }
            catch (Exception e)
            {
                WriteToLog(MainForm.updateLogPath, e.Message);
            }
        }

        public static string GetFileVersion(string filePath)
        {
            string version = string.Empty;

            // return the file version of a given assembly
            if (File.Exists(filePath))
            {
                version =  AssemblyName.GetAssemblyName(filePath).Version.ToString();
            }

            return version;
        }

        public static bool IsValidURLArchive(string URI)
        {
            if (Uri.TryCreate(URI, UriKind.Absolute, out Uri validURI)
                && (validURI.Scheme == Uri.UriSchemeHttp || validURI.Scheme == Uri.UriSchemeHttps))
            {
                return URI.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        public static string ManifestAssemblyInfo(string manifest, string key)
        {
            try
            {
                if (File.Exists(manifest))
                {
                    XDocument doc = XDocument.Load(manifest);
                    XElement root = doc.Element("Manifest").Element("Assembly").Element(key);

                    if (root != null)
                    {
                        // it's an element
                        string value = root.Value;
                        return (!string.IsNullOrEmpty(value)) ? value : string.Empty;
                    }
                    else
                    {
                        // it's an attribute
                        // element doesn't exist, try an attribute?
                        XAttribute rootAttr = doc.Element("Manifest").Element("Assembly").Attribute(key);
                        if (rootAttr != null)
                        {
                            string value = rootAttr.Value;
                            return (!string.IsNullOrEmpty(value)) ? value : string.Empty;
                        }
                        else
                        {
                            return string.Empty; // couldn't find it
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException("Cannot find or open the manifest at path: " + manifest);
                }
            }
            catch (Exception e)
            {
                WriteToLog(MainForm.updateLogPath, "Exception: " + e.Message);
                return string.Empty;
            }
        }

        public static string ManifestAssemblyVersion(string manifest)
        {
            try
            {
                if (File.Exists(manifest))
                {
                    XDocument doc = XDocument.Load(manifest);
                    XElement root = doc.Element("Assembly");

                    if (root != null)
                    {
                        string rootAttr = root.Attribute("Name").Value;
                        return (!string.IsNullOrEmpty(rootAttr)) ? rootAttr : "";
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    throw new FileNotFoundException("Cannot find or open the manifest at path: " + manifest);
                }
            }
            catch (Exception e)
            {
                WriteToLog(MainForm.updateLogPath, "Exception: " + e.Message);

                return string.Empty;
            }
        }

        public static List<string> ManifestFileList(string manifest)
        {
            List<string> output = new List<string>
            {
                ManifestAssemblyInfo(manifest, "Name")
            };

            XDocument doc = XDocument.Load(manifest);
            var roots = doc.Element("Manifest").Elements("Name").Attributes("Value");

            foreach (XAttribute n in roots)
            {
                output.Add(n.Value);
            }

            return output;
        }

        public static byte[] ReadFileInZip(string zipFile, string fileToRead)
        {
            // takes a zip file, reads a specific file inside it, outputs a byte array
            try
            {
                using (ZipArchive archive = ZipFile.Open(zipFile, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry entry = archive.GetEntry(fileToRead);

                    using (Stream stream = entry.Open())
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                WriteToLog(MainForm.updateLogPath, "Exception: " + e.Message);
                return null;
            }
        }

        public static bool ValidateManifest(string manifest)
        {
            // check if the manifest is valid
            if (File.Exists(manifest))
            {
                XDocument doc = XDocument.Load(manifest);
                XElement root = doc.Descendants("Assembly").FirstOrDefault();

                // assembly exists, file is probably okay
                return (root != null);
            }
            else
            {
                return false;
            }
        }

        public static void WriteToLog(string logPath, string message)
        {
            DateTime currentTime = DateTime.Now.ToLocalTime();
            FileInfo fileRef = new FileInfo(logPath);
            string outputString = string.Format("[{0}] {1}\r\n", currentTime, message);

            // make sure the log doesn't get too big (<1mb)
            if (fileRef.Exists && fileRef.Length > 1048576)
            {
                File.Delete(logPath);
            }

            // if it exists--we append, if it doesn't--we create
            using (FileStream fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter stream = new StreamWriter(fs))
            {
                stream.Write(outputString);
            }
        }
    }
}