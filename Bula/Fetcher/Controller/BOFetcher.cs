namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;

    using System.Collections;
    using Bula.Model;

    using Bula.Fetcher.Controller;
    using Bula.Fetcher.Model;

    /**
     * Logic for fetching data.
     */
    public class BOFetcher : Bula.Meta {
        private Logger oLogger = null;
        private DataSet dsCategories = null;

        public BOFetcher () {
            this.InitializeLog();
            this.PreLoadCategories();
        }

        ///Initialize logging.
        private void InitializeLog() {
            this.oLogger = new Logger();
            Config.Set("Log_Object", this.oLogger);
            var log = Request.GetOptionalInteger("log");
            if (!NUL(log) && log != -99999) { //TODO
                var filename_template = "C:/Temp/Log_{0}_{1}.html";
                var filename = Util.FormatString(filename_template, ARR("fetch_items", DateTimes.Format(DBConfig.SQL_DTS)));
                this.oLogger.Init(filename);
            }
        }

        ///Pre-load categories into DataSet.
        private void PreLoadCategories() {
            var doCategory = new DOCategory();
            this.dsCategories = doCategory.EnumCategories();
        }

        ///Fetch data from the source.
        /// <param name="oSource">Source object.</param>
        /// <returns>Resulting items.</returns>
        private Object[] FetchFromSource(Hashtable oSource) {
            var url = STR(oSource["s_Feed"]);
            if (url.Length == 0)
                return null;

            var source = STR(oSource["s_SourceName"]);
            if (Request.Contains("m") && !source.Equals(Request.Get("m")))
                return null;

            this.oLogger.Output("<br/>\r\nStarted ");

            if (url.IndexOf("https") != -1) {
                var enc_url = url.Replace("?", "%3F");
                enc_url = enc_url.Replace("&", "%26");
                url = Strings.Concat(Config.Site, "/get_ssl_rss.php?url=", enc_url);
            }
            this.oLogger.Output(CAT("[[[", url, "]]]<br/>\r\n"));
            var rss = Fetch_rss(url); //TODO PHP
            if (rss == null) {
                this.oLogger.Output("-- problems --<br/>\r\n");
                //problems++;
                //if (problems == 5) {
                //    this.oLogger.Output("<br/>\r\nToo many problems... Stopped.<br/>\r\n");
                //    break;
                //}
                return null;
            }
            return rss;
        }

        ///Parse data from the item.
        /// <param name="oSource">Source object.</param>
        /// <param name="item">Item object.</param>
        /// <returns>Result of executing SQL-query.</returns>
        private int ParseItemData(Hashtable oSource, Hashtable item) {
            // Load original values

            var source_name = STR(oSource["s_SourceName"]);
            var source_id = INT(oSource["i_SourceId"]);
            var boItem = new BOItem(source_name, item);
            var pubdate = STR(item["pubdate"]);
            var date = DateTimes.Format(Config.SQL_DTS, DateTimes.FromRss(pubdate));

            // Check whether item with the same link exists already
            var doItem = new DOItem();
            var dsItems = doItem.FindItemByLink(boItem.link, source_id);
            if (dsItems.GetSize() > 0)
                return 0;

            boItem.ProcessDescription();
            //boItem.ProcessCustomFields(); // Uncomment for processing custom fields
            boItem.ProcessCategory();
            boItem.ProcessCreator();

            // Try to add/embed standard categories from description
            boItem.AddStandardCategories(this.dsCategories, Config.Lang);

            var url = boItem.GetUrlTitle(true); //TODO -- Need to pass true if transliteration is required
            var fields = new Hashtable();
            fields["s_Link"] = boItem.link;
            fields["s_Title"] = boItem.title;
            fields["s_FullTitle"] = boItem.full_title;
            fields["s_Url"] = url;
            if (boItem.description != null)
                fields["t_Description"] = boItem.description;
            if (boItem.full_description != null)
                fields["t_FullDescription"] = boItem.full_description;
            fields["d_Date"] = date;
            fields["i_SourceLink"] = INT(oSource["i_SourceId"]);
            if (!BLANK(boItem.category))
                fields["s_Category"] = boItem.category;
            if (!BLANK(boItem.creator))
                fields["s_Creator"] = boItem.creator;
            if (!BLANK(boItem.custom1))
                fields["s_Custom1"] = boItem.custom1;
            if (!BLANK(boItem.custom2))
                fields["s_Custom2"] = boItem.custom2;

            var result = doItem.Insert(fields);
            return result;
        }

        ///Actual cleaning of cache folder.
        /// <param name="path_name">Cache folder name (path).</param>
        /// <param name="ext">Files extension to clean.</param>
        private void CleanCacheFolder(String path_name, String ext) {
            if (!Helper.DirExists(path_name))
                return;

            var entries = Helper.ListDirEntries(path_name);
            while (entries.MoveNext()) {
                var entry = CAT(entries.Current);

                if (Helper.IsFile(entry) && entry.EndsWith(ext)) {
                    this.oLogger.Output(CAT("Deleting of ", entry, " ...<br/>\r\n"));
                    Helper.DeleteFile(entry);
                }
                else if (Helper.IsDir(entry)) {
                    this.oLogger.Output(CAT("Drilling to ", entry, " ...<br/>\r\n"));
                    CleanCacheFolder(entry, ext);
                }
                //unlink(path_name); //Comment for now -- dangerous operation!!!
            }
        }

        ///Clean all cached info (both for Web and RSS).
        public void CleanCache() {
            // Clean cached rss content
            this.oLogger.Output(CAT("Cleaning Rss Folder ", Config.RssFolderRoot, " ...<br/>\r\n"));
            var rssFolder = Strings.Concat(Config.RssFolderRoot);
            CleanCacheFolder(rssFolder, ".xml");

            // Clean cached pages content
            this.oLogger.Output(CAT("Cleaning Cache Folder ", Config.CacheFolderRoot,  "...<br/>\r\n"));
            var cacheFolder = Strings.Concat(Config.CacheFolderRoot);
            CleanCacheFolder(cacheFolder, ".cache");

            this.oLogger.Output("<br/>... Done.<br/>\r\n");
        }

        ///Main logic.
        public void Execute() {
            this.oLogger.Output("Start logging<br/>\r\n");

            //TODO -- Purge old items
            //doItem = new DOItem();
            //doItem.PurgeOldItems(10);

            var doSource = new DOSource();
            var dsSources = doSource.EnumFetchedSources();

            var total_counter = 0;
            this.oLogger.Output(CAT("<br/>\r\nChecking ", dsSources.GetSize(), " sources..."));

            // Loop through sources
            for (int n = 0; n < dsSources.GetSize(); n++) {
                var oSource = dsSources.GetRow(n);

                Object[] items_array = this.FetchFromSource(oSource);
                if (items_array == null)
                    continue;

                // Fetch done for this source
                this.oLogger.Output(" fetched ");

                var items_counter = 0;
                // Loop through fetched items and parse their data
                for (int i = SIZE(items_array) - 1; i >= 0; i--) {
                    var hash = (Hashtable)items_array[i];
                    if (BLANK(hash["link"]))
                        continue;
                    var itemid = this.ParseItemData(oSource, hash);
                    if (itemid > 0) {
                        items_counter++;
                        total_counter++;
                    }
                }

                // Release connection after each source
                if (DBConfig.Connection != null) {
                    DBConfig.Connection.Close();
                    DBConfig.Connection = null;
                }

                this.oLogger.Output(CAT(" (", items_counter, " items) end<br/>\r\n"));
            }

            // Re-count categories
            this.RecountCategories();

            this.oLogger.Output(CAT("<hr/>Total items added - ", total_counter, "<br/>\r\n"));

            if (Config.CACHE_PAGES && total_counter > 0)
                this.CleanCache();
        }

        ///Execute re-counting of categories.
        private void RecountCategories() {
            this.oLogger.Output(CAT("Recount categories ... <br/>\r\n"));
            var doCategory = new DOCategory();
            var dsCategories = doCategory.EnumCategories();
            for (int n = 0; n < dsCategories.GetSize(); n++) {
                var oCategory = dsCategories.GetRow(n);
                var id = STR(oCategory["s_CatId"]);
                var filter = STR(oCategory["s_Filter"]);
                var doItem = new DOItem();
                var sql_filter = doItem.BuildSqlFilter(filter);
                var dsItems = doItem.EnumIds(sql_filter);
                var fields = new Hashtable();
                fields["i_Counter"] = dsItems.GetSize();
                var result = doCategory.UpdateById(id, fields);
                if (result < 0)
                    this.oLogger.Output("-- problems --<br/>\r\n");
            }
            this.oLogger.Output(CAT(" ... Done<br/>\r\n"));
        }

        private Object[] Fetch_rss(String url) {
            var items = new ArrayList();

            System.Xml.XmlDocument rssXmlDoc = new System.Xml.XmlDocument();

            System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(rssXmlDoc.NameTable);
            nsmgr.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");

            // Load the RSS file from the RSS URL
            rssXmlDoc.Load(url);

            // Parse the Items in the RSS file
            System.Xml.XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");

            System.Text.StringBuilder rssContent = new System.Text.StringBuilder();

            // Iterate through the items in the RSS file
            foreach (System.Xml.XmlNode rssNode in rssNodes)
            {
                var item = new Hashtable();

                System.Xml.XmlNode rssSubNode = rssNode.SelectSingleNode("title");
                if (rssSubNode != null)
                    item["title"] = rssSubNode.InnerText;

                rssSubNode = rssNode.SelectSingleNode("link");
                if (rssSubNode != null)
                    item["link"] = rssSubNode.InnerText;

                rssSubNode = rssNode.SelectSingleNode("description");
                if (rssSubNode != null)
                    item["description"] = rssSubNode.InnerText;

                rssSubNode = rssNode.SelectSingleNode("pubDate");
                if (rssSubNode != null)
                    item["pubdate"] = rssSubNode.InnerText; //Yes, lower case

                rssSubNode = rssNode.SelectSingleNode("dc:creator", nsmgr);
                if (rssSubNode != null) {
                    item["dc"] = new Hashtable();
                    ((Hashtable)item["dc"])["creator"] = rssSubNode.InnerText;
                }

                items.Add(item);
            }

            // Return the string that contain the RSS items
            return items.ToArray();
        }
    }
}