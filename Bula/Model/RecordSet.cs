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
            this.num_pages = INT(no / this.page_rows) + 1;
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
                var n = (pNo - 1) * this.page_rows;
                while (n-- > 0)
                    this.Next();
            }
        }

        public int GetPage() {
            return this.page_no;
        }

        ///Get next record from the result of operation.
        /// <returns>Status of operation:</returns>
        ///   1 - next record exists
        ///   0 - next record not exists
        public int Next() {
            var arr = DataAccess.FetchArray(this.result);

            if (arr != null) {
                this.record = (Hashtable)arr;
                return 1;
            }
            else
                return 0;
        }

        ///Get value from the record.
        /// <param name="par">Number of value.</param>
        public Object GetValue(int par) {
            return this.record[par];
        }

        ///Get String value from the record.
        /// <param name="par">Number of value.</param>
        public String GetString(int par) {
            return STR(this.record[par]);
        }

        ///Get DateTime value from the record.
        /// <param name="par">Number of value.</param>
        public String GetDate(int par) {
            return STR(this.record[par]);
        }

        ///Get integer value from the record.
        /// <param name="par">Number of value.</param>
        public int GetInt(int par) {
            return INT(this.record[par]);
        }

        ///Get real value from the record.
        /// <param name="par">Number of value.</param>
        public Double GetFloat(int par) {
            return FLOAT(this.record[par]);
        }

        ///Close this RecordSet.
        public void Close() {
            DataAccess.FreeResult(this.result);
        }
    }

}