namespace Bula.Model {
    using System;

    using System.Data;
    using MySql.Data.MySqlClient;
    using System.Collections;

    using Bula.Objects;

    /// <summary>
    /// Implement operations with record sets.
    /// </summary>
    public class RecordSet : Bula.Meta {
        /// Current result 
        public Object result = null;
        /// Current record 
        public Hashtable record = null;

        private int num_rows = 0;
        private int num_pages = 0;
        private int page_rows = 10;
        private int page_no = 0;

        public RecordSet () {
            this.num_rows = 0;
            this.num_pages = 0;
            this.page_rows = 10;
            this.page_no = 0;
        }

        public void SetPageRows(int no) {
            this.page_rows = no;
        }

        /// <summary>
        /// Set current number of rows (and pages) in the record set.
        /// </summary>
        /// <param name="no">Number of rows.</param>
        public void SetRows(int no) {
            this.num_rows = no;
            this.num_pages = INT((no - 1) / this.page_rows) + 1;
        }

        /// <summary>
        /// Get current number of rows in the record set.
        /// </summary>
        /// <returns>Number of rows.</returns>
        public int GetRows() {
            return this.num_rows;
        }

        /// <summary>
        /// Get current number of pages in the record set.
        /// </summary>
        /// <returns>Number of pages.</returns>
        public int GetPages() {
            return this.num_pages;
        }

        /// <summary>
        /// Set current page of the record set.
        /// </summary>
        /// <param name="no">Current page.</param>
        public void SetPage(int no) {
            this.page_no = no;
            if (no != 1) {
                var n = (no - 1) * this.page_rows;
                while (n-- > 0)
                    this.Next();
            }
        }

        /// <summary>
        /// Get current page of the record set.
        /// </summary>
        /// <returns>Current page number.</returns>
        public int GetPage() {
            return this.page_no;
        }

        /// <summary>
        /// Get next record from the result of operation.
        /// </summary>
        /// <returns>Status of operation:</returns>
        ///   1 - next record exists.
        ///   0 - next record not exists.
        public int Next() {
            var arr = DataAccess.FetchArray(this.result);

            if (arr != null) {
                this.record = (Hashtable)arr;
                return 1;
            }
            else
                return 0;
        }

        /// <summary>
        /// Get value from the record.
        /// </summary>
        /// <param name="par">Number of value.</param>
        public Object GetValue(int par) {
            return this.record[par];
        }

        /// <summary>
        /// Get String value from the record.
        /// </summary>
        /// <param name="par">Number of value.</param>
        public String GetString(int par) {
            return STR(this.record[par]);
        }

        /// <summary>
        /// Get DateTime value from the record.
        /// </summary>
        /// <param name="par">Number of value.</param>
        public String GetDate(int par) {
            return STR(this.record[par]);
        }

        /// <summary>
        /// Get integer value from the record.
        /// </summary>
        /// <param name="par">Number of value.</param>
        public int GetInt(int par) {
            return INT(this.record[par]);
        }

        /// <summary>
        /// Get real value from the record.
        /// </summary>
        /// <param name="par">Number of value.</param>
        public Double GetFloat(int par) {
            return FLOAT(this.record[par]);
        }

        /// <summary>
        /// Close this record set.
        /// </summary>
        public void Close() {
            DataAccess.FreeResult(this.result);
        }
    }

}