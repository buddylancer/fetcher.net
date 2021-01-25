// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Pages {
    using System;

    using System.Collections;
    using System.Text.RegularExpressions;

    using Bula.Fetcher;
    using Bula.Objects;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Base controller for Items block.
    /// </summary>
    public abstract class ItemsBase : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public ItemsBase(Context context) : base(context) { }

        /// <summary>
        /// Check list from current query.
        /// </summary>
        /// <returns>True - checked OK, False - error.</returns>
        public Boolean CheckList() {
            if (Request.Contains("list")) {
                if (!Request.IsInteger(Request.Get("list"))) {
                    var prepare = new Hashtable();
                    prepare["[#ErrMessage]"] = "Incorrect list number!";
                    this.Write("Bula/Fetcher/View/error.html", prepare);
                    return false;
                }
            }
            else
                Request.Set("list", "1");
            return true;
        }

        /// <summary>
        /// Check source name from current query.
        /// </summary>
        /// <returns>True - source exists, False - error.</returns>
        public Boolean CheckSource() {
            var errMessage = "";
            if (Request.Contains("source")) {
                var source = Request.Get("source");
                if (BLANK(source))
                    errMessage += ("Empty source name!<br/>");
                else if (!Request.IsDomainName("source"))
                    errMessage += ("Incorrect source name!<br/>");
            }
            if (errMessage.Length == 0)
                return true;

            var prepare = new Hashtable();
            prepare["[#ErrMessage]"] = errMessage;
            this.Write("Bula/Fetcher/View/error.html", prepare);
            return false;
        }

        /// <summary>
        /// Fill Row from Item.
        /// </summary>
        /// <param name="oItem">Original Item.</param>
        /// <param name="idField">Name of ID field.</param>
        /// <param name="count">The number of inserted Row in HTML table.</param>
        /// <returns>Resulting Row.</returns>
        protected Hashtable FillItemRow(Hashtable oItem, String idField, int count) {
            var row = new Hashtable();
            var itemId = INT(oItem[idField]);
            var urlTitle = STR(oItem["s_Url"]);
            var itemHref = this.context.ImmediateRedirect ? GetRedirectItemLink(itemId, urlTitle) :
                    GetViewItemLink(itemId, urlTitle);
            row["[#Link]"] = itemHref;
            if ((count % 2) == 0)
                row["[#Shade]"] = "1";

            if (Config.SHOW_FROM)
                row["[#Show_From]"] = 1;
            row["[#Source]"] = STR(oItem["s_SourceName"]);
            row["[#Title]"] = Util.Show(STR(oItem["s_Title"]));

            if (this.context.Contains("Name_Category") && oItem.ContainsKey("s_Category") && STR(oItem["s_Category"]) != "")
                row["[#Category]"] = STR(oItem["s_Category"]);

            if (this.context.Contains("Name_Creator") && oItem.ContainsKey("s_Creator") && STR(oItem["s_Creator"]) != "") {
                var s_Creator = STR(oItem["s_Creator"]);
                if (s_Creator != null) {
                    if (s_Creator.IndexOf("(") != -1)
                        s_Creator = s_Creator.Replace("(", "<br/>(");
                }
                else
                    s_Creator = (String)" "; //TODO -- "" doesn't works somehow, need to investigate
                row["[#Creator]"] = s_Creator;
            }
            if (this.context.Contains("Name_Custom1") && oItem.Contains("s_Custom1") && STR(oItem["s_Custom1"]) != "")
                row["[#Custom1]"] = oItem["s_Custom1"];
            if (this.context.Contains("Name_Custom2") && oItem.Contains("s_Custom2") && STR(oItem["s_Custom2"]) != "")
                row["[#Custom2]"] = oItem["s_Custom2"];

            var d_Date = Util.ShowTime(STR(oItem["d_Date"]));
            if (this.context.IsMobile)
                d_Date = Strings.Replace("-", " ", d_Date);
            else
                d_Date = Strings.ReplaceFirst(" ", "<br/>", d_Date);
            row["[#Date]"] = d_Date;
            return row;
        }

        /// <summary>
        /// Get link for redirecting to external item.
        /// </summary>
        /// <param name="itemId">Item ID.</param>
        /// <returns>Resulting external link.</returns>
        public String GetRedirectItemLink(int itemId) {
            return GetRedirectItemLink(itemId, null);
        }

        /// <summary>
        /// Get link for redirecting to external item.
        /// </summary>
        /// <param name="itemId">Item ID.</param>
        /// <param name="urlTitle">Normalized title (to include in the link).</param>
        /// <returns>Resulting external link.</returns>
        public String GetRedirectItemLink(int itemId, String urlTitle) {
            return CAT(
                Config.TOP_DIR,
                (this.context.FineUrls ? "redirect/item/" : CAT(Config.ACTION_PAGE, "?p=do_redirect_item&id=")), itemId,
                (urlTitle != null ? CAT(this.context.FineUrls ? "/" : "&title=", urlTitle) : null)
            );
        }

        /// <summary>
        /// Get link for redirecting to the item (internally).
        /// </summary>
        /// <param name="itemId">Item ID.</param>
        /// <returns>Resulting internal link.</returns>
        public String GetViewItemLink(int itemId) {
            return GetViewItemLink(itemId, null);
        }

        /// <summary>
        /// Get link for redirecting to the item (internally).
        /// </summary>
        /// <param name="itemId">Item ID.</param>
        /// <param name="urlTitle">Normalized title (to include in the link).</param>
        /// <returns>Resulting internal link.</returns>
        public String GetViewItemLink(int itemId, String urlTitle) {
            return CAT(
                Config.TOP_DIR,
                (this.context.FineUrls ? "item/" : CAT(Config.INDEX_PAGE, "?p=view_item&id=")), itemId,
                (urlTitle != null ? CAT(this.context.FineUrls ? "/" : "&title=", urlTitle) : null)
            );
        }

        /// <summary>
        /// Get internal link to the page.
        /// </summary>
        /// <param name="listNo">Page no.</param>
        /// <returns>Resulting internal link to the page.</returns>
        protected String GetPageLink(int listNo) {
            var href = CAT(
                Config.TOP_DIR,
                (this.context.FineUrls ?
                    "items" : CAT(Config.INDEX_PAGE, "?p=items")),
                (BLANK(Request.Get("source")) ? null :
                    CAT((this.context.FineUrls ? "/source/" : "&amp;source="), Request.Get("source"))),
                (!this.context.Contains("filter") || BLANK(this.context["filter"]) ? null :
                    CAT((this.context.FineUrls ? "/filter/" : "&amp;filter="), this.context["filter"])),
                (listNo == 1 ? null :
                    CAT((this.context.FineUrls ? "/list/" : "&list="), listNo))
            );
            return href;
        }

        //abstract void Execute();
    }
}