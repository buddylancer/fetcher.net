// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using System.Text.RegularExpressions;
    using Bula.Objects;
    using Bula.Model;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Controller for main Index page.
    /// </summary>
    public class Index : Page {
        private static Object[] pagesArray = null;

        private static void Initialize() {
            pagesArray = ARR(
                // page name,   class,          post,   code
                "home",         "Home",         0,      0,
                "items",        "Items",        0,      0,
                "view_item",    "ViewItem",     0,      0,
                "sources",      "Sources",      0,      0
            );
        }

        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public Index(Context context) : base(context) { }

        /// Execute main logic for Index block 
        public override void Execute() {
            if (pagesArray == null)
                Initialize();

            DataAccess.SetErrorDelegate(Bula.Objects.Response.End);

            var pageInfo = Request.TestPage(pagesArray, "home");

            // Test action name
            if (!pageInfo.ContainsKey("page"))
                Response.End("Error in parameters -- no page");

            var pageName = (String)pageInfo["page"];
            var className = (String)pageInfo["class"];

            Request.Initialize();
            if (INT(pageInfo["post_required"]) == 1)
                Request.ExtractPostVars();
            else
                Request.ExtractAllVars();
            //echo "In Index -- " . Print_r(this, true);
            this.context["Page"] = pageName;

            var engine = this.context.PushEngine(true);

            var prepare = new Hashtable();
            prepare["[#Site_Name]"] = Config.SITE_NAME;
            var pFromVars = Request.Contains("p") ? Request.Get("p") : "home";
            var idFromVars = Request.Contains("id") ? Request.Get("id") : null;
            var title = Config.SITE_NAME;
            if (pFromVars != "home")
                title = CAT(title, " :: ", pFromVars, (!NUL(idFromVars)? CAT(" :: ", idFromVars) : null));

            prepare["[#Title]"] = title; //TODO -- need unique title on each page
            prepare["[#Keywords]"] = Config.SITE_KEYWORDS;
            prepare["[#Description]"] = Config.SITE_DESCRIPTION;
            prepare["[#Styles]"] = CAT(
                    (this.context.TestRun ? null : Config.TOP_DIR),
                    this.context.IsMobile ? "styles2" : "styles");
            prepare["[#ContentType]"] = "text/html; charset=UTF-8";
            prepare["[#Top]"] = engine.IncludeTemplate("Bula/Fetcher/Controller/Top");
            prepare["[#Menu]"] = engine.IncludeTemplate("Bula/Fetcher/Controller/Menu");

            // Get included page either from cache or build it from the scratch
            var errorContent = engine.IncludeTemplate(CAT("Bula/Fetcher/Controller/Pages/", className), "check");
            if (!BLANK(errorContent)) {
                prepare["[#Page]"] = errorContent;
            }
            else {
                if (Config.CACHE_PAGES/* && !Config.DontCache.Contains(pageName)*/) //TODO!!!
                    prepare["[#Page]"] = Util.ShowFromCache(engine, this.context.CacheFolder, pageName, className);
                else
                    prepare["[#Page]"] = engine.IncludeTemplate(CAT("Bula/Fetcher/Controller/Pages/", className));
            }

            if (/*Config.RssAllowed != null && */Config.SHOW_BOTTOM) {
                // Get bottom block either from cache or build it from the scratch
                if (Config.CACHE_PAGES)
                    prepare["[#Bottom]"] = Util.ShowFromCache(engine, this.context.CacheFolder, "bottom", "Bottom");
                else
                    prepare["[#Bottom]"] = engine.IncludeTemplate("Bula/Fetcher/Controller/Bottom");
            }

            this.Write("Bula/Fetcher/View/index.html", prepare);

            // Fix <title>
            //TODO -- comment for now
            //newTitle = Util.ExtractInfo(content, "<input type=\"hidden\" name=\"s_Title\" value=\"", "\" />");
            //if (!BLANK(newTitle))
            //    content = Regex.Replace(content, "<title>(.*?)</title>", CAT("<title>", Config.SITE_NAME, " -- ", newTitle, "</title>"), RegexOptions.IgnoreCase);

            Response.Write(engine.GetPrintString());

            if (DBConfig.Connection != null) {
                DBConfig.Connection.Close();
                DBConfig.Connection = null;
            }
        }
    }
}