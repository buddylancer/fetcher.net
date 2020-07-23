namespace Bula.Fetcher.Controller.Actions {
    using System;

    using Bula.Objects;
    using System.Collections;
    using Bula.Model;
    using Bula.Fetcher;
    using Bula.Fetcher.Model;
    using Bula.Fetcher.Controller;

    /**
     * Testing sources for necessary fetching.
     */
    public class DoTestItems : Bula.Meta {
        private static String TOP = null;
        private static String BOTTOM = null;

        ///Initialize TOP & BOTTOM blocks.
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

        public static void Execute() {
            Boolean insert_required = false;
            Boolean update_required = false;

            DOTime doTime = new DOTime();

            DataSet dsTimes = doTime.GetById(1);
            int time_shift = 240; // 4 min
            int current_time = DateTimes.GetTime("now");
            if (dsTimes.GetSize() > 0) {
                Hashtable oTime = dsTimes.GetRow(0);
                if (current_time > DateTimes.GetTime(STR(oTime["d_Time"])) + time_shift)
                    update_required = true;
            }
            else
                insert_required = true;

            Response.Write(TOP);
            if (update_required || insert_required) {
                Response.Write("Fetching new items... Please wait...<br/>\r\n");

                BOFetcher boFetcher = new BOFetcher();
                boFetcher.Execute();

                doTime = new DOTime(); // Need for DB reopen
                Hashtable fields = new Hashtable();
                fields["d_Time"] = DateTimes.Format(Config.SQL_DTS, DateTimes.GetTime("now"));
                if (insert_required) {
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