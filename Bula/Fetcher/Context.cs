// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher {
    using System;
    using System.Collections;

    using Bula.Objects;
    using Bula.Fetcher.Controller;

    using Bula.Model;



    /// <summary>
    /// Class for request context.
    /// </summary>
    public class Context : Config {
        /** Default constructor. */
        public Context() { Initialize(); }

        /// <summary>
        /// Constructor for injecting TRequest and TResponse.
        /// </summary>
        /// <param name="request">Current request.</param>
        /// <param name="response">Current response.</param>
        public Context (Object request, Object response) {
            this.Request = new TRequest(request);
            this.Response = new TResponse(response);
            this.Request.response = this.Response;

            this.Connection = Connection.CreateConnection();

            this.Initialize();
        }

        /// Public desctructor 
        ~Context() {
            if (this.Connection != null) {
                this.Connection.Close();
                this.Connection = null;
            }
        }

        /// Current DB connection 
        public Connection Connection = null;
        /// Current request 
        public TRequest Request = null;
        /// Current response 
        public TResponse Response = null;

        /// Storage for internal variables 
        protected THashtable Values = new THashtable();

        /// <summary>
        /// Get internal variable.
        /// </summary>
        /// <param name="name">Name of internal variable.</param>
        /// <returns>Value of variable.</returns>
        public String Get(String name) {
            return (String)this.Values[name];
        }

        /// <summary>
        /// Set internal variable.
        /// </summary>
        /// <param name="name">Name of internal variable.</param>
        /// <param name="value">Value of internal variable to set.</param>
        public void Set(String name, Object value) {
            this.Values[name] = value;
        }

        /// <summary>
        /// Getter/Setter for internal variable.
        /// </summary>
        public Object this[String name] {
            get { return Get(name); }
            set { Set(name, value); }
        }

        /// <summary>
        /// Check whether variable is contained in internal storage.
        /// </summary>
        /// <param name="name">Name of internal variable.</param>
        /// <returns>True - variable exists, False - not exists.</returns>
        public Boolean Contains(String name) {
            return this.Values.ContainsKey(name);
        }

        /// Project root (where Bula folder is located) 
        public String LocalRoot;

        /// Host name (copied from request HOST_NAME) 
        public String Host;
        /// Site name (copied from Config.SITE_NAME) 
        public String Site;
        /// Is request for mobile version? 
        public Boolean IsMobile;
        /// Optional -- API used. Currently can be blank for HTML or "rest" (for REST API) 
        public String Api;
        /// Current language 
        public String Lang;
        /// Current file extension 
        /* Filename extension */
        public const String FILE_EXT = ".aspx";

        /// Root cache folder for pages 
        public String CacheFolderRoot;
        /// Cache folder for pages 
        public String CacheFolder;
        /// Root cache folder for output RSS-feeds 
        public String RssFolderRoot;
        /// Cache folder for output RSS-feeds 
        public String RssFolder;
        /// Cache folder for input RSS-feeds 
        public String FeedFolder;
        /// Unique host ID for current request 
        public String UniqueHostId;

        /// Use fine or full URLs 
        public Boolean FineUrls = Config.FINE_URLS;
        /// Show an item or immediately redirect to external source item 
        public Boolean ImmediateRedirect = Config.IMMEDIATE_REDIRECT;

        /// Storage for global constants 
        public THashtable GlobalConstants = null;

        /// Is current request from test script? 
        public Boolean TestRun = false;

        /// <summary>
        /// Check whether current request is from test script?
        /// </summary>
        public void CheckTestRun() {
            var httpTester = this.Request.GetVar(TRequest.INPUT_SERVER, "HTTP_USER_AGENT");
            if (httpTester == null)
                return;
            if (EQ(httpTester, "TestFull")) {
                this.TestRun = true;
                this.FineUrls = false;
                this.ImmediateRedirect = false;
                //this.Site = "http://www.test.com";
            }
            else if (EQ(httpTester, "TestFine")) {
                this.TestRun = true;
                this.FineUrls = true;
                this.ImmediateRedirect = false;
                //this.Site = "http://www.test.com";
            }
            else if (EQ(httpTester, "TestDirect")) {
                this.TestRun = true;
                this.FineUrls = true;
                this.ImmediateRedirect = true;
                //this.Site = "http://www.test.com";
            }
        }

        /// <summary>
        /// Initialize all variables for current request.
        /// </summary>
        public void Initialize() {
            //------------------------------------------------------------------------------
            // You can change something below this line if you know what are you doing :)
            var rootDir = Request.GetVar(TRequest.INPUT_SERVER, "APPL_PHYSICAL_PATH");
            rootDir = rootDir.Replace("\\", "/"); // Fix for IIS
            var removeSlashes =
                3;
            // Regarding that we have the ordinary local website (not virtual directory)
            for (int n = 0; n <= removeSlashes; n++) {
                var lastSlashIndex = rootDir.LastIndexOf("/");
                rootDir = rootDir.Substring(0, lastSlashIndex);
            }
            this.LocalRoot = rootDir += "/";

            this.Host = this.Request.GetVar(TRequest.INPUT_SERVER, "HTTP_HOST");
            this.Site = Strings.Concat("http://", this.Host);
            this.IsMobile = this.Host.IndexOf("m.") == 0;
            this.Lang = this.Host.LastIndexOf(".ru") != -1 ? "ru" : "en";

            this.CheckTestRun();
            this.UniqueHostId = Strings.Concat(
                this.IsMobile ? "mob_" : "www_",
                this.FineUrls ? (this.ImmediateRedirect ? "direct_" : "fine_") : "full_",
                this.Lang);
            this.CacheFolderRoot = Strings.Concat(this.LocalRoot, "local/cache/www");
            this.CacheFolder = Strings.Concat(this.CacheFolderRoot, "/", this.UniqueHostId);
            this.RssFolderRoot = Strings.Concat(this.LocalRoot, "local/cache/rss");
            this.RssFolder = Strings.Concat(this.RssFolderRoot, "/", this.UniqueHostId);
            this.FeedFolder = Strings.Concat(this.LocalRoot, "local/cache/feed");

            this.DefineConstants();
        }

        /// <summary>
        /// Define global constants.
        /// </summary>
        private void DefineConstants() {
            this.GlobalConstants = new THashtable();
            this.GlobalConstants["[#Site_Name]"] = Config.SITE_NAME;
            this.GlobalConstants["[#Site_Comments]"] = Config.SITE_COMMENTS;
            this.GlobalConstants["[#Top_Dir]"] = Config.TOP_DIR;

            if (!this.TestRun)
                this.GlobalConstants["[#File_Ext]"] = FILE_EXT;
            this.GlobalConstants["[#Index_Page]"] = this.TestRun ? Config.INDEX_PAGE :
                Strings.Replace("[#File_Ext]", FILE_EXT, Config.INDEX_PAGE);
            this.GlobalConstants["[#Action_Page]"] = this.TestRun ? Config.ACTION_PAGE :
                Strings.Replace("[#File_Ext]", FILE_EXT, Config.ACTION_PAGE);
            this.GlobalConstants["[#Rss_Page]"] = this.TestRun ? Config.RSS_PAGE :
                Strings.Replace("[#File_Ext]", FILE_EXT, Config.RSS_PAGE);

            //if (this.IsMobile)
            //    this.GlobalConstants["[#Is_Mobile]"] = "1";
            this.GlobalConstants["[#Lang]"] = this.Lang;

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
            this.GlobalConstants["[#Name_Item]"] = Config.NAME_ITEM;
            this.GlobalConstants["[#Name_Items]"] = Config.NAME_ITEMS;
            if (this.Contains("Name_Category"))
                this.GlobalConstants["[#Name_Category]"] = this["Name_Category"];
            if (this.Contains("Name_Categories"))
                this.GlobalConstants["[#Name_Categories]"] = this["Name_Categories"];
            if (this.Contains("Name_Creator"))
                this.GlobalConstants["[#Name_Creator]"] = this["Name_Creator"];
            if (this.Contains("Name_Custom1"))
                this.GlobalConstants["[#Name_Custom1]"] = this["Name_Custom1"];
            if (this.Contains("Name_Custom2"))
                this.GlobalConstants["[#Name_Custom2]"] = this["Name_Custom2"];
        }

        private TArrayList EngineInstances = null;
        private int EngineIndex = -1;

        /// <summary>
        /// Push engine.
        /// </summary>
        /// <param name="printFlag">Whether to print content immediately (true) or save it for further processing (false).</param>
        /// <returns>New Engine instance.</returns>
        public Engine PushEngine(Boolean printFlag) {
            var engine = new Engine(this);
            engine.SetPrintFlag(printFlag);
            this.EngineIndex++;
            if (this.EngineInstances == null)
                this.EngineInstances = new TArrayList();
            if (this.EngineInstances.Size() <= this.EngineIndex)
                this.EngineInstances.Add(engine);
            else
                this.EngineInstances[this.EngineIndex] = engine;
            return engine;
        }

        /// Pop engine back. 
        public void PopEngine() {
            if (this.EngineIndex == -1)
                return;
            var engine = (Engine)this.EngineInstances[this.EngineIndex];
            engine.SetPrintString(null);
            //TODO Dispose engine?
            this.EngineIndex--;
        }

        /// <summary>
        /// Get current engine
        /// </summary>
        /// <returns>Current engine instance.</returns>
        public Engine GetEngine() {
            return (Engine)this.EngineInstances[this.EngineIndex];
        }
    }
}