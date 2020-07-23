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
            Hashtable Prepare = new Hashtable();

            DOItem doItem = new DOItem();

            String all_items_href =
                CAT(Config.TOP_DIR, (Config.FineUrls ? null : CAT(Config.INDEX_PAGE, "?p=")), "items");
            Prepare["[#BrowseItemsLink]"] = all_items_href;

            String source = null;
            String search = null;
            int max_rows = Config.DB_HOME_ROWS;
            DataSet dsItems = doItem.EnumItems(source, search, 1, max_rows);
            int row_count = 1;
            ArrayList Items = new ArrayList();
            for (int n = 0; n < dsItems.GetSize(); n++) {
                Hashtable oItem = dsItems.GetRow(n);
                Hashtable Row = FillItemRow(oItem, doItem.GetIdField(), row_count);
                Items.Add(Row);
                row_count++;
            }
            Prepare["[#Items]"] = Items;
            Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/Pages/home.html", Prepare));
        }
    }
}