// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Model {
    using System;
    using System.Collections;

    using Bula.Objects;

    /// <summary>
    /// Non-typed data set implementation.
    /// </summary>
    public class DataSet : Bula.Meta {
        private TArrayList rows;
        private int pageSize;
        private int totalPages;

        /// Default public constructor 
        public DataSet () {
            this.rows = new TArrayList();
            this.pageSize = 10;
            this.totalPages = 0;
        }

        /// <summary>
        /// Get the size (number of rows) of the DataSet.
        /// </summary>
        /// <returns>DataSet size.</returns>
        public int GetSize() {
            return this.rows.Size();
        }

        /// <summary>
        /// Get a row from the DataSet.
        /// </summary>
        /// <param name="n">Number of the row.</param>
        /// <returns>Required row or null.</returns>
        public THashtable GetRow(int n) {
            return (THashtable) this.rows[n];
        }

        /// <summary>
        /// Add new row into the DataSet.
        /// </summary>
        /// <param name="row">New row to add.</param>
        public void AddRow(THashtable row) {
            this.rows.Add(row);
        }

        /// <summary>
        /// Get page size of the DataSet.
        /// </summary>
        /// <returns>Current page size.</returns>
        public int GetPageSize() {
            return this.pageSize;
        }

        /// <summary>
        /// Set page size of the DataSet.
        /// </summary>
        /// <param name="pageSize">Current page size.</param>
        public void SetPageSize(int pageSize) {
            this.pageSize = pageSize;
        }

        /// <summary>
        /// Get total number of pages in the DataSet.
        /// </summary>
        /// <returns>Number of pages.</returns>
        public int GetTotalPages() {
            return this.totalPages;
        }

        /// <summary>
        /// Set total number of pages in the DataSet.
        /// </summary>
        /// <param name="totalPages">Number of pages.</param>
        public void SetTotalPages(int totalPages) {
            this.totalPages = totalPages;
        }

        private String AddSpaces(int level) {
            var spaces = "";
            for (int n = 0; n < level; n++)
                spaces += "    ";
            return spaces;
        }

        /// <summary>
        /// Get serialized (XML) representation of the DataSet.
        /// </summary>
        /// <returns>Resulting representation.</returns>
        public String ToXml(String EOL) {
            var level = 0;
            var spaces = (String)null;
            var output = "";
            output += CAT("<DataSet Rows=\"", this.rows.Size(), "\">", EOL);
            for (int n = 0; n < this.GetSize(); n++) {
                var row = this.GetRow(n);
                level++; spaces = this.AddSpaces(level);
                output += CAT(spaces, "<Row>", EOL);
                var keys = new TEnumerator(row.Keys.GetEnumerator());
                while (keys.MoveNext()) {
                    level++; spaces = this.AddSpaces(level);
                    var key = (String)keys.GetCurrent();
                    var value = row[key];
                    if (NUL(value)) {
                        output += CAT(spaces, "<Item Name=\"", key, "\" IsNull=\"True\" />", EOL);
                    }
                    else {
                        output += CAT(spaces, "<Item Name=\"", key, "\">");
                        output += STR(row[key]);
                        output += CAT("</Item>", EOL);
                    }
                    level--; spaces = this.AddSpaces(level);
                }
                output += CAT(spaces, "</Row>", EOL);
                level--; spaces = this.AddSpaces(level);
            }
            output += CAT("</DataSet>", EOL);
            return output;
        }
    }
}