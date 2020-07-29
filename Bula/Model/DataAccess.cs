namespace Bula.Model {
    using System;

    using System.Collections;
    using MySql.Data.MySqlClient;
    using Bula.Model;
    using Bula.Objects;
    
    /// <summary>
    /// Facade class for operations with MySQL database.
    /// </summary>
    public class DataAccess
    {
        public delegate void ErrorDelegateType(String str);
        public delegate void PrintDelegateType(String str);
        private static ErrorDelegateType error_delegate = null;
        private static PrintDelegateType print_delegate = null;

        /// <summary>
        /// Connect to a database.
        /// </summary>
        /// <param name="host">DB host name</param>
        /// <param name="admin">DB admin</param>
        /// <param name="db">DB name</param>
        /// <param name="password">DB password</param>
        /// <param name="port">DB port</param>
        /// <returns>New connection</returns>
        public static MySqlConnection Connect(String host, String admin, String db, String password, int port)
        {
            MySqlConnection link = new MySqlConnection();
            link.ConnectionString =
                "server=" + host + ";port=" + port + ";uid=" + admin + ";pwd=" + password + ";database=" + db + ";";
            link.Open();
            return link;
        }

        /// <summary>
        /// Close DB connection.
        /// </summary>
        /// <param name="link">Connection to close.</param>
        public static void Close(MySqlConnection link)
        {
            link.Close();
            link = null;
        }

        /// <summary>
        /// Execute non-selection query.
        /// </summary>
        /// <param name="link">DB connection</param>
        /// <param name="query">SQL-query</param>
        /// <returns>Query result</returns>
        public static int NonQuery(Object link, String query)
        {
            return MySqlHelper.ExecuteNonQuery((MySqlConnection)link, query, new MySqlParameter[] { });
        }

        /// <summary>
        /// Execute selection query.
        /// </summary>
        /// <param name="link">DB connection</param>
        /// <param name="query">SQL-query</param>
        /// <returns>Query result</returns>
        public static Object SelectQuery(Object link, String query)
        {
            System.Data.DataSet sysDs = MySqlHelper.ExecuteDataset((MySqlConnection)link, query);
            return new Object[] { 0, sysDs };
        }

        /// <summary>
        /// Execute update query.
        /// </summary>
        /// <param name="link">DB connection</param>
        /// <param name="query">SQL-query</param>
        /// <returns>Query result</returns>
        public static Object UpdateQuery(Object link, String query)
        {
            System.Data.DataSet sysDs = MySqlHelper.ExecuteDataset((MySqlConnection)link, query);
            return new Object[] { 0, sysDs };
        }

        /// <summary>
        /// Get number of rows in current selection query.
        /// </summary>
        /// <param name="result">Query result</param>
        /// <returns>Number of rows</returns>
        public static int NumRows(Object result)
        {
            return ((System.Data.DataSet)((Object[])result)[1]).Tables[0].Rows.Count;
        }

        public static int AffectedRows(Object link) {
            return 0; //TODO
        }

        /// <summary>
        /// Get ID of just inserted row.
        /// </summary>
        /// <param name="link">DB connection</param>
        /// <returns>ID of inserted row</returns>
        public static int InsertId(Object link) {
            Object result = MySqlHelper.ExecuteScalar((MySqlConnection)link, "select last_insert_id()");
            return 0; //TODO;
        }

        /// <summary>
        /// Fetch next record from result.
        /// </summary>
        /// <param name="result">Query result</param>
        /// <returns>Next record</returns>
        public static Hashtable FetchArray(Object result)
        {
            int pointer = (int)((Object[])result)[0];
            System.Data.DataSet ds = (System.Data.DataSet)((Object[])result)[1];
            if (pointer >= ds.Tables[0].Rows.Count)
                return null; // No more rows to fetch

            Hashtable hash = new Hashtable();
            System.Data.DataRow row = ds.Tables[0].Rows[pointer];
            for (int n = 0; n < row.Table.Columns.Count; n++)
            {
                Object obj = row.ItemArray.GetValue(n);
                hash.Add(row.Table.Columns[n].ColumnName, obj);
            }
            ((Object[])result)[0] = ++pointer;
            return hash;
        }

        /// <summary>
        /// Free query result.
        /// </summary>
        /// <param name="result">Query result</param>
        public static void FreeResult(Object result)
        {
            System.Data.DataSet ds = (System.Data.DataSet)((Object[])result)[1];
            ds.Dispose();
            ((Object[])result)[0] = 0;
            ((Object[])result)[1] = null;
        }

        /// <summary>
        /// Set delegate function for error logging.
        /// </summary>
        /// <param name="delegateFunction"></param>
        public static void SetErrorDelegate(ErrorDelegateType delegateFunction) {
            error_delegate = delegateFunction;
        }

        /// <summary>
        /// Set delegate function for debug printing.
        /// </summary>
        /// <param name="delegateFunction"></param>
        public static void SetPrintDelegate(PrintDelegateType delegateFunction){
            print_delegate = delegateFunction;
        }

        /// <summary>
        /// Call delegate function for error logging.
        /// </summary>
        /// <param name="input">Text to log</param>
        public static void CallErrorDelegate(String input) {
            if (error_delegate != null)
                error_delegate.DynamicInvoke(new Object[] { input });
        }
    
        /// <summary>
        /// Call delegate function for debug printing.
        /// </summary>
        /// <param name="input">Text to pring</param>
        public static void CallPrintDelegate(String input) {
            if (print_delegate != null)
                print_delegate.DynamicInvoke(new Object[] { input });
        }
    }
}