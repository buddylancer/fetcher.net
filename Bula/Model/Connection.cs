// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Model {
    using System;

    using MySql.Data.MySqlClient;
    using Bula.Model;
    using Bula.Objects;

    /// <summary>
    /// Implement operations with connection to the database.
    /// </summary>
    public class Connection : Bula.Meta {
        private MySqlConnection link;
        private PreparedStatement stmt; // Prepared statement to use with connection

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
            return Open(host, port, admin, password, null); }

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