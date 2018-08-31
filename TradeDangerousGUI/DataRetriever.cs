using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;

namespace TDHelper
{
    /*
     * This class is a mostly stock implementation from: https://goo.gl/v03m8C
     */

    public interface IDataPageRetriever
    {
        DataTable SupplyPageOfData(int lowerPageBoundary, int rowsPerPage);
    }

    public class Cache
    {
        public DataTable table;
        private static int RowsPerPage;
        private DataPage[] cachePages;
        private DataRetriever dataSupply;

        public Cache(
            DataRetriever dataSupplier,
            int rowsPerPage)
        {
            dataSupply = dataSupplier;
            Cache.RowsPerPage = rowsPerPage;
            LoadFirstTwoPages();
        }

        public void RemoveRow(
            int rowIndex,
            int rowID)
        {
            int foundIndex = -1;

            if (IsRowCachedInPage(0, rowIndex))
            {
                foundIndex = GetIndexOfCachedRow(rowIndex, rowID);

                if (foundIndex >= 0)
                {
                    cachePages[0].table.Rows[foundIndex].Delete();
                    cachePages[0].table.AcceptChanges();
                    this.dataSupply.ResetRowCount();
                }
            }
            else if (IsRowCachedInPage(1, rowIndex))
            {
                foundIndex = GetIndexOfCachedRow(rowIndex, rowID);

                if (foundIndex >= 0)
                {
                    cachePages[1].table.Rows[foundIndex].Delete();
                    cachePages[1].table.AcceptChanges();
                    this.dataSupply.ResetRowCount();
                }
            }
        }

        public void RemoveRows(
            List<int> rowIndexer,
            List<int> rowIDIndexer)
        {
            // batch our removes to prevent race conditions in Retrieve()
            if (rowIndexer.Count > 0 && rowIDIndexer.Count > 0)
            {
                for (int i = 0; i < rowIndexer.Count; i++)
                {
                    RemoveRow(rowIndexer[i], rowIDIndexer[i]);
                }
            }
        }

        public string RetrieveElement(
            int rowIndex,
            int columnIndex)
        {
            string element = null;

            if (this.dataSupply.RowCount > 0
                && IfPageCached_ThenSetElement(rowIndex, columnIndex, ref element))
            {
                return element;
            }
            else
            {
                return RetrieveData_CacheIt_ThenReturnElement(rowIndex, columnIndex);
            }
        }

        private int GetIndexOfCachedRow(
            int rowIndex,
            int rowID)
        {
            int curID = -1;

            if (IsRowCachedInPage(0, rowIndex))
            {
                for (int i = 0; i < cachePages[0].table.Rows.Count; i++)
                {
                    curID = Convert.ToInt32(cachePages[0].table.Rows[i][0]);

                    if (curID == rowID)
                    {
                        return i;
                    }
                }
            }
            else if (IsRowCachedInPage(1, rowIndex))
            {
                for (int i = 0; i < cachePages[1].table.Rows.Count; i++)
                {
                    curID = Convert.ToInt32(cachePages[1].table.Rows[i][0]);

                    if (curID == rowID)
                    {
                        return i;
                    }
                }
            }

            return -1; // not found
        }

        // Returns the index of the cached page most distant from the given index
        // and therefore least likely to be reused.
        private int GetIndexToUnusedPage(int rowIndex)
        {
            if (rowIndex > cachePages[0].HighestIndex &&
                rowIndex > cachePages[1].HighestIndex)
            {
                int offsetFromPage0 = rowIndex - cachePages[0].HighestIndex;
                int offsetFromPage1 = rowIndex - cachePages[1].HighestIndex;

                if (offsetFromPage0 < offsetFromPage1)
                {
                    return 1;
                }

                return 0;
            }
            else
            {
                int offsetFromPage0 = cachePages[0].LowestIndex - rowIndex;
                int offsetFromPage1 = cachePages[1].LowestIndex - rowIndex;

                if (offsetFromPage0 < offsetFromPage1)
                {
                    return 1;
                }

                return 0;
            }
        }

