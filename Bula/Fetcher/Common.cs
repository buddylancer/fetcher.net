namespace Bula.Fetcher {
    using System;

    using Bula.Objects;
    using System.Collections;

    public class Common : Bula.Meta {
        static Common() { Initialize(); }

        protected static Hashtable Values = new Hashtable();
        public static String Get(String name) {
            return (String)Values[name];
        }
        public static void Set(String name, Object value) {
            Values[name] = value;
        }
        public static Boolean Contains(String name) {
            return Values.Contains(name);
        }

        public static String LocalRoot;

        public static String Host;
        public static String Site;
        public static Boolean IsMobile;
        public static String Lang;

        public static String CacheFolderRoot;
        public static String CacheFolder;
        public static String RssFolderRoot;
        public static String RssFolder;
        public static String FeedFolder;
        public static String UniqueHostId;

        public static Boolean FineUrls = Config.FINE_URLS;
        public static Boolean ImmediateRedirect = Config.IMMEDIATE_REDIRECT;

        public static Hashtable GlobalConstants = null;

        public static Boolean TestRun = false;
        public static void CheckTestRun() {
            String http_tester = Request.GetVar(Request.INPUT_SERVER, "HTTP_USER_AGENT");
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

        public static void Initialize() {
            //------------------------------------------------------------------------------
            // You can change something below this line if you know what are you doing :)
            String root_dir = Request.GetVar(Request.INPUT_SERVER, "APPL_PHYSICAL_PATH");
            for (int n = 0; n <= 3; n++) {
                int last_slash_index = root_dir.LastIndexOf("\\");
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
            /*Java
            TODO
            Java*/

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