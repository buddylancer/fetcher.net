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
        /// Fast check of input query parameters.
        /// </summary>
        /// <returns>Parsed parameters (or null in case of any error).</returns>
        public static Hashtable Check() {
            var error_message = "";

            var list = Request.Get("list");
            if (!NUL(list)) {
                if (BLANK(list))
                    error_message += ("Empty list number!");
                else if (!Request.IsInteger(list))
                    error_message += ("Incorrect list number!");
            }

            var source_name = Request.Get("source");
            if (!NUL(source_name)) {
                if (BLANK(source_name)) {
                    if (error_message.Length > 0)
                        error_message += ("<br/>");
                    error_message += ("Empty source name!");
                }
                else if (!Request.IsDomainName(source_name)) {
                    if (error_message.Length > 0)
                        error_message += ("<br/>");
                    error_message += ("Incorrect source name!");
                }
            }

            var filter_name = Request.Get("filter");
            if (!NUL(filter_name)) {
                if (BLANK(filter_name)) {
                    if (error_message.Length > 0)
                        error_message += ("<br/>");
                    error_message += ("Empty filter name!");
                }
                else if (!Request.IsName(filter_name)) {
                    if (error_message.Length > 0)
                        error_message += ("<br/>");
                    error_message += ("Incorrect filter name!");
                }
            }

            if (error_message.Length > 0) {
                var Prepare = new Hashtable();
                Prepare["[#ErrMessage]"] = error_message;
                Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/error.html", Prepare));
                return null;
            }

            var Pars = new Hashtable();
            if (!NUL(list))
                Pars["list"] = list;
            if (!NUL(source_name))
                Pars["source_name"] = source_name;
            if (!NUL(filter_name))
                Pars["filter_name"] = filter_name;
            return Pars;
        }

        /// <summary>
        /// Execute main logic for Items block.
        /// </summary>
        public static void Execute() {
            var Pars = Check();
            if (Pars == null)
                return;

            var list = (String)Pars["list"];
            var list_number = list == null ? 1 : INT(list);
            var source_name = (String)Pars["source_name"];
            var filter_name = (String)Pars["filter_name"];

            var error_message = "";
            var filter = (String)null;

            if (!NUL(filter_name)) {
                var doCategory = new DOCategory();
                Hashtable[] oCategory =
                    {new Hashtable()};
                if (!doCategory.CheckFilterName(filter_name, oCategory))
                    error_message += ("Non-existing filter name!");
                else
                    filter = STR(oCategory[0]["s_Filter"]);
            }

            if (!NUL(source_name)) {
                var doSource = new DOSource();
                Hashtable[] oSource =
                    {new Hashtable()};
                if (!doSource.CheckSourceName(source_name, oSource)) {
                    if (error_message.Length > 0)
                        error_message += ("<br/>");
                    error_message += ("Non-existing source name!");
                }
            }

            var Prepare = new Hashtable();
            if (error_message.Length > 0) {
                Prepare["[#ErrMessage]"] = error_message;
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

            var doItem = new DOItem();
            var dsItems = doItem.EnumItems(source_name, filter, list_number, max_rows);

            var list_total = dsItems.GetTotalPages();
            if (list_number > list_total) {
                Prepare["[#ErrMessage]"] = "List number is too large!";
                Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/error.html", Prepare));
                return;
            }
            if (list_total > 1) {
                Prepare["[#List_Total]"] = list_total;
                Prepare["[#List]"] = list_number;
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
                    if (n < list_number - chunk) {
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
                    if (n > list_number + chunk) {
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
                    if (list_number == n) {
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