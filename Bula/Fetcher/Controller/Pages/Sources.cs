// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Pages {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using Bula.Model;
    using Bula.Fetcher.Model;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Controller for Sources block.
    /// </summary>
    public class Sources : ItemsBase {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public Sources(Context context) : base(context) { }

        /// <summary>
        /// Fast check of input query parameters.
        /// </summary>
        /// <returns>Parsed parameters (or null in case of any error).</returns>
        public Hashtable Check() {
            return new Hashtable();
        }

        /// Execute main logic for Source block. 
        public override void Execute() {
            var prepare = new Hashtable();

            var doSource = new DOSource();
            var doItem = new DOItem();

            var dsSources = doSource.EnumSources();
            var count = 1;
            var sources = new ArrayList();
            for (int ns = 0; ns < dsSources.GetSize(); ns++) {
                var oSource = dsSources.GetRow(ns);
                var sourceName = STR(oSource["s_SourceName"]);

                var sourceRow = new Hashtable();
                sourceRow["[#SourceName]"] = sourceName;
                //sourceRow["[#RedirectSource]"] = Config.TOP_DIR .
                //    (Config.FINE_URLS ? "redirect/source/" : "action.php?p=do_redirect_source&source=") .
                //        oSource["s_SourceName"];
                sourceRow["[#RedirectSource]"] = this.GetLink(Config.INDEX_PAGE, "?p=items&source=", "items/source/", sourceName);

                var dsItems = doItem.EnumItemsFromSource(null, sourceName, null, 3);
                var items = new ArrayList();
                var itemCount = 0;
                for (int ni = 0; ni < dsItems.GetSize(); ni++) {
                    var oItem = dsItems.GetRow(ni);
                    items.Add(FillItemRow(oItem, doItem.GetIdField(), itemCount));
                    itemCount++;
                }
                sourceRow["[#Items]"] = items;

                sources.Add(sourceRow);
                count++;
            }
            prepare["[#Sources]"] = sources;

            this.Write("Pages/sources", prepare);
        }
    }
}