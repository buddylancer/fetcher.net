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
    /// Controller for Home block.
    /// </summary>
    public class Home : ItemsBase {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public Home(Context context) : base(context) { }

        /// <summary>
        /// Fast check of input query parameters.
        /// </summary>
        /// <returns>Parsed parameters (or null in case of any error).</returns>
        public Hashtable Check() {
            return new Hashtable();
        }

        /// Execute main logic for Home block. 
        public override void Execute() {
            var Pars = this.Check();
            if (Pars == null)
                return;

            var Prepare = new Hashtable();

            var doItem = new DOItem();

            var all_items_href =
                CAT(Config.TOP_DIR, (this.context.FineUrls ? null : CAT(Config.INDEX_PAGE, "?p=")), "items");
            Prepare["[#BrowseItemsLink]"] = all_items_href;

            var source = (String)null;
            var search = (String)null;
            var max_rows = Config.DB_HOME_ROWS;
            var dsItems = doItem.EnumItems(source, search, 1, max_rows);
            var row_count = 1;
            var Items = new ArrayList();
            for (int n = 0; n < dsItems.GetSize(); n++) {
                var oItem = dsItems.GetRow(n);
                var Row = FillItemRow(oItem, doItem.GetIdField(), row_count);
                Items.Add(Row);
                row_count++;
            }
            Prepare["[#Items]"] = Items;

            this.Write("Bula/Fetcher/View/Pages/home.html", Prepare);
        }
    }
}