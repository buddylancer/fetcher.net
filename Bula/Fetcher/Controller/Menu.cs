namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;
    using System.Collections;

    /**
     * Logic for generating Menu block.
     */
    public class Menu : Bula.Meta {
        public static void Execute() {
            ArrayList public_pages = new ArrayList();

            public_pages.Add("Home");
            public_pages.Add("home");
            if (Config.IsMobile) {
                public_pages.Add(Config.NAME_ITEMS); public_pages.Add("items");
                if (Config.SHOW_BOTTOM && Config.Contains("Name_Categories")) {
                    public_pages.Add(CAT("By ", Config.Get("Name_Categories")));
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
                if (Config.SHOW_BOTTOM && Config.Contains("Name_Categories")) {
                    public_pages.Add(CAT(Config.NAME_ITEMS, " by ", Config.Get("Name_Categories")));
                    public_pages.Add("#items_by_skills");

                    public_pages.Add("Read RSS Feeds");
                    public_pages.Add("#read_rss_feeds");
                }
                public_pages.Add("Sources");
                public_pages.Add("sources");
            }

            ArrayList MenuItems = new ArrayList();
            for (int n = 0; n < public_pages.Count; n += 2) {
                Hashtable Row = new Hashtable();
                String title = STR(public_pages[n+0]);
                String page = STR(public_pages[n+1]);
                String href = null;
                if (page.Equals("home"))
                    href = Config.TOP_DIR;
                else {
                    if (EQ(page.Substring(0, 1), "#"))
                        href = page;
                    else {
                        href = CAT(Config.TOP_DIR, Config.INDEX_PAGE, "?p=", page);
                        if (Config.FineUrls)
                            href = CAT(Config.TOP_DIR, page);
                    }
                }
                Row["[#Link]"] = href;
                Row["[#LinkText]"] = title;
                Row["[#Prefix]"] = n != 0 ? " &bull; " : " ";
                MenuItems.Add(Row);
            }

            Hashtable Prepare = new Hashtable();
            Prepare["[#MenuItems]"] = MenuItems;
            Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/menu.html", Prepare));
        }
    }

}