using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDHelper
{
    /*
     * This class is a mostly stock implementation from: https://goo.gl/v03m8C
     */
    public interface IDataPageRetriever
    {
        DataTable SupplyPageOfData(int lowerPageBoundary, int rowsPerPage);
    }

    public class DataRetriever : IDataPageRetriever
    {
        private string tableName;
        private SQLiteCommand countCmd, selectCmd;
        private string columnToSortBy;
        private SQLiteDataAdapter adapter = new SQLiteDataAdapter();


        public DataRetriever(SQLiteConnection dbConn, string tableName)
        {
            if (dbConn != null && dbConn.State == ConnectionState.Closed)
                dbConn.Open();

            countCmd = dbConn.CreateCommand();
            selectCmd = dbConn.CreateCommand();
            this.tableName = tableName;
        }

        private int rowCountValue = -1;
        public int RowCount
        {
            get
            {
                // Retrieve the row count from the database.
                countCmd.CommandText = "SELECT COUNT(*) FROM " + tableName;
                rowCountValue = Convert.ToInt32(countCmd.ExecuteScalar());
                return rowCountValue;
            }
        }

        public void ResetRowCount()
        {// force an update of the row count
            rowCountValue = -1;
            rowCountValue = this.RowCount;
        }

        private DataColumnCollection columnsValue;

        public DataColumnCollection Columns
        {
            get
            {
                // Return the existing value if it has already been determined. 
                if (columnsValue != null)
                {
                    return columnsValue;
                }

                // Retrieve the column information from the database.
                selectCmd.CommandText = "SELECT * FROM " + tableName;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter();
                adapter.SelectCommand = selectCmd;
                DataTable table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                adapter.FillSchema(table, SchemaType.Source);
                columnsValue = table.Columns;
                return columnsValue;
            }
        }

        private string commaSeparatedListOfColumnNamesValue = null;

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

        public DataTable SupplyPageOfData(int lowerPageBoundary, int rowsPerPage)
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

            // Retrieve the specified number of rows from the database, starting 
            // with the row specified by the lowerPageBoundary parameter.
            selectCmd.CommandText = "SELECT * FROM " + tableName + " ORDER BY Timestamp DESC, System DESC, Notes DESC LIMIT @lowerPageBoundary,@rowsPerPage";
            selectCmd.Parameters.AddWithValue("@lowerPageBoundary", lowerPageBoundary);
            selectCmd.Parameters.AddWithValue("@rowsPerPage", rowsPerPage);
            adapter.SelectCommand = selectCmd;

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;

            if (this.RowCount > 0)
            {// fill our selected page
                adapter.Fill(table);
            }
            else
            {// make sure we populate with columns from the source
                DataColumnCollection supplyColumns = this.Columns;
                foreach (DataColumn c in supplyColumns)
                {
                    if (supplyColumns.Count > 0 && table.Columns.Count < supplyColumns.Count)
                        table.Columns.Add(c.ColumnName, c.DataType);
                }
            }

            return table;
        }

    }

    public class Cache
    {
        public DataTable table;
        private static int RowsPerPage;
        private DataPage[] cachePages;
        private DataRetriever dataSupply;

        // Represents one page of data.   
        public struct DataPage
        {
            public DataTable table;
            private int lowestIndexValue;
            private int highestIndexValue;

            public DataPage(DataTable table, int rowIndex)
            {
                this.table = table;
                if (table.Rows.Count > 0)
                {
                    lowestIndexValue = MapToLowerBoundary(rowIndex);
                    highestIndexValue = MapToUpperBoundary(rowIndex);
                }
                else
                {// make sure we return 0 indexes for an empty page
                    lowestIndexValue = 0;
                    highestIndexValue = 0;
                }

                System.Diagnostics.Debug.Assert(lowestIndexValue >= 0);
                System.Diagnostics.Debug.Assert(highestIndexValue >= 0);
            }

            public int LowestIndex
            {
                get
                {
                    return lowestIndexValue;
                }
            }

            public int HighestIndex
            {
                get
                {
                    return highestIndexValue;
                }
            }

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

        public Cache(DataRetriever dataSupplier, int rowsPerPage)
        {
            dataSupply = dataSupplier;
            Cache.RowsPerPage = rowsPerPage;
            LoadFirstTwoPages();
        }

        // Sets the value of the element parameter if the value is in the cache. 
        private bool IfPageCached_ThenSetElement(int rowIndex,
            int columnIndex, ref string element)
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

        public string RetrieveElement(int rowIndex, int columnIndex)
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

        private void LoadFirstTwoPages()
        {
            cachePages = new DataPage[]{
            new DataPage(dataSupply.SupplyPageOfData(DataPage.MapToLowerBoundary(0), RowsPerPage), 0), 
            new DataPage(dataSupply.SupplyPageOfData(DataPage.MapToLowerBoundary(RowsPerPage), RowsPerPage), RowsPerPage)};
        }

        private string RetrieveData_CacheIt_ThenReturnElement(
            int rowIndex, int columnIndex)
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

        public void RemoveRow(int rowIndex, int rowID)
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

        public void RemoveRows(List<int> rowIndexer, List<int> rowIDIndexer)
        {// batch our removes to prevent race conditions in Retrieve()
            if (rowIndexer.Count > 0 && rowIDIndexer.Count > 0)
            {
                for (int i = 0; i < rowIndexer.Count; i++)
                {
                    RemoveRow(rowIndexer[i], rowIDIndexer[i]);
                }
            }
        }

        // Returns a value indicating whether the given row index is contained 
        // in the given DataPage.  
        private bool IsRowCachedInPage(int pageNumber, int rowIndex)
        {
            return rowIndex <= cachePages[pageNumber].HighestIndex && rowIndex >= cachePages[pageNumber].LowestIndex;
        }

        private int GetIndexOfCachedRow(int rowIndex, int rowID)
        {
            int curID = -1;
            if (IsRowCachedInPage(0, rowIndex))
            {
                for (int i = 0; i < cachePages[0].table.Rows.Count; i++)
                {
                    curID = Convert.ToInt32(cachePages[0].table.Rows[i][0]);
                    if (curID == rowID)
                        return i;
                }
            }
            else if (IsRowCachedInPage(1, rowIndex))
            {
                for (int i = 0; i < cachePages[1].table.Rows.Count; i++)
                {
                    curID = Convert.ToInt32(cachePages[1].table.Rows[i][0]);
                    if (curID == rowID)
                        return i;
                }
            }

            return -1; // not found
        }
    }
}