        // Sets the value of the element parameter if the value is in the cache.
        private bool IfPageCached_ThenSetElement(
            int rowIndex,
            int columnIndex,
            ref string element)
        {
            if (IsRowCachedInPage(0, rowIndex) && columnIndex < 4)
            {
                element = cachePages[0].table.Rows[rowIndex % RowsPerPage][columnIndex].ToString();
                return true;
            }
            else if (IsRowCachedInPage(1, rowIndex) && columnIndex < 4)
            {
                element = cachePages[1].table.Rows[rowIndex % RowsPerPage][columnIndex].ToString();
                return true;
            }

            return false;
        }

        // Returns a value indicating whether the given row index is contained
        // in the given DataPage.
        private bool IsRowCachedInPage(
            int pageNumber,
            int rowIndex)
        {
            return rowIndex <= cachePages[pageNumber].HighestIndex && rowIndex >= cachePages[pageNumber].LowestIndex;
        }

        private void LoadFirstTwoPages()
        {
            cachePages = new DataPage[]{
            new DataPage(dataSupply.SupplyPageOfData(DataPage.MapToLowerBoundary(0), RowsPerPage), 0),
            new DataPage(dataSupply.SupplyPageOfData(DataPage.MapToLowerBoundary(RowsPerPage), RowsPerPage), RowsPerPage)};
        }

        private string RetrieveData_CacheIt_ThenReturnElement(
            int rowIndex,
            int columnIndex)
        {
            // Retrieve a page worth of data containing the requested value.
            table = dataSupply.SupplyPageOfData(DataPage.MapToLowerBoundary(rowIndex), RowsPerPage);

            // Replace the cached page furthest from the requested cell
            // with a new page containing the newly retrieved data.
            string element = "";
            cachePages[GetIndexToUnusedPage(rowIndex)] = new DataPage(table, rowIndex);
            element = RetrieveElement(rowIndex, columnIndex);

            return element;
        }

        // Represents one page of data.
        public struct DataPage
        {
            public DataTable table;

            public DataPage(
                DataTable table,
                int rowIndex)
            {
                this.table = table;

                if (table.Rows.Count > 0)
                {
                    LowestIndex = MapToLowerBoundary(rowIndex);
                    HighestIndex = MapToUpperBoundary(rowIndex);
                }
                else
                {
                    // make sure we return 0 indexes for an empty page
                    LowestIndex = 0;
                    HighestIndex = 0;
                }

                System.Diagnostics.Debug.Assert(LowestIndex >= 0);
                System.Diagnostics.Debug.Assert(HighestIndex >= 0);
            }

            public int HighestIndex { get; }

            public int LowestIndex { get; }

            public static int MapToLowerBoundary(int rowIndex)
            {
                // Return the lowest index of a page containing the given index.
                return (rowIndex / RowsPerPage) * RowsPerPage;
            }

            private static int MapToUpperBoundary(int rowIndex)
            {
                // Return the highest index of a page containing the given index.
                return MapToLowerBoundary(rowIndex) + RowsPerPage - 1;
            }
        }
    }

    public class DataRetriever : IDataPageRetriever
    {
        private readonly string tableName;
        private SQLiteDataAdapter adapter = new SQLiteDataAdapter();
        private DataColumnCollection columnsValue;
        private string columnToSortBy;
        private string commaSeparatedListOfColumnNamesValue = null;
        private SQLiteCommand countCmd;
        private int rowCountValue = -1;
        private SQLiteCommand selectCmd;

        public DataRetriever(
            SQLiteConnection dbConn,
            string tableName)
        {
            countCmd = dbConn.CreateCommand();
            selectCmd = dbConn.CreateCommand();
            this.tableName = tableName;
        }

        public DataColumnCollection Columns
        {
            get
            {
                // Return the existing value if it has already been determined.
                if (columnsValue != null)
                {
                    return columnsValue;
                }

                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(selectCmd.Connection);

                    // Retrieve the column information from the database.
                    selectCmd.CommandText = "SELECT * FROM " + tableName;

                    SQLiteDataAdapter adapter = new SQLiteDataAdapter()
                    {
                        SelectCommand = selectCmd
                    };

                    DataTable table = new DataTable()
                    {
                        Locale = System.Globalization.CultureInfo.InvariantCulture
                    };

                    adapter.FillSchema(table, SchemaType.Source);
                    columnsValue = table.Columns;
                }
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(selectCmd.Connection);
                    }
                }

