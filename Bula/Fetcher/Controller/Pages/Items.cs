namespace Bula.Fetcher.Controller.Pages {
    using System;

    using Bula.Fetcher;
    using System.Collections;

    using Bula.Objects;
    using Bula.Model;

    using Bula.Fetcher.Controller;
    using Bula.Fetcher.Model;

    /**
     * Controller for Items block.
     */
    public class Items : ItemsBase {
        public static void Execute() {
            if (CheckList() == false)
                return;

            if (CheckSource() == false)
                return;

            String err_message = "";
            String filter_name = null;
            String filter = null;
            if (Request.Contains("filter")) {
                filter_name = Request.Get("filter");
                if (BLANK(filter_name))
                    err_message += ("Empty filter name!<br/>");
                else {
                    DOCategory doCategory = new DOCategory();
                    DataSet dsCategories = doCategory.GetCategoryById(filter_name);
                    if (dsCategories.GetSize() == 0)
                        err_message += ("Non-existing filter name!<br/>");
                    else {
                        Hashtable oCategory = dsCategories.GetRow(0);
                        filter = STR(oCategory["s_Filter"]);
                    }
                }
            }

            Hashtable Prepare = new Hashtable();
            String source_name = null;
            if (Request.Contains("source")) {
                // Check source exists
                source_name = Request.Get("source");
                DOSource doSource = new DOSource();
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

            String s_Title = CAT(
                "Browse ",
                Config.NAME_ITEMS,
                (Config.IsMobile ? "<br/>" : null),
                (!BLANK(source_name) ? CAT(" ... from '", source_name, "'") : null),
                (!BLANK(filter) ? CAT(" ... for '", filter_name, "'") : null)
            );

            Prepare["[#Title]"] = s_Title;

            int max_rows = Config.DB_ITEMS_ROWS;

            DOItem doItem = new DOItem();
            DataSet dsItems = doItem.EnumItems(source_name, filter, INT(Request.Get("list")), max_rows);

            int list_total = dsItems.GetTotalPages();
            if (INT(Request.Get("list")) > list_total) {
                Prepare["[#ErrMessage]"] = "List number is too large!";
                Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/error.html", Prepare));
                return;
            }
            if (list_total > 1) {
                Prepare["[#List_Total]"] = list_total;
                Prepare["[#List]"] = Request.Get("list");
            }

            int count = 1;
            ArrayList Rows = new ArrayList();
            for (int n = 0; n < dsItems.GetSize(); n++) {
                Hashtable oItem = dsItems.GetRow(n);
                Hashtable Row = FillItemRow(oItem, doItem.GetIdField(), count);
                count++;
                Rows.Add(Row);
            }
            Prepare["[#Rows]"] = Rows;

            if (list_total > 1) {
                int chunk = 2;
                Boolean before = false;
                Boolean after = false;

                ArrayList Pages = new ArrayList();
                for (int n = 1; n <= list_total; n++) {
                    Hashtable Page = new Hashtable();
                    if (n < INT(Request.Get("list")) - chunk) {
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
                    if (n > INT(Request.Get("list")) + chunk) {
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
                    if (INT(Request.Get("list")) == n) {
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