using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace TDHelper
{
    public partial class MainForm : Form
    {
        #region Props

        public Cache memoryCache;
        private List<string> box_outputNames = new List<string>();
        private DataTable localTable = new DataTable();
        private DataTable nonstn_table = new DataTable();
        private List<string> outputStationDetails = new List<string>();
        private List<string> outputStationShips = new List<string>();

        // Output
        private List<string> outputSysStnNames = new List<string>();

        private DataRetriever retriever;
        private DataTable ship_table = new DataTable();

        // Inputs
        private DataTable stn_table = new DataTable();

        private DataTable stnship_table = new DataTable();
        private string tdDBPath = "";
        private SQLiteConnection tdhDBConn;

        #endregion Props

        public static List<string> CollectLogPaths(string path, string pattern)
        {
            // only collect log paths that contain system names
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(path);
                SortedList<string, string> logPaths = new SortedList<string, string>();

                // check the make sure the directory is populated
                if (dInfo.Exists && dInfo.GetFiles().Length > 0)
                {
                    if (dInfo.GetFiles(pattern).OrderBy(f => f.LastWriteTime).FirstOrDefault() != null)
                    {
                        foreach (FileInfo f in dInfo.GetFiles(pattern).OrderBy(f => f.LastWriteTime))
                        {
                            string filePath = Path.Combine(path, f.ToString());
                            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            using (BufferedStream bs = new BufferedStream(fs))
                            using (StreamReader stream = new StreamReader(bs, Encoding.UTF8, true, 65536))
                            {
                                // check if there are any files that match the mask, return null if nothing matches
                                string tempLines = stream.ReadToEnd(); // pull into memory
                                Match timestampMatch = Regex.Match(tempLines, @"(\d{2,4}\-\d\d\-\d\d)[\s\-](\d\d:\d\d)\sGMT");
                                Match systemMatch = Regex.Match(tempLines, @"\{(\d\d:\d\d:\d\d).+System:""(.+)""");

                                if (systemMatch.Success && timestampMatch.Success)
                                {
                                    string timestampHeader = timestampMatch.Groups[1].Value.ToString() + " " + systemMatch.Groups[1].Value.ToString();
                                    logPaths.Add(timestampHeader, filePath);
                                }
                            }
                        }

                        return new List<string>(logPaths.Values);
                    }
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (DirectoryNotFoundException) { }
            catch (Exception e)
            {
                if (e is FileNotFoundException || e is DirectoryNotFoundException)
                {
                    return null; // prevent explosion
                }
            }

            return null; // should never reach this
        }

        private bool AddAtTimestampDBRow(SQLiteConnection dbConn, string timestamp)
        {
            // add a blank row with the timestamp from a given row, basically an insert-below-index during select()
            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO SystemLog VALUES (null,@Timestamp,null,null)", dbConn))
            {
                DateTime tempTimestamp = new DateTime();
                if (!string.IsNullOrEmpty(timestamp)
                    && DateTime.TryParseExact(timestamp, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempTimestamp))
                {
                    string var1 = timestamp; // we were successful in parsing the timestamp, probably safe

                    try
                    {
                        using (var transaction = dbConn.BeginTransaction())
                        {
                            cmd.Parameters.AddWithValue("@Timestamp", var1);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            transaction.Commit();
                        }
                    }
                    catch { throw; }

                    return true; // success!
                }
            }

            return false;
        }

        private bool AddDBRow(SQLiteConnection dbConn)
        {
            // add a blank row with the current timestamp
            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO SystemLog VALUES (null,@Timestamp,null,null)", dbConn))
            {
                try
                {
                    string var1 = CurrentTimestamp();
                    using (var transaction = dbConn.BeginTransaction())
                    {
                        cmd.Parameters.AddWithValue("@Timestamp", var1);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        transaction.Commit();
                    }
                }
                catch { throw; }

                LoadPilotsLogDB(tdhDBConn); // need a full refresh
                return true; // success!
            }
        }

        private void BuildOutput(bool refreshMethod)
        {
            /* NOTE: This method should ALWAYS be called from the UI thread.
             *
             * The intent here is for all the generator methods to feed into a single
             * refreshable list instead of building straight to the combobox. This
             * should be faster and cleaner.
             */

            if (!dropdownOpened && Monitor.TryEnter(readNetLock))
            {
                try
                { // discard the callback if we're busy
                    // for forced refresh or starting up
                    if (buttonCaller == 16 || !hasRun)
                    {
                        LoadDatabase();

                        this.Invoke(new Action(() =>
                        {
                            RefreshItems();
                        }));

                        hasLogLoaded = false;
                        loadedFromDB = false;
                    }

                    currentMarkedStations = ParseMarkedStations();

                    Stopwatch m_timer = Stopwatch.StartNew();

                    // let's rebind with fresh inputs
                    if (hasRun && refreshMethod)
                    {
                        // a full clear() and refresh of comboboxes
                        BuildPilotsLog(); // always before loading the recents
                        ReadNetLog(true); // load all logs

                        this.Invoke(new Action(() =>
                        {
                            // bind the recent systems/stations first
                            srcSystemComboBox.Items.Clear();
                            srcSystemComboBox.Items.Add("");
                            destSystemComboBox.Items.Clear();
                            destSystemComboBox.Items.Add("");

                            // we should add an indicator to every entry in our favorites
                            if (currentMarkedStations.Count > 0)
                                srcSystemComboBox.Items.AddRange(currentMarkedStations.Select(x => "!" + x).ToArray());
                            if (currentMarkedStations.Count > 0)
                                destSystemComboBox.Items.AddRange(currentMarkedStations.Select(x => "!" + x).ToArray());

                            srcSystemComboBox.Items.AddRange(output_unclean.ToArray());
                            destSystemComboBox.Items.AddRange(output_unclean.ToArray());

                            if (buttonCaller == 16)
                            {
                                // we're marked as needing a database refresh
                                // bind the database output for autocompletion
                                srcSystemComboBox.AutoCompleteCustomSource.Clear();
                                srcSystemComboBox.AutoCompleteCustomSource.AddRange(outputSysStnNames.ToArray());
                                commodityComboBox.SelectedIndex = 0;
                                destSystemComboBox.AutoCompleteCustomSource.Clear();
                                destSystemComboBox.AutoCompleteCustomSource.AddRange(outputSysStnNames.ToArray());
                                commodityComboBox.SelectedIndex = 0;
                            }
                        }));
                    }
                    else if (hasRun && !refreshMethod && output_unclean.Count > 0)
                    {
                        // non-destructively update with the newest system
                        ReadNetLog(false);

                        if (hasRefreshedRecents)
                        {
                            // only call if we have an update
                            this.Invoke(new Action(() =>
                            {
                                // rebind our dropdowns
                                srcSystemComboBox.Items.Clear();
                                destSystemComboBox.Items.Clear();
                                srcSystemComboBox.Items.Add(""); // add a blank entry to the top
                                destSystemComboBox.Items.Add("");

                                // favorites first
                                if (currentMarkedStations.Count > 0)
                                    srcSystemComboBox.Items.AddRange(currentMarkedStations.Select(x => "!" + x).ToArray());
                                if (currentMarkedStations.Count > 0)
                                    destSystemComboBox.Items.AddRange(currentMarkedStations.Select(x => "!" + x).ToArray());

                                // finally the system list
                                srcSystemComboBox.Items.AddRange(output_unclean.ToArray());
                                destSystemComboBox.Items.AddRange(output_unclean.ToArray());

                                // reset our cursor after the refresh if the user had input focus
                                if (srcSystemComboBox.Text.Length > 0)
                                    srcSystemComboBox.Select(srcSystemComboBox.Text.Length, 0);
                                if (destSystemComboBox.Text.Length > 0)
                                    destSystemComboBox.Select(srcSystemComboBox.Text.Length, 0);
                            }));
                            hasRefreshedRecents = false;
                        }
                    }
                    else if (!hasRun)
                    {
                        // we're starting fresh so bind all the things
                        BuildPilotsLog();
                        ReadNetLog(true);

                        this.Invoke(new Action(() =>
                        {
                            srcSystemComboBox.Items.Add("");
                            destSystemComboBox.Items.Add("");

                            if (currentMarkedStations.Count > 0)
                            {
                                srcSystemComboBox.Items.AddRange(currentMarkedStations.Select(x => "!" + x).ToArray());
                                destSystemComboBox.Items.AddRange(currentMarkedStations.Select(x => "!" + x).ToArray());
                            }

                            srcSystemComboBox.Items.AddRange(output_unclean.ToArray());
                            destSystemComboBox.Items.AddRange(output_unclean.ToArray());
                            srcSystemComboBox.AutoCompleteCustomSource.AddRange(outputSysStnNames.ToArray());
                            destSystemComboBox.AutoCompleteCustomSource.AddRange(outputSysStnNames.ToArray());
                        }));
                        hasRun = true; // set the semaphore so we don't hork our data tables
                    }

                    Debug.WriteLine("buildOutput combobox population took: " + m_timer.ElapsedMilliseconds + "ms");
                    m_timer.Stop();
                }
                finally
                {
                    if (hasRun)
                        testSystemsTimer.Start(); // start our background systems list updater (~10s)

                    Monitor.Exit(readNetLock);
                }
            }
        }

        private void BuildPilotsLog()
        {
            // here we either build or load our database
            tdhDBConn = new SQLiteConnection("Data Source=" + pilotsLogDBPath + ";Version=3;");

            if (pilotsLogDataGrid.Rows.Count == 0)
            {
                if (HasValidRows(tdhDBConn, "SystemLog"))
                    InvalidatedRowUpdate(true, -1);
                else
                    CreatePilotsLogDB(tdhDBConn); // make from scratch
            }
            else if (HasValidRows(tdhDBConn, "SystemLog"))
            {
                // load our physical database
                InvalidatedRowUpdate(true, -1);
            }
        }

        private void CreatePilotsLogDB(SQLiteConnection dbConn)
        {
            try
            {
                if (dbConn != null && dbConn.State == ConnectionState.Closed)
                    dbConn.Open();

                using (SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    if (!HasValidColumn(dbConn, "SystemLog", "System"))
                    {
                        // create our table first
                        cmd.CommandText = "CREATE TABLE SystemLog (ID INTEGER PRIMARY KEY AUTOINCREMENT, Timestamp NVARCHAR, System NVARCHAR, Notes NVARCHAR)";
                        cmd.ExecuteNonQuery();

                        // Create a unique index.
                        cmd.CommandText = "CREATE UNIQUE INDEX IF NOT EXISTS SystemTimestamp ON SystemLog (System, Timestamp)";
                        cmd.ExecuteNonQuery();
                    }
                    else if (HasValidRows(dbConn, "SystemLog"))
                    {
                        /*
                                                // wipe before inserting, ensures most recent records
                                                cmd.CommandText = "DELETE FROM SystemLog";
                                                cmd.ExecuteNonQuery();

                                                // clean up after ourselves
                                                cmd.CommandText = "VACUUM";
                                                cmd.ExecuteNonQuery();
                        */
                    }

                    if (netLogOutput.Count > 0)
                    {
                        using (var transaction = dbConn.BeginTransaction())
                        {
                            // always do our inserts in a batch for ideal performance
                            cmd.CommandText = @"INSERT INTO SystemLog (Timestamp, System) VALUES null,@Timestamp,@System,null)";

                            cmd.Parameters.AddWithValue("@Timestamp", string.Empty);
                            cmd.Parameters.AddWithValue("@System", string.Empty);

                            foreach (var s in netLogOutput)
                            {
                                try
                                {
                                    cmd.Parameters["@Timestamp"].Value = s.Key;
                                    cmd.Parameters["@System"].Value = s.Value;

                                    cmd.ExecuteNonQuery();
                                }
                                catch
                                {
                                    // Do nothing.
                                }
                            }
                            transaction.Commit();
                        }
                    }

                    this.Invoke(new Action(() =>
                    {
                        retriever = new DataRetriever(dbConn, "SystemLog");
                        foreach (DataColumn c in retriever.Columns)
                            pilotsLogDataGrid.Columns.Add(c.ColumnName, c.ColumnName);

                        // setup the gridview
                        UpdateLocalTable(tdhDBConn);
                        memoryCache = new Cache(retriever, 24);

                        pilotsLogDataGrid.RowCount = retriever.RowCount;
                        //pilotsLogDataGrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                        pilotsLogDataGrid.Columns["ID"].Visible = false;
                        pilotsLogDataGrid.Columns["Timestamp"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                        pilotsLogDataGrid.Columns["System"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                        pilotsLogDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }));
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        private void FilterDatabase()
        {
            /*
             * This method is only intended to repopulate the sorted/formatted output
             * and should never be called publicly.
             */

            // prevent null
            if (stn_table.Rows.Count > 0)
            {
                // try to minimize how much we iterate the datatable
                for (int i = 0; i < stn_table.Rows.Count; i++)
                {
                    outputSysStnNames.Add(stn_table.Rows[i]["sys_name"].ToString() + "/" + stn_table.Rows[i]["stn_name"].ToString());
                }
            }

            if (nonstn_table.Rows.Count > 0)
            {
                // add our orphaned systems to the end of the list
                for (int i = 0; i < nonstn_table.Rows.Count; i++)
                {
                    outputSysStnNames.Add(nonstn_table.Rows[i]["sys_name"].ToString());
                }
            }

            if (outputSysStnNames.Count == 0)
                Debug.WriteLine("outputSysStnNames is empty, must be a DB path failure or access violation");
        }

        private void FilterStationData()
        {
            // first the station data itself
            if (stnship_table.Rows.Count > 0)
            {
                outputStationDetails.Add(stnship_table.Rows[0]["stn_ls"].ToString()); // 0
                outputStationDetails.Add(stnship_table.Rows[0]["stn_padsize"].ToString()); // 1
                outputStationDetails.Add(stnship_table.Rows[0]["stn_rearm"].ToString()); // 2
                outputStationDetails.Add(stnship_table.Rows[0]["stn_refuel"].ToString()); // 3
                outputStationDetails.Add(stnship_table.Rows[0]["stn_repair"].ToString()); // 4
                outputStationDetails.Add(stnship_table.Rows[0]["stn_outfit"].ToString()); // 5
                outputStationDetails.Add(stnship_table.Rows[0]["stn_ship"].ToString()); // 6
                outputStationDetails.Add(stnship_table.Rows[0]["stn_items"].ToString()); // 7
                outputStationDetails.Add(stnship_table.Rows[0]["stn_bmkt"].ToString()); // 8
            }

            // then the shipvendor data
            if (ship_table.Rows.Count > 0)
            {
                for (int i = 0; i < ship_table.Rows.Count; i++)
                {
                    // save the ship cost, and display in an invariant format
                    string temp_cost = string.Format("{0:#,##0}", decimal.Parse(ship_table.Rows[i]["ship_cost"].ToString()));
                    outputStationShips.Add(ship_table.Rows[i]["ship_name"].ToString() + " [" + temp_cost + "cr]");
                }
            }

            if (outputStationDetails.Count == 0)
                Debug.WriteLine("outputStationDetails is empty, must be a DB path failure or access violation");
        }

        private string GetLastTimestamp()
        {
            string timestamp = string.Empty;

            using (SQLiteCommand query = new SQLiteCommand("SELECT MAX(Timestamp) FROM SystemLog", tdhDBConn))
            {
                try
                {
                    if (tdhDBConn != null && tdhDBConn.State == ConnectionState.Closed)
                    {
                        tdhDBConn.Open();
                    }

                    using (SQLiteDataReader reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            timestamp = reader.GetString(0);
                        }
                    }
                }
                catch { /* eat exceptions */ }
            }

            return timestamp;
        }

        private void GrabStationData(string inputSystem, string inputStation)
        {
            /*
             * We use this method to grab station data on the fly for the station
             * editor panel as a single row
             */

            if (ValidateDBPath())
            {
                try
                {
                    using (SQLiteConnection db = new SQLiteConnection("Data Source=" + tdDBPath + ";Version=3;"))
                    {
                        Stopwatch m_timer = Stopwatch.StartNew();

                        db.Open();

                        if (stnship_table.Rows.Count != 0)
                            stnship_table = new DataTable();

                        if (ship_table.Rows.Count != 0)
                            ship_table = new DataTable();

                        /*
                         * Extract station details into a single row array--the structure is as follows:
                         *
                         * sys_name | stn_name | stn_ls | stn_padsize | stn_rearm | stn_refuel | stn_repair | stn_outfit | stn_ship | stn_items | stn_bmkt
                         *
                         * The contents of each station field can be: 'Y', 'N', '?', 'S', 'M', 'L', or an int64/long
                         */

                        // check for one of the common columns
                        if (HasValidColumn(db, "Station", "rearm"))
                        {
                            SQLiteCommand query = new SQLiteCommand(
                                "SELECT Sys.name AS sys_name, Stn.name AS stn_name, Stn.ls_from_star AS stn_ls, Stn.max_pad_size AS stn_padsize, Stn.rearm AS stn_rearm, Stn.refuel AS stn_refuel, Stn.repair AS stn_repair, Stn.outfitting AS stn_outfit, Stn.shipyard AS stn_ship, Stn.market AS stn_items, Stn.blackmarket AS stn_bmkt FROM System as Sys, Station as Stn WHERE sys_name = \"" + inputSystem + "\" AND stn_name = \"" + inputStation + "\" AND Stn.system_id = Sys.system_id", db);
                            SQLiteDataReader reader = query.ExecuteReader();
                            stnship_table.Load(reader); // should be a single row/multi-column

                            // populate our shipvendor table as well
                            query = new SQLiteCommand("SELECT Sys.system_id AS sys_sysid, Stn.system_id AS stn_sysid, Stn.station_id AS stn_stnid, ShipV.station_id AS shipv_stnid, ShipV.ship_id AS shipv_shipid, Ship.ship_id AS ship_shipid, Sys.name AS sys_name, Stn.name AS stn_name, Ship.name AS ship_name, Ship.cost AS ship_cost FROM System AS Sys, Station AS Stn, ShipVendor AS ShipV, Ship WHERE stn_stnid = shipv_stnid AND shipv_shipid = ship_shipid AND sys_name = \"" + inputSystem + "\" AND stn_name = \"" + inputStation + "\" AND Stn.system_id = Sys.system_id ORDER BY ship_cost DESC", db);
                            reader = query.ExecuteReader();
                            ship_table.Load(reader); // ship data is in "ship_name" column

                            // unnecessary, but still explicit
                            reader.Close();
                            reader.Dispose();
                            db.Close();
                            db.Dispose();

                            outputStationDetails = new List<string>();
                            outputStationShips = new List<string>();
                            FilterStationData();

                            Debug.WriteLine("grabStationData query took: " + m_timer.ElapsedMilliseconds + "ms");
                            m_timer.Stop();
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        private bool HasValidColumn(SQLiteConnection dbConn, string tableName, string columnName)
        {
            // this method returns true if a column exists
            using (SQLiteCommand query = new SQLiteCommand("PRAGMA table_info(" + tableName + ")", dbConn))
            using (DataTable results = new DataTable())
            {
                try
                {
                    if (dbConn != null && dbConn.State == ConnectionState.Closed)
                        dbConn.Open();

                    using (SQLiteDataReader reader = query.ExecuteReader())
                    {
                        results.Load(reader);

                        foreach (DataRow r in results.Rows)
                        {
                            foreach (var i in r.ItemArray)
                            {
                                if (i.ToString() == columnName)
                                    return true;
                            }
                        }
                    }
                }
                catch { /* eat exceptions */ }
            }

            return false;
        }

        private bool HasValidRows(SQLiteConnection dbConn, string tableName)
        {
            // this method returns true if there are any valid rows
            using (SQLiteCommand query = new SQLiteCommand("SELECT COUNT(*) FROM SystemLog", dbConn))
            {
                try
                {
                    if (dbConn != null && dbConn.State == ConnectionState.Closed)
                        dbConn.Open();

                    using (SQLiteDataReader reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rows = reader.GetInt32(0);
                            if (rows > 0)
                                return true;
                        }
                    }
                }
                catch { /* eat exceptions */ }
            }

            return false;
        }

        private bool InvalidatedRowUpdate(bool refreshMode, int rowIndex)
        {
            if (refreshMode)
            {
                // full refresh
                if (rowIndex == -1)
                {
                    LoadPilotsLogDB(tdhDBConn);
                    return true;
                }

                // invalidate the cache pages
                UpdateLocalTable(tdhDBConn);
                retriever = new DataRetriever(tdhDBConn, "SystemLog");
                memoryCache = new Cache(retriever, 24);
                // force a refresh/repaint
                this.Invoke(new Action(() =>
                {
                    pilotsLogDataGrid.RowCount = retriever.RowCount;
                    pilotsLogDataGrid.Refresh();
                }));
            }
            else
            {
                // partial refresh
                UpdateLocalTable(tdhDBConn);
                retriever = new DataRetriever(tdhDBConn, "SystemLog");
                memoryCache = new Cache(retriever, 24);
                this.Invoke(new Action(() =>
                {
                    pilotsLogDataGrid.RowCount = retriever.RowCount;
                    pilotsLogDataGrid.InvalidateRow(rowIndex);
                }));
            }

            return false;
        }

        private void LoadDatabase()
        {
            /*
             * This method should grab systems and stations to an array and then
             * form a new array in the form of: SystemName/StationName for each
             * station in a given system.
             */

            if (ValidateDBPath())
            {
                try
                {
                    using (SQLiteConnection db = new SQLiteConnection("Data Source=" + tdDBPath + ";Version=3;"))
                    {
                        Stopwatch m_timer = Stopwatch.StartNew();

                        db.Open();

                        // wipe to prevent duplicates
                        if (stn_table.Rows.Count != 0)
                        {
                            stn_table = new DataTable(); // this is O(1) performant
                            nonstn_table = new DataTable(); // this is O(1) performant
                        }

                        // match on System.system_id = Station.system_id, output in "System | Station" format
                        SQLiteCommand query = new SQLiteCommand("SELECT A.name AS sys_name, B.name AS stn_name FROM System AS A, Station AS B WHERE A.system_id = B.system_id ORDER BY A.system_id", db);
                        SQLiteDataReader reader = query.ExecuteReader();
                        stn_table.Load(reader); // pre-sorted by matches/sys_id/alphabetical

                        // match on System.system_id != Station.system_id, output in "System" format
                        query = new SQLiteCommand("SELECT A.name AS sys_name FROM System AS A WHERE A.system_id NOT IN (SELECT B.system_id FROM Station AS B)", db);
                        reader = query.ExecuteReader();
                        nonstn_table.Load(reader); // pre-sorted by matches/sys_id/alphabetical

                        // unnecessary, but still explicit
                        reader.Close();
                        reader.Dispose();
                        db.Close();
                        db.Dispose();

                        Debug.WriteLine("loadDatabase query took: " + m_timer.ElapsedMilliseconds + "ms");
                        m_timer.Stop();
                    }

                    outputSysStnNames = new List<string>(); // wipe before we fill
                    FilterDatabase(); // filter and fill our output
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        private void LoadPilotsLogDB(SQLiteConnection dbConn)
        {
            if (HasValidColumn(dbConn, "SystemLog", "System"))
            {
                try
                {
                    UpdateLocalTable(tdhDBConn);
                    retriever = new DataRetriever(dbConn, "SystemLog");

                    this.Invoke(new Action(() =>
                    {
                        if (pilotsLogDataGrid.Columns.Count != localTable.Columns.Count)
                        {
                            foreach (DataColumn c in retriever.Columns)
                                pilotsLogDataGrid.Columns.Add(c.ColumnName, c.ColumnName);
                        }

                        pilotsLogDataGrid.Rows.Clear();
                        memoryCache = new Cache(retriever, 24);
                        pilotsLogDataGrid.Refresh();

                        pilotsLogDataGrid.RowCount = retriever.RowCount;
                        if (pilotsLogDataGrid.RowCount > 0)
                        {
                            pilotsLogDataGrid.Columns["ID"].Visible = false;
                            pilotsLogDataGrid.Columns["Timestamp"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                            pilotsLogDataGrid.Columns["System"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                            pilotsLogDataGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }));
                }
                catch (SQLiteException) { throw; }
            }
        }

        private List<string> LoadSystemsFromDB(List<string> logPaths)
        {
            // we should load from the DB if the newest entry is newer than the newest log file entry
            string pattern0 = @"^(\d\d\-\d\d\-\d\d).+?\((.+?)\sGMT"; // $1=Y, $2=M, $3=D; $4=GMT
            // grab the timestamp of this entry, and then the system name
            string pattern1 = @"\{(.*?)\}\sSystem.+?\((.*?)\)"; // $1=localtime, $2=system
            List<string> output = new List<string>(); // a list for our system names
            string logDatestamp = "";

            if (logPaths.Count > 0 && !string.IsNullOrEmpty(logPaths.Last())
                && !hasLogLoaded && !loadedFromDB && localSystemList.Count > 0)
            {
                // only process the very newest logfile if the database also exists
                using (FileStream fs = new FileStream(logPaths.Last(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader stream = new StreamReader(bs, Encoding.UTF8, true, 65536))
                {
                    string fileBuffer = stream.ReadToEnd();
                    Match match0 = Regex.Match(fileBuffer, pattern0, RegexOptions.Compiled);
                    Match match1 = Regex.Match(fileBuffer, pattern1, RegexOptions.Compiled);
                    // pull some variables for comparison
                    string firstTimestamp = "", firstSystem = "";

                    if (!string.IsNullOrEmpty(match0.Groups[0].Value)
                        && !string.IsNullOrEmpty(match1.Groups[1].Value)
                        && !string.IsNullOrEmpty(match1.Groups[2].Value))
                    {
                        // grab our first timestamped entry
                        logDatestamp = match0.Groups[1].Value;
                        firstTimestamp = logDatestamp + " " + match1.Groups[1].Value.ToString();
                        firstSystem = match1.Groups[2].Value.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").ToString().ToUpper();
                    }

                    if (!string.IsNullOrEmpty(firstTimestamp) && !string.IsNullOrEmpty(firstSystem)
                        && !TimestampIsNewer(firstTimestamp, localSystemList[0].Key)
                        || localSystemList[0].Value.Equals(firstSystem))
                    {
                        // we've got a database, let's pull from it instead of reading from the logs, for performance
                        if (output_unclean.Count == 0 || !StringInListExact(firstSystem, output_unclean))
                        {
                            for (int i = 0; i < localSystemList.Count; i++)
                            {
                                var row = localSystemList[i];
                                if (output.Count == 0 || !StringInListExact(row.Value, output) && output.Count < listLimit)
                                    output.Add(row.Value);
                            }
                        }
                    }

                    if (output.Count > 0)
                        loadedFromDB = true; // flag us
                }
            }

            return output;
        }

        private List<string> LoadSystemsFromLogs(bool refreshMode, List<string> filePaths)
        {
            // get the latest timestamp from the DB.
            string timestamp = this.GetLastTimestamp();
            int fileCount = 0;

            if (string.IsNullOrEmpty(timestamp))
            {
                timestamp = "000000000000";
            }
            else
            {
                timestamp = timestamp
                    .Replace(" ", string.Empty)
                    .Replace("-", string.Empty)
                    .Replace(":", string.Empty)
                    .Substring(2);
            }

            // Count the number of files that are later than the timestamp and add one if not all files returned.
            fileCount = filePaths
                .Where(x => string.Compare(
                    Path.GetFileNameWithoutExtension(x)
                        .Substring(7)
                        .Substring(0, 12),
                    timestamp) > 0)
                .Count();

            if (fileCount < filePaths.Count)
            {
                ++fileCount;
            }

            Splash splashForm = new Splash();

            Stopwatch stopWatch2 = new Stopwatch();

            stopWatch2.Start();

            // this method initially populates the recent systems and pilot's log in the absence of a DB
            // grab the timestamp of this particular netlog
            string fileBuffer = "";
            string pattern0 = @"(\d{2,4}\-\d\d\-\d\d)[\s\-](\d\d:\d\d)\sGMT"; // $1=Y, $2=M, $3=D; $4=GMT

            // grab the timestamp of this entry, and then the system name
            string pattern1 = @"\{(\d\d:\d\d:\d\d).+System:""(.+)"""; // $1=localtime, $2=system
            List<string> output = new List<string>(); // a list for our system names
            List<KeyValuePair<string, string>> netLogOutput = new List<KeyValuePair<string, string>>();
            string logDatestamp = "";

            if (filePaths.Count > 0 && !string.IsNullOrEmpty(filePaths[0]) && refreshMode)
            {
                // compile a list of all unique systems last visited in all valid log files, oldest to newest
                // Count the number of files that are later than the timestamp and add one.
                fileCount = latestLogPaths
                    .Where(x => string.Compare(
                        Path.GetFileNameWithoutExtension(x)
                            .Substring(7)
                            .Substring(0, 12),
                        timestamp) > 0)
                    .Count();

                if (fileCount < latestLogPaths.Count)
                {
                    ++fileCount;
                }

                foreach (string path in latestLogPaths.Skip(latestLogPaths.Count - fileCount).ToList())
                {
                    // Show the splash form if the process has run for more than 5 seconds.
                    if (!splashForm.Visible && stopWatch2.ElapsedMilliseconds > 5000)
                    {
                        this.Invoke(new Action(() =>
                        {
                            this.Enabled = false;
                            splashForm.StartPosition = FormStartPosition.Manual;
                            splashForm.Location = new Point(this.Location.X + (this.Width - splashForm.Width) / 2, this.Location.Y + (this.Height - splashForm.Height) / 2);
                            splashForm.Caption = "Reading Net Logs";
                            splashForm.Show(this); // center on our location
                            splashForm.Focus(); // force this to the top
                        }));
                    }
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (BufferedStream bs = new BufferedStream(fs))
                    using (StreamReader stream = new StreamReader(bs, Encoding.UTF8, true, 65536))
                    {
                        output_unclean = new List<string>();
                        fileBuffer = stream.ReadToEnd(); // pull into memory
                        Match matchCollector0 = Regex.Match(fileBuffer, pattern0, RegexOptions.Compiled);
                        MatchCollection matchCollector1 = Regex.Matches(fileBuffer, pattern1, RegexOptions.Compiled | RegexOptions.Multiline);

                        if (!string.IsNullOrEmpty(matchCollector0.Groups[0].Value))
                        {
                            // grab our datestamp from the header
                            logDatestamp = matchCollector0.Groups[1].Value;

                            if (logDatestamp.Substring(2, 1) == "-")
                            {
                                logDatestamp = "20" + logDatestamp;
                            }
                        }

                        foreach (Match m in matchCollector1)
                        {
                            string curTimestamp = logDatestamp + " " + m.Groups[1].Value.ToString();
                            string curSystem = m.Groups[2].Value.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").ToString().ToUpper();
                            string lastOutput = (output.Count != 0) ? output.Last() : "";

                            if (!curSystem.Equals(lastOutput))
                            {
                                // prevent consecutive duplicates, ensure the most recent is lowest on the list
                                if (output.Count > 0 && StringInListExact(curSystem, output))
                                {
                                    // remove our older duplicate at its index before adding the newest
                                    output.RemoveAt(IndexInListExact(curSystem, output));
                                }

                                if (output.Count + 1 > listLimit)
                                {
                                    // remove from the top of the list to avoid overloading the limit
                                    output.RemoveAt(0);
                                }

                                output.Add(curSystem); // add the new unique system to the list
                            }

                            if (netLogOutput.Count == 0
                                    || netLogOutput.Count > 0 && !netLogOutput.Last().Value.Equals(curSystem))
                            {
                                // prevent consecutive duplicates
                                netLogOutput.Add(new KeyValuePair<string, string>(curTimestamp, curSystem));
                            }
                        }
                    }
                }

                if (hasLogLoaded && localSystemList.Count > 0 && netLogOutput.Count > 0)
                {
                    // if we're loaded, any older entries than the newest in the db get tossed
                    for (int i = netLogOutput.Count - 1; i >= 0; i--)
                    {
                        // go in reverse to preserve the list
                        if (!TimestampIsNewer(netLogOutput[i].Key, localSystemList[0].Key))
                            netLogOutput.RemoveAt(i);
                    }
                }

                output.Reverse();
                output = output.Distinct().ToList();
            }
            else if (filePaths.Count > 0 && !string.IsNullOrEmpty(filePaths.Last()) && !refreshMode)
            {
                // only grab systems from the newest log file, assume we've got systems already
                using (FileStream fs = new FileStream(filePaths.Last(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader stream = new StreamReader(bs, Encoding.UTF8, true, 65536))
                {
                    fileBuffer = stream.ReadToEnd(); // pull into memory
                    Match matchCollector0 = Regex.Match(fileBuffer, pattern0, RegexOptions.Compiled);
                    MatchCollection matchCollector1 = Regex.Matches(fileBuffer, pattern1, RegexOptions.Compiled | RegexOptions.Multiline);

                    if (!string.IsNullOrEmpty(matchCollector0.Groups[0].Value))
                    {
                        // grab our datestamp from the header
                        logDatestamp = matchCollector0.Groups[1].Value;
                    }

                    foreach (Match m in matchCollector1)
                    {
                        string curTimestamp = logDatestamp + " " + m.Groups[1].Value.ToString();
                        string curSystem = m.Groups[2].Value.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").ToString().ToUpper();
                        string lastOutput = (output_unclean.Count != 0) ? output_unclean.First() : "";
                        string lastEntry = (output.Count != 0) ? output.Last() : "";
                        string firstEntry = (output.Count != 0) ? output.First() : "";

                        // we should only add new entries to our input list if they're not a duplicate
                        if (output.Count == 0 && !curSystem.Equals(lastOutput) || !curSystem.Equals(firstEntry))
                        {
                            // replace any existing previous duplicate with the newest iteration
                            int index = IndexInListExact(curSystem, output);
                            if (index >= 0)
                                output.RemoveAt(index);

                            output.Insert(0, curSystem); // add the new unique system to the list
                        }

                        if (netLogOutput.Count == 0
                                || netLogOutput.Count > 0 && !netLogOutput.Last().Value.Equals(curSystem))
                        {
                            // prevent consecutive duplicates
                            netLogOutput.Add(new KeyValuePair<string, string>(curTimestamp, curSystem));
                        }
                    }

                    if (output.Count > 0 && output_unclean.Count > 0
                        && ListInListDesc(output, output_unclean))
                    {
                        // check if our exact output list exists in our target list
                        output.Clear(); // no changes
                    }
                    else if (output.Count > 0 && output_unclean.Count > 0)
                    {
                        // our target list contains duplicates, but isn't an exact duplicate
                        for (int i = 0; i < output.Count; i++)
                        {
                            // remove duplicates from the target list before we concat()
                            int index = IndexInListExact(output[i], output_unclean);
                            if (index >= 0)
                                output_unclean.RemoveAt(index);
                        }
                    }
                }

                if (output_unclean.Count >= output.Count && output.Count > 0
                    && output_unclean.Count + output.Count > listLimit)
                {
                    // remove from the bottom of the list to stay within our limit
                    for (int i = output_unclean.Count - 1; output_unclean.Count + output.Count > listLimit; i--)
                    {
                        output_unclean.RemoveAt(i);
                    }
                }
            }

            // remove potential difference collisions before passing our table
            if (netLogOutput.Count > 0 && localSystemList.Count > 0)
            {
                if (netLogOutput.First().Value.Equals(localSystemList.First().Value))
                    netLogOutput.RemoveAt(0); // first vs first, match
                else if (localSystemList.Count >= netLogOutput.Count
                    && netLogOutput.First().Value.Equals(localSystemList[netLogOutput.Count - 1].Value))
                    netLogOutput.RemoveAt(0); // first vs offset, match
            }

            if (localSystemList.Count == 0 && netLogOutput.Count > 0)
                UpdatePilotsLogDB(tdhDBConn, netLogOutput); // pass just the table, no diffs
            else if (netLogOutput.Count > 0)
            {
                var exceptTable = netLogOutput.Except(localSystemList).ToList();
                UpdatePilotsLogDB(tdhDBConn, exceptTable); // pass just the diffs, no table
            }

            stopWatch2.Stop();

            if (splashForm.Visible)
            {
                this.Invoke(new Action(() =>
                {
                    this.Enabled = true;
                    splashForm.Close();
                }));
            }

            return output; // return our finished list
        }

        private void ParseItemCSV()
        {
            outputItems = new List<string>();

            // read items from csv files
            using (StreamReader reader1 = new StreamReader(File.OpenRead(t_itemListPath)))
            {
                // skip the first line of the item csv
                if (!reader1.EndOfStream)
                {
                    reader1.ReadLine();
                }

                // Read in the item list first
                while (!reader1.EndOfStream)
                {
                    string line = reader1.ReadLine();
                    string[] values = line.Split(',');
                    string output = Convert.ToString(values[1]).Trim(new char[] { '\'', '\"' });

                    outputItems.Add(output);
                }
            }

            using (var reader2 = new StreamReader(File.OpenRead(t_shipListPath)))
            {
                // skip the first line of the ship csv
                if (!reader2.EndOfStream)
                {
                    reader2.ReadLine();
                }

                // Read in the ship list
                while (!reader2.EndOfStream)
                {
                    string line = reader2.ReadLine();
                    string[] values = line.Split(',');
                    string output = Convert.ToString(values[1]).Trim(new char[] { '\'', '\"' });

                    outputItems.Add(output);
                }
            }

            if (outputItems.Count != 0)
            {
                // sort alphabetically
                outputItems = outputItems
                    .OrderBy(x => x)
                    .ToList();
            }
            else
            {
                Debug.WriteLine("outputItems is empty, must be a path or access failure");
            }
        }

        private void ReadNetLog(bool refreshMethod)
        {
            // override to avoid net log logic
            if (!settingsRef.DisableNetLogs)
            {
                Stopwatch m_timer = Stopwatch.StartNew();

                // always reacquire the most current file(s), newest to oldest
                if (Directory.Exists(settingsRef.NetLogPath))
                {
                    Splash splashForm = new Splash();
                    ValidateNetLogPath(null); // check our Verbose flag and input files

                    if (latestLogPaths != null && latestLogPaths.Count > 0)
                    {
                        // try to populate from the DB first for speed
                        List<string> checkBuffer = new List<string>();

                        if (refreshMethod)
                        {
                            // if refreshMethod is true, we read ALL netLogs
                            if (buttonCaller == 16 || !hasRun && localSystemList.Count == 0)
                            {
                                // open a splash window to alert the user to our activity
                                this.Invoke(new Action(() =>
                                {
                                    this.Enabled = false;
                                    splashForm.StartPosition = FormStartPosition.Manual;
                                    splashForm.Location = new Point(this.Location.X + (this.Width - splashForm.Width) / 2, this.Location.Y + (this.Height - splashForm.Height) / 2);
                                    splashForm.Show(this); // center on our location
                                    splashForm.Focus(); // force this to the top
                                }));
                            }

                            checkBuffer = LoadSystemsFromDB(latestLogPaths);

                            if (loadedFromDB && checkBuffer.Count > 0)
                            {
                                // we've got systems from the DB, push them to the dropdown
                                output_unclean.AddRange(checkBuffer);
                            }
                            else if (!hasLogLoaded && checkBuffer.Count == 0)
                            {
                                // do the initial populate if we can't get systems from the DB
                                checkBuffer = LoadSystemsFromLogs(true, latestLogPaths);
                                if (checkBuffer.Count > 0)
                                {
                                    output_unclean.AddRange(checkBuffer);
                                }
                            }
                        }
                        else
                        {
                            // only refresh from the newest
                            checkBuffer = LoadSystemsFromLogs(false, latestLogPaths);
                            if (checkBuffer.Count > 0)
                            {
                                output_unclean = checkBuffer.Concat(output_unclean).ToList();
                                hasRefreshedRecents = true;
                            }
                        }
                    }

                    // our most recently entered system is always on top
                    if (output_unclean.Count > 0 && !string.IsNullOrEmpty(output_unclean[0]))
                    {
                        t_lastSystem = output_unclean[0];
                        Debug.WriteLine("Last entered system name: " + t_lastSystem);
                        hasLogLoaded = true;
                    }

                    if (splashForm.Visible)
                    {
                        this.Invoke(new Action(() =>
                        {
                            this.Enabled = true;
                            splashForm.Close();
                        }));
                    }
                }
                else
                {
                    output_unclean = new List<string>();
                    Debug.WriteLine("Our input path is bad/blank");
                }

                Debug.WriteLine("readNetLog took: " + m_timer.ElapsedMilliseconds + "ms");
                m_timer.Stop();
            }
        }

        private void RefreshItems()
        {
            /*
             * NOTE: This method must be called on a UI thread in order
             *     to update the combobox!
             *
             * We update our commodities list to the combobox here.
             */

            Stopwatch m_timer = Stopwatch.StartNew();

            ParseItemCSV(); // refresh the commodities

            // avoid null
            if (outputItems.Count > 0 && hasRun)
            {
                // refreshing, let's rebind
                commodityComboBox.Items.Clear();
                commodityComboBox.Items.AddRange(outputItems.ToArray());
                commodityComboBox.SelectedIndex = 0;
            }
            else if (!hasRun)
            {
                // if we're starting fresh, just bind
                commodityComboBox.Items.AddRange(outputItems.ToArray());
                commodityComboBox.SelectedIndex = 0;
            }

            Debug.WriteLine("refreshItems took: " + m_timer.ElapsedMilliseconds + "ms");
            m_timer.Stop();
        }

        private bool RemoveDBRow(SQLiteConnection dbConn, int rowIndex)
        {
            if (rowIndex >= 0)
            {
                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM SystemLog WHERE ID = @ID", dbConn))
                {
                    try
                    {
                        var transaction = dbConn.BeginTransaction();
                        // delete all the rows specified by the index, using a transaction for efficiency
                        cmd.Parameters.AddWithValue("@ID", rowIndex);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        transaction.Commit();

                        VacuumPilotsLogDB(tdhDBConn);
                    }
                    catch (Exception) { throw; }
                    return true; // success!
                }
            }

            return false;
        }

        private bool RemoveDBRows(SQLiteConnection dbConn, List<int> rowsIndex)
        {
            // remove a batch of rows (faster than removeDBRow)
            if (rowsIndex.Count > 0)
            {
                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM SystemLog WHERE ID = @ID", dbConn))
                {
                    try
                    {
                        var transaction = dbConn.BeginTransaction();
                        foreach (int i in rowsIndex)
                        {
                            // delete all the rows specified by the index, using a transaction for efficiency
                            cmd.Parameters.AddWithValue("@ID", i);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        transaction.Commit();

                        VacuumPilotsLogDB(tdhDBConn);
                    }
                    catch (Exception) { throw; }
                    return true; // success!
                }
            }

            return false;
        }

        private bool UpdateDBRow(SQLiteConnection dbConn, List<DataRow> rows)
        {
            // insert/update an existing set of rows
            if (rows.Count > 0)
            {
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT OR REPLACE INTO SystemLog VALUES (@ID,@Timestamp,@System,@Notes)", dbConn))
                {
                    cmd.Parameters.AddWithValue("@ID", 0);
                    cmd.Parameters.AddWithValue("@Timestamp", string.Empty);
                    cmd.Parameters.AddWithValue("@System", string.Empty);
                    cmd.Parameters.AddWithValue("@Notes", string.Empty);

                    try
                    {
                        var transaction = dbConn.BeginTransaction();

                        for (int i = rows.Count - 1; i >= 0; i--)
                        {
                            DataRow row = rows[i];
                            int var1 = int.Parse(row["ID"].ToString());
                            string var2 = (row["Timestamp"] ?? "").ToString();
                            string var3 = (row["System"] ?? "").ToString();
                            string var4 = (row["Notes"] ?? "").ToString();

                            cmd.Parameters["@ID"].Value = var1;
                            cmd.Parameters["@Timestamp"].Value = var2;
                            cmd.Parameters["@System"].Value = var3;
                            cmd.Parameters["@Notes"].Value = var4;

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception) { throw; }

                    UpdateLocalTable(tdhDBConn);

                    return true; // success!
                }
            }

            return false;
        }

        private void UpdateLocalTable(SQLiteConnection dbConn)
        {
            if (dbConn != null && dbConn.State == ConnectionState.Closed)
                dbConn.Open();

            if (HasValidColumn(dbConn, "SystemLog", "System"))
            {
                try
                {
                    using (SQLiteCommand cmd = dbConn.CreateCommand())
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "SELECT * FROM SystemLog ORDER BY Timestamp DESC, System DESC, Notes DESC";
                        localTable.Locale = System.Globalization.CultureInfo.InvariantCulture;
                        localTable.Rows.Clear();
                        adapter.Fill(localTable);

                        localSystemList.Clear();
                        foreach (DataRow r in localTable.Rows)
                        {
                            // convert datatable to systemlist
                            localSystemList.Add(new KeyValuePair<string, string>(r.Field<string>("Timestamp"), r.Field<string>("System")));
                        }

                        localTable.PrimaryKey = new System.Data.DataColumn[] { localTable.Columns["ID"] };
                    }
                }
                catch (Exception) { throw; }
            }
        }

        private bool UpdatePilotsLogDB(SQLiteConnection dbConn, List<KeyValuePair<string, string>> exceptKey)
        {
            // here we take a non-intersect key to add systems to our DB
            int exceptCount = exceptKey.Count();

            if (exceptCount > 0)
            {
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO SystemLog VALUES (null,@Timestamp,@System,null)", dbConn))
                {
                    cmd.Parameters.AddWithValue("@Timestamp", string.Empty);
                    cmd.Parameters.AddWithValue("@System", string.Empty);

                    var transaction = dbConn.BeginTransaction();

                    foreach (var s in exceptKey)
                    {
                        try
                        {
                            cmd.Parameters["@Timestamp"].Value = s.Key;
                            cmd.Parameters["@System"].Value = s.Value;

                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                            // Do nothing.
                        }
                    }

                    transaction.Commit();

                    InvalidatedRowUpdate(true, 0);

                    return true; // success!
                }
            }

            return false;
        }

        private void VacuumPilotsLogDB(SQLiteConnection dbConn)
        {
            // a simple method to vacuum our database
            try
            {
                if (dbConn != null && dbConn.State == ConnectionState.Closed)
                    dbConn.Open();

                using (SQLiteCommand cmd = dbConn.CreateCommand())
                {
                    if (HasValidRows(dbConn, "SystemLog"))
                    {
                        // clean up after ourselves
                        cmd.CommandText = "VACUUM";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        private bool ValidateDBPath()
        {
            tdDBPath = Path.Combine(settingsRef.TDPath, @"data\TradeDangerous.db");

            try
            {
                return CheckIfFileOpens(tdDBPath);
            }
            catch
            {
                throw new Exception("Cannot open the TradeDangerous database, this is fatal");
            }
        }
    }
}