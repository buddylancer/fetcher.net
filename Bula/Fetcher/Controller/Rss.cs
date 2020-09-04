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

            var error_message = "";

            // Check source
            var source = Request.Get("source");
            if (!NUL(source)) {
                if (BLANK(source))
                    error_message += ("Empty source!");
                else {
                    var doSource = new DOSource();
                    Hashtable[] oSource =
                        {new Hashtable()};
                    if (!doSource.CheckSourceName(source, oSource))
                        error_message += (CAT("Incorrect source '", source, "'!"));
                }
            }

            var any_filter = false;
            if (Request.Contains("code")) {
                if (EQ(Request.Get("code"), Config.SECURITY_CODE))
                    any_filter = true;
            }

            // Check filter
            var filter = (String)null;
            var filter_name = (String)null;
            var doCategory = new DOCategory();
            var dsCategories = doCategory.EnumCategories();
            if (dsCategories.GetSize() > 0) {
                filter_name = Request.Get("filter");
                if (!NUL(filter_name)) {
                    if (BLANK(filter_name)) {
                        if (error_message.Length > 0)
                            error_message += (" ");
                        error_message += ("Empty filter!");
                    }
                    else {
                        Hashtable[] oCategory =
                            {new Hashtable()};
                        if (doCategory.CheckFilterName(filter_name, oCategory))
                            filter = STR(oCategory[0]["s_Filter"]);
                        else {
                            if (any_filter)
                                filter = filter_name;
                            else
                                error_message += (CAT("Incorrect filter '", filter_name, "'!"));
                        }
                    }
                }
            }

            // Check that parameters contain only 'source' or/and 'filter'
            var keys = Request.GetKeys();
            while (keys.MoveNext()) {
                var key = (String)keys.Current;
                if (key != "source" && key != "filter" && key != "code" && key != "count") {
                    if (error_message.Length > 0)
                        error_message += (" ");
                    error_message += (CAT("Incorrect parameter '", key, "'!"));
                }
            }

            if (error_message.Length > 0) {
                Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
                Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n");
                Response.Write(CAT("<data>", error_message, "</data>"));
                return;
            }

            var full_title = false;
            if (Request.Contains("title") && STR(Request.Get("title")) == "full")
                full_title = true;

            var count = Config.MAX_RSS_ITEMS;
            var count_set = false;
            if (Request.Contains("count")) {
                if (INT(Request.Get("count")) > 0) {
                    count = INT(Request.Get("count"));
                    if (count < Config.MIN_RSS_ITEMS)
                        count = Config.MIN_RSS_ITEMS;
                    if (count > Config.MAX_RSS_ITEMS)
                        count = Config.MAX_RSS_ITEMS;
                    count_set = true;
                }
            }

            // Get content from cache (if enabled and cache data exists)
            var cached_file = "";
            if (Config.CACHE_RSS && !count_set) {
                cached_file = Strings.Concat(
                    this.context.RssFolder, "/rss",
                    (BLANK(source) ? null : CAT("-s=", source)),
                    (BLANK(filter_name) ? null : CAT("-f=", filter_name)),
                    (full_title ? "-full" : null), ".xml");
                if (Helper.FileExists(cached_file)) {
                    Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
                    var temp_content = Helper.ReadAllText(cached_file);
                    //Response.Write(temp_content.Substring(3)); //TODO -- BOM?
                    Response.Write(temp_content); //TODO -- BOM?
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
            var items_content = "";
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

                var source_name = STR(oItem["s_SourceName"]);
                var description = STR(oItem["t_Description"]);
                if (!BLANK(description)) {
                    description = Regex.Replace(description, "<br/>", " ", RegexOptions.IgnoreCase);
                    description = Regex.Replace(description, "&nbsp;", " ");
                    description = Regex.Replace(description, "[ \r\n\t]+", " ");
                    if (description.Length > 512) {
                        description = description.Substring(0, 511);
                        var last_space_index = description.LastIndexOf(" ");
                        description = Strings.Concat(description.Substring(0, last_space_index), " ...");
                    }
                    //Boolean utfIsValid = Mb_check_encoding(description, "UTF-8");
                    //if (utfIsValid == false)
                    //    description = ""; //TODO
                }
                var item_title = CAT(
                    (full_title == true && !BLANK(custom2) ? CAT(custom2, " | ") : null),
                    Strings.RemoveTags(Strings.StripSlashes(STR(oItem["s_Title"]))),
                    (full_title == true ? CAT(" [", source_name, "]") : null)
                );

                var link = (String)null;
                if (this.context.ImmediateRedirect)
                    link = STR(oItem["s_Link"]);
                else {
                    var url = STR(oItem["s_Url"]);
                    var id_field = doItem.GetIdField();
                    link = CAT(
                        this.context.Site, Config.TOP_DIR,
                        (this.context.FineUrls ? "item/" : CAT(Config.INDEX_PAGE, "?p=view_item&amp;id=")),
                        oItem[id_field],
                        (BLANK(url) ? null : CAT((this.context.FineUrls ? "/" : "&amp;title="), url))
                    );
                }

                Object[] args = ARR(7);
                args[0] = link;
                args[1] = item_title;
                args[2] = CAT(this.context.Site, Config.TOP_DIR, Config.ACTION_PAGE, "?p=do_redirect_source&amp;source=", source_name);
                args[3] = source_name;
                args[4] = DateTimes.Format(Config.XML_DTS, DateTimes.GetTime(date));
                var additional = CAT(
                    (BLANK(creator) ? null : CAT(this.context["Name_Creator"], ": ", creator, "<br/>")),
                    (BLANK(category) ? null : CAT(this.context["Name_Categories"], ": ", category, "<br/>")),
                    (BLANK(custom2) ? null : CAT(this.context["Name_Custom2"], ": ", custom2, "<br/>")),
                    (BLANK(custom1) ? null : CAT(this.context["Name_Custom1"], ": ", custom1, "<br/>"))
                );
                var extended_description = (String)null;
                if (!BLANK(description)) {
                    if (BLANK(additional))
                        extended_description = description;
                    else
                        extended_description = CAT(additional, "<br/>", description);
                }
                else if (!BLANK(additional))
                    extended_description = additional;
                args[5] = extended_description;
                args[6] = category;

                var xml_template = Strings.Concat(
                    "<item>\r\n",
                    "<title><![CDATA[{1}]]></title>\r\n",
                    "<link>{0}</link>\r\n",
                    "<pubDate>{4}</pubDate>\r\n",
                    BLANK(args[5]) ? null : "<description><![CDATA[{5}]]></description>\r\n",
                    BLANK(args[6]) ? null : "<category><![CDATA[{6}]]></category>\r\n",
                    "<guid>{0}</guid>\r\n",
                    "</item>\r\n"
                );
                items_content += (Util.FormatString(xml_template, args));
                current++;
            }

            var rss_title = CAT(
                "Items for ", (BLANK(source) ? "ALL sources" : CAT("'", source, "'")),
                (BLANK(filter_name) ? null : CAT(" and filtered by '", filter_name, "'"))
            );
            var xml_content = Strings.Concat(
                "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n",
                "<rss version=\"2.0\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\">\r\n",
                "<channel>\r\n",
                //"<title>" . Config.SITE_NAME . "</title>\r\n",
                "<title>", rss_title, "</title>\r\n",
                "<link>", this.context.Site, Config.TOP_DIR, "</link>\r\n",
                "<description>", rss_title, "</description>\r\n",
                (this.context.Lang == "ru" ? "<language>ru-RU</language>\r\n" : "<language>en-US</language>\r\n"),
                "<pubDate>", pubDate, "</pubDate>\r\n",
                "<lastBuildDate>", pubDate, "</lastBuildDate>\r\n",
                "<generator>", Config.SITE_NAME, "</generator>\r\n"
            );

            xml_content += (items_content);

            xml_content += (CAT(
                "</channel>\r\n",
                "</rss>\r\n"));

            // Save content to cache (if applicable)
            if (Config.CACHE_RSS && !count_set)
            {
                Util.TestFileFolder(cached_file);
                //Helper.WriteText(cached_file, Strings.Concat("\xEF\xBB\xBF", xml_content));
                Helper.WriteText(cached_file, xml_content);
            }

            Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
            Response.Write(xml_content);

            if (DBConfig.Connection != null) {
                DBConfig.Connection.Close();
                DBConfig.Connection = null;
            }
        }
    }
}