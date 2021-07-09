// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher {
    using System;
    using System.Collections;

    /// <summary>
    /// Main class for configuring data.
    /// </summary>
    public class Config : Bula.Meta {
        /// Platform 
        public const String PLATFORM = "NET";
        /// Exactly the same as RewriteBase in .htaccess 
        public const String TOP_DIR = "/";
        /// Index page name 
        public const String INDEX_PAGE = "";
        /// Action page name 
        public const String ACTION_PAGE = "action[#File_Ext]";
        /// RSS-feeds page name 
        public const String RSS_PAGE = "rss[#File_Ext]";
        /// Current API output format (can be "Json" or "Xml" for now) 
        public const String API_FORMAT = "Json";
        /// Current API output content type (can be "application/json" or "text/xml" for now) 
        public const String API_CONTENT = "application/json";
        /// File prefix for constructing real path 
        public const String FILE_PREFIX = "";

        /// Security code 
        public const String SECURITY_CODE = "1234";

        /// Use fine or full URLs 
        public const Boolean FINE_URLS = false;

        /// Cache Web-pages 
        public const Boolean CACHE_PAGES = false;
        /// Cache RSS-feeds 
        public const Boolean CACHE_RSS = false;
        /// Show what source an item is originally from 
        public const Boolean SHOW_FROM = false;
        /// Whether to show images for sources 
        public const Boolean SHOW_IMAGES = true;
        /// File extension for images 
        public const String EXT_IMAGES = "gif";
        /// Show an item or immediately redirect to external source item 
        public const Boolean IMMEDIATE_REDIRECT = false;
        /// How much items to show on "Sources" page 
        public const int LATEST_ITEMS = 3;
        /// Minimum number of items in RSS-feeds 
        public const int MIN_RSS_ITEMS = 5;
        /// Maximum number of items in RSS-feeds 
        public const int MAX_RSS_ITEMS = 50;

        /// Default number of rows on page 
        public const int DB_ROWS = 20;
        /// Default number of rows on "Home" page 
        public const int DB_HOME_ROWS = 15;
        /// Default number of rows on "Items" page 
        public const int DB_ITEMS_ROWS = 25;

        // Fill these fields by your site data
        /// Site language (default - null) 
        public const String SITE_LANGUAGE = null;
        /// Site name 
        public const String SITE_NAME = "Buddy Fetcher";
        /// Site comments 
        public const String SITE_COMMENTS = "Latest News Headlines";
        /// Site keywords 
        public const String SITE_KEYWORDS = "Buddy Fetcher, rss, fetcher, aggregator, [#Platform], MySQL";
        /// Site description 
        public const String SITE_DESCRIPTION = "Buddy Fetcher is a simple RSS fetcher/aggregator written in [#Platform]/MySQL";

        /// Name of item (in singular form) 
        public const String NAME_ITEM = "Headline";
        /// Name of items (in plural form) 
        public const String NAME_ITEMS = "Headlines";
        // Uncomment what fields should be extracted and name them appropriately
        /// Name of category (in singular form) 
        public const String NAME_CATEGORY = "Region";
        /// Name of categories (in plural form) 
        public const String NAME_CATEGORIES = "Regions";
        /// Name of creator 
        public const String NAME_CREATOR = "Creator";
        /// Name of custom field 1 (comment when not extracted) 
        //const String NAME_CUSTOM1 = "Custom1";
        /// Name of custom field 2 (comment when not extracted) 
        //const String NAME_CUSTOM2 = "Custom2";

        /// Show bottom blocks (Filtering and RSS) 
        public const Boolean SHOW_BOTTOM = true;
        /// Show empty categories 
        public const Boolean SHOW_EMPTY = false;
        /// Sort categories by Id (s_CatId) or Name (s_Name) or NULL for default (as-is) 
        public const String SORT_CATEGORIES = null;

        /** Site time shift with respect to GMT (hours*100+minutes) */
        public const int TIME_SHIFT = 0;
        /// Site time zone name (GMT or any other) 
        public const String TIME_ZONE = "GMT";

        /// Powered By string 
        public const String POWERED_BY = "Buddy Fetcher for [#Platform]";
        /// GitHub repository 
        public const String GITHUB_REPO = "buddylancer/fetcher.[#Platform]";
    }
}