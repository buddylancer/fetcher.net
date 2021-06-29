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
        public THashtable Check() {
            var prepare = new THashtable();
            if (!this.context.Request.Contains("id")) {
                prepare["[#ErrMessage]"] = "Item ID is required!";
                this.Write("error", prepare);
                return null;
            }
            var id = this.context.Request["id"];
            if (!TRequest.IsInteger(id)) {
                prepare["[#ErrMessage]"] = "Item ID must be positive integer!";
                this.Write("error", prepare);
                return null;
            }

            var pars = new THashtable();
            pars["id"] = id;
            return pars;
        }

        /// Execute main logic for View Item block. 
        public override void Execute() {
            var pars = Check();
            if (pars == null)
                return;

            var id = (String)pars["id"];

            var prepare = new THashtable();

            var doItem = new DOItem();
            var dsItems = doItem.GetById(INT(id));
            if (dsItems == null || dsItems.GetSize() == 0) {
                prepare["[#ErrMessage]"] = "Wrong item ID!";
                this.Write("error", prepare);
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
            if (Config.SHOW_IMAGES)
                prepare["[#Show_Images]"] = 1;
            prepare["[#RedirectLink]"] = this.GetLink(Config.ACTION_PAGE, "?p=do_redirect_item&id=", "redirect/item/", oItem[idField]);
            prepare["[#LeftWidth]"] = leftWidth;
            prepare["[#Title]"] = Util.Show(title);
            prepare["[#InputTitle]"] = Util.Safe(title);
            prepare["[#RedirectSource]"] = this.GetLink(Config.ACTION_PAGE, "?p=do_redirect_source&source=", "redirect/source/", sourceName);
            prepare["[#SourceName]"] = sourceName;
            prepare["[#ExtImages]"] = Config.EXT_IMAGES;
            prepare["[#SourceLink]"] = this.GetLink(Config.INDEX_PAGE, "?p=items&source=", "items/source/", sourceName);
            prepare["[#Date]"] = Util.ShowTime(STR(oItem["d_Date"]));
            if (!NUL(oItem["s_Creator"]))
                prepare["[#Creator]"] = STR(oItem["s_Creator"]);
            if (oItem.ContainsKey("t_Description") && !BLANK(STR(oItem["t_Description"])))
                prepare["[#Description]"] = Util.Show(STR(oItem["t_Description"]));
            prepare["[#ItemID]"] = oItem[idField];
            if (this.context.Contains("Name_Category") && !NUL(oItem["s_Category"]))
                prepare["[#Category]"] = oItem["s_Category"];
            if (this.context.Contains("Name_Custom1") && !NUL(oItem["s_Custom1"]))
                prepare["[#Custom1]"] = oItem["s_Custom1"];
            if (this.context.Contains("Name_Custom2") && !NUL(oItem["s_Custom2"]))
                prepare["[#Custom2]"] = oItem["s_Custom2"];

            if (this.context.Lang == "ru" && !this.context.IsMobile)
                prepare["[#Share]"] = 1;

            var engine = this.context.GetEngine();

            if (Config.CACHE_PAGES)
                prepare["[#Home]"] = Util.ShowFromCache(engine, this.context.CacheFolder, "home", "Home", "p=home&from_view_item=1");
            else
                prepare["[#Home]"] = engine.IncludeTemplate("Pages/Home");

            this.Write("Pages/view_item", prepare);
        }
    }
}