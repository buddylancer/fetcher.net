// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Actions {
    using System;

    using Bula.Objects;
    using System.Collections;
    using Bula.Model;
    using Bula.Fetcher;
    using Bula.Fetcher.Model;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Testing sources for necessary fetching.
    /// </summary>
    public class DoTestItems : Page {
        private static String TOP = null;
        private static String BOTTOM = null;

        /// Initialize TOP and BOTTOM blocks. 
        private static void Initialize() {
            TOP = CAT(
                "<!DOCTYPE html>\r\n",
                "<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n",
                "    <head>\r\n",
                "        <title>Buddy Fetcher -- Test for new items</title>\r\n",
                "        <meta name=\"keywords\" content=\"Buddy Fetcher, rss, fetcher, aggregator, PHP, MySQL\" />\r\n",
                "        <meta name=\"description\" content=\"Buddy Fetcher is a simple RSS Fetcher/aggregator written in PHP/MySQL\" />\r\n",
                "        <meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" />\r\n",
                "    </head>\r\n",
                "    <body>\r\n"
            );
            BOTTOM = CAT(
                "    </body>\r\n",
                "</html>\r\n"
            );
        }

        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public DoTestItems(Context context) : base(context) { }

        /// Execute main logic for DoTestItems action 
        public override void Execute() {
            var insertRequired = false;
            var updateRequired = false;

            var doTime = new DOTime();

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

            Response.Write(TOP);
            if (updateRequired || insertRequired) {
                Response.Write("Fetching new items... Please wait...<br/>\r\n");

                var boFetcher = new BOFetcher(this.context);
                boFetcher.FetchFromSources();

                doTime = new DOTime(); // Need for DB reopen
                var fields = new Hashtable();
                fields["d_Time"] = DateTimes.Format(Config.SQL_DTS, DateTimes.GetTime());
                if (insertRequired) {
                    fields["i_Id"] = 1;
                    doTime.Insert(fields);
                }
                else
                    doTime.UpdateById(1, fields);
            }
            else
                Response.Write("<hr/>Fetch is not required<br/>\r\n");
            Response.Write(BOTTOM);
        }
    }
}