namespace Bula.Fetcher {
    using System;

    using Bula.Objects;
    using System.Collections;

    /// <summary>
    /// Class behind Configuration.
    /// </summary>
    public class Common : Bula.Meta {
        static Common() { Initialize(); }

        /// Storage for internal variables 
        protected static Hashtable Values = new Hashtable();

        /// <summary>
        /// Get internal variable.
        /// </summary>
        /// <param name="name">Name of internal variable.</param>
        /// <returns>Value of variable.</returns>
        public static String Get(String name) {
            return (String)Values[name];
        }

        /// <summary>
        /// Set internal variable.
        /// </summary>
        /// <param name="name">Name of internal variable.</param>
        /// <param name="value">Value of internal variable to set.</param>
        public static void Set(String name, Object value) {
            Values[name] = value;
        }

        ///  
        /// <summary>
        /// Check whether variable is contained in internal storage.
        /// </summary>
        /// <param name="name">Name of internal variable.</param>
        /// <returns>True - variable exists, False - not exists.</returns>
        public static Boolean Contains(String name) {
            return Values.Contains(name);
        }

        /// Project root (where Bula folder is located) 
        public static String LocalRoot;

        /// Host name (copied from request HOST_NAME) 
        public static String Host;
        /// Site name (copied from Config.SITE_NAME) 
        public static String Site;
        /// Is request for mobile version? 
        public static Boolean IsMobile;
        /// Current language 
        public static String Lang;

        /// Root cache folder for pages 
        public static String CacheFolderRoot;
        /// Cache folder for pages 
        public static String CacheFolder;
        /// Root cache folder for output RSS-feeds 
        public static String RssFolderRoot;
        /// Cache folder for output RSS-feeds 
        public static String RssFolder;
        /// Cache folder for input RSS-feeds 
        public static String FeedFolder;
        /// Unique host ID for current request 
        public static String UniqueHostId;

        /// Use fine or full URLs 
        public static Boolean FineUrls = Config.FINE_URLS;
        /// Show an item or immediately redirect to external source item 
        public static Boolean ImmediateRedirect = Config.IMMEDIATE_REDIRECT;

        /// Storage for global constants 
        public static Hashtable GlobalConstants = null;

        /// Is current request from test script? 
        public static Boolean TestRun = false;

        /// <summary>
        /// Check whether current request is from test script?
        /// </summary>
        public static void CheckTestRun() {
            var http_tester = Request.GetVar(Request.INPUT_SERVER, "HTTP_USER_AGENT");
            if (http_tester == null)
                return;
            if (EQ(http_tester, "TestFull")) {
                TestRun = true;
                FineUrls = false;
                ImmediateRedirect = false;
                Site = "http://www.test.com";
            }
            else if (EQ(http_tester, "TestFine")) {
                TestRun = true;
                FineUrls = true;
                ImmediateRedirect = false;
                Site = "http://www.test.com";
            }
            else if (EQ(http_tester, "TestDirect")) {
                TestRun = true;
                FineUrls = true;
                ImmediateRedirect = true;
                Site = "http://www.test.com";
            }
        }

        /// <summary>
        /// Initialize all variables for current request.
        /// </summary>
        public static void Initialize() {
            //------------------------------------------------------------------------------
            // You can change something below this line if you know what are you doing :)
            var root_dir = Request.GetVar(Request.INPUT_SERVER, "APPL_PHYSICAL_PATH");
            for (int n = 0; n <= 3; n++) {
                var last_slash_index = root_dir.LastIndexOf("\\");
                root_dir = root_dir.Substring(0, last_slash_index);
            }
            LocalRoot = root_dir += ("/");

            Host = Request.GetVar(Request.INPUT_SERVER, "HTTP_HOST");
            Site = Strings.Concat("http://", Host);
            IsMobile = Host.IndexOf("m.") == 0;
            Lang = Host.LastIndexOf(".ru") != -1 ? "ru" : "en";

            CheckTestRun();
            UniqueHostId = Strings.Concat(
                IsMobile ? "mob_" : "www_",
                FineUrls ? (ImmediateRedirect ? "direct_" : "fine_") : "full_",
                Config.Lang);
            CacheFolderRoot = Strings.Concat(LocalRoot, "local/cache/www");
            CacheFolder = Strings.Concat(CacheFolderRoot, "/", UniqueHostId);
            RssFolderRoot = Strings.Concat(LocalRoot, "local/cache/rss");
            RssFolder = Strings.Concat(RssFolderRoot, "/", UniqueHostId);
            FeedFolder = Strings.Concat(LocalRoot, "local/cache/feed");

            DefineConstants();
        }

        /// <summary>
        /// Define global constants.
        /// </summary>
        private static void DefineConstants() {
            GlobalConstants = new Hashtable();
            GlobalConstants["[#Site_Name]"] = Config.SITE_NAME;
            GlobalConstants["[#Site_Comments]"] = Config.SITE_COMMENTS;
            GlobalConstants["[#Top_Dir]"] = Config.TOP_DIR;
            GlobalConstants["[#Index_Page]"] = Config.INDEX_PAGE;
            GlobalConstants["[#Action_Page]"] = Config.ACTION_PAGE;
            GlobalConstants["[#Lang]"] = Config.Lang;

            System.Reflection.FieldInfo fieldInfo = typeof(Config).GetField("NAME_CATEGORY");
            if (fieldInfo != null) Set("Name_Category", fieldInfo.GetValue(null));
            fieldInfo = typeof(Config).GetField("NAME_CATEGORIES");
            if (fieldInfo != null) Set("Name_Categories", fieldInfo.GetValue(null));
            fieldInfo = typeof(Config).GetField("NAME_CREATOR");
            if (fieldInfo != null) Set("Name_Creator", fieldInfo.GetValue(null));
            fieldInfo = typeof(Config).GetField("NAME_CUSTOM1");
            if (fieldInfo != null) Set("Name_Custom1", fieldInfo.GetValue(null));
            fieldInfo = typeof(Config).GetField("NAME_CUSTOM2");
            if (fieldInfo != null) Set("Name_Custom2", fieldInfo.GetValue(null));

            // Map custom names
            GlobalConstants["[#Name_Item]"] = Config.NAME_ITEM;
            GlobalConstants["[#Name_Items]"] = Config.NAME_ITEMS;
            if (Contains("Name_Category"))
                GlobalConstants["[#Name_Category]"] = Get("Name_Category");
            if (Contains("Name_Categories"))
                GlobalConstants["[#Name_Categories]"] = Get("Name_Categories");
            if (Contains("Name_Creator"))
                GlobalConstants["[#Name_Creator]"] = Get("Name_Creator");
            if (Contains("Name_Custom1"))
                GlobalConstants["[#Name_Custom1]"] = Get("Name_Custom1");
            if (Contains("Name_Custom2"))
                GlobalConstants["[#Name_Custom2]"] = Get("Name_Custom2");
        }
    }
}