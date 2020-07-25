namespace Bula.Fetcher.Controller.Actions {
    using System;

    using Bula.Fetcher.Controller;

    /**
     * Action for cleaning cache.
     */
    public class DoCleanCache : Bula.Meta {
        public void Execute() {
            var boFetcher = new BOFetcher();
            boFetcher.CleanCache();
        }
    }
}