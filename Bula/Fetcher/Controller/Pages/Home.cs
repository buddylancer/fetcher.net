namespace Bula.Fetcher.Controller.Pages {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using Bula.Model;
    using Bula.Fetcher.Model;
    using Bula.Fetcher.Controller;

    /**
     * Controller for Home block.
     */
    public class Home : ItemsBase {
        public static void Execute() {
            var Prepare = new Hashtable();

            var doItem = new DOItem();

            var all_items_href =
                CAT(Config.TOP_DIR, (Config.FineUrls ? null : CAT(Config.INDEX_PAGE, "?p=")), "items");
            Prepare["[#BrowseItemsLink]"] = all_items_href;

            var source = (String)null;
            var search = (String)null;
            var max_rows = Config.DB_HOME_ROWS;
            var dsItems = doItem.EnumItems(source, search, 1, max_rows);
            var row_count = 1;
            var Items = new ArrayList();
            for (int n = 0; n < dsItems.GetSize(); n++) {
                var oItem = dsItems.GetRow(n);
                var Row = FillItemRow(oItem, doItem.GetIdField(), row_count);
                Items.Add(Row);
                row_count++;
            }
            Prepare["[#Items]"] = Items;
            Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/Pages/home.html", Prepare));
        }
    }
}