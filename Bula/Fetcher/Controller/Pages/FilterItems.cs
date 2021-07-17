// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Pages {
    using System;
    using System.Collections;

    using Bula.Fetcher;
    using Bula.Objects;
    using Bula.Model;
    using Bula.Fetcher.Model;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Controller for Filter Items block.
    /// </summary>
    public class FilterItems : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public FilterItems(Context context) : base(context) { }

        /// Execute main logic for FilterItems block. 
        public override void Execute() {
            var doSource = new DOSource(this.context.Connection);

            var source = (String)null;
            if (this.context.Request.Contains("source"))
                source = this.context.Request["source"];

            var prepare = new THashtable();
            if (this.context.FineUrls)
                prepare["[#Fine_Urls]"] = 1;
            prepare["[#Selected]"] = BLANK(source) ? " selected=\"selected\" " : "";
            var dsSources = (DataSet)null;
            //TODO -- This can be too long on big databases... Switch off counters for now.
            var useCounters = true;
            if (useCounters)
                dsSources = doSource.EnumSourcesWithCounters();
            else
                dsSources = doSource.EnumSources();
            var options = new TArrayList();
            for (int n = 0; n < dsSources.GetSize(); n++) {
                var oSource = dsSources.GetRow(n);
                var option = new THashtable();
                option["[#Selected]"] = (oSource["s_SourceName"].Equals(source) ? "selected=\"selected\"" : " ");
                option["[#Id]"] = STR(oSource["s_SourceName"]);
                option["[#Name]"] = STR(oSource["s_SourceName"]);
                if (useCounters)
                    option["[#Counter]"] = oSource["cntpro"];
                options.Add(option);
            }
            prepare["[#Options]"] = options;
            this.Write("Pages/filter_items", prepare);
        }
    }
}