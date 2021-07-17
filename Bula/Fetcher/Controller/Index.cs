// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;
    using System.Collections;

    using Bula.Fetcher;
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

            DataAccess.SetErrorDelegate(context.Response.End);

            var pageInfo = this.context.Request.TestPage(pagesArray, "home");

            // Test action name
            if (!pageInfo.ContainsKey("page")) {
                this.context.Response.End("Error in parameters -- no page");
                return;
            }

            var pageName = (String)pageInfo["page"];
            var className = (String)pageInfo["class"];

            //this.context.Request.Initialize();
            if (INT(pageInfo["post_required"]) == 1)
                this.context.Request.ExtractPostVars();
            else
                this.context.Request.ExtractAllVars();
            //echo "In Index -- " . Print_r(this, true);
            this.context["Page"] = pageName;

            var apiName = (String)pageInfo["api"];
            this.context.Api = BLANK(apiName) ? "" : apiName; // Blank (html) or "rest" for now

            var engine = this.context.PushEngine(true);

            var prepare = new THashtable();
            prepare["[#Site_Name]"] = Config.SITE_NAME;
            var pFromVars = this.context.Request.Contains("p") ? this.context.Request["p"] : "home";
            var idFromVars = this.context.Request.Contains("id") ? this.context.Request["id"] : null;
            var title = Config.SITE_NAME;
            if (pFromVars != "home")
                title = CAT(title, " :: ", pFromVars, (!NUL(idFromVars) ? CAT(" :: ", idFromVars) : null));

            prepare["[#Title]"] = title; //TODO -- need unique title on each page
            prepare.Put("[#Keywords]",
                this.context.TestRun ? Config.SITE_KEYWORDS :
                Strings.Replace("[#Platform]", Config.PLATFORM, Config.SITE_KEYWORDS)
            );
            prepare.Put("[#Description]",
                this.context.TestRun ? Config.SITE_DESCRIPTION :
                Strings.Replace("[#Platform]", Config.PLATFORM, Config.SITE_DESCRIPTION)
            );
            prepare["[#Styles]"] = CAT(
                    (this.context.TestRun ? null : Config.TOP_DIR),
                    this.context.IsMobile ? "styles2" : "styles");
            prepare["[#ContentType]"] = "text/html; charset=UTF-8";
            prepare["[#Top]"] = engine.IncludeTemplate("Top");
            prepare["[#Menu]"] = engine.IncludeTemplate("Menu");

            // Get included page either from cache or build it from the scratch
            var errorContent = engine.IncludeTemplate(CAT("Pages/", className), "check");
            if (!BLANK(errorContent)) {
                prepare["[#Page]"] = errorContent;
            }
            else {
                if (Config.CACHE_PAGES/* && !Config.DontCache.Contains(pageName)*/) //TODO!!!
                    prepare["[#Page]"] = Util.ShowFromCache(engine, this.context.CacheFolder, pageName, className);
                else
                    prepare["[#Page]"] = engine.IncludeTemplate(CAT("Pages/", className));
            }

            if (/*Config.RssAllowed != null && */Config.SHOW_BOTTOM) {
                // Get bottom block either from cache or build it from the scratch
                if (Config.CACHE_PAGES)
                    prepare["[#Bottom]"] = Util.ShowFromCache(engine, this.context.CacheFolder, BLANK(apiName) ? "bottom" : CAT(apiName, "_bottom"), "Bottom");
                else
                    prepare["[#Bottom]"] = engine.IncludeTemplate("Bottom");
            }

            prepare.Put("[#Github_Repo]",
                this.context.TestRun ? Config.GITHUB_REPO :
                Strings.Replace("[#Platform]", Strings.ToLowerCase(Config.PLATFORM), Config.GITHUB_REPO)
            );
            prepare.Put("[#Powered_By]",
                this.context.TestRun ? Config.POWERED_BY :
                Strings.Replace("[#Platform]", Config.PLATFORM, Config.POWERED_BY)
            );

            this.context.Response.WriteHeader("Content-type", CAT(
                (BLANK(apiName) ? "text/html" : Config.API_CONTENT), "; charset=UTF-8")
            );
            this.Write("index", prepare);

            // Fix <title>
            //TODO -- comment for now
            //newTitle = Util.ExtractInfo(content, "<input type=\"hidden\" name=\"s_Title\" value=\"", "\" />");
            //if (!BLANK(newTitle))
            //    content = Regex.Replace(content, "<title>(.*?)</title>", CAT("<title>", Config.SITE_NAME, " -- ", newTitle, "</title>"), RegexOptions.IgnoreCase);

            this.context.Response.Write(engine.GetPrintString());
            this.context.Response.End();
        }
    }
}