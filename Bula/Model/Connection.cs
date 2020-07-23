namespace Bula.Model {
    using System;

    using MySql.Data.MySqlClient;
    using Bula.Model;
    using Bula.Objects;

    /**
     * Implement operations with connection to the database.
     */
    public class Connection : Bula.Meta {
        private MySqlConnection link;
        private PreparedStatement stmt; // Prepared statement to use with connection

        public int Open(String host, int port, String admin, String password, String db) {
            return Open(host, port, admin, password, null); }

        ///Open connection to the database.
        /// <param name="host">Host name.</param>
        /// <param name="port">Port number.</param>
        /// <param name="admin">Admin name.</param>
        /// <param name="password">Admin password.</param>
        /// <param name="db">DB name.</param>
        /// <param name="charset">DB charset.</param>
        /// <returns>Result of operation (1 - OK, -1 - error).</returns>
        public int Open(String host, int port, String admin, String password, String db, String charset) {
            this.link = Mysqli_connect(host, admin, password, db, port); //TODO PHP
            if (this.link == null)
                return -1;
            //TODO!!!
            //if (charset != null)
            //    Mysqli_query(this.link, CAT("set names ", charset));
            return 1;
        }

        ///Close connection to the database.
        public void Close() {
            Mysqli_close(this.link);
            this.link = null;
        }

        ///Prepare statement.
        /// @param String sql
        public PreparedStatement PrepareStatement(String sql) {
            this.stmt = new PreparedStatement();
            this.stmt.SetLink(this.link);
            this.stmt.SetSql(sql);
            return this.stmt;
        }

        private MySqlConnection Mysqli_connect(String host, String admin, String db, String password, int port) {
            MySqlConnection link = new MySqlConnection();
            link.ConnectionString =
                CAT("server=", host, ";port=", port, ";uid=", admin, ";pwd=", password, ";database=", db, ";");
            link.Open();
            return link;
        }

        private void Mysqli_close(MySqlConnection link) {
            link.Close();
            link = null;
        }
    }
}