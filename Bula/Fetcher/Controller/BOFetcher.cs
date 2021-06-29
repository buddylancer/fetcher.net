// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;
    using System.Collections;

    using Bula;
    using Bula.Fetcher;
    using Bula.Objects;
    using Bula.Model;
    using Bula.Fetcher.Model;
    using Bula.Fetcher.Controller.Actions;

    /// <summary>
    /// Logic for fetching data.
    /// </summary>
    public class BOFetcher : Bula.Meta {
        private Context context = null;
        private Logger oLogger = null;
        private DataSet dsCategories = null;
        private DataSet dsRules = null;

        /// Public default constructor 
        public BOFetcher (Context context) {
            this.context = context;
            this.InitializeLog();
            this.PreLoadCategories();
        }

        /// <summary>
        /// Initialize logging.
        /// </summary>
        private void InitializeLog() {
            this.oLogger = new Logger();
            this.context["Log_Object"] = this.oLogger;
            var log = this.context.Request.GetOptionalInteger("log");
            if (!NUL(log) && log != -99999) { //TODO
                var filenameTemplate = (String)CAT(this.context.LocalRoot, "local/logs/{0}_{1}.html");
                var filename = Util.FormatString(filenameTemplate, ARR("fetch_items", DateTimes.Format(DateTimes.LOG_DTS)));
                this.oLogger.InitFile(filename);
            }
            else
                this.oLogger.InitResponse(this.context.Response);
        }

        /// <summary>
        /// Pre-load categories into DataSet.
        /// </summary>
        private void PreLoadCategories() {
            var doCategory = new DOCategory();
            this.dsCategories = doCategory.EnumCategories();
            var doRule = new DORule();
            this.dsRules = doRule.EnumAll();
        }

        /// <summary>
        /// Fetch data from the source.
        /// </summary>
        /// <param name="oSource">Source object.</param>
        /// <returns>Resulting items.</returns>
        /// <param name="from">Addition to feed URL (for testing purposes)</param>
        private Object[] FetchFromSource(THashtable oSource, String from) {
            var url = STR(oSource["s_Feed"]);
            if (url.Length == 0)
                return null;

            if (!NUL(from))
                url = Strings.Concat(url, "&from=", from);

            var source = STR(oSource["s_SourceName"]);
            if (this.context.Request.Contains("m") && !source.Equals(this.context.Request["m"]))
                return null;

            this.oLogger.Output(CAT("<br/>", EOL, "Started "));

            //if (url.IndexOf("https") != -1) {
            //    String encUrl = url.Replace("?", "%3F");
            //    encUrl = encUrl.Replace("&", "%26");
            //    url = Strings.Concat(Config.Site, "/get_ssl_rss.php?url=", encUrl);
            //}
            this.oLogger.Output(CAT("[[[", url, "]]]"));
            Object[] rss = Internal.FetchRss(url);
            if (rss == null) {
                this.oLogger.Output(CAT("-- problems --<br/>", EOL));
                //problems++;
                //if (problems == 5) {
                //    this.oLogger.Output(CAT("<br/>", EOL, "Too many problems... Stopped.<br/>", EOL));
                //    break;
                //}
                return null;
            }
            return rss;
        }

        /// <summary>
        /// Parse data from the item.
        /// </summary>
        /// <param name="oSource">Source object.</param>
        /// <param name="item">Item object.</param>
        /// <returns>Result of executing SQL-query.</returns>
        private int ParseItemData(THashtable oSource, THashtable item) {
            // Load original values

            var sourceName = STR(oSource["s_SourceName"]);
            var sourceId = INT(oSource["i_SourceId"]);
            var boItem = new BOItem(sourceName, item);
            var pubDate = STR(item["pubdate"]);
            if (BLANK(pubDate) && !BLANK(item["dc"])) { //TODO implement [dc][time]
                var temp = (THashtable)item["dc"];
                if (!BLANK(temp["date"]))
                    pubDate = STR(temp["date"]);
            }
            //TODO -- workaround for life.ru (error - pubDate inside guid)

            if (BLANK(pubDate)) pubDate = STR(item["guid_pubdate"]);

            var date = DateTimes.GmtFormat(DateTimes.SQL_DTS, DateTimes.FromRss(pubDate));

            boItem.ProcessDescription();
            //boItem.ProcessCustomFields(); // Uncomment for processing custom fields
            boItem.ProcessCategory();
            boItem.ProcessCreator();

            // Process rules AFTER processing description (as some info can be extracted from it)
            boItem.ProcessRules(this.dsRules);

            if (BLANK(boItem.link)) //TODO - what we can do else?
                return 0;

            // Check whether item with the same link exists already
            var doItem = new DOItem();
            var dsItems = doItem.FindItemByLink(boItem.link, sourceId);
            if (dsItems.GetSize() > 0)
                return 0;

            // Try to add/embed standard categories from description
            var countCategories = boItem.AddStandardCategories(this.dsCategories, this.context.Lang);
            //print "countCategories: '" . countCategories . "'<br/>\r\n";

            // Check the link once again after processing rules
            if (dsItems == null && !BLANK(boItem.link)) {
                doItem.FindItemByLink(boItem.link, sourceId);
                if (dsItems.GetSize() > 0)
                    return 0;
            }

            var url = boItem.GetUrlTitle(true); //TODO -- Need to pass true if transliteration is required
            var fields = new THashtable();
            fields["s_Link"] = boItem.link;
            fields["s_Title"] = boItem.title;
            fields["s_FullTitle"] = boItem.fullTitle;
            fields["s_Url"] = url;
            fields["i_Categories"] = countCategories;
            if (boItem.description != null)
                fields["t_Description"] = boItem.description;
            if (boItem.fullDescription != null)
                fields["t_FullDescription"] = boItem.fullDescription;
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

        /// <summary>
        /// Main logic.
        /// </summary>
        /// <param name="from">Addition to feed URL (for testing purposes)</param>
        public void FetchFromSources(String from) {
            this.oLogger.Output(CAT("Start logging<br/>", EOL));

            //TODO -- Purge old items
            //doItem = new DOItem();
            //doItem.PurgeOldItems(10);

            var doSource = new DOSource();
            var dsSources = doSource.EnumFetchedSources();

            var totalCounter = 0;
            this.oLogger.Output(CAT("<br/>", EOL, "Checking ", dsSources.GetSize(), " sources..."));

            // Loop through sources
            for (int n = 0; n < dsSources.GetSize(); n++) {
                var oSource = dsSources.GetRow(n);

                Object[] itemsArray = this.FetchFromSource(oSource, from);
                if (itemsArray == null)
                    continue;

                // Fetch done for this source
                //this.oLogger.Output(" fetched ");

                var itemsCounter = 0;
                // Loop through fetched items and parse their data
                for (int i = SIZE(itemsArray) - 1; i >= 0; i--) {
                    var hash = (THashtable)itemsArray[i];
                    if (BLANK(hash["link"]))
                        continue;
                    var itemid = this.ParseItemData(oSource, hash);
                    if (itemid > 0) {
                        itemsCounter++;
                        totalCounter++;
                    }
                }

                // Release connection after each source
                if (DBConfig.Connection != null) {
                    DBConfig.Connection.Close();
                    DBConfig.Connection = null;
                }

                this.oLogger.Output(CAT("<br/>", EOL, "... fetched (", itemsCounter, " items) end"));
            }

            // Re-count categories
            this.RecountCategories();

            this.oLogger.Output(CAT("<br/>", EOL, "<hr/>Total items added - ", totalCounter, "<br/>", EOL));

            if (Config.CACHE_PAGES && totalCounter > 0) {
                var doCleanCache = new DoCleanCache(this.context);
                doCleanCache.CleanCache(this.oLogger);
            }
        }

        /// <summary>
        /// Execute re-counting of categories.
        /// </summary>
        private void RecountCategories() {
            this.oLogger.Output(CAT("<br/>", EOL, "Recount categories ... "));
            var doCategory = new DOCategory();
            var doItem = new DOItem();
            var dsCategories = doCategory.EnumCategories();
            for (int n = 0; n < dsCategories.GetSize(); n++) {
                var oCategory = dsCategories.GetRow(n);
                var categoryId = STR(oCategory["s_CatId"]);
                var oldCounter = INT(oCategory["i_Counter"]);

                //String filter = STR(oCategory["s_Filter"]);
                //String sqlFilter = DOItem.BuildSqlByFilter(filter);

                var categoryName = STR(oCategory["s_Name"]);
                var sqlFilter = DOItem.BuildSqlByCategory(categoryName);

                var dsCounters = doItem.EnumIds(CAT("_this.b_Counted = 0 AND ", sqlFilter));
                if (dsCounters.GetSize() == 0)
                    continue;

                var newCounter = INT(dsCounters.GetSize());

                //Update category
                var categoryFields = new THashtable();
                categoryFields["i_Counter"] = oldCounter + newCounter;
                doCategory.UpdateById(categoryId, categoryFields);
            }

            doItem.Update("_this.b_Counted = 1", "_this.b_Counted = 0");

            this.oLogger.Output(CAT(" ... Done<br/>", EOL));
        }
    }
}