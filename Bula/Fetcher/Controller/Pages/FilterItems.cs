namespace Bula.Fetcher.Controller.Pages {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;
    using System.Collections;
    using Bula.Model;
    using Bula.Fetcher.Model;
    using Bula.Fetcher.Controller;

    /**
     * Controller for Filter Items block.
     */
    public class FilterItems : Bula.Meta {
        public static void Execute() {
            var doSource = new DOSource();

            var source = (String)null;
            if (Request.Contains("source"))
                source = Request.Get("source");

            var Prepare = new Hashtable();
            if (Config.FineUrls)
                Prepare["[#Fine_Urls]"] = 1;
            Prepare["[#Selected]"] = BLANK(source) ? " selected=\"selected\" " : null;
            var dsSources = (DataSet)null;
            //TODO -- This can be too long on big databases... Switch off counters for now.
            var useCounters = true;
            if (useCounters)
                dsSources = doSource.EnumSourcesWithCounters();
            else
                dsSources = doSource.EnumSources();
            var Options = new ArrayList();
            for (int n = 0; n < dsSources.GetSize(); n++) {
                var oSource = dsSources.GetRow(n);
                var Option = new Hashtable();
                Option["[#Selected]"] = (oSource["s_SourceName"].Equals(source) ? "selected=\"selected\"" : " ");
                Option["[#Id]"] = STR(oSource["s_SourceName"]);
                Option["[#Name]"] = STR(oSource["s_SourceName"]);
                if (useCounters)
                    Option["[#Counter]"] = oSource["cntpro"];
                Options.Add(Option);
            }
            Prepare["[#Options]"] = Options;
            Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/Pages/filter_items.html", Prepare));
        }
    }
}