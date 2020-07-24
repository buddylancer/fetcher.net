namespace Bula.Model {
    using System;

    using System.Collections;
    using Bula.Objects;

    public class DOBase : Bula.Meta {
        private Connection db_connection = null;
        protected String table_name;
        protected String id_field;
        protected int page_no;

        public DOBase () {
            if (DBConfig.Connection == null)
                DBConfig.Connection = this.CreateConnection();

            this.db_connection = DBConfig.Connection;
            this.page_no = 0;
        }

        // Create connection to the database given parameters from DBConfig.
        private Connection CreateConnection() {
            Connection oConn = new Connection();
            String db_admin = DBConfig.DB_ADMIN != null ? DBConfig.DB_ADMIN : DBConfig.DB_NAME;
            String db_password = DBConfig.DB_PASSWORD != null ? DBConfig.DB_PASSWORD : DBConfig.DB_NAME;
            int ret = 0;
            if (DBConfig.DB_CHARSET != null)
                ret = oConn.Open(DBConfig.DB_HOST, DBConfig.DB_PORT, db_admin, db_password, DBConfig.DB_NAME, DBConfig.DB_CHARSET);
            else
                ret = oConn.Open(DBConfig.DB_HOST, DBConfig.DB_PORT, db_admin, db_password, DBConfig.DB_NAME);
            if (ret == -1) {
                oConn = null;
                DataAccess.CallErrorDelegate("Can't open DB! Check whether it exists.");
            }
            return oConn;
        }

        public Connection GetConnection() {
            return this.db_connection;
        }

        public String GetIdField() {
            return this.id_field;
        }

        ///Get DataSet based on query & parameters (all records).
        /// <param name="query">SQL-query to execute.</param>
        /// <param name="pars">Query parameters.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet GetDataSet(String query, Object[] pars) {
            PreparedStatement oStmt = this.db_connection.PrepareStatement(query);
            if (pars != null && SIZE(pars) > 0) {
                int n = 1;
                for (int i = 0; i < SIZE(pars); i += 2) {
                    String type = (String)pars[i];
                    Object value = pars[i+1];
                    CALL(oStmt, type, ARR(n, value));
                    n++;
                }
            }
            RecordSet oRs = oStmt.ExecuteQuery();
            if (oRs == null) {
                oStmt.Close();
                return null;
            }

            DataSet ds = new DataSet();
            while (oRs.Next() != 0) {
                ds.AddRow(oRs.record);
            }
             oRs.Close();
            oStmt.Close();
            return ds;
        }

        ///Get DataSet based on query & parameters (only records of the list with rows length).
        /// <param name="query">SQL-query to execute.</param>
        /// <param name="pars">Query parameters.</param>
        /// <param name="list">List number.</param>
        /// <param name="rows">Number of rows in a list.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet GetDataSetList(String query, Object[] pars, int list, int rows) {
            if (rows <= 0 || list <= 0)
                return this.GetDataSet(query, pars);

            PreparedStatement oStmt = this.db_connection.PrepareStatement(query);
            if (SIZE(pars) > 0) {
                int n = 1;
                for (int p = 0; p < SIZE(pars); p += 2) {
                    String type = (String) pars[p];
                    Object value = pars[p+1];
                    CALL(oStmt, type, ARR(n, value));
                    n++;
                }
            }
            RecordSet oRs = oStmt.ExecuteQuery();
            if (oRs == null) {
                DataAccess.CallErrorDelegate(CAT("Error in query: ", query, "<hr/>"));
            }
            DataSet ds = new DataSet();
            int total_rows = oRs.GetRows();
            ds.SetTotalPages(INT((total_rows - 1)/ rows) + 1);

            int count = 0;
            if (list != 1) {
                count = (list - 1) * rows;
                while (oRs.Next() != 0) {
                    count--;
                    if (count == 0)
                        break;
                }
            }

            count = 0;
            while (oRs.Next() != 0) {
                if (count == rows)
                    break;
                ds.AddRow(oRs.record);
                //ds.SetSize(ds.GetSize() + 1);
                count++;
            }

            oRs.Close();
            oStmt.Close();
            return ds;
        }

        protected int UpdateInternal(String query, Object[] Pars) {
            return UpdateInternal(query, Pars, "update");}

        ///Update database using query and parameters
        /// <param name="query">SQL-query to execute.</param>
        /// <param name="pars">Query parameters.</param>
        /// <param name="operation">Operation - "update" (default) or "insert".</param>
        /// <returns>Update status (or inserted ID for "insert" operation).</returns>
        protected int UpdateInternal(String query, Object[] pars, Object operation) {
            PreparedStatement oStmt = this.db_connection.PrepareStatement(query);
            if (SIZE(pars) > 0) {
                int n = 1;
                for (int i = 0; i < SIZE(pars); i += 2) {
                    String type = (String)pars[i];
                    Object value = pars[i+1];
                    CALL(oStmt, type, ARR(n, value));
                    n++;
                }
            }
            int ret = oStmt.ExecuteUpdate();
            if (ret > 0 && EQ(operation, "insert"))
                ret = oStmt.GetInsertId();
            oStmt.Close();
            if (ret < 0)
                DataAccess.CallErrorDelegate(CAT("Error in update query: ", query, "<hr/>"));
            return ret;
        }

        ///Get DataSet based on record ID.
        /// <param name="id">Unique ID.</param>
        /// <returns>Resulting data set.</returns>
        public virtual DataSet GetById(int id) {
            String query = Strings.Concat(
                " select * from ", this.table_name,
                " where ", this.id_field, " = ?"
            );
            Object[] pars = ARR("SetInt", id);
            return this.GetDataSet(query, pars);
        }

        public DataSet EnumIds() { return EnumIds(null, null); }
        public DataSet EnumIds(String where) { return EnumIds(where, null); }

        ///Get DataSet containing IDs only.
        /// <param name="where">Where condition [optional].</param>
        /// <param name="order">Field to order by [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumIds(String where, String order) {
            String query = Strings.Concat(
                " select ", this.id_field, " from ", this.table_name, " _this ",
                (BLANK(where) ? null : CAT(" where ", where)),
                " order by ",
                (BLANK(order) ? this.id_field : order)
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        public DataSet EnumAll() { return EnumAll(null, null); }
        public DataSet EnumAll(String where) { return EnumAll(where, null); }

        ///Get DataSet with all records enumerated.
        /// <param name="where">Where condition [optional].</param>
        /// <param name="order">Field to order by [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumAll(String where, String order) {
            String query = Strings.Concat(
                " select * from ", this.table_name, " _this ",
                (BLANK(where) ? null : CAT(" where ", where)),
                (BLANK(order) ? null : CAT(" order by ", order))
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        public DataSet EnumFields(String fields) { return EnumFields(fields, null, null); }
        public DataSet EnumFields(String fields, String where) { return EnumFields(fields, where, null); }

        ///Get DataSet containing only required fields.
        /// <param name="fields">Fields to include (divided by ',').</param>
        /// <param name="where">Where condition [optional].</param>
        /// <param name="order">Field to order by [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumFields(String fields, String where, String order) {
            String query = Strings.Concat(
                " select ", fields, " from ", this.table_name, " _this ",
                (BLANK(where) ? null : CAT(" where ", where)),
                (BLANK(order) ? null : CAT(" order by ", order))
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        public DataSet Select() { return Select(null, null, null); }
        public DataSet Select(String fields) { return Select(fields, null, null); }
        public DataSet Select(String fields, String where) { return Select(fields, where, null); }

        ///Get DataSet containing only required fields or all fields [default].
        /// <param name="fields">Fields to include (divided by ',').</param>
        /// <param name="where">Where condition [optional].</param>
        /// <param name="order">Field to order by [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet Select(String fields, String where, String order) {
            if (fields == null)
                fields = "_this.*";

            String query = Strings.Concat(
                " select ", fields,
                " from ", this.table_name, " _this ",
                (BLANK(where) ? null : CAT(" where ", where)),
                (BLANK(order) ? null : CAT(" order by ", order))
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        // Get DataSet containing only required fields (only records of the list with list_size length)
        public DataSet SelectList(int list, int list_size) {
            return SelectList(list, list_size, null, null, null); }
        public DataSet SelectList(int list, int list_size, String fields) {
            return SelectList(list, list_size, fields, null, null); }
        public DataSet SelectList(int list, int list_size, String fields, String where) {
            return SelectList(list, list_size, fields, where, null); }

        ///Get DataSet containing only required fields or all fields.
        /// <param name="list">List number.</param>
        /// <param name="rows">Number of rows in a list.</param>
        /// <param name="fields">Fields to include (divided by ',').</param>
        /// <param name="where">Where condition [optional].</param>
        /// <param name="order">Field to order by [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet SelectList(int list, int rows, String fields, String where, String order) {
            if (fields == null)
                fields = "_this.*";
            String query = Strings.Concat(
                " select ",  fields,
                " from ", this.table_name, " _this ",
                (BLANK(where) ? null : CAT(" where ", where)),
                (BLANK(order) ? null : CAT(" order by ", order))
            );

            Object[] pars = ARR();
            DataSet ds = this.GetDataSetList(query, pars, list, rows);
            return ds;
        }

        ///Delete record by ID.
        /// <param name="id">Unique ID.</param>
        /// <returns>Result of operation.</returns>
        public int DeleteById(int id) {
            String query = Strings.Concat(
                " delete from ", this.table_name,
                " where ", this.id_field, " = ?"
            );
            Object[] pars = ARR("SetInt", id);
            return this.UpdateInternal(query, pars, "update");
        }

        // Insert new record based on fields array.
        public int Insert(Hashtable fields) {
            IEnumerator keys = fields.Keys.GetEnumerator();
            String field_names = "";
            String field_values = "";
            Object[] pars = ARR();
            //pars.SetPullValues(true);
            int n = 0;
            while (keys.MoveNext()) {
                String key = (String)keys.Current;
                if (n != 0) field_names += (", ");
                if (n != 0) field_values += (", ");
                field_names += (key);
                field_values += ("?");
                pars = ADD(pars, this.SetFunction(key), fields[key]);
                n++;
            }
            String query = Strings.Concat(
                " insert into ", this.table_name, " (", field_names, ") ",
                " values (", field_values, ")"
            );
            return this.UpdateInternal(query, pars, "insert");
        }

        // Update existing record by ID based on fields array.
        public int UpdateById(Object id, Hashtable fields) {
            IEnumerator keys = fields.Keys.GetEnumerator();
            String set_values = "";
            Object[] pars = ARR();
            int n = 0;
            while (keys.MoveNext()) {
                String key = (String)keys.Current;
                if (key == this.id_field) //TODO PHP
                    continue;
                if (n != 0)
                    set_values += (", ");
                set_values += (CAT(key, " = ?"));
                pars = ADD(pars, this.SetFunction(key), fields[key]);
                n++;
            }
            pars = ADD(pars, this.SetFunction(this.id_field), id);
            String query = Strings.Concat(
                " update ", this.table_name, " set ", set_values,
                " where (", this.id_field, " = ?)"
            );
            return this.UpdateInternal(query, pars, "update");
        }

        // Map for setting parameters.
        private String SetFunction(String key) {
            String prefix = key.Substring(0, 2);
            String func = "SetString";
            if (prefix.Equals("s_") || prefix.Equals("t_"))
                func = "SetString";
            else if (prefix.Equals("i_") || prefix.Equals("b_"))
                func = "SetInt";
            else if (prefix.Equals("f_"))
                func = "SetFloat";
            else if (prefix.Equals("d_"))
                func = "SetDate";
            return func;
        }
    }
}