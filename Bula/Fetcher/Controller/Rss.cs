// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using System.Text.RegularExpressions;
    using Bula.Objects;

    using Bula.Model;
    using Bula.Fetcher.Model;

    using Bula.Fetcher.Controller;

    /// <summary>
    /// Main logic for generating RSS-feeds.
    /// </summary>
    public class Rss : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public Rss(Context context) : base(context) { }

        /// <summary>
        /// Execute main logic for generating RSS-feeds.
        /// </summary>
        public override void Execute() {
            Request.Initialize();
            Request.ExtractAllVars();

            var errorMessage = "";

            // Check source
            var source = Request.Get("source");
            if (!NUL(source)) {
                if (BLANK(source))
                    errorMessage += ("Empty source!");
                else {
                    var doSource = new DOSource();
                    Hashtable[] oSource =
                        {new Hashtable()};
                    if (!doSource.CheckSourceName(source, oSource))
                        errorMessage += (CAT("Incorrect source '", source, "'!"));
                }
            }

            var anyFilter = false;
            if (Request.Contains("code")) {
                if (EQ(Request.Get("code"), Config.SECURITY_CODE))
                    anyFilter = true;
            }

            // Check filter
            var filter = (String)null;
            var filterName = (String)null;
            var doCategory = new DOCategory();
            var dsCategories = doCategory.EnumCategories();
            if (dsCategories.GetSize() > 0) {
                filterName = Request.Get("filter");
                if (!NUL(filterName)) {
                    if (BLANK(filterName)) {
                        if (errorMessage.Length > 0)
                            errorMessage += (" ");
                        errorMessage += ("Empty filter!");
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
                                    errorMessage += (" ");
                                errorMessage += (CAT("Incorrect filter '", filterName, "'!"));
                            }
                        }
                    }
                }
            }

            // Check that parameters contain only 'source' or/and 'filter'
            var keys = Request.GetKeys();
            while (keys.MoveNext()) {
                var key = (String)keys.Current;
                if (key != "source" && key != "filter" && key != "code" && key != "count") {
                    if (errorMessage.Length > 0)
                        errorMessage += (" ");
                    errorMessage += (CAT("Incorrect parameter '", key, "'!"));
                }
            }

            if (errorMessage.Length > 0) {
                Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
                Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n");
                Response.Write(CAT("<data>", errorMessage, "</data>"));
                return;
            }

            var fullTitle = false;
            if (Request.Contains("title") && STR(Request.Get("title")) == "full")
                fullTitle = true;

            var count = Config.MAX_RSS_ITEMS;
            var countSet = false;
            if (Request.Contains("count")) {
                if (INT(Request.Get("count")) > 0) {
                    count = INT(Request.Get("count"));
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
                    Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
                    var tempContent = Helper.ReadAllText(cachedFile);
                    //Response.Write(tempContent.Substring(3)); //TODO -- BOM?
                    Response.Write(tempContent); //TODO -- BOM?
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

            var pubDate = DateTimes.Format(Config.XML_DTS);
            var nowTime = DateTimes.GetTime(pubDate);
            var fromDate = DateTimes.GmtFormat(Config.XML_DTS, nowTime - 6*60*60);
            var dsItems = doItem.EnumItemsFromSource(fromDate, source, filter, count);
            var current = 0;
            var itemsContent = "";
            for (int n = 0; n < dsItems.GetSize(); n++) {
                var oItem = dsItems.GetRow(n);
                var date = STR(oItem["d_Date"]);
                if (DateTimes.GetTime(date) > nowTime)
                    continue;

                if (current == 0)
                    pubDate = DateTimes.Format(Config.XML_DTS, DateTimes.GetTime(date));

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
                    link = CAT(
                        this.context.Site, Config.TOP_DIR,
                        (this.context.FineUrls ? "item/" : CAT(Config.INDEX_PAGE, "?p=view_item&amp;id=")),
                        oItem[idField],
                        (BLANK(url) ? null : CAT((this.context.FineUrls ? "/" : "&amp;title="), url))
                    );
                }

                Object[] args = ARR(7);
                args[0] = link;
                args[1] = itemTitle;
                args[2] = CAT(this.context.Site, Config.TOP_DIR, Config.ACTION_PAGE, "?p=do_redirect_source&amp;source=", sourceName);
                args[3] = sourceName;
                args[4] = DateTimes.Format(Config.XML_DTS, DateTimes.GetTime(date));
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

                var xmlTemplate = Strings.Concat(
                    "<item>\r\n",
                    "<title><![CDATA[{1}]]></title>\r\n",
                    "<link>{0}</link>\r\n",
                    "<pubDate>{4}</pubDate>\r\n",
                    BLANK(args[5]) ? null : "<description><![CDATA[{5}]]></description>\r\n",
                    BLANK(args[6]) ? null : "<category><![CDATA[{6}]]></category>\r\n",
                    "<guid>{0}</guid>\r\n",
                    "</item>\r\n"
                );
                itemsContent += (Util.FormatString(xmlTemplate, args));
                current++;
            }

            var rssTitle = CAT(
                "Items for ", (BLANK(source) ? "ALL sources" : CAT("'", source, "'")),
                (BLANK(filterName) ? null : CAT(" and filtered by '", filterName, "'"))
            );
            var xmlContent = Strings.Concat(
                "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n",
                "<rss version=\"2.0\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\">\r\n",
                "<channel>\r\n",
                //"<title>" . Config.SITE_NAME . "</title>\r\n",
                "<title>", rssTitle, "</title>\r\n",
                "<link>", this.context.Site, Config.TOP_DIR, "</link>\r\n",
                "<description>", rssTitle, "</description>\r\n",
                (this.context.Lang == "ru" ? "<language>ru-RU</language>\r\n" : "<language>en-US</language>\r\n"),
                "<pubDate>", pubDate, "</pubDate>\r\n",
                "<lastBuildDate>", pubDate, "</lastBuildDate>\r\n",
                "<generator>", Config.SITE_NAME, "</generator>\r\n"
            );

            xmlContent += (itemsContent);

            xmlContent += (CAT(
                "</channel>\r\n",
                "</rss>\r\n"));

            // Save content to cache (if applicable)
            if (Config.CACHE_RSS && !countSet)
            {
                Helper.TestFileFolder(cachedFile);
                //Helper.WriteText(cachedFile, Strings.Concat("\xEF\xBB\xBF", xmlContent));
                Helper.WriteText(cachedFile, xmlContent);
            }

            Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
            Response.Write(xmlContent);

            if (DBConfig.Connection != null) {
                DBConfig.Connection.Close();
                DBConfig.Connection = null;
            }
        }
    }
}