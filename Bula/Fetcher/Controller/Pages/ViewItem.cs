// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Pages {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;
    using System.Collections;
    using Bula.Model;
    using Bula.Fetcher.Model;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Controller for View Item block.
    /// </summary>
    public class ViewItem : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public ViewItem(Context context) : base(context) { }

        /// <summary>
        /// Fast check of input query parameters.
        /// </summary>
        /// <returns>Parsed parameters (or null in case of any error).</returns>
        public Hashtable Check() {
            var prepare = new Hashtable();
            if (!Request.Contains("id")) {
                prepare["[#ErrMessage]"] = "Item ID is required!";
                this.Write("Bula/Fetcher/View/error.html", prepare);
                return null;
            }
            var id = Request.Get("id");
            if (!Request.IsInteger(id)) {
                prepare["[#ErrMessage]"] = "Item ID must be positive integer!";
                this.Write("Bula/Fetcher/View/error.html", prepare);
                return null;
            }

            var pars = new Hashtable();
            pars["id"] = id;
            return pars;
        }

        /// Execute main logic for View Item block. 
        public override void Execute() {
            var pars = Check();
            if (pars == null)
                return;

            var id = (String)pars["id"];

            var prepare = new Hashtable();

            var doItem = new DOItem();
            var dsItems = doItem.GetById(INT(id));
            if (dsItems == null || dsItems.GetSize() == 0) {
                prepare["[#ErrMessage]"] = "Wrong item ID!";
                this.Write("Bula/Fetcher/View/error.html", prepare);
                return;
            }

            var oItem = dsItems.GetRow(0);
            var title = STR(oItem["s_Title"]);
            var sourceName = STR(oItem["s_SourceName"]);

            this.context["Page_Title"] = title;
            var leftWidth = "25%";
            if (this.context.IsMobile)
                leftWidth = "20%";

            var idField = doItem.GetIdField();
            var redirectItem = CAT(Config.TOP_DIR,
                (this.context.FineUrls ? "redirect/item/" : CAT(Config.ACTION_PAGE, "?p=do_redirect_item&id=")),
                oItem[idField]);
            prepare["[#RedirectLink]"] = redirectItem;
            prepare["[#LeftWidth]"] = leftWidth;
            prepare["[#Title]"] = Util.Show(title);
            prepare["[#InputTitle]"] = Util.Safe(title);

            var redirectSource = CAT(
                Config.TOP_DIR,
                (this.context.FineUrls ? "redirect/source/" : CAT(Config.ACTION_PAGE, "?p=do_redirect_source&source=")),
                sourceName
            );
            prepare["[#RedirectSource]"] = redirectSource;
            prepare["[#SourceName]"] = sourceName;
            prepare["[#Date]"] = Util.ShowTime(STR(oItem["d_Date"]));
            prepare["[#Creator]"] = STR(oItem["s_Creator"]);
            prepare["[#Description]"] = oItem.ContainsKey("t_Description") ? Util.Show(STR(oItem["t_Description"])) : "";
            prepare["[#ItemID]"] = oItem[idField];
            if (this.context.Contains("Name_Category")) prepare["[#Category]"] = STR(oItem["s_Category"]);
            if (this.context.Contains("Name_Custom1")) prepare["[#Custom1]"] = oItem["s_Custom1"];
            if (this.context.Contains("Name_Custom2")) prepare["[#Custom2]"] = oItem["s_Custom2"];

            if (this.context.Lang == "ru" && !this.context.IsMobile)
                prepare["[#Share]"] = 1;

            var engine = this.context.GetEngine();

            if (Config.CACHE_PAGES)
                prepare["[#Home]"] = Util.ShowFromCache(engine, this.context.CacheFolder, "home", "Home", "p=home&from_view_item=1");
            else
                prepare["[#Home]"] = engine.IncludeTemplate("Bula/Fetcher/Controller/Pages/Home");

            this.Write("Bula/Fetcher/View/Pages/view_item.html", prepare);
        }
    }
}