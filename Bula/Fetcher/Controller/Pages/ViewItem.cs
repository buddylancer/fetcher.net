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
            var Prepare = new Hashtable();
            if (!Request.Contains("id")) {
                Prepare["[#ErrMessage]"] = "Item ID is required!";
                this.Write("Bula/Fetcher/View/error.html", Prepare);
                return null;
            }
            var id = Request.Get("id");
            if (!Request.IsInteger(id)) {
                Prepare["[#ErrMessage]"] = "Item ID must be positive integer!";
                this.Write("Bula/Fetcher/View/error.html", Prepare);
                return null;
            }

            var Pars = new Hashtable();
            Pars["id"] = id;
            return Pars;
        }

        /// Execute main logic for View Item block. 
        public override void Execute() {
            var Pars = Check();
            if (Pars == null)
                return;

            var id = (String)Pars["id"];

            var Prepare = new Hashtable();

            var doItem = new DOItem();
            var dsItems = doItem.GetById(INT(id));
            if (dsItems == null || dsItems.GetSize() == 0) {
                Prepare["[#ErrMessage]"] = "Wrong item ID!";
                this.Write("Bula/Fetcher/View/error.html", Prepare);
                return;
            }

            var oItem = dsItems.GetRow(0);
            var title = STR(oItem["s_Title"]);
            var source_name = STR(oItem["s_SourceName"]);

            this.context["Page_Title"] = title;
            var left_width = "25%";
            if (this.context.IsMobile)
                left_width = "20%";

            var id_field = doItem.GetIdField();
            var redirect_item = CAT(Config.TOP_DIR,
                (this.context.FineUrls ? "redirect/item/" : CAT(Config.ACTION_PAGE, "?p=do_redirect_item&id=")),
                oItem[id_field]);
            Prepare["[#RedirectLink]"] = redirect_item;
            Prepare["[#LeftWidth]"] = left_width;
            Prepare["[#Title]"] = Util.Show(title);
            Prepare["[#InputTitle]"] = Util.Safe(title);

            var redirect_source = CAT(
                Config.TOP_DIR,
                (this.context.FineUrls ? "redirect/source/" : CAT(Config.ACTION_PAGE, "?p=do_redirect_source&source=")),
                source_name
            );
            Prepare["[#RedirectSource]"] = redirect_source;
            Prepare["[#SourceName]"] = source_name;
            Prepare["[#Date]"] = Util.ShowTime(STR(oItem["d_Date"]));
            Prepare["[#Creator]"] = STR(oItem["s_Creator"]);
            Prepare["[#Description]"] = oItem.ContainsKey("t_Description") ? Util.Show(STR(oItem["t_Description"])) : "";
            Prepare["[#ItemID]"] = oItem[id_field];
            if (this.context.Contains("Name_Category")) Prepare["[#Category]"] = STR(oItem["s_Category"]);
            if (this.context.Contains("Name_Custom1")) Prepare["[#Custom1]"] = oItem["s_Custom1"];
            if (this.context.Contains("Name_Custom2")) Prepare["[#Custom2]"] = oItem["s_Custom2"];

            if (this.context.Lang == "ru" && !this.context.IsMobile)
                Prepare["[#Share]"] = 1;

            var engine = this.context.GetEngine();

            if (Config.CACHE_PAGES)
                Prepare["[#Home]"] = Util.ShowFromCache(engine, this.context.CacheFolder, "home", "Home", "p=home&from_view_item=1");
            else
                Prepare["[#Home]"] = engine.IncludeTemplate("Bula/Fetcher/Controller/Pages/Home");

            this.Write("Bula/Fetcher/View/Pages/view_item.html", Prepare);
        }
    }
}