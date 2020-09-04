// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
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
            var Prepare = new Hashtable();

            var doSource = new DOSource();
            var doItem = new DOItem();

            var dsSources = doSource.EnumSources();
            var count = 1;
            var Sources = new ArrayList();
            for (int ns = 0; ns < dsSources.GetSize(); ns++) {
                var oSource = dsSources.GetRow(ns);
                var source_name = STR(oSource["s_SourceName"]);

                var SourceRow = new Hashtable();
                SourceRow["[#SourceName]"] = source_name;
                //SourceRow["[#RedirectSource]"] = Config.TOP_DIR .
                //    (Config.FINE_URLS ? "redirect/source/" : "action.php?p=do_redirect_source&source=") .
                //        oSource["s_SourceName"];
                SourceRow["[#RedirectSource]"] = CAT(Config.TOP_DIR,
                    (this.context.FineUrls ? "items/source/" : CAT(Config.INDEX_PAGE, "?p=items&source=")), source_name);

                var dsItems = doItem.EnumItemsFromSource(null, source_name, null, 3);
                var Items = new ArrayList();
                var item_count = 0;
                for (int ni = 0; ni < dsItems.GetSize(); ni++) {
                    var oItem = dsItems.GetRow(ni);
                    Items.Add(FillItemRow(oItem, doItem.GetIdField(), item_count));
                    item_count++;
                }
                SourceRow["[#Items]"] = Items;

                Sources.Add(SourceRow);
                count++;
            }
            Prepare["[#Sources]"] = Sources;

            this.Write("Bula/Fetcher/View/Pages/sources.html", Prepare);
        }
    }
}