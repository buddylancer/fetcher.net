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

    using Bula.Fetcher.Controller;
    using Bula.Fetcher.Model;

    /// <summary>
    /// Controller for Items block.
    /// </summary>
    public class Items : ItemsBase {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public Items(Context context) : base(context) { }

        /// <summary>
        /// Fast check of input query parameters.
        /// </summary>
        /// <returns>Parsed parameters (or null in case of any error).</returns>
        public THashtable Check() {
            var errorMessage = "";

            var list = this.context.Request["list"];
            if (!NUL(list)) {
                if (BLANK(list))
                    errorMessage += "Empty list number!";
                else if (!TRequest.IsInteger(list))
                    errorMessage += "Incorrect list number!";
            }

            var sourceName = this.context.Request["source"];
            if (!NUL(sourceName)) {
                if (BLANK(sourceName)) {
                    if (errorMessage.Length > 0)
                        errorMessage += "<br/>";
                    errorMessage += "Empty source name!";
                }
                else if (!TRequest.IsDomainName(sourceName)) {
                    if (errorMessage.Length > 0)
                        errorMessage += "<br/>";
                    errorMessage += "Incorrect source name!";
                }
            }

            var filterName = this.context.Request["filter"];
            if (!NUL(filterName)) {
                if (BLANK(filterName)) {
                    if (errorMessage.Length > 0)
                        errorMessage += "<br/>";
                    errorMessage += "Empty filter name!";
                }
                else if (!TRequest.IsName(filterName)) {
                    if (errorMessage.Length > 0)
                        errorMessage += "<br/>";
                    errorMessage += "Incorrect filter name!";
                }
            }

            if (errorMessage.Length > 0) {
                var prepare = new THashtable();
                prepare["[#ErrMessage]"] = errorMessage;
                this.Write("error", prepare);
                return null;
            }

            var pars = new THashtable();
            if (!NUL(list))
                pars["list"] = list;
            if (!NUL(sourceName))
                pars["source_name"] = sourceName;
            if (!NUL(filterName))
                pars["filter_name"] = filterName;
            return pars;
        }

        /// Execute main logic for Items block. 
        public override void Execute() {
            var pars = this.Check();
            if (pars == null)
                return;

            var list = (String)pars["list"];
            var listNumber = list == null ? 1 : INT(list);
            var sourceName = (String)pars["source_name"];
            var filterName = (String)pars["filter_name"];

            var errorMessage = "";
            var filter = (String)null;
            var category = (String)null;

            if (!NUL(filterName)) {
                var doCategory = new DOCategory();
                THashtable[] oCategory =
                    {new THashtable()};
                if (!doCategory.CheckFilterName(filterName, oCategory))
                    errorMessage += "Non-existing filter name!";
                else  {
                    category = STR(oCategory[0]["s_Name"]);
                    filter = STR(oCategory[0]["s_Filter"]);
                }
            }

            var sourceId = -1;
            if (!NUL(sourceName)) {
                var doSource = new DOSource();
                THashtable[] oSource =
                    {new THashtable()};
                if (!doSource.CheckSourceName(sourceName, oSource)) {
                    if (errorMessage.Length > 0)
                        errorMessage += "<br/>";
                    errorMessage += "Non-existing source name!";
                }
                else
                    sourceId = INT(oSource[0]["i_SourceId"]);
            }

            var engine = this.context.GetEngine();

            var prepare = new THashtable();
            if (errorMessage.Length > 0) {
                prepare["[#ErrMessage]"] = errorMessage;
                this.Write("error", prepare);
                return;
            }

            if (Config.SHOW_IMAGES)
                prepare["[#Show_Images]"] = 1;
            prepare["[#ColSpan]"] = Config.SHOW_IMAGES ? 4 : 3;

            // Uncomment to enable filtering by source and/or category
            prepare["[#FilterItems]"] = engine.IncludeTemplate("Pages/FilterItems");

            var s_Title = CAT(
                "Browse ",
                Config.NAME_ITEMS,
                (this.context.IsMobile ? "<br/>" : null),
                (!BLANK(sourceName) ? CAT(" ... from '", sourceName, "'") : null),
                (!BLANK(filter) ? CAT(" ... for '", category, "'") : null)
            );

            prepare["[#Title]"] = s_Title;

            var maxRows = Config.DB_ITEMS_ROWS;

            var doItem = new DOItem();
            //String realFilter = DOItem.BuildSqlByFilter(filter);
            var realFilter = DOItem.BuildSqlByCategory(category);
            var dsItems = doItem.EnumItems(sourceName, realFilter, listNumber, maxRows);

            var listTotal = dsItems.GetTotalPages();
            if (listNumber > listTotal) {
                if (listTotal > 0) {
                    prepare["[#ErrMessage]"] = "List number is too large!";
                    this.Write("error", prepare);
                    return;
                }
                else {
                    prepare["[#ErrMessage]"] = "Empty list!";
                    this.Write("error", prepare);
                    return;
                }
            }
            if (listTotal > 1) {
                prepare["[#List_Total]"] = listTotal;
                prepare["[#List]"] = listNumber;
            }

            var count = 1;
            var rows = new TArrayList();
            for (int n = 0; n < dsItems.GetSize(); n++) {
                var oItem = dsItems.GetRow(n);
                var row = FillItemRow(oItem, doItem.GetIdField(), count);
                count++;
                rows.Add(row);
            }
            prepare["[#Rows]"] = rows;

            if (listTotal > 1) {
                var chunk = 2;
                var before = false;
                var after = false;

                var pages = new TArrayList();
                for (int n = 1; n <= listTotal; n++) {
                    var page = new THashtable();
                    if (n < listNumber - chunk) {
                        if (!before) {
                            before = true;
                            page["[#Text]"] = "1";
                            page["[#Link]"] = GetPageLink(1);
                            pages.Add(page);
                            page = new THashtable();
                            page["[#Text]"] = " ... ";
                            //row.Remove("[#Link]");
                            pages.Add(page);
                        }
                        continue;
                    }
                    if (n > listNumber + chunk) {
                        if (!after) {
                            after = true;
                            page["[#Text]"] = " ... ";
                            pages.Add(page);
                            page = new THashtable();
                            page["[#Text]"] = listTotal;
                            page["[#Link]"] = GetPageLink(listTotal);
                            pages.Add(page);
                        }
                        continue;
                    }
                    if (listNumber == n) {
                        page["[#Text]"] = CAT("=", n, "=");
                        pages.Add(page);
                    }
                    else {
                        if (n == 1) {
                            page["[#Link]"] = GetPageLink(1);
                            page["[#Text]"] = 1;
                        }
                        else  {
                            page["[#Link]"] = GetPageLink(n);
                            page["[#Text]"] = n;
                        }
                        pages.Add(page);
                    }
                }
                prepare["[#Pages]"] = pages;
            }

            this.Write("Pages/items", prepare);
        }
    }
}