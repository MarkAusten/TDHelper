﻿using System;
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

        private SQLiteConnection tdConn = null;
        private SQLiteConnection pilotsLogConn = null;

        #endregion Props

        public static List<string> CollectLogPaths(
            string path,
            string pattern)
        {
            // only collect log paths that contain system names
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(path);
                SortedList<string, string> logPaths = new SortedList<string, string>();

                FileInfo[] fileList = dInfo.GetFiles(pattern);

                // check the make sure the directory is populated
                if (dInfo.Exists && fileList.Length > 0)
                {
                    foreach (FileInfo f in fileList.OrderBy(f => f.LastWriteTime))
                    {
                        string filePath = Path.Combine(path, f.ToString());

                        using (TextReader reader = new StreamReader(filePath, Encoding.UTF8))
                        {
                            // check if there are any files that match the mask, return null if nothing matches
                            string tempLines = reader.ReadToEnd(); // pull into memory
                            Match timestampMatch = Regex.Match(tempLines, @"(\d{2,4}\-\d\d\-\d\d)[\s\-](\d\d:\d\d)\sGMT");
                            Match systemMatch = Regex.Match(tempLines, @"\{(\d\d:\d\d:\d\d).+System:""(.+)""");

                            if (systemMatch.Success && timestampMatch.Success)
                            {
                                string timestampHeader
                                    = timestampMatch.Groups[1].Value.ToString()
                                    + " "
                                    + systemMatch.Groups[1].Value.ToString();

                                logPaths.Add(timestampHeader, filePath);
                            }
                        }
                    }

                    return new List<string>(logPaths.Values);
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

        /// <summary>
        /// Carry out any pre-close operations and then close the connection.
        /// </summary>
        /// <param name="conn">The connection to be closed.</param>
        public void CloseConnection(SQLiteConnection conn)
        {
            if (conn != null &&
                conn.State == ConnectionState.Open)
            {
                using (SQLiteCommand query = new SQLiteCommand("PRAGMA optimise", conn))
                {
                    query.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public bool IsConnectionBusy(SQLiteConnection conn)
        {
            bool isBusy = false;

            switch (conn.State)
            {
                case ConnectionState.Connecting:
                case ConnectionState.Executing:
                case ConnectionState.Fetching:
                    isBusy = true;
                    break;
            }

            return isBusy;
        }

        /// <summary>
        /// Open the specified connection.
        /// </summary>
        /// <param name="conn">The connection to be opened.</param>
        public void OpenConnection(SQLiteConnection conn)
        {
            // If either of the connections is null then set the connections.
            if (tdConn == null || pilotsLogConn == null)
            {
                SetConnections();
            }

            if (conn != null && conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        private bool AddAtTimestampDBRow(string timestamp)
        {
            // add a blank row with the timestamp from a given row, basically an insert-below-index during select()
            OpenConnection(pilotsLogConn);

            try
            {
                using (SQLiteCommand cmd = pilotsLogConn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO SystemLog VALUES (null,@Timestamp,null,null)";
                    DateTime tempTimestamp = new DateTime();

                    if (!string.IsNullOrEmpty(timestamp)
                        && DateTime.TryParseExact(timestamp, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempTimestamp))
                    {
                        string var1 = timestamp; // we were successful in parsing the timestamp, probably safe

                        try
                        {
                            using (var transaction = pilotsLogConn.BeginTransaction())
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
            }
            finally
            {
                CloseConnection(pilotsLogConn);
            }

            return false;
        }

        private bool AddDBRow()
        {
            OpenConnection(pilotsLogConn);

            try
            {
                // add a blank row with the current timestamp
                using (SQLiteCommand cmd = pilotsLogConn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO SystemLog VALUES (null,@Timestamp,null,null)";

                    try
                    {
                        string var1 = CurrentTimestamp();

                        using (var transaction = pilotsLogConn.BeginTransaction())
                        {
                            cmd.Parameters.AddWithValue("@Timestamp", var1);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        throw;
                    }

                    LoadPilotsLogDB(); // need a full refresh

                    return true; // success!
                }
            }
            finally
            {
                CloseConnection(pilotsLogConn);
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
                            SetSourceAndDestinationLists(true);
                            // bind the recent systems/stations first

                            cboSourceSystem.DataSource = null;
                            cboSourceSystem.DataSource = SourceList;

                            cboRunOptionsDestination.DataSource = null;
                            cboRunOptionsDestination.DataSource = DestinationList;

                            if (buttonCaller == 16)
                            {
                                // we're marked as needing a database refresh
                                // bind the database output for autocompletion
                                cboSourceSystem.AutoCompleteCustomSource.Clear();
                                cboSourceSystem.AutoCompleteCustomSource.AddRange(outputSysStnNames.ToArray());

                                //cboRunOptionsDestination.AutoCompleteCustomSource.Clear();
                                //cboRunOptionsDestination.AutoCompleteCustomSource.AddRange(outputSysStnNames.ToArray());

                                //cboBuyOptionsItem.SelectedIndex = 0;
                                //cboSellOptionsItem.SelectedIndex = 0;
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
                                SetSourceAndDestinationLists();

                                cboSourceSystem.DataSource = null;
                                cboSourceSystem.DataSource = SourceList;

                                //cboRunOptionsDestination.DataSource = null;
                                //cboRunOptionsDestination.DataSource = DestinationList;

                                // reset our cursor after the refresh if the user had input focus
                                if (cboSourceSystem.Text.Length > 0)
                                {
                                    cboSourceSystem.Select(cboSourceSystem.Text.Length, 0);
                                }

                                if (cboRunOptionsDestination.Text.Length > 0)
                                {
                                    cboRunOptionsDestination.Select(cboSourceSystem.Text.Length, 0);
                                }
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
                            SetSourceAndDestinationLists(true);

                            cboSourceSystem.DataSource = null;
                            cboSourceSystem.DataSource = SourceList;

                            //cboRunOptionsDestination.DataSource = null;
                            //cboRunOptionsDestination.DataSource = DestinationList;

                            cboSourceSystem.AutoCompleteCustomSource.AddRange(outputSysStnNames.ToArray());
                            //cboRunOptionsDestination.AutoCompleteCustomSource.AddRange(outputSysStnNames.ToArray());
                        }));

                        hasRun = true; // set the semaphore so we don't hork our data tables
                    }

                    Debug.WriteLine("buildOutput combobox population took: " + m_timer.ElapsedMilliseconds + "ms");

                    m_timer.Stop();
                }
                finally
                {
                    if (hasRun)
                    {
                        testSystemsTimer.Start(); // start our background systems list updater (~10s)
                    }

                    Monitor.Exit(readNetLock);
                }
            }
        }

        private void BuildPilotsLog()
        {
            // here we either build or load our database
            if (grdPilotsLog.Rows.Count == 0)
            {
                if (HasValidRows("SystemLog"))
                {
                    InvalidatedRowUpdate(true, -1);
                }
                else
                {
                    CreatePilotsLogDB(); // make from scratch
                }
            }
            else if (HasValidRows("SystemLog"))
            {
                // load our physical database
                InvalidatedRowUpdate(true, -1);
            }
        }

        private void CheckAndWaitForUnlock(string databaseFile)
        {
            bool retry = true;
            bool isLocked = true;

            // Keep trying until either retry is false or the database is not locked.
            while (retry && isLocked)
            {
                // The inner loop is 5 time 1 second.
                int retryCount = 5;

                while (--retryCount > 0 && isLocked)
                {
                    isLocked = IsDatabaseLocked(databaseFile);

                    if (isLocked)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                if (isLocked)
                {
                    // Display database locked message and ask for input.
                    DialogResult dialog = TopMostMessageBox.Show(
                        true,
                        true,
                        "The Trade Dangerous database is locked. Try to connect again?",
                        "TD Helper - Error",
                        MessageBoxButtons.YesNo);

                    retry = dialog == DialogResult.Yes;
                }
            }

            // If the database is locked and the user does not want to retry, shut down THD.
            if (isLocked)
            {
                Environment.Exit(0);
            }
        }

        private void CreatePilotsLogDB()
        {
            try
            {
                OpenConnection(pilotsLogConn);

                using (SQLiteCommand cmd = pilotsLogConn.CreateCommand())
                {
                    if (!HasValidColumn(pilotsLogConn, "SystemLog", "System"))
                    {
                        // create our table first
                        cmd.CommandText = "CREATE TABLE SystemLog (ID INTEGER PRIMARY KEY AUTOINCREMENT, Timestamp NVARCHAR, System NVARCHAR, Notes NVARCHAR)";
                        cmd.ExecuteNonQuery();

                        // Create a unique index.
                        cmd.CommandText = "CREATE UNIQUE INDEX IF NOT EXISTS SystemTimestamp ON SystemLog (System, Timestamp)";
                        cmd.ExecuteNonQuery();
                    }
                    else if (HasValidRows("SystemLog"))
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
                        using (var transaction = pilotsLogConn.BeginTransaction())
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
                        retriever = new DataRetriever(pilotsLogConn, "SystemLog", this);

                        foreach (DataColumn c in retriever.Columns)
                        {
                            grdPilotsLog.Columns.Add(c.ColumnName, c.ColumnName);
                        }

                        // setup the gridview
                        UpdateLocalTable();
                        memoryCache = new Cache(retriever, 24);

                        grdPilotsLog.RowCount = retriever.RowCount;

                        grdPilotsLog.Columns["ID"].Visible = false;
                        grdPilotsLog.Columns["Timestamp"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                        grdPilotsLog.Columns["System"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                        grdPilotsLog.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                CloseConnection(pilotsLogConn);
            }
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
                    outputSysStnNames.Add(string.Format(
                        "{0}/{1}",
                        stn_table.Rows[i]["sys_name"],
                        stn_table.Rows[i]["stn_name"]));
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
            {
                Debug.WriteLine("outputSysStnNames is empty, must be a DB path failure or access violation");
            }
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
                    string temp_cost = string.Format(
                        "{0:#,##0}",
                        decimal.Parse(ship_table.Rows[i]["ship_cost"].ToString()));

                    outputStationShips.Add(string.Format(
                        "{0} [{1} cr]",
                        ship_table.Rows[i]["ship_name"],
                        temp_cost));
                }
            }

            if (outputStationDetails.Count == 0)
            {
                Debug.WriteLine("outputStationDetails is empty, must be a DB path failure or access violation");
            }
        }

        /// <summary>
        /// Set the connections to the databases.
        /// </summary>
        private void SetConnections()
        {
            tdPath = Path.Combine(settingsRef.TDPath, @"data\TradeDangerous.db");

            tdConn = GetConnection(tdPath);
            pilotsLogConn = GetConnection(pilotsLogDBPath);
        }

        /// <summary>
        /// Create a conection to the specified database file.
        /// </summary>
        /// <param name="path">The path to the database file to which the connection should be made.</param>
        /// <returns>An SQLite Connection.</returns>
        private SQLiteConnection GetConnection(string path)
        {
            return new SQLiteConnection("Data Source=" + path + ";Version=3;FKSupport=False;");
        }

        private string GetLastTimestamp()
        {
            string timestamp = string.Empty;

            try
            {
                OpenConnection(pilotsLogConn);

                using (SQLiteCommand cmd = pilotsLogConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT MAX(Timestamp) FROM SystemLog";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            timestamp = reader.GetString(0);
                        }
                    }
                }
            }
            catch
            {
                /* eat exceptions */
            }
            finally
            {
                CloseConnection(pilotsLogConn);
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
                        Stopwatch m_timer = Stopwatch.StartNew();

                        OpenConnection(tdConn);

                        if (stnship_table.Rows.Count != 0)
                        {
                            stnship_table = new DataTable();
                        }

                        if (ship_table.Rows.Count != 0)
                        {
                            ship_table = new DataTable();
                        }

                        /*
                         * Extract station details into a single row array--the structure is as follows:
                         *
                         * sys_name | stn_name | stn_ls | stn_padsize | stn_rearm | stn_refuel | stn_repair | stn_outfit | stn_ship | stn_items | stn_bmkt
                         *
                         * The contents of each station field can be: 'Y', 'N', '?', 'S', 'M', 'L', or an int64/long
                         */

                        // check for one of the common columns
                        if (HasValidColumn(tdConn, "Station", "rearm"))
                        {
                            using (SQLiteCommand cmd = tdConn.CreateCommand())
                            {
                                cmd.CommandText
                                    = " select "
                                    + "    sys.name as sys_name,"
                                    + "    stn.name as stn_name,"
                                    + "    stn.ls_from_star as stn_ls,"
                                    + "    stn.max_pad_size as stn_padsize,"
                                    + "    stn.rearm as stn_rearm,"
                                    + "    stn.refuel as stn_refuel,"
                                    + "    stn.repair as stn_repair,"
                                    + "    stn.outfitting as stn_outfit,"
                                    + "    stn.shipyard as stn_ship,"
                                    + "    stn.market as stn_items,"
                                    + "    stn.blackmarket as stn_bmkt"
                                    + " from"
                                    + "    system sys"
                                    + " join"
                                    + "    station stn on stn.system_id = sys.system_id"
                                    + " where"
                                    + "    sys.name = @system and"
                                    + "    stn.name = @station";

                                cmd.Parameters.AddWithValue("@system", inputSystem);
                                cmd.Parameters.AddWithValue("@station", inputStation);

                                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                                {
                                    adapter.Fill(stnship_table);
                                }

                                // populate our shipvendor table as well
                                cmd.CommandText
                                    = " select"
                                    + " 	sys.system_id as sys_sysid,"
                                    + " 	stn.system_id as stn_sysid,"
                                    + " 	stn.station_id as stn_stnid,"
                                    + " 	shipv.station_id as shipv_stnid,"
                                    + " 	shipv.ship_id as shipv_shipid,"
                                    + " 	ship.ship_id as ship_shipid,"
                                    + " 	sys.name as sys_name,"
                                    + " 	stn.name as stn_name,"
                                    + " 	ship.name as ship_name,"
                                    + " 	ship.cost as ship_cost"
                                    + " from "
                                    + " 	system sys"
                                    + " join"
                                    + " 	station stn on stn.system_id = sys.system_id"
                                    + " join"
                                    + " 	shipvendor shipv on stn.station_id = shipv.station_id"
                                    + " join"
                                    + " 	ship on shipv.ship_id = ship.ship_id"
                                    + " where"
                                    + "     sys.name = @system and"
                                    + "     stn.name = @station"
                                    + " order by"
                                    + "     ship.cost desc";

                                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                                {
                                    adapter.Fill(ship_table);
                                }
                            }

                            outputStationDetails = new List<string>();
                            outputStationShips = new List<string>();

                            FilterStationData();

                            Debug.WriteLine("grabStationData query took: " + m_timer.ElapsedMilliseconds + "ms");

                            m_timer.Stop();
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        CloseConnection(tdConn);
                    }
            }
        }

        private bool HasValidColumn(
            SQLiteConnection conn,
            string tableName,
            string columnName)
        {
            // this method returns true if a column exists
            using (SQLiteCommand query = new SQLiteCommand("PRAGMA table_info(" + tableName + ")", conn))
            using (DataTable results = new DataTable())
            {
                try
                {
                    OpenConnection(conn);

                    using (SQLiteDataReader reader = query.ExecuteReader())
                    {
                        results.Load(reader);

                        foreach (DataRow r in results.Rows)
                        {
                            foreach (var i in r.ItemArray)
                            {
                                if (i.ToString() == columnName)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    /* eat exceptions */
                }
                finally
                {
                    CloseConnection(conn);
                }
            }

            return false;
        }

        private bool HasValidRows(string tableName)
        {
            OpenConnection(pilotsLogConn);

            try
            {
                // this method returns true if there are any valid rows
                using (SQLiteCommand query = pilotsLogConn.CreateCommand())
                {
                    query.CommandText = "SELECT COUNT(*) FROM SystemLog";

                    try
                    {
                        using (SQLiteDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int rows = reader.GetInt32(0);

                                if (rows > 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    catch
                    {
                        /* eat exceptions */
                    }
                }
            }
            finally
            {
                CloseConnection(pilotsLogConn);
            }

            return false;
        }

        /// <summary>
        /// Stop the timer and hide the splash form is required.
        /// </summary>
        /// <param name="splashForm">The splash form to be closed.</param>
        /// <param name="stopWatch">The timer to be stopped.</param>
        private void HideSplashForm(
            Splash splashForm,
            Stopwatch stopWatch)
        {
            stopWatch.Stop();

            if (splashForm.Visible)
            {
                this.Invoke(new Action(() =>
                {
                    this.Enabled = true;
                    splashForm.Close();
                }));
            }
        }

        private bool InvalidatedRowUpdate(
            bool refreshMode,
            int rowIndex)
        {
            if (refreshMode)
            {
                // full refresh
                if (rowIndex == -1)
                {
                    LoadPilotsLogDB();

                    return true;
                }

                // invalidate the cache pages
                UpdateLocalTable();

                retriever = new DataRetriever(tdConn, "SystemLog", this);
                memoryCache = new Cache(retriever, 24);

                // force a refresh/repaint
                this.Invoke(new Action(() =>
                {
                    grdPilotsLog.RowCount = retriever.RowCount;
                    grdPilotsLog.Refresh();
                }));
            }
            else
            {
                // partial refresh
                UpdateLocalTable();

                retriever = new DataRetriever(tdConn, "SystemLog", this);
                memoryCache = new Cache(retriever, 24);

                this.Invoke(new Action(() =>
                {
                    grdPilotsLog.RowCount = retriever.RowCount;
                    grdPilotsLog.InvalidateRow(rowIndex);
                }));
            }

            return false;
        }

        /// <summary>
        /// Check to see if the database is currently locked.
        /// </summary>
        /// <param name="databaseFile">The path to the database being checked.</param>
        /// <returns>True if the database is locked.</returns>
        private bool IsDatabaseLocked(string databaseFile)
        {
            bool isLocked = false;

            // Connect to the specified database.
            using (SQLiteConnection conn = GetConnection(databaseFile))
            {
                OpenConnection(conn);

                // Issue a 'BEGIN IMMEDIATE' command
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "begin immediate";

                    try
                    {
                        cmd.ExecuteNonQuery();

                        // It worked so rollback.
                        cmd.CommandText = "rollback";

                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        // It did not work so the database is locked.
                        isLocked = true;
                    }
                }

                CloseConnection(conn);
            }

            return isLocked;
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
                CheckAndWaitForUnlock(tdPath);

                string stationName = string.Empty;
                string systemName = string.Empty;

                    try
                    {
                        Stopwatch m_timer = Stopwatch.StartNew();

                        OpenConnection(tdConn);

                        // wipe to prevent duplicates
                        if (stn_table.Rows.Count != 0)
                        {
                            stn_table = new DataTable(); // this is O(1) performant
                            nonstn_table = new DataTable(); // this is O(1) performant
                        }

                        // match on System.system_id = Station.system_id, output in "System | Station" format
                        using (SQLiteCommand cmd = tdConn.CreateCommand())
                        {
                            cmd.CommandText
                                = " select"
                                + "     sys.name as sys_name,"
                                + "     stn.name as stn_name"
                                + " from"
                                + "     System sys"
                                + " join"
                                + "     Station stn on sys.system_id = stn.system_id";

                            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                            {
                                adapter.Fill(stn_table);
                            }

                            cmd.CommandText
                                = " select "
                                + "     a.name as sys_name"
                                + " from"
                                + "     system as a"
                                + " where"
                                + "     a.system_id not in"
                                + "     ("
                                + "         select b.system_id from station as b"
                                + "     )";

                            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                            {
                                adapter.Fill(nonstn_table);
                            }
                        }

                        Debug.WriteLine("loadDatabase query took: " + m_timer.ElapsedMilliseconds + "ms");

                        m_timer.Stop();

                        outputSysStnNames = new List<string>(); // wipe before we fill

                        FilterDatabase(); // filter and fill our output
                    }
                    catch (SQLiteException)
                    {
                        //
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        CloseConnection(tdConn);
                    }
            }
        }

        private void LoadPilotsLogDB()
        {
            if (HasValidColumn(pilotsLogConn, "SystemLog", "System"))
            {
                try
                {
                    UpdateLocalTable();

                    retriever = new DataRetriever(pilotsLogConn, "SystemLog", this);

                    this.Invoke(new Action(() =>
                    {
                        if (grdPilotsLog.Columns.Count != localTable.Columns.Count)
                        {
                            foreach (DataColumn c in retriever.Columns)
                            {
                                grdPilotsLog.Columns.Add(c.ColumnName, c.ColumnName);
                            }
                        }

                        grdPilotsLog.Rows.Clear();

                        memoryCache = new Cache(retriever, 24);

                        grdPilotsLog.Refresh();

                        grdPilotsLog.RowCount = retriever.RowCount;

                        if (grdPilotsLog.RowCount > 0)
                        {
                            grdPilotsLog.Columns["ID"].Visible = false;
                            grdPilotsLog.Columns["Timestamp"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                            grdPilotsLog.Columns["System"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                            grdPilotsLog.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }));
                }
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private List<string> LoadSystemsFromDB(List<string> logPaths)
        {
            // we should load from the DB if the newest entry is newer than the newest log file entry
            string pattern0 = @"^(\d\d\-\d\d\-\d\d).+?\((.+?)\sGMT"; // $1=Y, $2=M, $3=D; $4=GMT

            // grab the timestamp of this entry, and then the system name
            string pattern1 = @"\{(.*?)\}\sSystem.+?\((.*?)\)"; // $1=localtime, $2=system

            List<string> output = new List<string>(); // a list for our system names
            string logDatestamp = string.Empty;

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
                    string firstTimestamp = string.Empty, firstSystem = string.Empty;

                    if (!string.IsNullOrEmpty(match0.Groups[0].Value)
                        && !string.IsNullOrEmpty(match1.Groups[1].Value)
                        && !string.IsNullOrEmpty(match1.Groups[2].Value))
                    {
                        // grab our first timestamped entry
                        logDatestamp = match0.Groups[1].Value;
                        firstTimestamp = logDatestamp + " " + match1.Groups[1].Value.ToString();
                        firstSystem = match1.Groups[2].Value.Replace("\r\n", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).ToString().ToUpper();
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
                                {
                                    output.Add(row.Value);
                                }
                            }
                        }
                    }

                    if (output.Count > 0)
                    {
                        loadedFromDB = true; // flag us
                    }
                }
            }

            return output;
        }

        private List<string> LoadSystemsFromLogs(
            bool refreshMode,
            List<string> filePaths)
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
            string fileBuffer = string.Empty;
            string pattern0 = @"(\d{2,4}\-\d\d\-\d\d)[\s\-](\d\d:\d\d)\sGMT"; // $1=Y, $2=M, $3=D; $4=GMT

            // grab the timestamp of this entry, and then the system name
            string pattern1 = @"\{(\d\d:\d\d:\d\d).+System:""(.+)"""; // $1=localtime, $2=system

            List<string> output = new List<string>(); // a list for our system names
            List<KeyValuePair<string, string>> netLogOutput = new List<KeyValuePair<string, string>>();
            string logDatestamp = string.Empty;

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
                    this.ShowSplashForm(splashForm, stopWatch2, "Reading Net Logs");

                    using (TextReader reader = new StreamReader(path, Encoding.UTF8))
                    {
                        output_unclean = new List<string>();
                        fileBuffer = reader.ReadToEnd(); // pull into memory
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
                            string curSystem = m.Groups[2].Value
                                .Replace("\r\n", string.Empty)
                                .Replace("\r", string.Empty)
                                .Replace("\n", string.Empty)
                                .ToString()
                                .ToUpper();

                            string lastOutput = (output.Count != 0) ? output.Last() : string.Empty;

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
                        {
                            netLogOutput.RemoveAt(i);
                        }
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
                        string curSystem = m.Groups[2].Value.Replace("\r\n", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).ToString().ToUpper();
                        string lastOutput = (output_unclean.Count != 0) ? output_unclean.First() : string.Empty;
                        string lastEntry = (output.Count != 0) ? output.Last() : string.Empty;
                        string firstEntry = (output.Count != 0) ? output.First() : string.Empty;

                        // we should only add new entries to our input list if they're not a duplicate
                        if (output.Count == 0 && !curSystem.Equals(lastOutput) || !curSystem.Equals(firstEntry))
                        {
                            // replace any existing previous duplicate with the newest iteration
                            int index = IndexInListExact(curSystem, output);

                            if (index >= 0)
                            {
                                output.RemoveAt(index);
                            }

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
                            {
                                output_unclean.RemoveAt(index);
                            }
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
                {
                    netLogOutput.RemoveAt(0); // first vs first, match
                }
                else if (localSystemList.Count >= netLogOutput.Count
                    && netLogOutput.First().Value.Equals(localSystemList[netLogOutput.Count - 1].Value))
                {
                    netLogOutput.RemoveAt(0); // first vs offset, match
                }
            }

            if (localSystemList.Count == 0 && netLogOutput.Count > 0)
            {
                UpdatePilotsLogDB(netLogOutput); // pass just the table, no diffs
            }
            else if (netLogOutput.Count > 0)
            {
                var exceptTable = netLogOutput.Except(localSystemList).ToList();
                UpdatePilotsLogDB(exceptTable); // pass just the diffs, no table
            }

            this.HideSplashForm(splashForm, stopWatch2);

            return output; // return our finished list
        }

        /// <summary>
        /// Optimise all the databases.
        /// </summary>
        private void OptimiseAllDatabases()
        {
            OptimiseDatabase(pilotsLogConn);
            OptimiseDatabase(tdConn);
        }

        /// <summary>
        /// Send the analyse command to the SQLite databases.
        /// </summary>
        private void AnalyseAllDatabases()
        {
            CloseConnection(tdConn);
            CloseConnection(pilotsLogConn);

            AnalyseDatabase(GetConnection(tdPath));
            AnalyseDatabase(GetConnection(pilotsLogDBPath));
        }

        /// <summary>
        /// Send the analyse command to the SQLite databases.
        /// </summary>
        /// <param name="conn">The connection to the database.</param>
        private void AnalyseDatabase(SQLiteConnection conn)
        {
            OpenConnection(conn);

            using (SQLiteCommand cmd = tdConn.CreateCommand())
            {
                OpenConnection(cmd.Connection);

                cmd.CommandText = "ANALYZE";

                cmd.ExecuteNonQuery();
            }

            CloseConnection(conn);
        }

        /// <summary>
        /// Optimise the specified database.
        /// </summary>
        /// <param name="conn">The connection to the database to be optimised.</param>
        private void OptimiseDatabase(SQLiteConnection conn)
        {
            OpenConnection(conn);
            CloseConnection(conn);
        }

        private void ParseItemCSV()
        {
            List<string> commodities = new List<string>();

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

                    commodities.Add(output);
                }
            }

            CommoditiesList.Clear();
            CommoditiesList.AddRange(commodities);

            commodities.Clear();

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

                    commodities.Add(output);
                }
            }

            ShipList.Clear();
            ShipList.AddRange(commodities);

            CommoditiesList = CommoditiesList.OrderBy(x => x)
                .ToList();

            ShipList = ShipList.OrderBy(x => x)
                .ToList();

            CommodityAndShipList.Clear();

            if (CommoditiesList.Count > 0)
            {
                CommodityAndShipList.AddRange(CommoditiesList);
            }

            if (ShipList.Count > 0)
            {
                CommodityAndShipList.AddRange(ShipList);
            }

            if (CommodityAndShipList.Count == 0)
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
                                this.ShowSplashForm(splashForm, m_timer, string.Empty, true);
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

                    this.HideSplashForm(splashForm, m_timer);
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

            RefreshCurrentOptionsPanel();

            Debug.WriteLine("refreshItems took: " + m_timer.ElapsedMilliseconds + "ms");
            m_timer.Stop();
        }

        private bool RemoveDBRow(int rowIndex)
        {
            if (rowIndex >= 0)
            {
                OpenConnection(pilotsLogConn);

                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM SystemLog WHERE ID = @ID", pilotsLogConn))
                    {
                        try
                        {
                            var transaction = pilotsLogConn.BeginTransaction();
                            // delete all the rows specified by the index, using a transaction for efficiency
                            cmd.Parameters.AddWithValue("@ID", rowIndex);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            transaction.Commit();

                            VacuumPilotsLogDB();
                        }
                        catch (Exception) { throw; }

                        return true; // success!
                    }
                }
                finally
                {
                    CloseConnection(pilotsLogConn);
                }
            }

            return false;
        }

        private bool RemoveDBRows(List<int> rowsIndex)
        {
            // remove a batch of rows (faster than removeDBRow)
            if (rowsIndex.Count > 0)
            {
                OpenConnection(pilotsLogConn);

                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM SystemLog WHERE ID = @ID", pilotsLogConn))
                    {
                        try
                        {
                            using (SQLiteTransaction transaction = pilotsLogConn.BeginTransaction())
                            {
                                foreach (int i in rowsIndex)
                                {
                                    // delete all the rows specified by the index, using a transaction for efficiency
                                    cmd.Parameters.AddWithValue("@ID", i);
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                }

                                transaction.Commit();
                            }

                            VacuumPilotsLogDB();
                        }
                        catch (Exception) { throw; }

                        return true; // success!
                    }
                }
                finally
                {
                    CloseConnection(pilotsLogConn);
                }
            }

            return false;
        }

        private void SetSourceAndDestinationLists(bool first = false)
        {
            // bind the recent systems/stations first
            SourceList.Clear();

            // Add a blank entry at the top is this is the first run.
            if (first)
            {
                SourceList.Add(string.Empty);
            }

            // we should add an indicator to every entry in our favorites
            if (currentMarkedStations.Count > 0)
            {
                SourceList.AddRange(currentMarkedStations.Select(x => "!" + x).ToArray());
            }

            SourceList.AddRange(output_unclean.ToArray());

            // Clone the source list to the destination list.
            DestinationList.AddRange(SourceList.GetRange(0, SourceList.Count));
        }

        /// <summary>
        /// Show the splash form if required.
        /// </summary>
        /// <param name="splashForm">The splash form to be shown.</param>
        /// <param name="stopWatch">The elapsed timer.</param>
        /// <param name="prompt">The prompt to be displayed in the form.</param>
        private void ShowSplashForm(
            Splash splashForm,
            Stopwatch stopWatch,
            string prompt = "",
            bool showNow = false)
        {
            // Show the splash form if the process has run for more than 5 seconds.
            bool showSplash = showNow || stopWatch.ElapsedMilliseconds > 5000;

            if (!splashForm.Visible && showSplash)
            {
                this.Invoke(new Action(() =>
                {
                    this.Enabled = false;

                    splashForm.StartPosition = FormStartPosition.Manual;
                    splashForm.Location = new Point(
                        this.Location.X + (this.Width - splashForm.Width) / 2,
                        this.Location.Y + (this.Height - splashForm.Height) / 2);

                    if (!string.IsNullOrEmpty(prompt))
                    {
                        splashForm.Caption = prompt;
                    }

                    splashForm.Show(this); // center on our location
                    splashForm.Focus(); // force this to the top
                }));
            }
        }

        private bool UpdateDBRow(List<DataRow> rows)
        {
            // insert/update an existing set of rows
            if (rows.Count > 0)
            {
                OpenConnection(pilotsLogConn);

                try
                {
                    using (SQLiteCommand cmd = pilotsLogConn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT OR REPLACE INTO SystemLog VALUES (@ID,@Timestamp,@System,@Notes)";

                        cmd.Parameters.AddWithValue("@ID", 0);
                        cmd.Parameters.AddWithValue("@Timestamp", string.Empty);
                        cmd.Parameters.AddWithValue("@System", string.Empty);
                        cmd.Parameters.AddWithValue("@Notes", string.Empty);

                        try
                        {
                            using (SQLiteTransaction transaction = pilotsLogConn.BeginTransaction())
                            {
                                for (int i = rows.Count - 1; i >= 0; i--)
                                {
                                    DataRow row = rows[i];
                                    int var1 = int.Parse(row["ID"].ToString());
                                    string var2 = (row["Timestamp"] ?? string.Empty).ToString();
                                    string var3 = (row["System"] ?? string.Empty).ToString();
                                    string var4 = (row["Notes"] ?? string.Empty).ToString();

                                    cmd.Parameters["@ID"].Value = var1;
                                    cmd.Parameters["@Timestamp"].Value = var2;
                                    cmd.Parameters["@System"].Value = var3;
                                    cmd.Parameters["@Notes"].Value = var4;

                                    cmd.ExecuteNonQuery();
                                }

                                transaction.Commit();
                            }
                        }
                        catch (Exception) { throw; }

                        UpdateLocalTable();

                        return true; // success!
                    }
                }
                finally
                {
                    CloseConnection(pilotsLogConn);
                }
            }

            return false;
        }

        private void UpdateLocalTable()
        {
            try
            {
                OpenConnection(pilotsLogConn);

                if (HasValidColumn(pilotsLogConn, "SystemLog", "System"))
                {
                    try
                    {
                        using (SQLiteCommand cmd = pilotsLogConn.CreateCommand())
                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "SELECT * FROM SystemLog ORDER BY Timestamp DESC, System DESC, Notes DESC";
                            localTable.Locale = CultureInfo.InvariantCulture;

                            localTable.Rows.Clear();
                            adapter.Fill(localTable);

                            localSystemList.Clear();

                            foreach (DataRow r in localTable.Rows)
                            {
                                // convert datatable to systemlist
                                localSystemList.Add(new KeyValuePair<string, string>(r.Field<string>("Timestamp"), r.Field<string>("System")));
                            }

                            localTable.PrimaryKey = new DataColumn[] { localTable.Columns["ID"] };
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            finally
            {
                CloseConnection(pilotsLogConn);
            }
        }

        private bool UpdatePilotsLogDB(List<KeyValuePair<string, string>> exceptKey)
        {
            // here we take a non-intersect key to add systems to our DB
            int exceptCount = exceptKey.Count();

            if (exceptCount > 0)
            {
                OpenConnection(pilotsLogConn);

                using (SQLiteCommand cmd = pilotsLogConn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO SystemLog VALUES (null,@Timestamp,@System,null)";

                    cmd.Parameters.AddWithValue("@Timestamp", string.Empty);
                    cmd.Parameters.AddWithValue("@System", string.Empty);

                    using (SQLiteTransaction transaction = pilotsLogConn.BeginTransaction())
                    {
                        foreach (KeyValuePair<string, string> s in exceptKey)
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
                    }

                    InvalidatedRowUpdate(true, 0);

                    return true; // success!
                }
            }

            return false;
        }

        private void VacuumPilotsLogDB()
        {
            // a simple method to vacuum our database
            try
            {
                OpenConnection(pilotsLogConn);

                using (SQLiteCommand cmd = pilotsLogConn.CreateCommand())
                {
                    if (HasValidRows("SystemLog"))
                    {
                        // clean up after ourselves
                        cmd.CommandText = "VACUUM";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                CloseConnection(pilotsLogConn);
            }
        }

        private bool ValidateDBPath()
        {
            try
            {
                return CheckIfFileOpens(tdPath);
            }
            catch
            {
                throw new Exception("Cannot open the TradeDangerous database, this is fatal");
            }
        }
    }
}