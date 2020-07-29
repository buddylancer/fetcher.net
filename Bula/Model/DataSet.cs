namespace Bula.Model {
    using System;

    using System.Collections;
    using Bula.Objects;

    /// <summary>
    /// Non-typed data set implementation.
    /// </summary>
    public class DataSet : Bula.Meta {
    	private ArrayList rows;
    	private int page_size;
    	private int total_pages;

    	/// Default public constructor 
        public DataSet () {
    		this.rows = new ArrayList();
    		this.page_size = 10;
    		this.total_pages = 0;
    	}

        /// <summary>
        /// Get the size (number of rows) of the DataSet.
        /// </summary>
        /// @return int 
    	public int GetSize() {
            return this.rows.Count;
        }

    	/// <summary>
    	/// Get a row from the DataSet.
    	/// </summary>
        /// <param name="n">Number of the row.</param>
        /// <returns>Required row or null.</returns>
        public Hashtable GetRow(int n) {
            return (Hashtable) this.rows[n];
        }

    	/// <summary>
    	/// Add new row into the DataSet.
    	/// </summary>
        /// <param name="row">New row to add.</param>
        public void AddRow(Hashtable row) {
            this.rows.Add(row);
        }

    	/// <summary>
    	/// Get page size of the DataSet.
    	/// </summary>
        /// <returns>Current page size.</returns>
        public int GetPageSize() {
            return this.page_size;
        }

    	/// <summary>
    	/// Set page size of the DataSet.
    	/// </summary>
        /// <param name="page_size">Current page size.</param>
    	public void SetPageSize(int page_size) {
            this.page_size = page_size;
        }

    	/// <summary>
    	/// Get total number of pages in the DataSet.
    	/// </summary>
        /// <returns>Number of pages.</returns>
        public int GetTotalPages() {
            return this.total_pages;
        }

    	/// <summary>
    	/// Set total number of pages in the DataSet.
    	/// </summary>
        /// <param name="total_pages">Number of pages.</param>
        public void SetTotalPages(int total_pages) {
            this.total_pages = total_pages;
        }

        private String AddSpaces(int level) {
            var spaces = "";
            for (int n = 0; n < level; n++)
                spaces += ("    ");
            return spaces;
        }

        /// <summary>
        /// Get serialized (XML) representation of the DataSet.
        /// </summary>
        /// <returns>Resulting representation.</returns>
        public String Serialize() {
            var level = 0;
            var spaces = (String)null;
            var output = "";
            output += (CAT("<DataSet Rows=\"", this.rows.Count, "\">\n"));
            for (int n = 0; n < this.GetSize(); n++) {
                var row = this.GetRow(n);
                level++; spaces = this.AddSpaces(level);
                output += (CAT(spaces, "<Row>\n"));
                var keys = row.Keys.GetEnumerator();
                while (keys.MoveNext()) {
                    level++; spaces = this.AddSpaces(level);
                    var key = (String)keys.Current;
                    output += (CAT(spaces, "<Item Name=\"", key, "\">"));
                    output += (row[key]);
                    output += ("</Item>\n");
                    level--; spaces = this.AddSpaces(level);
                }
                output += (CAT(spaces, "</Row>\n"));
                level--; spaces = this.AddSpaces(level);
            }
            output += ("</DataSet>\n");
            return output;
        }
    }
}