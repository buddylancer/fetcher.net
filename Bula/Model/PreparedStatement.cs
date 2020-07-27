namespace Bula.Model {
    using System;

    using System.Data;
    using MySql.Data.MySqlClient;
    using System.Collections;

    using Bula.Objects;

    public class PreparedStatement : Bula.Meta {
        private Object link; // Link to database instance
        private String sql; // Initial SQL-query
        private ArrayList pars; // List of parameters
        private String query; // Formed SQL-query

        ///Resulting record set of the last operation.
        /// @var RecordSet
        public RecordSet record_set;

        public PreparedStatement () {
            this.pars = new ArrayList();
            this.pars.Add("dummy"); // Parameter number will start from 1.
        }

        ///Execute selection query.
        public RecordSet ExecuteQuery() {
            this.record_set = new RecordSet();
            if (this.FormQuery()) {
                DataAccess.CallPrintDelegate(CAT("Executing selection query [", this.query, "] ..."));
                var result = DataAccess.SelectQuery(this.link, this.query);
                if (result == null) {
                    DataAccess.CallErrorDelegate(CAT("Selection query failed [", this.query, "]"));
                    return null;
                }
                this.record_set.result = result;
                this.record_set.SetRows(DataAccess.NumRows(result));
                this.record_set.SetPage(1);
                return this.record_set;
            }
            else {
                DataAccess.CallErrorDelegate(CAT("Error in query: ", this.query, "<hr/>"));
                return null;
            }
        }

        ///Execute updating query.
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

        ///Get ID for just inserted record.
        public int GetInsertId() {
            return DataAccess.InsertId(this.link);
        }

        ///Form query (replace '?' marks with real parameters).
        private Boolean FormQuery() {
            var question_index = -1;
            var start_from = 0;
            var n = 1;
            var str = this.sql;
            while ((question_index = str.IndexOf("?", start_from)) != -1) {
                var value = (String)this.pars[n];
                var before = str.Substring(0, question_index);
                var after = str.Substring(question_index + 1);
                str = before; str += (value); start_from = str.Length;
                str += (after);
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

        ///Set int parameter.
        /// <param name="n">Parameter number.</param>
        /// <param name="val">Parameter value.</param>
        public void SetInt(int n, int val) {
            SetValue(n, CAT(val));
        }

        ///Set String parameter.
        /// <param name="n">Parameter number.</param>
        /// <param name="val">Parameter value.</param>
        public void SetString(int n, String val) {
            SetValue(n, CAT("'", Strings.AddSlashes(val), "'"));
        }

        ///Set DateTime parameter.
        /// <param name="n">Parameter number.</param>
        /// <param name="val">Parameter value.</param>
        public void SetDate(int n, String val) {
            SetValue(n, CAT("'", DateTimes.Format(DBConfig.SQL_DTS, DateTimes.GetTime(val)), "'"));
        }

        ///Set Float parameter.
        /// <param name="n">Parameter number.</param>
        /// <param name="val">Parameter value.</param>
        public void SetFloat(int n, Double val) {
            SetValue(n, CAT(val));
        }

        ///Close.
        public void Close() {
            this.link = null;
        }

        ///Set DB link.
        /// @param Object link
        public void SetLink(Object link) {
            this.link = link;
        }

        ///Set SQL-query,
        /// @param String sql
        public void SetSql(String sql) {
            this.sql = sql;
        }
    }
}