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
            int log = Request.GetOptionalInteger("log");
            if (!NUL(log)) {
                String filename_template = STR("C:/Temp/Log_{0}_{1}.html");
                String filename = Util.FormatString(filename_template, ARR("fetch_items", DateTimes.Format("Y-m-d_h-i-s")));
                this.oLogger.Init(filename);
            }
        }

        ///Pre-load categories into DataSet.
        private void PreLoadCategories() {
            DOCategory doCategory = new DOCategory();
            this.dsCategories = doCategory.EnumCategories();
        }

        ///Fetch data from the source.
        /// <param name="oSource">Source object.</param>
        /// <returns>Resulting items.</returns>
        private Object[] FetchFromSource(Hashtable oSource) {
            String url = STR(oSource["s_Feed"]);
            if (url.Length == 0)
                return null;

            String source = STR(oSource["s_SourceName"]);
            if (Request.Contains("m") && !source.Equals(Request.Get("m")))
                return null;

            this.oLogger.Output("<br/>\r\nStarted ");

            if (url.IndexOf("https") != -1) {
                String enc_url = url.Replace("?", "%3F");
                enc_url = enc_url.Replace("&", "%26");
                url = Strings.Concat(Config.Site, "/get_ssl_rss.php?url=", enc_url);
            }
            this.oLogger.Output(CAT("[[[", url, "]]]<br/>\r\n"));
            Object rss = Fetch_rss(url); //TODO PHP
            if (!rss) {
                this.oLogger.Output("-- problems --<br/>\r\n");
                //problems++;
                //if (problems == 5) {
                //    this.oLogger.Output("<br/>\r\nToo many problems... Stopped.<br/>\r\n");
                //    break;
                //}
                return null;
            }
            return rss.items;
        }

        ///Parse data from the item.
        /// <param name="oSource">Source object.</param>
        /// <param name="item">Item object.</param>
        /// <returns>Result of executing SQL-query.</returns>
        private int ParseItemData(Hashtable oSource, Hashtable item) {
            // Load original values

            String source_name = STR(oSource["s_SourceName"]);
            int source_id = INT(oSource["i_SourceId"]);
            BOItem boItem = new BOItem(source_name, item);
            String pubdate = STR(item["pubdate"]);
            String date = Strings.GetSqlDate(pubdate);

            // Check whether item with the same link exists already
            DOItem doItem = new DOItem();
            DataSet dsItems = doItem.FindItemByLink(boItem.link, source_id);
            if (dsItems.GetSize() > 0)
                return 0;

            boItem.ProcessDescription();
            //boItem.ProcessCustomFields(); // Uncomment for processing custom fields
            boItem.ProcessCategory();
            boItem.ProcessCreator();

            // Try to add/embed standard categories from description
            boItem.AddStandardCategories(this.dsCategories, Config.Lang);

            String url = boItem.GetUrlTitle(true); //TODO -- Need to pass true if transliteration is required
            Hashtable fields = new Hashtable();
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

            int result = doItem.Insert(fields);
            return result;
        }

        ///Actual cleaning of cache folder.
        /// <param name="path_name">Cache folder name (path).</param>
        /// <param name="ext">Files extension to clean.</param>
        private void CleanCacheFolder(String path_name, String ext) {
            if (!Helper.DirExists(path_name))
                return;

            IEnumerator entries = Helper.ListDirEntries(path_name);
            while (entries.MoveNext()) {
                String entry = CAT(entries.Current);

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
            String rssFolder = Strings.Concat(Config.RssFolderRoot);
            CleanCacheFolder(rssFolder, ".xml");

            // Clean cached pages content
            this.oLogger.Output(CAT("Cleaning Cache Folder ", Config.CacheFolderRoot,  "...<br/>\r\n"));
            String cacheFolder = Strings.Concat(Config.CacheFolderRoot);
            CleanCacheFolder(cacheFolder, ".cache");

            this.oLogger.Output("<br/>... Done.<br/>\r\n");
        }

        ///Main logic.
        public void Execute() {
            this.oLogger.Output("Start logging<br/>\r\n");

            //TODO -- Purge old items
            //doItem = new DOItem();
            //doItem.PurgeOldItems(10);

            Define("MAGPIE_CACHE_ON", true);
            Define("MAGPIE_OUTPUT_ENCODING", "UTF-8");
            Define("MAGPIE_DEBUG", 1);
            Define("MAGPIE_FETCH_TIME_OUT", 30);
            Define("MAGPIE_CACHE_DIR", CAT(Config.FeedFolder));
            DOSource doSource = new DOSource();
            DataSet dsSources = doSource.EnumFetchedSources();

            int total_counter = 0;
            this.oLogger.Output(CAT("<br/>\r\nChecking ", dsSources.GetSize(), " sources..."));

            // Loop through sources
            for (int n = 0; n < dsSources.GetSize(); n++) {
                Hashtable oSource = dsSources.GetRow(n);

                Object[] items_array = this.FetchFromSource(oSource);
                if (items_array == null)
                    continue;

                // Fetch done for this source
                this.oLogger.Output(" fetched ");

                int items_counter = 0;
                // Loop through fetched items and parse their data
                for (int i = SIZE(items_array) - 1; i >= 0; i--) {
                    Hashtable hash = Arrays.CreateHashtable(items_array[i]);
                    if (BLANK(hash["link"]))
                        continue;
                    int itemid = this.ParseItemData(oSource, hash);
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
            DOCategory doCategory = new DOCategory();
            DataSet dsCategories = doCategory.EnumCategories();
            for (int n = 0; n < dsCategories.GetSize(); n++) {
                Hashtable oCategory = dsCategories.GetRow(n);
                String id = STR(oCategory["s_CatId"]);
                String filter = STR(oCategory["s_Filter"]);
                DOItem doItem = new DOItem();
                String sql_filter = doItem.BuildSqlFilter(filter);
                DataSet dsItems = doItem.EnumIds(sql_filter);
                Hashtable fields = new Hashtable();
                fields["i_Counter"] = dsItems.GetSize();
                int result = doCategory.UpdateById(id, fields);
                if (result < 0)
                    this.oLogger.Output("-- problems --<br/>\r\n");
            }
            this.oLogger.Output(CAT(" ... Done<br/>\r\n"));
        }
    }
}