namespace Bula.Fetcher.Controller.Actions {
    using System;

    using Bula.Fetcher.Controller;

    /// <summary>
    /// Action for cleaning cache.
    /// </summary>
    public class DoCleanCache : Bula.Meta {
        public void Execute() {
            var boFetcher = new BOFetcher();
            boFetcher.CleanCache();
        }
    }
}