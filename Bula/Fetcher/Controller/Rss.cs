// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;

    /// <summary>
    /// Main logic for generating RSS-feeds.
    /// </summary>
    public class Rss : RssBase {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public Rss(Context context) : base(context) { }

        public override void WriteErrorMessage(String errorMessage) {
            Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
            Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n");
            Response.Write(CAT("<data>", errorMessage, "</data>"));
        }

        public override String WriteStart(String source, String filterName, String pubDate) {
            var rssTitle = CAT(
                "Items for ", (BLANK(source) ? "ALL sources" : CAT("'", source, "'")),
                (BLANK(filterName) ? null : CAT(" and filtered by '", filterName, "'"))
            );
            var xmlContent = Strings.Concat(
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
            Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
            Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n");
            Response.Write(xmlContent);
            return xmlContent;
        }

        public override String WriteEnd() {
            var xmlContent = Strings.Concat(
                "</channel>\r\n",
                "</rss>\r\n");
            Response.Write(xmlContent);
            Response.End("");
            return xmlContent;
        }

        public override String WriteItem(Object[] args) {
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
            var itemContent = Util.FormatString(xmlTemplate, args);
            Response.Write(itemContent);
            return itemContent;
        }
    }
}