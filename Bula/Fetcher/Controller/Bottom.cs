namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using Bula.Model;
    using Bula.Fetcher.Model;

    /**
     * Logic for generating Bottom block.
     */
    public class Bottom : Bula.Meta {
        public static void Execute() {
            var Prepare = new Hashtable();

            var filter_link = CAT(Config.TOP_DIR,
                (Config.FineUrls ? "items/filter/" : CAT(Config.INDEX_PAGE, "?p=items&filter=")));

            var doCategory = new DOCategory();
            var dsCategory = doCategory.EnumAll("_this.i_Counter <> 0");
            var size = dsCategory.GetSize();
            int size3 = size % 3;
            int n1 = INT(size / 3) + (size3 == 0 ? 0 : 1);
            int n2 = n1 * 2;
            Object[] nn = ARR(0, n1, n2, size);
            var FilterBlocks = new ArrayList();
            for (int td = 0; td < 3; td++) {
                var FilterBlock = new Hashtable();
                var Rows = new ArrayList();
                for (int n = INT(nn[td]); n < INT(nn[td+1]); n++) {
                    var oCategory = dsCategory.GetRow(n);
                    var counter = INT(oCategory["i_Counter"]);
                    if (INT(counter) == 0)
                        continue;
                    var key = STR(oCategory["s_CatId"]);
                    var name = STR(oCategory["s_Name"]);
                    var Row = new Hashtable();
                    var href = CAT(filter_link, key);
                    Row["[#Link]"] = href;
                    Row["[#LinkText]"] = name;
                    //if (counter > 0)
                        Row["[#Counter]"] = counter;
                    Rows.Add(Row);
                }
                FilterBlock["[#Rows]"] = Rows;
                FilterBlocks.Add(FilterBlock);
            }
            Prepare["[#FilterBlocks]"] = FilterBlocks;

            if (!Config.IsMobile) {
                filter_link = CAT(Config.TOP_DIR,
                    (Config.FineUrls ? "rss/" : CAT(Config.RSS_PAGE, "?filter=")));
                dsCategory = doCategory.EnumAll();
                size = dsCategory.GetSize(); //50
                size3 = size % 3; //2
                n1 = INT(size / 3) + (size3 == 0 ? 0 : 1); //17.3
                n2 = n1 * 2; //34.6
                nn = ARR(0, n1, n2, size);
                var RssBlocks = new ArrayList();
                for (int td = 0; td < 3; td++) {
                    var RssBlock = new Hashtable();
                    var Rows = new ArrayList();
                    for (int n = INT(nn[td]); n < INT(nn[td+1]); n++) {
                        var oCategory = dsCategory.GetRow(n);
                        var key = STR(oCategory["s_CatId"]);
                        var name = STR(oCategory["s_Name"]);
                        //counter = INT(oCategory["i_Counter"]);
                        var Row = new Hashtable();
                        var href = CAT(filter_link, key, (Config.FineUrls ? ".xml" : null));
                        Row["[#Link]"] = href;
                        Row["[#LinkText]"] = name;
                        Rows.Add(Row);
                    }
                    RssBlock["[#Rows]"] = Rows;
                    RssBlocks.Add(RssBlock);
                }
                Prepare["[#RssBlocks]"] = RssBlocks;
            }
            Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/bottom.html", Prepare));
        }
    }
}