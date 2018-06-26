using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

namespace TDHelper
{
    public partial class Form1 : Form
    {
        #region Response
        class EDSCRecents
        {
            public string timestamp { get; set; }
            public string response { get; set; }
        }
        #endregion

        #region Output
        [DataContract]
        class Wrapper
        {
            [DataMember(Name="data")]
            public EDSCData EDSCData { get; set; }
        }

        [DataContract]
        class origin
        {
            [DataMember]
            public string name { get; set; }
        }

        [DataContract]
        class reference
        {
            [DataMember(Order = 0, IsRequired=true)]
            public string name { get; set; }
            [DataMember(Order = 1, IsRequired=true)]
            public double dist { get; set; }
        }

        [DataContract]
        class coordsphere
        {
            [DataMember(Order = 0, IsRequired = true)]
            public double radius { get; set; }
            [DataMember(Order = 1, IsRequired = true)]
            public int[] origin { get; set; }
        }

        [DataContract]
        class filter
        {
            [DataMember(Order = 0, IsRequired = false, EmitDefaultValue = false)]
            public int knownstatus { get; set; }
            [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
            public string systemname { get; set; }
            [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
            public int cr { get; set; }
            [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
            public string date { get; set; }
            [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
            public int[,] coordcube { get; set; }
            [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
            public coordsphere coordsphere { get; set; }
        }

        [DataContract]
        class EDSCData
        {
            [DataMember(Order = 0, IsRequired=true)]
            public int ver { get; set; }
            [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
            public bool test { get; set; }
            [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
            public int outputmode { get; set; }
            [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
            public string commander { get; set; }
            [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
            public origin p0 { get; set; }
            [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
            public List<reference> refs { get; set; }
            [DataMember(Order = 6, IsRequired = false, EmitDefaultValue = false)]
            public filter filter { get; set; }
        }
        #endregion

        public void SubmitSystem(string input, string refsys1, double refdist1, string refsys2, double refdist2,
            string refsys3, double refdist3, string refsys4, double refdist4, string refsys5, double refdist5, string cmdrName)
        {// here we take our enums and make valid JSON for submission
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Wrapper));
            MemoryStream stream = new MemoryStream();
            string postURL = "http://edstarcoordinator.com/api.asmx/SubmitDistances";

            reference ref1 = new reference() { name = refsys1, dist = refdist1 };
            reference ref2 = new reference() { name = refsys2, dist = refdist2 };
            reference ref3 = new reference() { name = refsys3, dist = refdist3 };
            reference ref4 = new reference() { name = refsys4, dist = refdist4 };
            reference ref5 = new reference() { name = refsys5, dist = refdist5 };
            List<reference> refList = new List<reference>() { ref1, ref2, ref3, ref4, ref5 };

            Wrapper objRef = new Wrapper
            {
                EDSCData = new EDSCData
                {
                    ver = 2,
                    commander = cmdrName,
                    p0 = new origin
                    {
                        name = input
                    },
                    refs = refList,
                }
            };

            // copy to the memorystream, then convert to a string format
            serializer.WriteObject(stream, objRef);
            string convertedObject = Encoding.UTF8.GetString(stream.ToArray());
            Debug.WriteLine("Input: " + convertedObject);

            try
            {
                string result = "";
                using (WebClient request = new WebClient())
                {
                    request.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    result = request.UploadString(postURL, "POST", convertedObject);
                    Debug.WriteLine("Response: " + result);

                    this.Invoke(new Action(() =>
                    {// do this on the UI thread
                        td_outputBox.Text = prettifyResult(result);
                    }));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public bool GetSystems(string input, int credit)
        {// here we take our enums and make valid JSON for submission
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Wrapper));
            MemoryStream stream = new MemoryStream();
            string postURL = "http://edstarcoordinator.com/api.asmx/GetSystems";
            int t_credit = -1;
            Wrapper objRef = new Wrapper();

            if (credit > 0 && credit <= 20)
                t_credit = credit;
            else
                t_credit = 1;

            if (stationIndex == 1 && !String.IsNullOrEmpty(input))
            {// lookup
                objRef = new Wrapper
                {
                    EDSCData = new EDSCData
                    {
                        ver = 2,
                        outputmode = 2,
                        filter = new filter
                        {
                            knownstatus = 0,
                            systemname = input,
                            cr = t_credit,
                            date = "2013-01-01"
                        }
                    }
                };
            }
            else if (stationIndex == 2)
            {// recent (from up to 10 days ago)
                string dateNow = String.Format("{0:yyyy-MM-dd}", DateTime.UtcNow.AddDays(-10));
                objRef = new Wrapper
                {
                    EDSCData = new EDSCData
                    {
                        ver = 2,
                        outputmode = 2,
                        filter = new filter
                        {
                            knownstatus = 0,
                            cr = t_credit,
                            date = dateNow
                        }
                    }
                };
            }
            else
                return false;

            // copy to the memorystream, then convert to a string format
            serializer.WriteObject(stream, objRef);
            string convertedObject = Encoding.UTF8.GetString(stream.ToArray());
            Debug.WriteLine("Input: " + convertedObject);

            try
            {
                string result = "";
                using (WebClient request = new WebClient())
                {
                    request.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    result = request.UploadString(postURL, "POST", convertedObject);
                    Debug.WriteLine("Response: " + result);

                    this.Invoke((MethodInvoker)delegate
                    {// do this on the UI thread
                        td_outputBox.Text = prettifyResult(result);
                    });
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return false;
        }

        private string prettifyResult(string input)
        {// here we try to deserialize and prettify our result
            if (!String.IsNullOrEmpty(input))
            {
                List<EDSCRecents> systems = new List<EDSCRecents>();
                StringBuilder t_result = new StringBuilder();

                // mode1 expressions
                string msgExpr1 = @"msg"":""(.+?)""";
                string sysIdsExpr = @"id"":(\d+),";
                string sysNamesExpr = @"name"":""(.+?)""";
                string coordsExpr = @"coord"":\[(.+?),(.+?),(.+?)\]";
                string creditsExpr = @"cr"":(\d+?),";
                string cmdrCreatesExpr = @"commandercreate"":""(.*?)"",";
                string createDatesExpr = @"createdate"":""(.*?)"",";
                string cmdsUpdatesExpr = @"commanderupdate"":""(.*?)"",";
                string updateDatesExpr = @"updatedate"":""(.*?)""";
                // mode2 expressions
                string msgExpr2 = @".+?msg"":""((?!Success).+?)""";
                string originSysExpr = @"system1"":""(.+?)""";
                string systemsExpr = @"system2"":""(.+?)""";
                string distExpr = @",""dist"":((?!\[).+?)\}";
                
                // lookup/recent mode matches
                Match msgMatch1 = Regex.Match(input, msgExpr1, RegexOptions.Compiled);
                MatchCollection sysIdsMatch = Regex.Matches(input, sysIdsExpr, RegexOptions.Compiled);
                MatchCollection sysNames = Regex.Matches(input, sysNamesExpr, RegexOptions.Compiled);
                MatchCollection coords = Regex.Matches(input, coordsExpr, RegexOptions.Compiled);
                MatchCollection credits = Regex.Matches(input, creditsExpr, RegexOptions.Compiled);
                MatchCollection cmdrCreates = Regex.Matches(input, cmdrCreatesExpr, RegexOptions.Compiled);
                MatchCollection createDates = Regex.Matches(input, createDatesExpr, RegexOptions.Compiled);
                MatchCollection cmdrUpdates = Regex.Matches(input, cmdsUpdatesExpr, RegexOptions.Compiled);
                MatchCollection updateDates = Regex.Matches(input, updateDatesExpr, RegexOptions.Compiled);

                // submit mode matches
                Match originSystem = Regex.Match(input, originSysExpr, RegexOptions.Compiled);
                MatchCollection msgMatch2 = Regex.Matches(input, msgExpr2, RegexOptions.Compiled);
                MatchCollection trilatSystems = Regex.Matches(input, systemsExpr, RegexOptions.Compiled);
                MatchCollection dists = Regex.Matches(input, distExpr, RegexOptions.Compiled);

                // assemble our output in pretty format
                if (sysIdsMatch.Count > 0)
                {// Lookup/Recents mode
                    t_result.Append("Status: " + msgMatch1.Groups[1].Value + "\r\n");
                    if (sysIdsMatch.Count > 0)
                    {
                        for (int i = 0; i < sysNames.Count; i++)
                        {
                            double pos_x = double.Parse(coords[i].Groups[1].Value), pos_y = double.Parse(coords[i].Groups[2].Value), pos_z = double.Parse(coords[i].Groups[3].Value);

                            t_result.Clear();
                            t_result.Append("ID: " + sysIdsMatch[i].Groups[1].Value + " / CR: " + credits[i].Groups[1].Value);
                            t_result.Append("\r\nSystem: " + sysNames[i].Groups[1].Value);
                            t_result.Append("\r\nCoords: " + pos_x.ToString("0.0####") + "," + pos_y.ToString("0.0####") + "," + pos_z.ToString("0.0####"));
                            t_result.Append("\r\nCreated By: " + cmdrCreates[i].Groups[1].Value + " On: " + createDates[i].Groups[1].Value);
                            t_result.Append("\r\nUpdated By: " + cmdrUpdates[i].Groups[1].Value + " On: " + updateDates[i].Groups[1].Value);
                            // add a TD formatted system line for the user to insert into the System.csv
                            t_result.Append("\r\n\'" + sysNames[i].Groups[1].Value.ToUpper() + "\',"
                                + pos_x.ToString("0.0####") + "," + pos_y.ToString("0.0####") + "," + pos_z.ToString("0.0####") + ",\'Release 1.00-EDStar\',\'" + updateDates[i].Groups[1].Value + "\'\r\n\r\n");

                            // add our data from our response class template
                            systems.Add(new EDSCRecents { timestamp = updateDates[i].Groups[1].Value, response = t_result.ToString() });
                        }
                    }

                    // sort by our timestamp
                    systems = systems.OrderByDescending(x => x.timestamp).Select(y => y).ToList();
                    t_result.Clear();

                    // rebuild results back into t_result
                    foreach (EDSCRecents r in systems)
                    {
                        t_result.Append(r.response);
                    }
                }
                else if (msgMatch2.Count > 0)
                {// Submit mode
                    t_result.Append("Origin: " + originSystem.Groups[1].Value + "\r\n");
                    for (int i = 0; i < trilatSystems.Count; i++)
                    {
                        t_result.Append("\r\nStatus: " + msgMatch2[i].Groups[1].Value);
                        t_result.Append("\r\nSystem: " + trilatSystems[i].Groups[1].Value);
                        t_result.Append("\r\nDist: " + dists[i].Groups[1].Value + "\r\n");
                    }
                }
                else
                {
                    t_result.Append("Status: Possible failure, didn't receive a valid or populated response.\r\nResponse follows:\r\n\r\n");
                    t_result.Append(input);
                }

                return t_result.ToString();
            }
            else
                return "";
        }

        private void sortSystemCSV(string path)
        {// read our CSV all at once, then sort alphabetically
            string header = "";
            List<string> lines = new List<string>(), sortedLines = new List<string>();

            if (checkIfFileOpens(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader stream = new StreamReader(fs))
                {
                    string line;
                    if (!stream.EndOfStream) { header = stream.ReadLine(); }

                    while ((line = stream.ReadLine()) != null)
                    {
                        if (!String.IsNullOrEmpty(line)) { lines.Add(line); }
                    }

                    // now we sort by the system name, and remove duplicates
                    sortedLines = lines.OrderBy(x => x.Split(',')[0].Replace("\'","")).Distinct().ToList();

                    try
                    {
                        using (FileStream fs2 = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                        using (StreamWriter writer = new StreamWriter(fs2))
                        {// always overwrite in unix format
                            writer.Write(header + "\n");
                            foreach (String s in sortedLines)
                            {
                                writer.Write(s + "\n");
                            }
                        }
                    }
                    catch { throw; }
                }
            }
        }

        public static bool validateSystemCSVLine(string input)
        {// takes a string and validates it compared to the System.csv pattern
            string csvExpr = "^\'(.+?)\',(.+?),(.+?),(.+?),\'(.+?)\',\'(.+?)\'$";
            MatchCollection csvMatches = Regex.Matches(input, csvExpr, RegexOptions.Compiled);

            // make sure each field in this line matches our basic pattern
            if (csvMatches.Count > 0 && csvMatches[0].Groups.Count == 7)
                return true;
            else
                return false;
        }
    }
}
