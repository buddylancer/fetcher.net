// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Model {
    using System;
    using System.Collections;

    /// <summary>
    /// Set info for database connection here.
    /// </summary>
    public class DBConfig : Bula.Meta {
        /// Database host 
        public const String DB_HOST = "localhost";
        /// Database name 
        public const String DB_NAME = "dbusnews";
        /// Database administrator name (if null - DB_NAME will be used) 
        public const String DB_ADMIN = null;
        /// Database password  (if null - DB_NAME will be used) 
        public const String DB_PASSWORD = null;
        /// Database character set 
        public const String DB_CHARSET = "utf8";
        /// Database port 
        public const int DB_PORT = 3306;
        /// Date/time format used for DB operations 
        public const String SQL_DTS = "yyyy-MM-dd HH:mm:ss";
        /// Database connection is stored here 
        public static Connection Connection = null;
    }
}