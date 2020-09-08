// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher {
    using System;

    /// <summary>
    /// Main class for configuring data.
    /// </summary>
    public class Config : Bula.Meta {
        /// Exactly the same as RewriteBase in .htaccess 
        public const String TOP_DIR = "/";
        /// Index page name 
        public const String INDEX_PAGE = "";
        /// Action page name 
        public const String ACTION_PAGE = "action.aspx";
        /// RSS-feeds page name 
        public const String RSS_PAGE = "rss.aspx";

        /// Security code 
        public const String SECURITY_CODE = "1234";

        /// Use fine or full URLs 
        public const Boolean FINE_URLS = false;

        /// Cache Web-pages 
        public const Boolean CACHE_PAGES = true;
        /// Cache RSS-feeds 
        public const Boolean CACHE_RSS = true;
        /// Show what source an item is originally from 
        public const Boolean SHOW_FROM = false;
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
        /// Date/time format for processing GMT date/times 
        public const String GMT_DTS = "dd-MMM-yyyy HH:mm \\G\\M\\T";
        /// Date/time format for RSS operations 
        public const String XML_DTS = "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T";
        /// Date/time format for DB operations 
        public const String SQL_DTS = "yyyy-MM-dd HH:mm:ss";
        public const String LOG_DTS = "yyyy-MM-dd_HH-mm-ss";

        // Fill these fields by your site data
        /// Site name 
        public const String SITE_NAME = "Buddy Fetcher";
        /// Site comments 
        public const String SITE_COMMENTS = "Latest Items";
        /// Site keywords 
        public const String SITE_KEYWORDS = "Buddy Fetcher, rss, fetcher, aggregator, NET, MySQL";
        /// Site description 
        public const String SITE_DESCRIPTION = "Buddy Fetcher is a simple RSS fetcher/aggregator written in NET/MySQL";

        /// Name of item (in singular form) 
        public const String NAME_ITEM = "Item";
        /// Name of items (in plural form) 
        public const String NAME_ITEMS = "Items";
        // Uncomment what fields should be extracted and name them appropriately
        /// Name of category (in singular form) 
        public const String NAME_CATEGORY = "Category";
        /// Name of categories (in plural form) 
        public const String NAME_CATEGORIES = "Categories";
        /// Name of creator 
        public const String NAME_CREATOR = "Creator";
        /// Name of custom field 1 (comment when not extracted) 
        //const String NAME_CUSTOM1 = "Custom1";
        /// Name of custom field 2 (comment when not extracted) 
        //const String NAME_CUSTOM2 = "Custom2";

        /// Show bottom blocks (Filtering and RSS) 
        public const Boolean SHOW_BOTTOM = true;

        public const String POWERED_BY = "Buddy Fetcher for NET";
        public const String GITHUB_REPO = "buddylancer/fetcher.net";
    } 
}