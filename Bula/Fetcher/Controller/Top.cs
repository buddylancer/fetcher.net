namespace Bula.Fetcher.Controller {
    using System;

    using System.Collections;
    using Bula.Fetcher;
    using Bula.Objects;

    /// <summary>
    /// Logic for generating Top block.
    /// </summary>
    public class Top : Bula.Meta {
        /// <summary>
        /// Execute main logic for Top block.
        /// </summary>
        public static void Execute() {
            var Prepare = new Hashtable();
            Prepare["[#ImgWidth]"] = Config.IsMobile ? 234 : 468;
            Prepare["[#ImgHeight]"] = Config.IsMobile ? 30 : 60;
            if (Config.TestRun)
                Prepare["[#Date]"] = "28-Jun-2020 16:49 GMT";
            else
                Prepare["[#Date]"] = Util.ShowTime(DateTimes.GmtFormat(Config.SQL_DTS));
            Engine.Write(Engine.ShowTemplate("Bula/Fetcher/View/top.html", Prepare));
        }
    }
}