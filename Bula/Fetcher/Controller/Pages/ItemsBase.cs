// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
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
                    var Prepare = new Hashtable();
                    Prepare["[#ErrMessage]"] = "Incorrect list number!";
                    this.Write("Bula/Fetcher/View/error.html", Prepare);
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
            var err_message = "";
            if (Request.Contains("source")) {
                var source = Request.Get("source");
                if (BLANK(source))
                    err_message += ("Empty source name!<br/>");
                else if (!Request.IsDomainName("source"))
                    err_message += ("Incorrect source name!<br/>");
            }
            if (err_message.Length == 0)
                return true;

            var Prepare = new Hashtable();
            Prepare["[#ErrMessage]"] = err_message;
            this.Write("Bula/Fetcher/View/error.html", Prepare);
            return false;
        }

        /// <summary>
        /// Fill Row from Item.
        /// </summary>
        /// <param name="oItem">Original Item.</param>
        /// <param name="id_field">Name of ID field.</param>
        /// <param name="count">The number of inserted Row in HTML table.</param>
        /// <returns>Resulting Row.</returns>
        protected Hashtable FillItemRow(Hashtable oItem, String id_field, int count) {
            var Row = new Hashtable();
            var item_id = INT(oItem[id_field]);
            var url_title = STR(oItem["s_Url"]);
            var item_href = this.context.ImmediateRedirect ? GetRedirectItemLink(item_id, url_title) :
                    GetViewItemLink(item_id, url_title);
            Row["[#Link]"] = item_href;
            if ((count % 2) == 0)
                Row["[#Shade]"] = "1";

            if (Config.SHOW_FROM)
                Row["[#Show_From]"] = 1;
            Row["[#Source]"] = STR(oItem["s_SourceName"]);
            Row["[#Title]"] = Util.Show(STR(oItem["s_Title"]));

            if (this.context.Contains("Name_Category") && oItem.ContainsKey("s_Category") && STR(oItem["s_Category"]) != "")
                Row["[#Category]"] = STR(oItem["s_Category"]);

            if (this.context.Contains("Name_Creator") && oItem.ContainsKey("s_Creator") && STR(oItem["s_Creator"]) != "") {
                var s_Creator = STR(oItem["s_Creator"]);
                if (s_Creator != null) {
                    if (s_Creator.IndexOf("(") != -1)
                        s_Creator = s_Creator.Replace("(", "<br/>(");
                }
                else
                    s_Creator = (String)" "; //TODO -- "" doesn't works somehow, need to investigate
                Row["[#Creator]"] = s_Creator;
            }
            if (this.context.Contains("Name_Custom1") && oItem.Contains("s_Custom1") && STR(oItem["s_Custom1"]) != "")
                Row["[#Custom1]"] = oItem["s_Custom1"];
            if (this.context.Contains("Name_Custom2") && oItem.Contains("s_Custom2") && STR(oItem["s_Custom2"]) != "")
                Row["[#Custom2]"] = oItem["s_Custom2"];

            var d_Date = Util.ShowTime(STR(oItem["d_Date"]));
            if (this.context.IsMobile)
                d_Date = Strings.Replace("-", " ", d_Date);
            else
                d_Date = Strings.ReplaceFirst(" ", "<br/>", d_Date);
            Row["[#Date]"] = d_Date;
            return Row;
        }

        /// <summary>
        /// Get link for redirecting to external item.
        /// </summary>
        /// <param name="item_id">Item ID.</param>
        /// <returns>Resulting external link.</returns>
        public String GetRedirectItemLink(int item_id) {
            return GetRedirectItemLink(item_id, null);
        }

        /// <summary>
        /// Get link for redirecting to external item.
        /// </summary>
        /// <param name="item_id">Item ID.</param>
        /// <param name="url_title">Normalized title (to include in the link).</param>
        /// <returns>Resulting external link.</returns>
        public String GetRedirectItemLink(int item_id, String url_title) {
            return CAT(
                Config.TOP_DIR,
                (this.context.FineUrls ? "redirect/item/" : CAT(Config.ACTION_PAGE, "?p=do_redirect_item&id=")), item_id,
                (url_title != null ? CAT(this.context.FineUrls ? "/" : "&title=", url_title) : null)
            );
        }

        /// <summary>
        /// Get link for redirecting to the item (internally).
        /// </summary>
        /// <param name="item_id">Item ID.</param>
        /// <returns>Resulting internal link.</returns>
        public String GetViewItemLink(int item_id) {
            return GetViewItemLink(item_id, null);
        }

        /// <summary>
        /// Get link for redirecting to the item (internally).
        /// </summary>
        /// <param name="item_id">Item ID.</param>
        /// <param name="url_title">Normalized title (to include in the link).</param>
        /// <returns>Resulting internal link.</returns>
        public String GetViewItemLink(int item_id, String url_title) {
            return CAT(
                Config.TOP_DIR,
                (this.context.FineUrls ? "item/" : CAT(Config.INDEX_PAGE, "?p=view_item&id=")), item_id,
                (url_title != null ? CAT(this.context.FineUrls ? "/" : "&title=", url_title) : null)
            );
        }

        /// <summary>
        /// Get internal link to the page.
        /// </summary>
        /// <param name="list_no">Page no.</param>
        /// <returns>Resulting internal link to the page.</returns>
        protected String GetPageLink(int list_no) {
            var href = CAT(
                Config.TOP_DIR,
                (this.context.FineUrls ?
                    "items" : CAT(Config.INDEX_PAGE, "?p=items")),
                (BLANK(Request.Get("source")) ? null :
                    CAT((this.context.FineUrls ? "/source/" : "&amp;source="), Request.Get("source"))),
                (!this.context.Contains("filter") || BLANK(this.context["filter"]) ? null :
                    CAT((this.context.FineUrls ? "/filter/" : "&amp;filter="), this.context["filter"])),
                (list_no == 1 ? null :
                    CAT((this.context.FineUrls ? "/list/" : "&list="), list_no))
            );
            return href;
        }

        //abstract void Execute();
    }
}