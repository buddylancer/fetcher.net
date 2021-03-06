// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Pages {
    using System;
    using System.Collections;

    using Bula.Fetcher;
    using Bula.Objects;
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
        public THashtable Check() {
            return new THashtable();
        }

        /// Execute main logic for Home block. 
        public override void Execute() {
            var pars = this.Check();
            if (pars == null)
                return;

            var prepare = new THashtable();

            var doItem = new DOItem();

            prepare["[#BrowseItemsLink]"] = this.GetLink(Config.INDEX_PAGE, "?p=", null, "items");
            if (Config.SHOW_IMAGES)
                prepare["[#Show_Images]"] = 1;

            var source = (String)null;
            var search = (String)null;
            var maxRows = Config.DB_HOME_ROWS;
            var dsItems = doItem.EnumItems(source, search, 1, maxRows);
            var rowCount = 1;
            var items = new TArrayList();
            for (int n = 0; n < dsItems.GetSize(); n++) {
                var oItem = dsItems.GetRow(n);
                var row = FillItemRow(oItem, doItem.GetIdField(), rowCount);
                items.Add(row);
                rowCount++;
            }
            prepare["[#Items]"] = items;

            this.Write("Pages/home", prepare);
        }
    }
}