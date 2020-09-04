// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;
    using System.Collections;

    /// <summary>
    /// Logic for generating Menu block.
    /// </summary>
    public class Menu : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public Menu(Context context) : base(context) { }

        /// Execute main logic for Menu block 
        public override void Execute() {
            var public_pages = new ArrayList();

            public_pages.Add("Home");
            public_pages.Add("home");
            if (this.context.IsMobile) {
                public_pages.Add(Config.NAME_ITEMS); public_pages.Add("items");
                if (Config.SHOW_BOTTOM && this.context.Contains("Name_Categories")) {
                    public_pages.Add(CAT("By ", this.context["Name_Categories"]));
                    public_pages.Add("#items_by_skills");
                    //public_pages.Add("RSS Feeds");
                    //public_pages.Add("#read_rss_feeds");
                }
                public_pages.Add("Sources");
                public_pages.Add("sources");
            }
            else {
                public_pages.Add(CAT("Browse ", Config.NAME_ITEMS));
                public_pages.Add("items");
                if (Config.SHOW_BOTTOM && this.context.Contains("Name_Categories")) {
                    public_pages.Add(CAT(Config.NAME_ITEMS, " by ", this.context["Name_Categories"]));
                    public_pages.Add("#items_by_skills");

                    public_pages.Add("Read RSS Feeds");
                    public_pages.Add("#read_rss_feeds");
                }
                public_pages.Add("Sources");
                public_pages.Add("sources");
            }

            var MenuItems = new ArrayList();
            for (int n = 0; n < public_pages.Count; n += 2) {
                var Row = new Hashtable();
                var title = STR(public_pages[n+0]);
                var page = STR(public_pages[n+1]);
                var href = (String)null;
                if (page.Equals("home"))
                    href = Config.TOP_DIR;
                else {
                    if (EQ(page.Substring(0, 1), "#"))
                        href = page;
                    else {
                        href = CAT(Config.TOP_DIR, Config.INDEX_PAGE, "?p=", page);
                        if (this.context.FineUrls)
                            href = CAT(Config.TOP_DIR, page);
                    }
                }
                Row["[#Link]"] = href;
                Row["[#LinkText]"] = title;
                Row["[#Prefix]"] = n != 0 ? " &bull; " : " ";
                MenuItems.Add(Row);
            }

            var Prepare = new Hashtable();
            Prepare["[#MenuItems]"] = MenuItems;
            this.Write("Bula/Fetcher/View/menu.html", Prepare);
        }
    }

}