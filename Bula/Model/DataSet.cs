namespace Bula.Model {
    using System;

    using System.Collections;
    using Bula.Objects;

    // Non-typed data set
    public class DataSet : Bula.Meta {
    	private ArrayList rows;
    	private int page_size;
    	private int total_pages;

    	public DataSet () {
    		this.rows = new ArrayList();
    		this.page_size = 10;
    		this.total_pages = 0;
    	}

        ///Get the size (number of rows) of the DataSet.
        /// @return int 
    	public int GetSize() {
            return this.rows.Count;
        }

    	///Get a row from the DataSet.
        /// <param name="n">Number of the row.</param>
        /// <returns>Required row or null.</returns>
        public Hashtable GetRow(int n) {
            return (Hashtable) this.rows[n];
        }

    	///Add new row into the DataSet.
        /// <param name="row">New row to add.</param>
        public void AddRow(Hashtable row) {
            this.rows.Add(row);
        }

    	///Get page size of the DataSet.
        /// <returns>Current page size.</returns>
        public int GetPageSize() {
            return this.page_size;
        }
    	public void SetPageSize(int page_size) {
            this.page_size = page_size;
        }

    	public int GetTotalPages() {
            return this.total_pages;
        }
    	public void SetTotalPages(int total_pages) {
            this.total_pages = total_pages;
        }

        private String AddSpaces(int level) {
            var spaces = "";
            for (int n = 0; n < level; n++)
                spaces += ("    ");
            return spaces;
        }

        ///Get serialized (XML) representation of the DataSet.
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