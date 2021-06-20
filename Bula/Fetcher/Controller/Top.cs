// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;
    using System.Collections;

    using Bula.Objects;
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
            var prepare = new THashtable();
            prepare["[#ImgWidth]"] = this.context.IsMobile ? 234 : 468;
            prepare["[#ImgHeight]"] = this.context.IsMobile ? 30 : 60;
            if (this.context.TestRun)
                prepare["[#Date]"] = "28-Jun-2020 16:49 GMT";
            else
                prepare["[#Date]"] = Util.ShowTime(DateTimes.GmtFormat(DateTimes.SQL_DTS));

            this.Write("top", prepare);
        }
    }
}