                return columnsValue;
            }
        }

        public int RowCount
        {
            get
            {
                bool isOpen = false;

                try
                {
                    isOpen = OpenConnection(countCmd.Connection);

                    // Retrieve the row count from the database.
                    countCmd.CommandText = "SELECT COUNT(*) FROM " + tableName;

                    rowCountValue = Convert.ToInt32(countCmd.ExecuteScalar());
                }
                finally
                {
                    if (!isOpen)
                    {
                        CloseConnection(countCmd.Connection);
                    }
                }

                return rowCountValue;
            }
        }

        private string CommaSeparatedListOfColumnNames
        {
            get
            {
                // Return the existing value if it has already been determined.
                if (commaSeparatedListOfColumnNamesValue != null)
                {
                    return commaSeparatedListOfColumnNamesValue;
                }

                // Store a list of column names for use in the
                // SupplyPageOfData method.
                System.Text.StringBuilder commaSeparatedColumnNames =
                    new System.Text.StringBuilder();
                bool firstColumn = true;

                foreach (DataColumn column in Columns)
                {
                    if (!firstColumn)
                    {
                        commaSeparatedColumnNames.Append(", ");
                    }

                    commaSeparatedColumnNames.Append(column.ColumnName);
                    firstColumn = false;
                }

                commaSeparatedListOfColumnNamesValue = commaSeparatedColumnNames.ToString();

                return commaSeparatedListOfColumnNamesValue;
            }
        }

        /// <summary>
        /// Carry out any pre-close operations and then close the connection.
        /// </summary>
        /// <param name="conn">The connection to be closed.</param>
        public void CloseConnection(SQLiteConnection conn)
        {
            try
            {
                if (conn != null &&
                    conn.State == ConnectionState.Open)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand("PRAGMA optimise", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Open the specified connection if required
        /// </summary>
        /// <param name="conn">The connection to be opened.</param>
        /// <returns>True if the initial state of the connection was open.</returns>
        public bool OpenConnection(SQLiteConnection conn)
        {
            bool isOpen = conn != null && conn.State == ConnectionState.Open;

            if (!isOpen)
            {
                conn.Open();
            }

            return isOpen;
        }

        public void ResetRowCount()
        {
            // force an update of the row count
            rowCountValue = -1;
            rowCountValue = this.RowCount;
        }

        public DataTable SupplyPageOfData(
            int lowerPageBoundary,
            int rowsPerPage)
        {
            // Store the name of the ID column. This column must contain unique
            // values so the SQL below will work properly.
            if (columnToSortBy == null)
            {
                columnToSortBy = this.Columns[0].ColumnName;
            }

            if (!this.Columns[columnToSortBy].Unique)
            {
                throw new InvalidOperationException(string.Format(
                    "Column {0} must contain unique values.", columnToSortBy));
            }

            DataTable table = new DataTable()
            {
                Locale = System.Globalization.CultureInfo.InvariantCulture
            };

            bool isOpen = false;

            try
            {
                isOpen = OpenConnection(selectCmd.Connection);

                // Retrieve the specified number of rows from the database, starting
                // with the row specified by the lowerPageBoundary parameter.
                selectCmd.CommandText = "SELECT * FROM " + tableName + " ORDER BY Timestamp DESC, System DESC, Notes DESC LIMIT @lowerPageBoundary,@rowsPerPage";

                selectCmd.Parameters.AddWithValue("@lowerPageBoundary", lowerPageBoundary);
                selectCmd.Parameters.AddWithValue("@rowsPerPage", rowsPerPage);

                adapter.SelectCommand = selectCmd;

                if (selectCmd.Connection.State != ConnectionState.Open)
                {
                    System.Diagnostics.Debugger.Break();
                }

                if (this.RowCount > 0)
                {
                    // fill our selected page
                    adapter.Fill(table);
                }
                else
                {
                    // make sure we populate with columns from the source
                    DataColumnCollection supplyColumns = this.Columns;

                    foreach (DataColumn c in supplyColumns)
                    {
                        if (supplyColumns.Count > 0 && table.Columns.Count < supplyColumns.Count)
                        {
                            table.Columns.Add(c.ColumnName, c.DataType);
                        }
                    }
                }
            }
            finally
            {
                if (!isOpen)
                {
                    CloseConnection(selectCmd.Connection);
                }
            }

            return table;
        }
    }
}