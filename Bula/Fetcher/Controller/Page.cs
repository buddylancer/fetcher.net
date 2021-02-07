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
    /// Basic logic for generating Page block.
    /// </summary>
    public abstract class Page : Bula.Meta {
        /// Current context 
        protected Context context = null;

        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public Page (Context context) {
            this.context = context;
            //echo "In Page constructor -- " . Print_r(context, true);
        }

        /// Execute main logic for page block 
        abstract public void Execute();

        /// <summary>
        /// Merge template with variables and write to engine.
        /// </summary>
        /// <param name="template">Template name.</param>
        /// <param name="prepare">Prepared variables.</param>
        public void Write(String template, Hashtable prepare) {
            var engine = this.context.GetEngine();
            engine.Write(engine.ShowTemplate(template, prepare));
        }

        public String GetLink(String page, String ordinaryUrl, String fineUrl, Object extraData = null) {
            if (!BLANK(this.context.Api))
                return this.GetAbsoluteLink(page, ordinaryUrl, fineUrl, extraData);
            else
                return this.GetRelativeLink(page, ordinaryUrl, fineUrl, extraData);
        }

        public String GetRelativeLink(String page, String ordinaryUrl, String fineUrl, Object extraData = null) {
            var link = CAT(
                Config.TOP_DIR,
                (this.context.FineUrls ? fineUrl : CAT(page, this.QuoteLink(ordinaryUrl))),
                extraData);
            return link;
        }

        public String GetAbsoluteLink(String page, String ordinaryUrl, String fineUrl, Object extraData = null) {
            return CAT(this.context.Site, this.GetRelativeLink(page, ordinaryUrl, fineUrl, extraData));
        }

        public String AppendLink(String link, String ordinaryUrl, String fineUrl, Object extraData = null) {
            return CAT(link, (this.context.FineUrls ? fineUrl : this.QuoteLink(ordinaryUrl)), extraData);
        }

        public String QuoteLink(String link) {
            return !BLANK(this.context.Api) && EQ(Config.API_FORMAT, "Xml") ? Util.Safe(link) : link;
        }
    }
}