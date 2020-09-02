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
        private static Object[] pages_array = null;

        private static void Initialize() {
            pages_array = ARR(
                // page name,   class,          post,   code
                "home",         "Home",         0,      0,
                "items",        "Items",        0,      0,
                "view_item",    "ViewItem",     0,      0,
                "sources",      "Sources",      0,      0
            );
        }

        public Index(Context context) : base(context) { }

        public override void Execute() {
            if (pages_array == null)
                Initialize();

            DataAccess.SetErrorDelegate(Bula.Objects.Response.End);

            var page_info = Request.TestPage(pages_array, "home");

            // Test action name
            if (!page_info.ContainsKey("page"))
                Response.End("Error in parameters -- no page");

            var page_name = (String)page_info["page"];
            var class_name = (String)page_info["class"];

            Request.Initialize();
            if (INT(page_info["post_required"]) == 1)
                Request.ExtractPostVars();
            else
                Request.ExtractAllVars();
            //echo "In Index -- " . Print_r(this, true);
            this.context["Page"] = page_name;

            var engine = this.context.PushEngine(true);

            var Prepare = new Hashtable();
            Prepare["[#Site_Name]"] = Config.SITE_NAME;
            var p_from_vars = Request.Contains("p") ? Request.Get("p") : "home";
            var id_from_vars = Request.Contains("id") ? Request.Get("id") : null;
            var title = Config.SITE_NAME;
            if (p_from_vars != "home")
                title = CAT(title, " :: ", p_from_vars, (!NUL(id_from_vars)? CAT(" :: ", id_from_vars) : null));

            Prepare["[#Title]"] = title; //TODO -- need unique title on each page
            Prepare["[#Keywords]"] = Config.SITE_KEYWORDS;
            Prepare["[#Description]"] = Config.SITE_DESCRIPTION;
            Prepare["[#Styles]"] = CAT(
                    (this.context.TestRun ? null : Config.TOP_DIR),
                    this.context.IsMobile ? "styles2" : "styles");
            Prepare["[#ContentType]"] = "text/html; charset=UTF-8";
            Prepare["[#Top]"] = engine.IncludeTemplate("Bula/Fetcher/Controller/Top");
            Prepare["[#Menu]"] = engine.IncludeTemplate("Bula/Fetcher/Controller/Menu");

            // Get included page either from cache or build it from the scratch
            var error_content = engine.IncludeTemplate(CAT("Bula/Fetcher/Controller/Pages/", class_name), "check");
            if (!BLANK(error_content)) {
                Prepare["[#Page]"] = error_content;
            }
            else {
                if (Config.CACHE_PAGES/* && !Config.DontCache.Contains(page_name)*/) //TODO!!!
                    Prepare["[#Page]"] = Util.ShowFromCache(engine, this.context.CacheFolder, page_name, class_name);
                else
                    Prepare["[#Page]"] = engine.IncludeTemplate(CAT("Bula/Fetcher/Controller/Pages/", class_name));
            }

            if (/*Config.RssAllowed != null && */Config.SHOW_BOTTOM) {
                // Get bottom block either from cache or build it from the scratch
                if (Config.CACHE_PAGES)
                    Prepare["[#Bottom]"] = Util.ShowFromCache(engine, this.context.CacheFolder, "bottom", "Bottom");
                else
                    Prepare["[#Bottom]"] = engine.IncludeTemplate("Bula/Fetcher/Controller/Bottom");
            }

            this.Write("Bula/Fetcher/View/index.html", Prepare);

            // Fix <title>
            //TODO -- comment for now
            //new_title = Util.ExtractInfo(content, "<input type=\"hidden\" name=\"s_Title\" value=\"", "\" />");
            //if (!BLANK(new_title))
            //    content = Regex.Replace(content, "<title>(.*?)</title>", CAT("<title>", Config.SITE_NAME, " -- ", new_title, "</title>"), RegexOptions.IgnoreCase);

            Response.Write(engine.GetPrintString());

            if (DBConfig.Connection != null) {
                DBConfig.Connection.Close();
                DBConfig.Connection = null;
            }
        }
    }
}