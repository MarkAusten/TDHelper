using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
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

        private static SQLiteConnection tdhConn = null;
        private static SQLiteConnection tdConn = null;

        //        public Cache memoryCache;
        private List<string> box_outputNames = new List<string>();

        private long LoadedRecords = 0;
        private DataTable localTable = new DataTable();
        private DataTable nonstn_table = new DataTable();
        private List<string> outputStationDetails = new List<string>();
        private List<string> outputStationShips = new List<string>();

        // Output
        private List<string> outputSysStnNames = new List<string>();

        private DataTable ship_table = new DataTable();

        // Inputs
        private DataTable stn_table = new DataTable();

        private DataTable stnship_table = new DataTable();


        #endregion Props

        /// <summary>
        /// Carry out any pre-close operations and then close the connection.
        /// </summary>
        /// <param name="conn">The connection to be closed.</param>
        public static void CloseConnection(SQLiteConnection conn)
        {
            if (conn != null &&
                conn.State == ConnectionState.Open)
            {
                //using (SQLiteCommand cmd = new SQLiteCommand("PRAGMA optimise", conn))
                //{
                //    cmd.ExecuteNonQuery();
                //}

                conn.Close();
            }
        }

        public static List<string> CollectLogPaths(
                    string path,
            string pattern)
        {
            CheckPilotsLogDatabaseSchema();

            // Get a list of the previously scanned net logs.
            IDictionary<string, NetLogModel> alreadyScanned = GetPreviouslyScanned();

            // only collect log paths that contain system names
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(path);
                SortedList<string, string> logPaths = new SortedList<string, string>();

                FileInfo[] fileList = dInfo.GetFiles(pattern);

                // check the make sure the directory is populated
                if (dInfo.Exists && fileList.Length > 0)
                {
                    using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
                    {
                        bool isOpen = false;
                        SQLiteTransaction transaction = null;

                        try
                        {
                            isOpen = OpenConnection(cmd.Connection);

                            using (transaction = cmd.Connection.BeginTransaction())
                            {
                                foreach (FileInfo f in fileList.OrderBy(f => f.LastWriteTime))
                                {
                                    string filename = f.ToString();
                                    string filePath = Path.Combine(path, filename);

                                    if (alreadyScanned.ContainsKey(filename))
                                    {
                                        NetLogModel model = alreadyScanned[filename];

                                        if (model.HasSystems)
                                        {
                                            logPaths.Add(model.HeaderTimestamp, filePath);
                                        }
                                    }
                                    else
                                    {
                                        string timestampHeader = string.Empty;

                                        using (TextReader reader = new StreamReader(filePath, Encoding.UTF8))
                                        {
                                            // check if there are any files that match the mask, return null if nothing matches
                                            string tempLines = reader.ReadToEnd(); // pull into memory
                                            Match timestampMatch = Regex.Match(tempLines, @"(\d{2,4}\-\d\d\-\d\d)[\s\-](\d\d:\d\d)");
                                            Match systemMatch = Regex.Match(tempLines, @"\{(\d\d:\d\d:\d\d).+System:""(.+)""");
                                            timestampHeader = string.Empty;

                                            if (timestampMatch.Success)
                                            {
                                                if (systemMatch.Success)
                                                {
                                                    timestampHeader
                                                        = timestampMatch.Groups[1].Value.ToString()
                                                        + " "
                                                        + systemMatch.Groups[1].Value.ToString();

                                                    logPaths.Add(timestampHeader, filePath);
                                                }
                                                else
                                                {
                                                    timestampHeader = timestampMatch.Groups[1].Value.ToString();
                                                }
                                            }

                                            AddToNetLogsTable(filename, systemMatch.Success && timestampMatch.Success, timestampHeader);
                                        }
                                    }
                                }

                                transaction.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Debug.WriteLine(ex.GetFullMessage());
                        }
                        finally
                        {
                            if (!isOpen)
                            {
                                CloseConnection(cmd.Connection);
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
        /// Open the specified connection if required
        /// </summary>
        /// <param name="conn">The connection to be opened.</param>
        /// <returns>True if the initial state of the connection was open.</returns>
        public static bool OpenConnection(SQLiteConnection conn)
        {
            bool isOpen = conn != null && conn.State == ConnectionState.Open;

            if (!isOpen)
            {
                conn.Open();
            }

            return isOpen;
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
        /// Add a new record to the NetLogs table.
        /// </summary>
        /// <param name="filename">The name of the netlog file.</param>
        /// <param name="hasSystems">True if the netlog contains at least one system.</param>
        /// <param name="headerTimestamp">The timestamp of the log header.</param>
        private static void AddToNetLogsTable(
            string filename,
            bool hasSystems,
            string headerTimestamp)
        {
            using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
            {
                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(cmd.Connection);

                    cmd.CommandText = "insert into NetLogs values (@Filename, @HasSystems, @HeaderTimestamp)";

                    cmd.Parameters.AddWithValue("@Filename", filename);
                    cmd.Parameters.AddWithValue("@HasSystems", (hasSystems ? 1 : 0));
                    cmd.Parameters.AddWithValue("@HeaderTimestamp", headerTimestamp);

                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    // We should not normally get here as the method id only called if the record does not exist.
                    // If there was an exception then the entry somehow already existed so do nothing.
                }
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(cmd.Connection);
                    }
                }
            }
        }

        /// <summary>
        /// Create a conection to the specified database file.
        /// </summary>
        /// <param name="path">The path to the database file to which the connection should be made.</param>
        /// <returns>An SQLite Connection.</returns>
        private static SQLiteConnection GetConnection(string path)
        {
            return new SQLiteConnection("Data Source=" + path + ";Version=3;FKSupport=true;");
        }

        /// <summary>
        /// Set the connections to the databases.
        /// </summary>
        private static void SetConnections()
        {
            tdPath = Path.Combine(settingsRef.TDPath, @"data\TradeDangerous.db");
            tdhPath = Path.Combine(assemblyPath, "TDHelper.db");

            tdConn = GetConnection(tdPath);
            tdhConn = GetConnection(tdhPath);
        }

        /// <summary>
        /// Create a command object from the connection or the named connection.
        /// </summary>
        /// <param name="conn">The connection to the database.</param>
        /// <param name="name">The identifier of the database if the connections have not yet been set up.</param>
        /// <returns>A command object.</returns>
        private static SQLiteCommand GetCommandObject(
            SQLiteConnection conn, 
            string name)
        {
            SQLiteConnection connection = conn;

            if (conn == null)
            {
                SetConnections();

                switch (name.ToUpper())
                {
                    case "TDH":
                        connection = tdhConn;
                        break;

                    case "TD":
                        connection = tdConn;
                        break;

                }
            }

            return connection.CreateCommand();
        }

        /// <summary>
        /// Ensure that the pilot's log DB has the correct schema.
        /// </summary>
        private static void CheckPilotsLogDatabaseSchema()
        {
            using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
            {
                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(cmd.Connection);

                    bool hasTable = HasTable(cmd.Connection, "SystemLog");

                    // Check the SystemLog schema.
                    if (!hasTable ||
                        !HasValidColumn(cmd.Connection, "SystemLog", "ID"))
                    {
                        // Either systemLog does not exist or it does not have an ID column.
                        if (hasTable)
                        {
                            // The table exists but does not have an ID column so we have to drop the table and recreate it
                            // as we cannot add a PK column to an existing SQLite table.
                            cmd.CommandText = "drop table SystemLog";
                            cmd.ExecuteNonQuery();
                        }

                        // SystemLog does not exist so create it entirely.
                        cmd.CommandText
                            = " create table SystemLog ("
                            + " ID integer primary key autoincrement,"
                            + " Timestamp nvarchar,"
                            + " System nvarchar,"
                            + " Notes nvarchar)";

                        cmd.ExecuteNonQuery();

                        // Create a unique index.
                        cmd.CommandText = "create unique index if not exists SystemTimestamp on SystemLog (System, Timestamp)";
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // The table exists so check the columns.
                        if (!HasValidColumn(cmd.Connection, "SystemLog", "Notes"))
                        {
                            // Notes column is missing so add it.
                            cmd.CommandText = "alter table SystemLog add column Notes nvarchar";
                            cmd.ExecuteNonQuery();
                        }

                        if (!HasValidColumn(cmd.Connection, "SystemLog", "System"))
                        {
                            // System column is missing so add it.
                            cmd.CommandText = "alter table SystemLog add column System nvarchar";
                            cmd.ExecuteNonQuery();
                        }

                        if (!HasValidColumn(cmd.Connection, "SystemLog", "Timestamp"))
                        {
                            // Timestamp column is missing so add it.
                            cmd.CommandText = "alter table SystemLog add column Timestamp nvarchar";
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Check the NetLogs schema.
                    hasTable = HasTable(cmd.Connection, "NetLogs");

                    if (!hasTable ||
                        !HasValidColumn(cmd.Connection, "NetLogs", "Filename"))
                    {
                        // Either NetLogs does not exist or it does not have an Filename column.
                        if (hasTable)
                        {
                            // The table exists but does not have an ID column so we have to drop the table and recreate it
                            // as we cannot add a PK column to an existing SQLite table.
                            cmd.CommandText = "drop table NetLogs";
                            cmd.ExecuteNonQuery();
                        }

                        // NetLogs does not exist so create it entirely.
                        cmd.CommandText
                            = " create table NetLogs ("
                            + " Filename nvarchar primary key,"
                            + " HasSystems int,"
                            + " HeaderTimestamp nvarchar)";

                        cmd.ExecuteNonQuery();

                        // Create an index.
                        cmd.CommandText = "create index if not exists FileHasSystems on NetLogs (HasSystems)";
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // The table exists so check the columns.
                        if (!HasValidColumn(cmd.Connection, "NetLogs", "HasSystems"))
                        {
                            // HasSystems column is missing so add it.
                            cmd.CommandText = "alter table NetLogs add column HasSystems int";
                            cmd.ExecuteNonQuery();
                        }

                        if (!HasValidColumn(cmd.Connection, "NetLogs", "HeaderTimestamp"))
                        {
                            // HeaderTimestamp column is missing so add it.
                            cmd.CommandText = "alter table NetLogs add column HeaderTimestamp nvarchar";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetFullMessage());
                }
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(cmd.Connection);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve a list of netlog filenames that have already been scanned.
        /// </summary>
        /// <returns>A lisst of netlog filenames and </returns>
        private static IDictionary<string, NetLogModel> GetPreviouslyScanned()
        {
            IDictionary<string, NetLogModel> result = new Dictionary<string, NetLogModel>();

            using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
            {
                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(cmd.Connection);

                    cmd.CommandText = "select Filename, HasSystems, HeaderTimestamp from NetLogs";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(
                                (string)reader[0],
                                new NetLogModel()
                                {
                                    Filename = (string)reader[0],
                                    HasSystems = (int)reader[1] == 1,
                                    HeaderTimestamp = (string)reader[2]
                                });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetFullMessage());
                }
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(cmd.Connection);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Check to see if the database has the expected table.
        /// </summary>
        /// <param name="conn">The connection to the database.</param>
        /// <param name="tableName">The name of the expected table.</param>
        /// <returns>True if the table exists.</returns>
        private static bool HasTable(
            SQLiteConnection conn,
            string tableName)
        {
            bool hasTable = false;

            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(cmd.Connection);

                    cmd.CommandText
                        = " select count(*) as total"
                        + " from sqlite_master"
                        + " where type = 'table' and "
                        + " name = '{0}'".With(tableName);

                    long output = (long)cmd.ExecuteScalar();

                    hasTable = output == 1;
                }
                //catch
                //{
                //    throw;
                //}
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(cmd.Connection);
                    }
                }
            }

            return hasTable;
        }

        private static bool HasValidColumn(
            SQLiteConnection conn,
            string tableName,
            string columnName)
        {
            // this method returns true if a column exists
            using (SQLiteCommand cmd = new SQLiteCommand("PRAGMA table_info(" + tableName + ")", conn))
            using (DataTable results = new DataTable())
            {
                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(cmd.Connection);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
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
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetFullMessage());
                }
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(cmd.Connection);
                    }
                }
            }

            return false;
        }

        private bool AddAtTimestampDBRow(string timestamp)
        {
            // add a blank row with the timestamp from a given row, basically an insert-below-index during select()
            using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
            {
                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(cmd.Connection);

                    cmd.CommandText = "INSERT INTO SystemLog VALUES (null,@Timestamp,null,null)";
                    DateTime tempTimestamp = new DateTime();

                    if (!string.IsNullOrEmpty(timestamp)
                        && DateTime.TryParseExact(timestamp, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempTimestamp))
                    {
                        string var1 = timestamp; // we were successful in parsing the timestamp, probably safe

                        using (var transaction = cmd.Connection.BeginTransaction())
                        {
                            cmd.Parameters.AddWithValue("@Timestamp", var1);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            transaction.Commit();
                        }

                        return true; // success!
                    }
                }
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(cmd.Connection);
                    }
                }
            }

            return false;
        }

        private bool AddDBRow()
        {
            // add a blank row with the current timestamp
            using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
            {
                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(cmd.Connection);

                    cmd.CommandText = "INSERT INTO SystemLog VALUES (null,@Timestamp,null,null)";

                    string var1 = CurrentTimestamp();

                    using (var transaction = cmd.Connection.BeginTransaction())
                    {
                        cmd.Parameters.AddWithValue("@Timestamp", var1);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        transaction.Commit();
                    }

                    LoadPilotsLogDB(); // need a full refresh

                    return true; // success!
                }
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(cmd.Connection);
                    }
                }
            }
        }

        /// <summary>
        /// Send the analyse command to the SQLite databases.
        /// </summary>
        private void AnalyseAllDatabases()
        {
            CloseConnection(tdConn);
            CloseConnection(tdhConn);

            AnalyseDatabase(GetConnection(tdPath));
            AnalyseDatabase(GetConnection(tdhPath));
        }

        /// <summary>
        /// Send the analyse command to the SQLite databases.
        /// </summary>
        /// <param name="conn">The connection to the database.</param>
        private void AnalyseDatabase(SQLiteConnection conn)
        {
            SendNonQueryCommandToDatabase("ANALYZE", conn);
        }

        private void BuildOutput(bool refreshMethod)
        {
            /* NOTE: This method should ALWAYS be called from the UI thread.
             *
             * The intent here is for all the generator methods to feed into a single
             * refreshable list instead of building straight to the combobox. This
             * should be faster and cleaner.
             */

            if (!dropdownOpened)
            {
                bool lockTaken = false;

                try
                {
                    Monitor.TryEnter(readNetLock, ref lockTaken);

                    if (lockTaken)
                    {
                        // discard the callback if we're busy
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

                                cboSourceSystem.AutoCompleteCustomSource.AddRange(outputSysStnNames.ToArray());
                            }));

                            hasRun = true; // set the semaphore so we don't hork our data tables
                        }

                        Debug.WriteLine("buildOutput combobox population took: " + m_timer.ElapsedMilliseconds + "ms");

                        m_timer.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetFullMessage());
                }
                finally
                {
                    if (hasRun)
                    {
                        testSystemsTimer.Start(); // start our background systems list updater
                    }

                    if (lockTaken)
                    {
                        Monitor.Exit(readNetLock);
                    }
                }
            }
        }

        private void BuildPilotsLog()
        {
            // Ensure that the database has the correct schema.
            CheckPilotsLogDatabaseSchema();

            // Populate the SystemLog tavble.
            PopulatePilotsLogDB();

            // here we either build or load our database
            if (grdPilotsLog.Rows.Count == 0)
            {
                if (HasTable(tdhConn, "SystemLog") &&
                    HasValidRows("SystemLog"))
                {
                    InvalidatedRowUpdate(true, -1);
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
                        Thread.Sleep(1000);
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

        private string GetLastTimestamp()
        {
            string timestamp = string.Empty;

            if (HasValidRows("SystemLog"))
            {
                using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
                {
                    bool isOpen = false;

                    try
                    {
                        isOpen = OpenConnection(cmd.Connection);

                        cmd.CommandText = "SELECT MAX(Timestamp) FROM SystemLog";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                timestamp = reader.GetString(0);
                            }
                        }
                    }
                    //catch
                    //{
                    //    throw;
                    //}
                    finally
                    {
                        if (!isOpen)
                        {
                            CloseConnection(cmd.Connection);
                        }
                    }
                }
            }

            return timestamp;
        }

        /// <summary>
        /// Get the total record count from the specified table.
        /// </summary>
        /// <param name="tableName">The name of the required table.</param>
        /// <returns>The total number of records in the table.</returns>
        private long GetTotalRecordCount(string tableName)
        {
            long total = 0;

            using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
            {
                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(cmd.Connection);

                    cmd.CommandText = "select count(*) from {0}".With(tableName);

                    total = (long)cmd.ExecuteScalar();
                }
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(cmd.Connection);
                    }
                }
            }

            return total;
        }

        private void GrabStationData(
            string inputSystem, 
            string inputStation)
        {
            /*
             * We use this method to grab station data on the fly for the station
             * editor panel as a single row
             */

            if (ValidateDBPath())
            {
                Stopwatch m_timer = Stopwatch.StartNew();

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
                    using (SQLiteCommand cmd = GetCommandObject(tdConn, "TD"))
                    {
                        bool isOpen = false;

                        try
                        {
                            isOpen = OpenConnection(cmd.Connection);

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
                        finally
                        {
                            if (!isOpen)
                            {
                                CloseConnection(cmd.Connection);
                            }
                        }
                    }

                    outputStationDetails = new List<string>();
                    outputStationShips = new List<string>();

                    FilterStationData();

                    Debug.WriteLine("grabStationData query took: " + m_timer.ElapsedMilliseconds + "ms");

                    m_timer.Stop();
                }
            }
        }

        /// <summary>
        /// Determine if there are any rows in the specified table.
        /// </summary>
        /// <param name="tableName">The name of the required table.</param>
        /// <returns>True if the table has any rows.</returns>
        private bool HasValidRows(string tableName)
        {
            return GetTotalRecordCount(tableName) != 0;
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
            }

            // partial refresh
            UpdateLocalTable();

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
                // Issue a 'BEGIN IMMEDIATE' command
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    bool isOpen = false;

                    try
                    {
                        isOpen = OpenConnection(cmd.Connection);

                        cmd.CommandText = "begin immediate";

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
                    finally
                    {
                        if (!isOpen)
                        {
                            CloseConnection(cmd.Connection);
                        }
                    }
                }
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

                Stopwatch m_timer = Stopwatch.StartNew();

                // wipe to prevent duplicates
                if (stn_table.Rows.Count != 0)
                {
                    stn_table = new DataTable(); // this is O(1) performant
                    nonstn_table = new DataTable(); // this is O(1) performant
                }

                // match on System.system_id = Station.system_id, output in "System | Station" format
                using (SQLiteCommand cmd = GetCommandObject(tdConn, "TD"))
                {
                    bool isOpen = false;

                    try
                    {
                        isOpen = OpenConnection(cmd.Connection);

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
                    finally
                    {
                        if (!isOpen)
                        {
                            CloseConnection(cmd.Connection);
                        }
                    }
                }

                Debug.WriteLine("loadDatabase query took: " + m_timer.ElapsedMilliseconds + "ms");

                m_timer.Stop();

                outputSysStnNames = new List<string>(); // wipe before we fill

                FilterDatabase(); // filter and fill our output
            }
        }

        /// <summary>
        /// Load the next 50 visited systems.
        /// </summary>
        private void LoadNext50VisitedSystems()
        {
            UpdateLocalTable(LoadedRecords + 50);
        }

        /// <summary>
        /// Load all the visited systems.
        /// </summary>
        private void LoadAllVisitedSystems()
        {
            UpdateLocalTable(GetTotalRecordCount("SystemLog"));
        }

        private void LoadPilotsLogDB()
        {
            if (HasValidColumn(tdhConn, "SystemLog", "System"))
            {
                try
                {
                    UpdateLocalTable();

                    grdPilotsLog.DataSource = null;
                    grdPilotsLog.DataSource = localTable;

                    grdPilotsLog.Refresh();

                    if (grdPilotsLog.RowCount > 0)
                    {
                        grdPilotsLog.Columns["ID"].Visible = false;

                        grdPilotsLog.Columns["Timestamp"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                        grdPilotsLog.Columns["Timestamp"].ReadOnly = true;

                        grdPilotsLog.Columns["System"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                        grdPilotsLog.Columns["System"].ReadOnly = true;

                        grdPilotsLog.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        grdPilotsLog.Columns["Notes"].ReadOnly = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetFullMessage());
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

            Stopwatch stopWatch2 = new Stopwatch();

            stopWatch2.Start();

            // this method initially populates the recent systems and pilot's log in the absence of a DB
            // grab the timestamp of this particular netlog
            string fileBuffer = string.Empty;
            string pattern0 = @"(\d{2,4}\-\d\d\-\d\d)[\s\-](\d\d:\d\d)"; // $1=Y, $2=M, $3=D;

            // grab the timestamp of this entry, and then the system name
            string pattern1 = @"\{(\d\d:\d\d:\d\d).+System:""(.+)"""; // $1=localtime, $2=system

            List<string> output = new List<string>(); // a list for our system names
            List<KeyValuePair<string, string>> netLogOutput = new List<KeyValuePair<string, string>>();
            string logDatestamp = string.Empty;

            if (filePaths.Count > 0 && 
                !string.IsNullOrEmpty(filePaths[0]) && 
                refreshMode)
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

                SetSplashScreenStatus("Reading Net Logs...");

                foreach (string path in latestLogPaths.Skip(latestLogPaths.Count - fileCount).ToList())
                {
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

            return output; // return our finished list
        }

        /// <summary>
        /// Optimise all the databases.
        /// </summary>
        private void OptimiseAllDatabases()
        {
            OptimiseDatabase(tdhConn);
            OptimiseDatabase(tdConn);
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

            // Check to see if the items file is available.
            if (CheckIfFileOpens(t_itemListPath))
            {
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
            }

            commodities.Clear();

            // check to se if the ships file is available.
            if (CheckIfFileOpens(t_shipListPath))
            {
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
            }

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

        private void PopulatePilotsLogDB()
        {
            if (netLogOutput.Count > 0)
            {
                using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
                {
                    bool isOpen = false;

                    try
                    {
                        isOpen = OpenConnection(cmd.Connection);

                        using (var transaction = tdhConn.BeginTransaction())
                        {
                            // always do our inserts in a batch for ideal performance
                            cmd.CommandText = @"INSERT INTO SystemLog (Timestamp, System) VALUES null,@Timestamp,@System,null)";

                            cmd.Parameters.AddWithValue("@Timestamp", string.Empty);
                            cmd.Parameters.AddWithValue("@System", string.Empty);

                            foreach (var s in netLogOutput)
                            {
                                cmd.Parameters["@Timestamp"].Value = s.Key;
                                cmd.Parameters["@System"].Value = s.Value;

                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.GetFullMessage());
                    }
                    finally
                    {
                        if (!isOpen)
                        {
                            CloseConnection(cmd.Connection);
                        }
                    }
                }
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
                                SplashScreen.SetStatus("Building database...");
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
                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM SystemLog WHERE ID = @ID", tdhConn))
                {
                    bool isOpen = false;

                    try
                    {
                        OpenConnection(cmd.Connection);

                        var transaction = tdhConn.BeginTransaction();
                        isOpen = OpenConnection(cmd.Connection);

                        // delete all the rows specified by the index, using a transaction for efficiency
                        cmd.Parameters.AddWithValue("@ID", rowIndex);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        transaction.Commit();

                        VacuumPilotsLogDB();
                    }
                    finally
                    {
                        if (!isOpen)
                        {
                            CloseConnection(cmd.Connection);
                        }
                    }

                    return true; // success!
                }
            }

            return false;
        }

        private bool RemoveDBRows(List<int> rowsIndex)
        {
            // remove a batch of rows (faster than removeDBRow)
            if (rowsIndex.Count > 0)
            {
                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM SystemLog WHERE ID = @ID", tdhConn))
                {
                    bool isOpen = false;

                    try
                    {
                        isOpen = OpenConnection(cmd.Connection);

                        using (SQLiteTransaction transaction = tdhConn.BeginTransaction())
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
                    finally
                    {
                        if (!isOpen)
                        {
                            CloseConnection(cmd.Connection);
                        }
                    }

                    return true; // success!
                }
            }

            return false;
        }

        /// <summary>
        /// Send the non-query command to the SQLite databases.
        /// </summary>
        /// <param name="command">The command to be issued to the database.</param>
        /// <param name="conn">The connection to the database.</param>
        private void SendNonQueryCommandToDatabase(
            string command,
            SQLiteConnection conn)
        {
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(cmd.Connection);

                    cmd.CommandText = command;

                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(cmd.Connection);
                    }
                }
            }
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

        private bool UpdateDBRow(List<DataRow> rows)
        {
            // insert/update an existing set of rows
            if (rows.Count > 0)
            {
                using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
                {
                    bool isOpen = false;

                    try
                    {
                        isOpen = OpenConnection(cmd.Connection);

                        cmd.CommandText = "INSERT OR REPLACE INTO SystemLog VALUES (@ID,@Timestamp,@System,@Notes)";

                        cmd.Parameters.AddWithValue("@ID", 0);
                        cmd.Parameters.AddWithValue("@Timestamp", string.Empty);
                        cmd.Parameters.AddWithValue("@System", string.Empty);
                        cmd.Parameters.AddWithValue("@Notes", string.Empty);

                        using (SQLiteTransaction transaction = tdhConn.BeginTransaction())
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

                        UpdateLocalTable();

                        return true; // success!
                    }
                    finally
                    {
                        if (!isOpen)
                        {
                            CloseConnection(cmd.Connection);
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Load the loca table that is the Datasource for the pliot's log grid.
        /// </summary>
        /// <param name="recordCount">The number of records to load.</param>
        private void UpdateLocalTable(long recordCount = 50)
        {
            if (HasValidColumn(tdhConn, "SystemLog", "System"))
            {
                using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                {
                    bool isOpen = false;

                    try
                    {
                        isOpen = OpenConnection(cmd.Connection);

                        cmd.CommandText
                            = " select *"
                            + " from SystemLog"
                            + " order by"
                            + "     Timestamp desc,"
                            + "     System desc,"
                            + "     Notes desc"
                            + " limit {0}".With(recordCount);

                        localTable.Locale = CultureInfo.InvariantCulture;

                        localTable.Rows.Clear();
                        adapter.Fill(localTable);

                        LoadedRecords = localTable.Rows.Count;

                        localSystemList.Clear();

                        foreach (DataRow r in localTable.Rows)
                        {
                            // convert datatable to systemlist
                            localSystemList.Add(new KeyValuePair<string, string>(
                                r.Field<string>("Timestamp"),
                                r.Field<string>("System")));
                        }

                        localTable.PrimaryKey = new DataColumn[] { localTable.Columns["ID"] };
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.GetFullMessage());
                    }
                    finally
                    {
                        if (!isOpen)
                        {
                            CloseConnection(cmd.Connection);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update the notes for the specified record.
        /// </summary>
        /// <param name="id">The ID of the record to be updated.</param>
        /// <param name="notes">The notes to be saved.</param>
        private void UpdateNotes(
            int id,
            string notes)
        {
            using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
            {
                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(cmd.Connection);

                    cmd.CommandText = "update SystemLog set Notes = @Notes where ID = @ID";

                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Notes", notes);

                    using (SQLiteTransaction transaction = cmd.Connection.BeginTransaction())
                    {
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }

                    UpdateLocalTable();
                }
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(cmd.Connection);
                    }
                }
            }
        }

        private bool UpdatePilotsLogDB(List<KeyValuePair<string, string>> exceptKey)
        {
            // here we take a non-intersect key to add systems to our DB
            int exceptCount = exceptKey.Count();

            if (exceptCount > 0)
            {
                using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
                {
                    bool isOpen = false;

                    try
                    {
                        isOpen = OpenConnection(cmd.Connection);

                        cmd.CommandText = "INSERT INTO SystemLog VALUES (null,@Timestamp,@System,null)";

                        cmd.Parameters.AddWithValue("@Timestamp", string.Empty);
                        cmd.Parameters.AddWithValue("@System", string.Empty);

                        using (SQLiteTransaction transaction = tdhConn.BeginTransaction())
                        {
                            foreach (KeyValuePair<string, string> s in exceptKey)
                            {
                                cmd.Parameters["@Timestamp"].Value = s.Key;
                                cmd.Parameters["@System"].Value = s.Value;

                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                    }
                    finally
                    {
                        if (!isOpen)
                        {
                            CloseConnection(cmd.Connection);
                        }
                    }

                    InvalidatedRowUpdate(true, 0);

                    return true; // success!
                }
            }

            return false;
        }

        /// <summary>
        /// Send the vacuum command to the SQLite databases.
        /// </summary>
        private void VacuumAllDatabases()
        {
            CloseConnection(tdConn);
            CloseConnection(tdhConn);

            VacuumDatabase(GetConnection(tdPath));
            VacuumDatabase(GetConnection(tdhPath));
        }

        /// <summary>
        /// Send the vacuum command to the SQLite databases.
        /// </summary>
        /// <param name="conn">The connection to the database.</param>
        private void VacuumDatabase(SQLiteConnection conn)
        {
            SendNonQueryCommandToDatabase("VACUUM", conn);
        }

        private void VacuumPilotsLogDB()
        {
            // a simple method to vacuum our database
            using (SQLiteCommand cmd = GetCommandObject(tdhConn, "TDH"))
            {
                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(cmd.Connection);

                    if (HasValidRows("SystemLog"))
                    {
                        // clean up after ourselves
                        cmd.CommandText = "VACUUM";
                        cmd.ExecuteNonQuery();
                    }
                }
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(cmd.Connection);
                    }
                }
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