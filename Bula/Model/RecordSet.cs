// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Model {
    using System;
    using System.Collections;

    using System.Data;
    using MySql.Data.MySqlClient;
    using Bula.Objects;

    /// <summary>
    /// Implement operations with record sets.
    /// </summary>
    public class RecordSet : Bula.Meta {
        /// Current result 
        public Object result = null;
        /// Current record 
        public THashtable record = null;

        private int numRows = 0;
        private int numPages = 0;
        private int pageRows = 10;
        private int pageNo = 0;

        /// Public constructor 
        public RecordSet () {
            this.numRows = 0;
            this.numPages = 0;
            this.pageRows = 10;
            this.pageNo = 0;
        }

        /// <summary>
        /// Set number of page rows in record set.
        /// </summary>
        /// <param name="no">Number of rows.</param>
        public void SetPageRows(int no) {
            this.pageRows = no;
        }

        /// <summary>
        /// Set current number of rows (and pages) in the record set.
        /// </summary>
        /// <param name="no">Number of rows.</param>
        public void SetRows(int no) {
            this.numRows = no;
            this.numPages = INT((no - 1) / this.pageRows) + 1;
        }

        /// <summary>
        /// Get current number of rows in the record set.
        /// </summary>
        /// <returns>Number of rows.</returns>
        public int GetRows() {
            return this.numRows;
        }

        /// <summary>
        /// Get current number of pages in the record set.
        /// </summary>
        /// <returns>Number of pages.</returns>
        public int GetPages() {
            return this.numPages;
        }

        /// <summary>
        /// Set current page of the record set.
        /// </summary>
        /// <param name="no">Current page.</param>
        public void SetPage(int no) {
            this.pageNo = no;
            if (no != 1) {
                var n = (no - 1) * this.pageRows;
                while (n-- > 0)
                    this.Next();
            }
        }

        /// <summary>
        /// Get current page of the record set.
        /// </summary>
        /// <returns>Current page number.</returns>
        public int GetPage() {
            return this.pageNo;
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
                this.record = (THashtable)arr;
                return 1;
            }
            else
                return 0;
        }

        /// <summary>
        /// Get value from the record.
        /// </summary>
        /// <param name="par">Number of value.</param>
        public Object GetValue(String par) {
            return this.record[par];
        }

        /// <summary>
        /// Get String value from the record.
        /// </summary>
        /// <param name="par">Number of value.</param>
        public String GetString(String par) {
            return STR(this.record[par]);
        }

        /// <summary>
        /// Get DateTime value from the record.
        /// </summary>
        /// <param name="par">Number of value.</param>
        public String GetDate(String par) {
            return STR(this.record[par]);
        }

        /// <summary>
        /// Get integer value from the record.
        /// </summary>
        /// <param name="par">Number of value.</param>
        public int GetInt(String par) {
            return INT(this.record[par]);
        }

        /// <summary>
        /// Get real value from the record.
        /// </summary>
        /// <param name="par">Number of value.</param>
        public float GetFloat(String par) {
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