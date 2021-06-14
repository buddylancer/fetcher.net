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
    /// Implement operations with prepared statement.
    /// </summary>
    public class PreparedStatement : Bula.Meta {
        /// Link to database instance 
        private Object link;
        /// Initial SQL-query 
        private String sql;
        /// List of parameters 
        private ArrayList pars;
        /// Formed (prepared) SQL-query 
        private String query;

        /// <summary>
        /// Resulting record set of the last operation.
        /// </summary>
        /// @var RecordSet
        public RecordSet recordSet;

        /// Default public constructor 
        public PreparedStatement () {
            this.pars = new ArrayList();
            this.pars.Add("dummy"); // Parameter number will start from 1.
        }

        /// <summary>
        /// Execute selection query.
        /// </summary>
        public RecordSet ExecuteQuery() {
            this.recordSet = new RecordSet();
            if (this.FormQuery()) {
                DataAccess.CallPrintDelegate(CAT("Executing selection query [", this.query, "] ..."));
                var result = DataAccess.SelectQuery(this.link, this.query);
                if (result == null) {
                    DataAccess.CallErrorDelegate(CAT("Selection query failed [", this.query, "]"));
                    return null;
                }
                this.recordSet.result = result;
                this.recordSet.SetRows(DataAccess.NumRows(result));
                this.recordSet.SetPage(1);
                return this.recordSet;
            }
            else {
                DataAccess.CallErrorDelegate(CAT("Error in query: ", this.query, "<hr/>"));
                return null;
            }
        }

        /// <summary>
        /// Execute updating query.
        /// </summary>
        ///   -1 - error during form query.
        ///   -2 - error during execution.
        public int ExecuteUpdate() {
            if (this.FormQuery()) {
                DataAccess.CallPrintDelegate(CAT("Executing update query [", this.query, "] ..."));
                var result = DataAccess.UpdateQuery(this.link, this.query);
                if (result == null) {
                    DataAccess.CallErrorDelegate(CAT("Query update failed [", this.query, "]"));
                    return -2;
                }
                var ret = DataAccess.AffectedRows(this.link);
                return ret;
            }
            else {
                DataAccess.CallErrorDelegate(CAT("Error in update query [", this.query, "]"));
                return -1;
            }
        }

        /// <summary>
        /// Get ID for just inserted record.
        /// </summary>
        public int GetInsertId() {
            return DataAccess.InsertId(this.link);
        }

        /// <summary>
        /// Form query (replace '?' marks with real parameters).
        /// </summary>
        private Boolean FormQuery() {
            var questionIndex = -1;
            var startFrom = 0;
            var n = 1;
            var str = (String)this.sql;
            while ((questionIndex = str.IndexOf("?", startFrom)) != -1) {
                var value = (String)this.pars[n];
                var before = str.Substring(0, questionIndex);
                var after = str.Substring(questionIndex + 1);
                str = before; str += value; startFrom = str.Length;
                str += after;
                n++;
            }
            this.query = str;
            return true;
        }

        // Set parameter value
        private void SetValue(int n, String val) {
            if (n >= SIZE(this.pars))
                this.pars.Add(val);
            else
                this.pars[n] = val;
        }

        /// <summary>
        /// Set int parameter.
        /// </summary>
        /// <param name="n">Parameter number.</param>
        /// <param name="val">Parameter value.</param>
        public void SetInt(int n, int val) {
            SetValue(n, CAT(val));
        }

        /// <summary>
        /// Set String parameter.
        /// </summary>
        /// <param name="n">Parameter number.</param>
        /// <param name="val">Parameter value.</param>
        public void SetString(int n, String val) {
            SetValue(n, CAT("'", Strings.AddSlashes(val), "'"));
        }

        /// <summary>
        /// Set DateTime parameter.
        /// </summary>
        /// <param name="n">Parameter number.</param>
        /// <param name="val">Parameter value.</param>
        public void SetDate(int n, String val) {
            SetValue(n, CAT("'", DateTimes.Format(DateTimes.SQL_DTS, DateTimes.GetTime(val)), "'"));
        }

        /// <summary>
        /// Set float parameter.
        /// </summary>
        /// <param name="n">Parameter number.</param>
        /// <param name="val">Parameter value.</param>
        public void SetFloat(int n, Double val) {
            SetValue(n, CAT(val));
        }

        /// <summary>
        /// Close.
        /// </summary>
        public void Close() {
            this.link = null;
        }

        /// <summary>
        /// Set DB link.
        /// </summary>
        /// @param Object link
        public void SetLink(Object link) {
            this.link = link;
        }

        /// <summary>
        /// Set SQL-query,
        /// </summary>
        /// @param String sql
        public void SetSql(String sql) {
            this.sql = sql;
        }
    }
}