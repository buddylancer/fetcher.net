namespace Bula.Model {
    using System;

    /**
     * Set info for database connection here.
     */
    public class DBConfig : Bula.Meta {
        public const String DB_HOST = "localhost";
        public const String DB_NAME = "dbusnews";
        public const String DB_ADMIN = null; // If null - DB_NAME will be used
        public const String DB_PASSWORD = null; // If null - DB_NAME will be used
        public const String DB_CHARSET = "utf8";
        public const int DB_PORT = 3306;
        public const String SQL_DTS = "yyyy-MM-dd HH:mm:ss";
        public static Connection Connection = null; // Do not change this! This is placeholder!
    }
}