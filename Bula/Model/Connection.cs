// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Model {
    using System;
    using System.Collections;

    using MySql.Data.MySqlClient;
    using Bula.Model;
    using Bula.Objects;

    /// <summary>
    /// Implement operations with connection to the database.
    /// </summary>
    public class Connection : Bula.Meta {
        private MySqlConnection link = null;
        private PreparedStatement stmt; // Prepared statement to use with connection

        // Create connection to the database given parameters from DBConfig.
        public static Connection CreateConnection() {
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
        /// Open connection to the database.
        /// </summary>
        /// <param name="host">Host name.</param>
        /// <param name="port">Port number.</param>
        /// <param name="admin">Admin name.</param>
        /// <param name="password">Admin password.</param>
        /// <param name="db">DB name.</param>
        /// <returns>Result of operation (1 - OK, -1 - error).</returns>
        public int Open(String host, int port, String admin, String password, String db) {
            return Open(host, port, admin, password, db, null); }

        /// <summary>
        /// Open connection to the database.
        /// </summary>
        /// <param name="host">Host name.</param>
        /// <param name="port">Port number.</param>
        /// <param name="admin">Admin name.</param>
        /// <param name="password">Admin password.</param>
        /// <param name="db">DB name.</param>
        /// <param name="charset">DB charset.</param>
        /// <returns>Result of operation (1 - OK, -1 - error).</returns>
        public int Open(String host, int port, String admin, String password, String db, String charset) {
            this.link = DataAccess.Connect(host, admin, password, db, port); //TODO PHP
            if (this.link == null) {
                DataAccess.CallErrorDelegate("Can't open DB! Check whether it exists!");
                return -1;
            }
            if (charset != null)
                DataAccess.NonQuery(this.link, CAT("set names ", charset));
            return 1;
        }

        /// <summary>
        /// Close connection to the database.
        /// </summary>
        public void Close() {
            DataAccess.Close(this.link);
            this.link = null;
        }

        /// <summary>
        /// Prepare statement.
        /// </summary>
        /// <param name="sql">SQL-query.</param>
        /// <returns>statement.</returns>
        public PreparedStatement PrepareStatement(String sql) {
            this.stmt = new PreparedStatement();
            this.stmt.SetLink(this.link);
            this.stmt.SetSql(sql);
            return this.stmt;
        }
    }
}