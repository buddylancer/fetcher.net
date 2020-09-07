// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using Bula.Model;
    using Bula.Fetcher.Model;

    /// <summary>
    /// Logic for generating Bottom block.
    /// </summary>
    public class Bottom : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public Bottom(Context context) : base(context) { }

        /// Execute main logic for Bottom block 
        public override void Execute() {
            var prepare = new Hashtable();

            var filterLink = CAT(Config.TOP_DIR,
                (this.context.FineUrls ? "items/filter/" : CAT(Config.INDEX_PAGE, "?p=items&filter=")));

            var doCategory = new DOCategory();
            var dsCategory = doCategory.EnumAll("_this.i_Counter <> 0");
            var size = dsCategory.GetSize();
            int size3 = size % 3;
            int n1 = INT(size / 3) + (size3 == 0 ? 0 : 1);
            int n2 = n1 * 2;
            Object[] nn = ARR(0, n1, n2, size);
            var filterBlocks = new ArrayList();
            for (int td = 0; td < 3; td++) {
                var filterBlock = new Hashtable();
                var rows = new ArrayList();
                for (int n = INT(nn[td]); n < INT(nn[td+1]); n++) {
                    var oCategory = dsCategory.GetRow(n);
                    var counter = INT(oCategory["i_Counter"]);
                    if (INT(counter) == 0)
                        continue;
                    var key = STR(oCategory["s_CatId"]);
                    var name = STR(oCategory["s_Name"]);
                    var row = new Hashtable();
                    var href = CAT(filterLink, key);
                    row["[#Link]"] = href;
                    row["[#LinkText]"] = name;
                    //if (counter > 0)
                        row["[#Counter]"] = counter;
                    rows.Add(row);
                }
                filterBlock["[#Rows]"] = rows;
                filterBlocks.Add(filterBlock);
            }
            prepare["[#FilterBlocks]"] = filterBlocks;

            if (!this.context.IsMobile) {
                filterLink = CAT(Config.TOP_DIR,
                    (this.context.FineUrls ? "rss/" : CAT(Config.RSS_PAGE, "?filter=")));
                dsCategory = doCategory.EnumAll();
                size = dsCategory.GetSize(); //50
                size3 = size % 3; //2
                n1 = INT(size / 3) + (size3 == 0 ? 0 : 1); //17.3
                n2 = n1 * 2; //34.6
                nn = ARR(0, n1, n2, size);
                var rssBlocks = new ArrayList();
                for (int td = 0; td < 3; td++) {
                    var rssBlock = new Hashtable();
                    var rows = new ArrayList();
                    for (int n = INT(nn[td]); n < INT(nn[td+1]); n++) {
                        var oCategory = dsCategory.GetRow(n);
                        var key = STR(oCategory["s_CatId"]);
                        var name = STR(oCategory["s_Name"]);
                        //counter = INT(oCategory["i_Counter"]);
                        var row = new Hashtable();
                        var href = CAT(filterLink, key, (this.context.FineUrls ? ".xml" : null));
                        row["[#Link]"] = href;
                        row["[#LinkText]"] = name;
                        rows.Add(row);
                    }
                    rssBlock["[#Rows]"] = rows;
                    rssBlocks.Add(rssBlock);
                }
                prepare["[#RssBlocks]"] = rssBlocks;
            }
            this.Write("Bula/Fetcher/View/bottom.html", prepare);
        }
    }
}