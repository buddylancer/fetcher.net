// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;
    using System.Collections;

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

        /// <summary>
        /// Write error message.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public override void WriteErrorMessage(String errorMessage) {
            this.context.Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
            this.context.Response.Write(CAT("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>", EOL));
            this.context.Response.Write(CAT("<data>", errorMessage, "</data>"));
        }

        /// <summary>
        /// Write starting block of RSS-feed.
        /// </summary>
        /// <param name="source">RSS-feed source name.</param>
        /// <param name="filterName">RSS-feed 'filtered by' value.</param>
        /// <param name="pubDate">Publication date.</param>
        /// <returns>Resulting XML-content of starting block.</returns>
        public override String WriteStart(String source, String filterName, String pubDate) {
            var rssTitle = CAT(
                "Items for ", (BLANK(source) ? "ALL sources" : CAT("'", source, "'")),
                (BLANK(filterName) ? null : CAT(" and filtered by '", filterName, "'"))
            );
            var xmlContent = Strings.Concat(
                "<rss version=\"2.0\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\">\r\n",
                "<channel>", EOL,
                //"<title>" . Config.SITE_NAME . "</title>", EOL,
                "<title>", rssTitle, "</title>", EOL,
                "<link>", this.context.Site, Config.TOP_DIR, "</link>", EOL,
                "<description>", rssTitle, "</description>", EOL,
                (this.context.Lang == "ru" ? "<language>ru-RU</language>\r\n" : "<language>en-US</language>"), EOL,
                "<pubDate>", pubDate, "</pubDate>", EOL,
                "<lastBuildDate>", pubDate, "</lastBuildDate>", EOL,
                "<generator>", Config.SITE_NAME, "</generator>", EOL
            );
            this.context.Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
            this.context.Response.Write(CAT("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>", EOL));
            this.context.Response.Write(xmlContent);
            return xmlContent;
        }

        /// <summary>
        /// Write ending block of RSS-feed.
        /// </summary>
        public override String WriteEnd() {
            var xmlContent = Strings.Concat(
                "</channel>", EOL,
                "</rss>", EOL);
            this.context.Response.Write(xmlContent);
            this.context.Response.End();
            return xmlContent;
        }

        /// <summary>
        /// Write an item of RSS-feed.
        /// </summary>
        /// <param name="args">Array of item parameters.</param>
        /// <returns>Resulting XML-content of an item.</returns>
        public override String WriteItem(Object[] args) {
            var xmlTemplate = Strings.Concat(
                "<item>", EOL,
                "<title><![CDATA[{1}]]></title>", EOL,
                "<link>{0}</link>", EOL,
                "<pubDate>{4}</pubDate>", EOL,
                BLANK(args[5]) ? null : CAT("<description><![CDATA[{5}]]></description>", EOL),
                BLANK(args[6]) ? null : CAT("<category><![CDATA[{6}]]></category>", EOL),
                "<guid>{0}</guid>", EOL,
                "</item>", EOL
            );
            var itemContent = Util.FormatString(xmlTemplate, args);
            this.context.Response.Write(itemContent);
            return itemContent;
        }
    }
}