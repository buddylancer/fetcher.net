// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;

    using System.Collections;
    using Bula.Fetcher;
    using Bula.Objects;

    /// <summary>
    /// Logic for generating Top block.
    /// </summary>
    public class Top : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public Top(Context context) : base(context) { }

        /// Execute main logic for Top block 
        public override void Execute() {
            var Prepare = new Hashtable();
            Prepare["[#ImgWidth]"] = this.context.IsMobile ? 234 : 468;
            Prepare["[#ImgHeight]"] = this.context.IsMobile ? 30 : 60;
            if (this.context.TestRun)
                Prepare["[#Date]"] = "28-Jun-2020 16:49 GMT";
            else
                Prepare["[#Date]"] = Util.ShowTime(DateTimes.GmtFormat(Config.SQL_DTS));

            this.Write("Bula/Fetcher/View/top.html", Prepare);
        }
    }
}