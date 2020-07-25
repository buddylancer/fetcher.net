namespace Bula.Fetcher.Controller.Pages {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using Bula.Model;
    using Bula.Fetcher.Model;
    using Bula.Fetcher.Controller;

    /**
     * Controller for Sources block.
     */
    public class Sources : ItemsBase {
        public static void Execute() {
            var Prepare = new Hashtable();

            var doSource = new DOSource();
            var doItem = new DOItem();

            var dsSources = doSource.EnumSources();
            var count = 1;
            var Sources = new ArrayList();
            for (int ns = 0; ns < dsSources.GetSize(); ns++) {
                var oSource = dsSources.GetRow(ns);
                var source_name = STR(oSource["s_SourceName"]);

                var SourceRow = new Hashtable();
                SourceRow["[#SourceName]"] = source_name;
                //SourceRow["[#RedirectSource]"] = Config.TOP_DIR .
                //    (Config.FINE_URLS ? "redirect/source/" : "action.php?p=do_redirect_source&source=") .
                //        oSource["s_SourceName"];
                SourceRow["[#RedirectSource]"] = CAT(Config.TOP_DIR,
                    (Config.FineUrls ? "items/source/" : CAT(Config.INDEX_PAGE, "?p=items&source=")), source_name);

                var dsItems = doItem.EnumItemsFromSource(null, source_name, null, 3);
                var Items = new ArrayList();
                var item_count = 0;
                for (int ni = 0; ni < dsItems.GetSize(); ni++) {
                    var oItem = dsItems.GetRow(ni);
                    Items.Add(FillItemRow(oItem, doItem.GetIdField(), item_count));
                    item_count++;
                }
                SourceRow["[#Items]"] = Items;

                Sources.Add(SourceRow);
                count++;
            }
            Prepare["[#Sources]"] = Sources;
            Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/Pages/sources.html", Prepare));
        }
    }
}