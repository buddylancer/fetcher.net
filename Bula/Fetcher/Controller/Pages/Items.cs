namespace Bula.Fetcher.Controller.Pages {
    using System;

    using Bula.Fetcher;
    using System.Collections;

    using Bula.Objects;
    using Bula.Model;

    using Bula.Fetcher.Controller;
    using Bula.Fetcher.Model;

    /// <summary>
    /// Controller for Items block.
    /// </summary>
    public class Items : ItemsBase {
        /// <summary>
        /// Execute main logic for Items block.
        /// </summary>
        public static void Execute() {
            if (CheckList() == false)
                return;

            if (CheckSource() == false)
                return;

            var err_message = "";
            var filter_name = (String)null;
            var filter = (String)null;
            if (Request.Contains("filter")) {
                filter_name = Request.Get("filter");
                if (BLANK(filter_name))
                    err_message += ("Empty filter name!<br/>");
                else {
                    var doCategory = new DOCategory();
                    Hashtable[] oCategory =
                        {new Hashtable()};
                    if (!doCategory.CheckFilterName(filter_name, oCategory))
                        err_message += ("Non-existing filter name!<br/>");
                    else
                        filter = STR(oCategory[0]["s_Filter"]);
                }
            }

            var Prepare = new Hashtable();
            var source_name = (String)null;
            if (Request.Contains("source")) {
                // Check source exists
                source_name = Request.Get("source");
                var doSource = new DOSource();
                Hashtable[] oSource =
                    {new Hashtable()};
                if (!doSource.CheckSourceName(source_name, oSource))
                    err_message += ("Non-existing source name!<br/>");
            }

            if (err_message.Length > 0) {
                Prepare["[#ErrMessage]"] = err_message;
                Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/error.html", Prepare));
                return;
            }

            // Uncomment to enable filtering by source and/or category
            Prepare["[#FilterItems]"] = Engine.IncludeTemplate("Bula/Fetcher/Controller/Pages/FilterItems");

            var s_Title = CAT(
                "Browse ",
                Config.NAME_ITEMS,
                (Config.IsMobile ? "<br/>" : null),
                (!BLANK(source_name) ? CAT(" ... from '", source_name, "'") : null),
                (!BLANK(filter) ? CAT(" ... for '", filter_name, "'") : null)
            );

            Prepare["[#Title]"] = s_Title;

            var max_rows = Config.DB_ITEMS_ROWS;
            var list = INT(Request.Get("list"));

            var doItem = new DOItem();
            var dsItems = doItem.EnumItems(source_name, filter, list, max_rows);

            var list_total = dsItems.GetTotalPages();
            if (list > list_total) {
                Prepare["[#ErrMessage]"] = "List number is too large!";
                Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/error.html", Prepare));
                return;
            }
            if (list_total > 1) {
                Prepare["[#List_Total]"] = list_total;
                Prepare["[#List]"] = Request.Get("list");
            }

            var count = 1;
            var Rows = new ArrayList();
            for (int n = 0; n < dsItems.GetSize(); n++) {
                var oItem = dsItems.GetRow(n);
                var Row = FillItemRow(oItem, doItem.GetIdField(), count);
                count++;
                Rows.Add(Row);
            }
            Prepare["[#Rows]"] = Rows;

            if (list_total > 1) {
                var chunk = 2;
                var before = false;
                var after = false;

                var Pages = new ArrayList();
                for (int n = 1; n <= list_total; n++) {
                    var Page = new Hashtable();
                    if (n < list - chunk) {
                        if (!before) {
                            before = true;
                            Page["[#Text]"] = "1";
                            Page["[#Link]"] = GetPageLink(1);
                            Pages.Add(Page);
                            Page = new Hashtable();
                            Page["[#Text]"] = " ... ";
                            //Row.Remove("[#Link]");
                            Pages.Add(Page);
                        }
                        continue;
                    }
                    if (n > list + chunk) {
                        if (!after) {
                            after = true;
                            Page["[#Text]"] = " ... ";
                            Pages.Add(Page);
                            Page = new Hashtable();
                            Page["[#Text]"] = list_total;
                            Page["[#Link]"] = GetPageLink(list_total);
                            Pages.Add(Page);
                        }
                        continue;
                    }
                    if (list == n) {
                        Page["[#Text]"] = CAT("=", n, "=");
                        Pages.Add(Page);
                    }
                    else {
                        if (n == 1) {
                            Page["[#Link]"] = GetPageLink(1);
                            Page["[#Text]"] = 1;
                        }
                        else  {
                            Page["[#Link]"] = GetPageLink(n);
                            Page["[#Text]"] = n;
                        }
                        Pages.Add(Page);
                    }
                }
                Prepare["[#Pages]"] = Pages;
            }
            Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/Pages/items.html", Prepare));
        }
    }
}