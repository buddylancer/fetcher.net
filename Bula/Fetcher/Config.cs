namespace Bula.Fetcher {
    using System;

    public class Config : Common {
        public const String TOP_DIR = "/"; // Exactly the same as RewriteBase in .htaccess
        public const String INDEX_PAGE = "index.aspx";
        public const String ACTION_PAGE = "action.aspx";
        public const String RSS_PAGE = "rss.aspx";

        public const String SECURITY_CODE = "1234";

        public const Boolean FINE_URLS = false; // Use fine or full URLs

        public const Boolean CACHE_PAGES = false;
        public const Boolean CACHE_RSS = false;
        public const Boolean SHOW_FROM = false;
        public const Boolean IMMEDIATE_REDIRECT = false; //Show an item or immediately redirect to external source item
        public const int LATEST_ITEMS = 3;
        public const int MIN_RSS_ITEMS = 5;
        public const int MAX_RSS_ITEMS = 50;

        public const int DB_ROWS = 20;
        public const int DB_HOME_ROWS = 15;
        public const int DB_ITEMS_ROWS = 25;
        public const String GMT_DTS = "dd-MMM-yyyy HH:mm \\G\\M\\T";
        public const String XML_DTS = "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T";
        public const String SQL_DTS = "yyyy-MM-dd HH:mm:ss";

        // Fill these fields by your site data
        public const String SITE_NAME = "Buddy Fetcher";
        public const String SITE_COMMENTS = "Latest Items";
        public const String SITE_KEYWORDS = "Buddy Fetcher, rss, fetcher, aggregator, PHP, MySQL";
        public const String SITE_DESCRIPTION = "Buddy Fetcher is a simple RSS fetcher/aggregator written in PHP/MySQL";

        public const String NAME_ITEM = "Item";
        public const String NAME_ITEMS = "Items";
        // Uncomment what fields should be extracted and name them appropriately
        public const String NAME_CATEGORY = "Category";
        public const String NAME_CATEGORIES = "Categories";
        public const String NAME_CREATOR = "Creator";
        //const String NAME_CUSTOM1 = "Custom1";
        //const String NAME_CUSTOM2 = "Custom2";

        public const Boolean SHOW_BOTTOM = true; // Show bottom blocks (Filtering and Rss)
    } 
}