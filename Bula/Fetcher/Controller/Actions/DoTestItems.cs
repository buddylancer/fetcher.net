// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Actions {
    using System;
    using System.Collections;

    using Bula.Objects;
    using Bula.Model;
    using Bula.Fetcher;
    using Bula.Fetcher.Model;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Testing sources for necessary fetching.
    /// </summary>
    public class DoTestItems : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public DoTestItems(Context context) : base(context) { Initialize(); }

        private static String TOP = null;
        private static String BOTTOM = null;

        /// Initialize TOP and BOTTOM blocks. 
        public static void Initialize() {
            TOP = CAT(
                "<!DOCTYPE html>", EOL,
                "<html xmlns=\"http://www.w3.org/1999/xhtml\">", EOL,
                "    <head>", EOL,
                "        <title>Buddy Fetcher -- Test for new items</title>", EOL,
                "        <meta name=\"keywords\" content=\"Buddy Fetcher, rss, fetcher, aggregator, ", Config.PLATFORM, ", MySQL\" />", EOL,
                "        <meta name=\"description\" content=\"Buddy Fetcher is a simple RSS Fetcher/aggregator written in ", Config.PLATFORM, "/MySQL\" />", EOL,
                "        <meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" />", EOL,
                "    </head>", EOL,
                "    <body>", EOL
            );
            BOTTOM = CAT(
                "    </body>", EOL,
                "</html>", EOL
            );
        }

        /// Execute main logic for DoTestItems action 
        public override void Execute() {
            var insertRequired = false;
            var updateRequired = false;

            var doTime = new DOTime(this.context.Connection);

            var dsTimes = doTime.GetById(1);
            var timeShift = 240; // 4 min
            var currentTime = DateTimes.GetTime();
            if (dsTimes.GetSize() > 0) {
                var oTime = dsTimes.GetRow(0);
                if (currentTime > DateTimes.GetTime(STR(oTime["d_Time"])) + timeShift)
                    updateRequired = true;
            }
            else
                insertRequired = true;

            var from = (String)null;
            if (this.context.Request.Contains("from"))
                from = this.context.Request["from"];

            this.context.Response.Write(TOP);
            if (updateRequired || insertRequired) {
                this.context.Response.Write(CAT("Fetching new items... Please wait...<br/>", EOL));

                var boFetcher = new BOFetcher(this.context);
                boFetcher.FetchFromSources(from);

                doTime = new DOTime(this.context.Connection); // Need for DB reopen
                var fields = new THashtable();
                fields["d_Time"] = DateTimes.Format(DateTimes.SQL_DTS, DateTimes.GetTime());
                if (insertRequired) {
                    fields["i_Id"] = 1;
                    doTime.Insert(fields);
                }
                else
                    doTime.UpdateById(1, fields);
            }
            else
                this.context.Response.Write(CAT("<hr/>Fetch is not required<br/>", EOL));
            this.context.Response.Write(BOTTOM);
        }
    }
}