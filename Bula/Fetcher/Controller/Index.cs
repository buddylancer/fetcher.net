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
    public class Index : Bula.Meta {
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

        /// <summary>
        /// Execute main logic for Index page.
        /// </summary>
        public static void Execute() {
            if (pages_array == null)
                Initialize();

            DataAccess.SetErrorDelegate(Bula.Objects.Response.End);

            var page_info = Request.TestPage(pages_array, "home");

            // Test action name
            if (!page_info.ContainsKey("page"))
                Response.End("Error in parameters -- no page");

            Request.Initialize();
            if (INT(page_info["post_required"]) == 1)
                Request.ExtractPostVars();
            else
                Request.ExtractAllVars();
            Config.Set("Page", page_info["page"]);

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
                    (Config.TestRun ? null : Config.TOP_DIR),
                    Config.IsMobile ? "styles2" : "styles");
            Prepare["[#ContentType]"] = "text/html; charset=UTF-8";
            Prepare["[#Top]"] = Engine.IncludeTemplate("Bula/Fetcher/Controller/Top");
            Prepare["[#Menu]"] = Engine.IncludeTemplate("Bula/Fetcher/Controller/Menu");
            if (!page_info.ContainsKey("page"))
                Prepare["[#Page]"] = Engine.ShowTemplate("Bula/Fetcher/View/no_such_file.html");
            else {
                // Get included page either from cache or build it from the scratch
                if (Config.CACHE_PAGES/* && !Config.DontCache.Contains(page_info["page"])*/) //TODO!!!
                    Prepare["[#Page]"] = Util.ShowFromCache(Config.CacheFolder, STR(page_info["page"]));
                else
                    Prepare["[#Page]"] = Engine.IncludeTemplate(CAT("Bula/Fetcher/Controller/Pages/", page_info["page"]));
            }
            if (/*Config.RssAllowed != null && */Config.SHOW_BOTTOM) {
                // Get bottom block either from cache or build it from the scratch
                if (Config.CACHE_PAGES)
                    Prepare["[#Bottom]"] = Util.ShowFromCache(Config.CacheFolder, "bottom");
                else
                    Prepare["[#Bottom]"] = Engine.IncludeTemplate("Bula/Fetcher/Controller/Bottom");
            }

            // All is ready - apply template
            Engine.Push(true);
            var content = Engine.ShowTemplate("Bula/Fetcher/View/index.html", Prepare);

            // Fix <title>
            //TODO -- comment for now
            //new_title = Util.ExtractInfo(content, "<input type=\"hidden\" name=\"s_Title\" value=\"", "\" />");
            //if (!BLANK(new_title))
            //    content = Regex.Replace(content, "<title>(.*?)</title>", CAT("<title>", Config.SITE_NAME, " -- ", new_title, "</title>"), RegexOptions.IgnoreCase);

            Response.Write(content);
            if (DBConfig.Connection != null) {
                DBConfig.Connection.Close();
                DBConfig.Connection = null;
            }
        }
    }
}