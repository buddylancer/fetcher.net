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
            var publicPages = new ArrayList();

            publicPages.Add("Home");
            publicPages.Add("home");
            if (this.context.IsMobile) {
                publicPages.Add(Config.NAME_ITEMS); publicPages.Add("items");
                if (Config.SHOW_BOTTOM && this.context.Contains("Name_Categories")) {
                    publicPages.Add(CAT("By ", this.context["Name_Categories"]));
                    publicPages.Add("#items_by_skills");
                    //publicPages.Add("RSS Feeds");
                    //publicPages.Add("#read_rss_feeds");
                }
                publicPages.Add("Sources");
                publicPages.Add("sources");
            }
            else {
                publicPages.Add(CAT("Browse ", Config.NAME_ITEMS));
                publicPages.Add("items");
                if (Config.SHOW_BOTTOM && this.context.Contains("Name_Categories")) {
                    publicPages.Add(CAT(Config.NAME_ITEMS, " by ", this.context["Name_Categories"]));
                    publicPages.Add("#items_by_skills");

                    publicPages.Add("Read RSS Feeds");
                    publicPages.Add("#read_rss_feeds");
                }
                publicPages.Add("Sources");
                publicPages.Add("sources");
            }

            var menuItems = new ArrayList();
            for (int n = 0; n < publicPages.Count; n += 2) {
                var row = new Hashtable();
                var title = STR(publicPages[n+0]);
                var page = STR(publicPages[n+1]);
                var href = (String)null;
                if (EQ(page, "home"))
                    href = Config.TOP_DIR;
                else {
                    if (EQ(page.Substring(0, 1), "#"))
                        href = page;
                    else {
                        href = this.GetLink(Config.INDEX_PAGE, "?p=", null, page);
                    }
                }
                row["[#Link]"] = href;
                row["[#LinkText]"] = title;
                row["[#Prefix]"] = n != 0 ? " &bull; " : " ";
                menuItems.Add(row);
            }

            var prepare = new Hashtable();
            prepare["[#MenuItems]"] = menuItems;
            this.Write("menu", prepare);
        }
    }

}