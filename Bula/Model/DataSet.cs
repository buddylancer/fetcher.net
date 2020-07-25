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

    	public int GetSize() {
            return this.rows.Count;
        }

    	public Hashtable GetRow(int n) {
            return (Hashtable) this.rows[n];
        }
    	public void AddRow(Hashtable row) {
            this.rows.Add(row);
        }

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
            String spaces = "";
            for (int n = 0; n < level; n++)
                spaces += ("    ");
            return spaces;
        }

        public String Serialize() {
            int level = 0;
            String spaces = null;
            String output = "";
            output += ("<DataSet>\n");
            for (int n = 0; n < this.GetSize(); n++) {
                Hashtable row = this.GetRow(n);
                level++; spaces = this.AddSpaces(level);
                output += (CAT(spaces, "<Row>\n"));
                IEnumerator keys = row.Keys.GetEnumerator();
                while (keys.MoveNext()) {
                    level++; spaces = this.AddSpaces(level);
                    String key = (String)keys.Current;
                    output += (CAT(spaces, "<Item name=\"", key, "\">"));
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