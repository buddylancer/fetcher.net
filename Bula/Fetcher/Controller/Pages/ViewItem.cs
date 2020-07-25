namespace Bula.Fetcher.Controller.Pages {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;
    using System.Collections;
    using Bula.Model;
    using Bula.Fetcher.Model;
    using Bula.Fetcher.Controller;

    /**
     * Controller for View Item block.
     */
    public class ViewItem : Bula.Meta {
        public static void Execute() {
            var Prepare = new Hashtable();

            if (!Request.Contains("id")) {
                Prepare["[#ErrMessage]"] = "Item ID is required!";
                Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/error.html", Prepare));
                return;
            }
            var id = Request.Get("id");
            if (!Request.IsInteger(id)) {
                Prepare["[#ErrMessage]"] = "Item ID must be integer!";
                Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/error.html", Prepare));
                return;
            }

            var doItem = new DOItem();
            var dsItems = doItem.GetById(INT(Request.Get("id")));
            if (dsItems == null || dsItems.GetSize() == 0) {
                Prepare["[#ErrMessage]"] = "Wrong item ID!";
                Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/error.html", Prepare));
                //if (Config.CACHE_PAGES)
                //    Print(Util.ShowFromCache("home", "p=home&from_view_item=1"));
                //else
                //    Print(Engine.IncludeTemplate("Bula/Fetcher/Controller/Pages/Home"));
                return;
            }

            var oItem = dsItems.GetRow(0);
            var title = STR(oItem["s_Title"]);
            var source_name = STR(oItem["s_SourceName"]);

            Config.Set("Page_Title", title);
            var left_width = "25%";
            if (Config.IsMobile)
                left_width = "20%";

            var id_field = doItem.GetIdField();
            var redirect_item = CAT(Config.TOP_DIR,
                (Config.FineUrls ? "redirect/item/" : CAT(Config.ACTION_PAGE, "?p=do_redirect_item&id=")),
                oItem[id_field]);
            Prepare["[#RedirectLink]"] = redirect_item;
            Prepare["[#LeftWidth]"] = left_width;
            Prepare["[#Title]"] = Util.Show(title);
            Prepare["[#InputTitle]"] = Util.Safe(title);

            var redirect_source = CAT(
                Config.TOP_DIR,
                (Config.FineUrls ? "redirect/source/" : CAT(Config.ACTION_PAGE, "?p=do_redirect_source&source=")),
                source_name
            );
            Prepare["[#RedirectSource]"] = redirect_source;
            Prepare["[#SourceName]"] = source_name;
            Prepare["[#Date]"] = Util.ShowTime(STR(oItem["d_Date"]));
            Prepare["[#Creator]"] = oItem["s_Creator"];
            Prepare["[#Description]"] = oItem.ContainsKey("t_Description") ? Util.Show(STR(oItem["t_Description"])) : "";
            Prepare["[#ItemID]"] = oItem[id_field];
            if (Config.Contains("Name_Category")) Prepare["[#Category]"] = STR(oItem["s_Category"]);
            if (Config.Contains("Name_Custom1")) Prepare["[#Custom1]"] = oItem["s_Custom1"];
            if (Config.Contains("Name_Custom2")) Prepare["[#Custom2]"] = oItem["s_Custom2"];

            if (Config.Lang == "ru" && !Config.IsMobile)
                Prepare["[#Share]"] = 1;

            if (Config.CACHE_PAGES)
                Prepare["[#Home]"] = Util.ShowFromCache(Config.CacheFolder, "home", "p=home&from_view_item=1");
            else
                Prepare["[#Home]"] = Engine.IncludeTemplate("Bula/Fetcher/Controller/Pages/Home");

            Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/Pages/view_item.html", Prepare));
        }
    }
}