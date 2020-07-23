namespace Bula.Model {
    using System;

    using System.Data;
    using MySql.Data.MySqlClient;
    using System.Collections;

    using Bula.Objects;

    public class PreparedStatement : Bula.Meta {
        private Object link; // Link to mysql instance.
        private String sql; // Initial SQL-query
        private ArrayList pars; // List of parameters
        private String query; // Formed SQL-query

        public RecordSet record_set; // Resulting record set

        public PreparedStatement () {
            this.pars = new ArrayList();
            this.pars.Add("dummy"); // Parameter number will start from 1.
        }

        ///Execute query.
        public RecordSet ExecuteQuery() {
            this.record_set = new RecordSet();
            if (this.FormQuery()) {
                //Response.Write("[" . this.query . "]<br/>\r\n");
                Object result = Mysqli_query(this.link, this.query);
                    //or STOP("Query selection failed");
                if (result == null)
                    return null;
                this.record_set.result = result;
                this.record_set.SetRows(Mysqli_num_rows(result));
                this.record_set.SetPage(1);
                return this.record_set;
            }
            else
                return null;
        }

        ///Execute update query.
        ///   -1 - error during form query.
        ///   -2 - error during execution.
        public int ExecuteUpdate() {
            if (this.FormQuery()) {
                //PR("[this.query]<br/>\r\n");
                Object result = Mysqli_query(this.link, this.query);
                if (result == null) {
                    //info = CAT("[", this.query, "]<hr/>");
                    //if (Isset(CFG["Log_Object"]))
                    //    CFG["Log_Object"].Out(info);
                    //else
                    //    PR(info);
                    return -2;
                    //STOP("Query update failed");
                }
                int ret = Mysqli_affected_rows(this.link);
                return ret;
            }
            else
                return -1;
        }

        ///Get ID for just inserted record.
        public int GetInsertId() {
            return Mysqli_insert_id(this.link);
        }

        ///Form query (replace '?' marks with real parameters).
        private Boolean FormQuery() {
            int question_index = -1;
            int start_from = 0;
            int n = 1;
            String str = this.sql;
            while ((question_index = str.IndexOf("?", start_from)) != -1) {
                String value = (String)this.pars[n];
                String before = str.Substring(0, question_index);
                String after = str.Substring(question_index + 1);
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

        // Set int parameter.
        public void SetInt(int n, int val) {
            SetValue(n, CAT(val));
        }

        // Set String parameter.
        public void SetString(int n, String val) {
            SetValue(n, CAT("'", Strings.AddSlashes(val), "'"));
        }

        // Set DateTime parameter.
        public void SetDate(int n, String val) {
            SetValue(n, CAT("'", DateTimes.Format("Y-m-d H:i:s", DateTimes.GetTime(val)), "'"));
        }

        // Set Float parameter.
        public void SetFloat(int n, Double val) {
            SetValue(n, CAT(val));
        }

        public void Close() {
            this.link = null;
        }

        public void SetLink(Object link) {
            this.link = link;
        }

        public void SetSql(String sql) {
            this.sql = sql;
        }

        private Object Mysqli_query(Object link, String query) {
            MySqlCommand oCmd = ((MySqlConnection)link).CreateCommand();
            oCmd.CommandText = query;
            oCmd.Prepare();
            System.Data.DataSet sysDs = new System.Data.DataSet();
            MySqlDataAdapter oAdapter = new MySqlDataAdapter(oCmd);
            oAdapter.Fill(sysDs);
            return new Object[] { 0, sysDs };
        }

        private int Mysqli_num_rows(Object result) {
            return ((System.Data.DataSet)((Object[])result)[1]).Tables[0].Rows.Count;
        }

        private int Mysqli_affected_rows(Object link) {
            return 0; //TODO
        }

        private int Mysqli_insert_id(Object link) {
            return 0; //TODO
        }
    }
}