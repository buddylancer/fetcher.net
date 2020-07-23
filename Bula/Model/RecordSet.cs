namespace Bula.Model {
    using System;

    using System.Data;
    using MySql.Data.MySqlClient;
    using System.Collections;

    using Bula.Objects;

    public class RecordSet : Bula.Meta {
        public Object result = null;
        public int num_rows = 0;
        public int num_pages = 0;
        public int page_rows = 10;
        public int page_no = 0;
        public Hashtable record = null;

        public RecordSet () {
            this.num_rows = 0;
            this.num_pages = 0;
            this.page_rows = 10;
            this.page_no = 0;
        }

        public void SetPageRows(int no) {
            this.page_rows = no;
        }

        public void SetRows(int no) {
            this.num_rows = no;
            this.num_pages = INT(no/this.page_rows) + 1;
        }

        public int GetRows() {
            return this.num_rows;
        }

        public int GetPages() {
            return this.num_pages;
        }

        public void SetPage(int pNo) {
            this.page_no = pNo;
            if (pNo != 1) {
                int n = (pNo - 1) * this.page_rows;
                while (n-- > 0)
                    this.Next();
            }
        }

        public int GetPage() {
            return this.page_no;
        }

        public int Next() {
            Object arr = Mysqli_fetch_array(this.result);

            if (arr != null) {
                this.record = (Hashtable)arr;
                return 1;
            }
            else
                return 0;
        }

        public Object GetValue(int par) {
            return this.record[par];
        }

        public String GetString(int par) {
            return STR(this.record[par]);
        }

        public String GetDate(int par) {
            return STR(this.record[par]);
        }

        public int GetInt(int par) {
            return INT(this.record[par]);
        }

        public Double GetFloat(int par) {
            return FLOAT(this.record[par]);
        }

        public void Close() {
            Mysqli_free_result(this.result);
        }

        private Hashtable Mysqli_fetch_array(Object result) {
            int pointer = (int)((Object[])result)[0];
            System.Data.DataSet ds = (System.Data.DataSet)((Object[])result)[1];
            if (pointer >= ds.Tables[0].Rows.Count)
                return null; // No more rows to fetch

            Hashtable hash = new Hashtable();
            System.Data.DataRow row = ds.Tables[0].Rows[pointer];
            for (int n = 0; n < row.Table.Columns.Count; n++) {
                Object obj = row.ItemArray.GetValue(n);
                hash.Add(row.Table.Columns[n].ColumnName, obj);
            }
            ((Object[])result)[0] = ++pointer;
            return hash;
        }

        private void Mysqli_free_result(Object result) {
            System.Data.DataSet ds = (System.Data.DataSet)((Object[])result)[1];
            ds.Dispose();
            ((Object[])result)[0] = 0;
            ((Object[])result)[1] = null;
        }
    }

}