// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Model {
    using System;
    using System.Collections;

    using Bula.Objects;

    /// <summary>
    /// Base class for manipulating with DB objects.
    /// </summary>
    public class DOBase : Bula.Meta {
        private Connection dbConnection = null;

        /// <summary>
        /// Name of a DB table.
        /// </summary>
        /// @var String
        protected String tableName;

        /// <summary>
        /// Name of a table ID field.
        /// </summary>
        /// @var String
        protected String idField;

        /// Public constructor 
        public DOBase () {
            if (DBConfig.Connection == null)
                DBConfig.Connection = this.CreateConnection();

            this.dbConnection = DBConfig.Connection;
        }

        // Create connection to the database given parameters from DBConfig.
        private Connection CreateConnection() {
            var oConn = new Connection();
            var dbAdmin = DBConfig.DB_ADMIN != null ? DBConfig.DB_ADMIN : DBConfig.DB_NAME;
            var dbPassword = DBConfig.DB_PASSWORD != null ? DBConfig.DB_PASSWORD : DBConfig.DB_NAME;
            var ret = 0;
            if (DBConfig.DB_CHARSET != null)
                ret = oConn.Open(DBConfig.DB_HOST, DBConfig.DB_PORT, dbAdmin, dbPassword, DBConfig.DB_NAME, DBConfig.DB_CHARSET);
            else
                ret = oConn.Open(DBConfig.DB_HOST, DBConfig.DB_PORT, dbAdmin, dbPassword, DBConfig.DB_NAME);
            if (ret == -1)
                oConn = null;
            return oConn;
        }

        /// <summary>
        /// Get current connection.
        /// </summary>
        /// <returns>Current connection.</returns>
        public Connection GetConnection() {
            return this.dbConnection;
        }

        /// <summary>
        /// Get current ID field name.
        /// </summary>
        public String GetIdField() {
            return this.idField;
        }

        /// <summary>
        /// Get DataSet based on query and parameters (all records).
        /// </summary>
        /// <param name="query">SQL-query to execute.</param>
        /// <param name="pars">Query parameters.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet GetDataSet(String query, Object[] pars) {
            var oStmt = this.dbConnection.PrepareStatement(query);
            if (pars != null && SIZE(pars) > 0) {
                var n = 1;
                for (int i = 0; i < SIZE(pars); i += 2) {
                    var type = (String)pars[i];
                    var value = pars[i+1];
                    CALL(oStmt, type, ARR(n, value));
                    n++;
                }
            }
            var oRs = oStmt.ExecuteQuery();
            if (oRs == null) {
                oStmt.Close();
                return null;
            }

            var ds = new DataSet();
            while (oRs.Next() != 0) {
                ds.AddRow(oRs.record);
            }
            oRs.Close();
            oStmt.Close();
            return ds;
        }

        /// <summary>
        /// Get DataSet based on query and parameters (only records of the list with rows length).
        /// </summary>
        /// <param name="query">SQL-query to execute.</param>
        /// <param name="pars">Query parameters.</param>
        /// <param name="list">List number.</param>
        /// <param name="rows">Number of rows in a list.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet GetDataSetList(String query, Object[] pars, int list, int rows) {
            if (rows <= 0 || list <= 0)
                return this.GetDataSet(query, pars);

            var oStmt = this.dbConnection.PrepareStatement(query);
            if (SIZE(pars) > 0) {
                var n = 1;
                for (int p = 0; p < SIZE(pars); p += 2) {
                    var type = (String) pars[p];
                    var value = pars[p+1];
                    CALL(oStmt, type, ARR(n, value));
                    n++;
                }
            }
            var oRs = oStmt.ExecuteQuery();
            if (oRs == null)
                return null;

            var ds = new DataSet();
            var totalRows = oRs.GetRows();
            ds.SetTotalPages(INT((totalRows - 1) / rows + 1));

            var count = 0;
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

        /// <summary>
        /// Update database using query and parameters
        /// </summary>
        /// <param name="query">SQL-query to execute.</param>
        /// <param name="pars">Query parameters.</param>
        /// <returns>Update status.</returns>
        protected int UpdateInternal(String query, Object[] pars) {
            return UpdateInternal(query, pars, "update");}

        /// <summary>
        /// Update database using query and parameters
        /// </summary>
        /// <param name="query">SQL-query to execute.</param>
        /// <param name="pars">Query parameters.</param>
        /// <param name="operation">Operation - "update" (default) or "insert".</param>
        /// <returns>Update status (or inserted ID for "insert" operation).</returns>
        protected int UpdateInternal(String query, Object[] pars, Object operation) {
            var oStmt = this.dbConnection.PrepareStatement(query);
            if (SIZE(pars) > 0) {
                var n = 1;
                for (int i = 0; i < SIZE(pars); i += 2) {
                    var type = (String)pars[i];
                    var value = pars[i+1];
                    CALL(oStmt, type, ARR(n, value));
                    n++;
                }
            }
            var ret = oStmt.ExecuteUpdate();
            if (ret > 0 && EQ(operation, "insert"))
                ret = oStmt.GetInsertId();
            oStmt.Close();
            return ret;
        }

        /// <summary>
        /// Get DataSet based on record ID.
        /// </summary>
        /// <param name="id">Unique ID.</param>
        /// <returns>Resulting data set.</returns>
        public virtual DataSet GetById(int id) {
            var query = Strings.Concat(
                " select * from ", this.tableName,
                " where ", this.idField, " = ?"
            );
            Object[] pars = ARR("SetInt", id);
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Get DataSet containing IDs only.
        /// </summary>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumIds() {
            return EnumIds(null, null); }

        /// <summary>
        /// Get DataSet containing IDs only.
        /// </summary>
        /// <param name="where">Where condition.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumIds(String where) {
            return EnumIds(where, null); }

        /// <summary>
        /// Get DataSet containing IDs only.
        /// </summary>
        /// <param name="where">Where condition [optional].</param>
        /// <param name="order">Field to order by [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumIds(String where, String order) {
            var query = Strings.Concat(
                " select ", this.idField, " from ", this.tableName, " _this ",
                (BLANK(where) ? null : CAT(" where ", where)),
                " order by ",
                (BLANK(order) ? this.idField : order)
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Get DataSet containing counter only.
        /// </summary>
        /// <returns>Resulting data set.</returns>
        public DataSet CountIds() {
            return CountIds(null); }

        /// <summary>
        /// Get DataSet containing counter only.
        /// </summary>
        /// <param name="where">Where condition [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet CountIds(String where) {
            var query = Strings.Concat(
                " select Count(", this.idField, ") as i_Counter from ", this.tableName, " _this ",
                (BLANK(where) ? null : CAT(" where ", where))
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Get DataSet with all records enumerated.
        /// </summary>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumAll() { return EnumAll(null, null); }

        /// <summary>
        /// Get DataSet with all records enumerated.
        /// </summary>
        /// <param name="where">Where condition.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumAll(String where) { return EnumAll(where, null); }

        /// <summary>
        /// Get DataSet with all records enumerated.
        /// </summary>
        /// <param name="where">Where condition [optional].</param>
        /// <param name="order">Field to order by [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumAll(String where, String order) {
            var query = Strings.Concat(
                " select * from ", this.tableName, " _this ",
                (BLANK(where) ? null : CAT(" where ", where)),
                (BLANK(order) ? null : CAT(" order by ", order))
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Get DataSet containing only required fields.
        /// </summary>
        /// <param name="fields">Fields to include (divided by ',').</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumFields(String fields) {
            return EnumFields(fields, null, null); }

        /// <summary>
        /// Get DataSet containing only required fields.
        /// </summary>
        /// <param name="fields">Fields to include (divided by ',').</param>
        /// <param name="where">Where condition [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumFields(String fields, String where) {
            return EnumFields(fields, where, null); }

        /// <summary>
        /// Get DataSet containing only required fields.
        /// </summary>
        /// <param name="fields">Fields to include (divided by ',').</param>
        /// <param name="where">Where condition [optional].</param>
        /// <param name="order">Field to order by [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumFields(String fields, String where, String order) {
            var query = Strings.Concat(
                " select ", fields, " from ", this.tableName, " _this ",
                (BLANK(where) ? null : CAT(" where ", where)),
                (BLANK(order) ? null : CAT(" order by ", order))
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Get DataSet containing all fields.
        /// </summary>
        /// <returns>Resulting data set.</returns>
        public DataSet Select() {
            return Select(null, null, null); }

        /// <summary>
        /// Get DataSet containing only required fields.
        /// </summary>
        /// <param name="fields">Fields to include (divided by ',').</param>
        /// <returns>Resulting data set.</returns>
        public DataSet Select(String fields) {
            return Select(fields, null, null); }

        /// <summary>
        /// Get DataSet containing only required fields.
        /// </summary>
        /// <param name="fields">Fields to include (divided by ',').</param>
        /// <param name="where">Where condition.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet Select(String fields, String where) {
            return Select(fields, where, null); }

        /// <summary>
        /// Get DataSet containing only required fields or all fields [default].
        /// </summary>
        /// <param name="fields">Fields to include (divided by ',').</param>
        /// <param name="where">Where condition [optional].</param>
        /// <param name="order">Field to order by [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet Select(String fields, String where, String order) {
            if (fields == null)
                fields = "_this.*";

            var query = Strings.Concat(
                " select ", fields,
                " from ", this.tableName, " _this ",
                (BLANK(where) ? null : CAT(" where ", where)),
                (BLANK(order) ? null : CAT(" order by ", order))
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Get DataSet containing only the given list of rows.
        /// </summary>
        /// <param name="list">List number.</param>
        /// <param name="rows">Number of rows in a list.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet SelectList(int list, int rows) {
            return SelectList(list, rows, null, null, null); }

        /// <summary>
        /// Get DataSet containing only the given list of rows (with required fields).
        /// </summary>
        /// <param name="list">List number.</param>
        /// <param name="rows">Number of rows in a list.</param>
        /// <param name="fields">Fields to include (divided by ',').</param>
        /// <returns>Resulting data set.</returns>
        public DataSet SelectList(int list, int rows, String fields) {
            return SelectList(list, rows, fields, null, null); }

        /// <summary>
        /// Get DataSet containing only the given list of rows (with required fields).
        /// </summary>
        /// <param name="list">List number.</param>
        /// <param name="rows">Number of rows in a list.</param>
        /// <param name="fields">Fields to include (divided by ',').</param>
        /// <param name="where">Where condition [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet SelectList(int list, int rows, String fields, String where) {
            return SelectList(list, rows, fields, where, null); }

        /// <summary>
        /// Get DataSet containing only the given list of rows (with required fields or all fields).
        /// </summary>
        /// <param name="list">List number.</param>
        /// <param name="rows">Number of rows in a list.</param>
        /// <param name="fields">Fields to include (divided by ',').</param>
        /// <param name="where">Where condition [optional].</param>
        /// <param name="order">Field to order by [optional].</param>
        /// <returns>Resulting data set.</returns>
        public DataSet SelectList(int list, int rows, String fields, String where, String order) {
            if (fields == null)
                fields = "_this.*";
            var query = Strings.Concat(
                " select ",  fields,
                " from ", this.tableName, " _this ",
                (BLANK(where) ? null : CAT(" where ", where)),
                (BLANK(order) ? null : CAT(" order by ", order))
            );

            Object[] pars = ARR();
            var ds = this.GetDataSetList(query, pars, list, rows);
            return ds;
        }

        /// <summary>
        /// Delete record by ID.
        /// </summary>
        /// <param name="id">Unique ID.</param>
        /// <returns>Result of operation.</returns>
        public int DeleteById(int id) {
            var query = Strings.Concat(
                " delete from ", this.tableName,
                " where ", this.idField, " = ?"
            );
            Object[] pars = ARR("SetInt", id);
            return this.UpdateInternal(query, pars, "update");
        }

        /// <summary>
        /// Insert new record based on given fields.
        /// </summary>
        /// <param name="fields">The set of fields.</param>
        /// <returns>Result of SQL-query execution.</returns>
        public int Insert(THashtable fields) {
            var keys = new TEnumerator(fields.Keys.GetEnumerator());
            var fieldNames = "";
            var fieldValues = "";
            Object[] pars = ARR();
            //pars.SetPullValues(true);
            var n = 0;
            while (keys.MoveNext()) {
                var key = (String)keys.GetCurrent();
                if (n != 0) fieldNames += ", ";
                if (n != 0) fieldValues += ", ";
                fieldNames += key;
                fieldValues += "?";
                pars = ADD(pars, this.SetFunction(key), fields[key]);
                n++;
            }
            var query = Strings.Concat(
                " insert into ", this.tableName, " (", fieldNames, ") ",
                " values (", fieldValues, ")"
            );
            return this.UpdateInternal(query, pars, "insert");
        }

        /// <summary>
        /// Execute update query.
        /// </summary>
        /// <param name="setValues">String with "set" clause.</param>
        /// <param name="where">String with "where" clause.</param>
        /// <returns>Number of records updated.</returns>
        public int Update(String setValues, String where) {
            var query = Strings.Concat(
                " update ", this.tableName, " _this set ", setValues,
                " where (", where, ")"
            );
            Object[] pars = ARR();
            return this.UpdateInternal(query, pars, "update");
        }

        /// <summary>
        /// Update existing record by ID based on given fields.
        /// </summary>
        /// <param name="id">Unique record ID.</param>
        /// <param name="fields">The set of fields.</param>
        /// <returns>Result of SQL-query execution.</returns>
        public int UpdateById(Object id, THashtable fields) {
            var keys = new TEnumerator(fields.Keys.GetEnumerator());
            var setValues = "";
            Object[] pars = ARR();
            var n = 0;
            while (keys.MoveNext()) {
                var key = (String)keys.GetCurrent();
                if (key == this.idField) //TODO PHP
                    continue;
                if (n != 0)
                    setValues += ", ";
                setValues += CAT(key, " = ?");
                pars = ADD(pars, this.SetFunction(key), fields[key]);
                n++;
            }
            pars = ADD(pars, this.SetFunction(this.idField), id);
            var query = Strings.Concat(
                " update ", this.tableName, " set ", setValues,
                " where (", this.idField, " = ?)"
            );
            return this.UpdateInternal(query, pars, "update");
        }

        /// <summary>
        /// Map for setting parameters.
        /// </summary>
        /// <param name="key"> Field name.</param>
        /// <returns>Function name for setting that field.</returns>
        private String SetFunction(String key) {
            var prefix = key.Substring(0, 2);
            var func = "SetString";
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