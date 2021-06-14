// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;
    using System.Collections;

    using Bula.Fetcher;
    using System.Text.RegularExpressions;
    using Bula.Objects;
    using Bula.Model;
    using Bula.Fetcher.Model;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Main logic for generating RSS-feeds and REST API responses.
    /// </summary>
    public abstract class RssBase : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public RssBase(Context context) : base(context) { }

        /// <summary>
        /// Execute main logic for generating RSS-feeds.
        /// </summary>
        public override void Execute() {
            //this.context.Request.Initialize();
            this.context.Request.ExtractAllVars();

            var errorMessage = "";

            // Check source
            var source = this.context.Request["source"];
            if (!NUL(source)) {
                if (BLANK(source))
                    errorMessage += "Empty source!";
                else {
                    var doSource = new DOSource();
                    Hashtable[] oSource =
                        {new Hashtable()};
                    if (!doSource.CheckSourceName(source, oSource))
                        errorMessage += CAT("Incorrect source '", source, "'!");
                }
            }

            var anyFilter = false;
            if (this.context.Request.Contains("code")) {
                if (EQ(this.context.Request["code"], Config.SECURITY_CODE))
                    anyFilter = true;
            }

            // Check filter
            var filter = (String)null;
            var filterName = (String)null;
            var doCategory = new DOCategory();
            var dsCategories = doCategory.EnumCategories();
            if (dsCategories.GetSize() > 0) {
                filterName = this.context.Request["filter"];
                if (!NUL(filterName)) {
                    if (BLANK(filterName)) {
                        if (errorMessage.Length > 0)
                            errorMessage += " ";
                        errorMessage += "Empty filter!";
                    }
                    else {
                        Hashtable[] oCategory =
                            {new Hashtable()};
                        if (doCategory.CheckFilterName(filterName, oCategory))
                            filter = STR(oCategory[0]["s_Filter"]);
                        else {
                            if (anyFilter)
                                filter = filterName;
                            else {
                                if (errorMessage.Length > 0)
                                    errorMessage += " ";
                                errorMessage += CAT("Incorrect filter '", filterName, "'!");
                            }
                        }
                    }
                }
            }

            // Check that parameters contain only 'source' or/and 'filter'
            var keys = this.context.Request.GetKeys();
            while (keys.MoveNext()) {
                var key = (String)keys.Current;
                if (EQ(key, "source") || EQ(key, "filter") || EQ(key, "code") || EQ(key, "count")) {
                    //OK
                }
                else {
                    //Not OK
                    if (errorMessage.Length > 0)
                        errorMessage += " ";
                    errorMessage += CAT("Incorrect parameter '", key, "'!");
                }
            }

            if (errorMessage.Length > 0) {
                this.WriteErrorMessage(errorMessage);
                return;
            }

            var fullTitle = false;
            if (this.context.Request.Contains("title") && STR(this.context.Request["title"]) == "full")
                fullTitle = true;

            var count = Config.MAX_RSS_ITEMS;
            var countSet = false;
            if (this.context.Request.Contains("count")) {
                if (INT(this.context.Request["count"]) > 0) {
                    count = INT(this.context.Request["count"]);
                    if (count < Config.MIN_RSS_ITEMS)
                        count = Config.MIN_RSS_ITEMS;
                    if (count > Config.MAX_RSS_ITEMS)
                        count = Config.MAX_RSS_ITEMS;
                    countSet = true;
                }
            }

            // Get content from cache (if enabled and cache data exists)
            var cachedFile = "";
            if (Config.CACHE_RSS && !countSet) {
                cachedFile = Strings.Concat(
                    this.context.RssFolder, "/rss",
                    (BLANK(source) ? null : CAT("-s=", source)),
                    (BLANK(filterName) ? null : CAT("-f=", filterName)),
                    (fullTitle ? "-full" : null), ".xml");
                if (Helper.FileExists(cachedFile)) {
                    this.context.Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
                    var tempContent = Helper.ReadAllText(cachedFile);
                    //this.context.Response.Write(tempContent.Substring(3)); //TODO -- BOM?
                    this.context.Response.Write(tempContent); //TODO -- BOM?
                    return;
                }
            }

            var doItem = new DOItem();

            // 0 - item url
            // 1 - item title
            // 2 - marketplace url
            // 3 - marketplace name
            // 4 - date
            // 5 - description
            // 6 - category

            var pubDate = DateTimes.Format(DateTimes.XML_DTS);
            var nowDate = DateTimes.Format(DateTimes.SQL_DTS);
            var nowTime = DateTimes.GetTime(nowDate);
            var fromDate = DateTimes.GmtFormat(DateTimes.SQL_DTS, nowTime - 6*60*60);
            var dsItems = doItem.EnumItemsFromSource(fromDate, source, filter, count);
            var current = 0;

            var contentToCache = "";
            if (dsItems.GetSize() == 0)
                contentToCache = this.WriteStart(source, filterName, pubDate);

            for (int n = 0; n < dsItems.GetSize(); n++) {
                var oItem = dsItems.GetRow(n);
                var date = STR(oItem["d_Date"]);
                if (DateTimes.GetTime(date) > nowTime)
                    continue;

                if (current == 0) {
                    // Get puDate from the first item and write starting block
                    pubDate = DateTimes.Format(DateTimes.XML_DTS, DateTimes.GetTime(date));
                    contentToCache = this.WriteStart(source, filterName, pubDate);
                }

                var category = this.context.Contains("Name_Category") ? STR(oItem["s_Category"]) : null;
                var creator = this.context.Contains("Name_Creator") ? STR(oItem["s_Creator"]) : null;
                String custom1 = this.context.Contains("Name_Custom1") ? STR(oItem["s_Custom1"]) : null;
                String custom2 = this.context.Contains("Name_Custom2") ? STR(oItem["s_Custom2"]) : null;

                var sourceName = STR(oItem["s_SourceName"]);
                var description = STR(oItem["t_Description"]);
                if (!BLANK(description)) {
                    description = Regex.Replace(description, "<br/>", " ", RegexOptions.IgnoreCase);
                    description = Regex.Replace(description, "&nbsp;", " ");
                    description = Regex.Replace(description, "[ \r\n\t]+", " ");
                    if (description.Length > 512) {
                        description = description.Substring(0, 511);
                        var lastSpaceIndex = description.LastIndexOf(" ");
                        description = Strings.Concat(description.Substring(0, lastSpaceIndex), " ...");
                    }
                    //Boolean utfIsValid = Mb_check_encoding(description, "UTF-8");
                    //if (utfIsValid == false)
                    //    description = ""; //TODO
                }
                var itemTitle = CAT(
                    (fullTitle == true && !BLANK(custom2) ? CAT(custom2, " | ") : null),
                    Strings.RemoveTags(Strings.StripSlashes(STR(oItem["s_Title"]))),
                    (fullTitle == true ? CAT(" [", sourceName, "]") : null)
                );

                var link = (String)null;
                if (this.context.ImmediateRedirect)
                    link = STR(oItem["s_Link"]);
                else {
                    var url = STR(oItem["s_Url"]);
                    var idField = doItem.GetIdField();
                    link = this.GetAbsoluteLink(Config.INDEX_PAGE, "?p=view_item&amp;id=", "item/", oItem[idField]);
                    if (!BLANK(url))
                        link = this.AppendLink(link, "&amp;title=", "/", url);
                }

                Object[] args = ARR(7);
                args[0] = link;
                args[1] = itemTitle;
                args[2] = this.GetAbsoluteLink(Config.ACTION_PAGE, "?p=do_redirect_source&amp;source=", "redirect/source/", sourceName);
                args[3] = sourceName;
                args[4] = DateTimes.Format(DateTimes.XML_DTS, DateTimes.GetTime(date));
                var additional = CAT(
                    (BLANK(creator) ? null : CAT(this.context["Name_Creator"], ": ", creator, "<br/>")),
                    (BLANK(category) ? null : CAT(this.context["Name_Categories"], ": ", category, "<br/>")),
                    (BLANK(custom2) ? null : CAT(this.context["Name_Custom2"], ": ", custom2, "<br/>")),
                    (BLANK(custom1) ? null : CAT(this.context["Name_Custom1"], ": ", custom1, "<br/>"))
                );
                var extendedDescription = (String)null;
                if (!BLANK(description)) {
                    if (BLANK(additional))
                        extendedDescription = description;
                    else
                        extendedDescription = CAT(additional, "<br/>", description);
                }
                else if (!BLANK(additional))
                    extendedDescription = additional;
                args[5] = extendedDescription;
                args[6] = category;

                var itemContent = this.WriteItem(args);
                if (!BLANK(itemContent))
                    contentToCache += itemContent;

                current++;
            }

            var endContent = this.WriteEnd();
            if (!BLANK(endContent))
                contentToCache += endContent;

            // Save content to cache (if applicable)
            if (Config.CACHE_RSS && !countSet) {
                Helper.TestFileFolder(cachedFile);
                //Helper.WriteText(cachedFile, Strings.Concat("\xEF\xBB\xBF", xmlContent));
                Helper.WriteText(cachedFile, contentToCache);
            }

            if (DBConfig.Connection != null) {
                DBConfig.Connection.Close();
                DBConfig.Connection = null;
            }
        }

        /// <summary>
        /// Write error message.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public abstract void WriteErrorMessage(String errorMessage);

        /// <summary>
        /// Write start block (header) of an RSS-feed.
        /// </summary>
        /// <param name="source">Source selected (or empty).</param>
        /// <param name="filterName">Filter name selected (or empty).</param>
        /// <param name="pubDate">Date shown in the header.</param>
        public abstract String WriteStart(String source, String filterName, String pubDate);

        /// <summary>
        /// Write end block of an RSS-feed.
        /// </summary>
        public abstract String WriteEnd();

        /// <summary>
        /// Write RSS-feed item.
        /// </summary>
        /// <param name="args">Parameters to fill an item.</param>
        public abstract String WriteItem(Object[] args);
    }
}