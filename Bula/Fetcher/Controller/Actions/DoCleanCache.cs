// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Actions {
    using System;

    using Bula.Objects;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Action for cleaning cache.
    /// </summary>
    public class DoCleanCache : Page {
        public DoCleanCache(Context context) : base(context) { }

        /// Execute main logic for this action 
        public override void Execute() {
            var oLogger = new Logger();
            var log = Request.GetOptionalInteger("log");
            if (!NUL(log) && log != -99999) {
                var filename_template = (String)"C:/Temp/Log_{0}_{1}.html";
                var filename = Util.FormatString(filename_template, ARR("do_clean_cache", DateTimes.Format(Config.SQL_DTS)));
                oLogger.Init(filename);
            }
            this.CleanCache(oLogger);
        }

        /// <summary>
        /// Actual cleaning of cache folder.
        /// </summary>
        /// <param name="path_name">Cache folder name (path).</param>
        /// <param name="ext">Files extension to clean.</param>
        private void CleanCacheFolder(Logger oLogger, String path_name, String ext) {
            if (!Helper.DirExists(path_name))
                return;

            var entries = Helper.ListDirEntries(path_name);
            while (entries.MoveNext()) {
                var entry = (String)entries.Current;

                if (Helper.IsFile(entry) && entry.EndsWith(ext)) {
                    oLogger.Output(CAT("Deleting of ", entry, " ...<br/>\r\n"));
                    Helper.DeleteFile(entry);
                }
                else if (Helper.IsDir(entry)) {
                    oLogger.Output(CAT("Drilling to ", entry, " ...<br/>\r\n"));
                    CleanCacheFolder(oLogger, entry, ext);
                }
                //unlink(path_name); //Comment for now -- dangerous operation!!!
            }
        }

        /// <summary>
        /// Clean all cached info (both for Web and RSS).
        /// </summary>
        public void CleanCache(Logger oLogger) {
            // Clean cached rss content
            oLogger.Output(CAT("Cleaning Rss Folder ", this.context.RssFolderRoot, " ...<br/>\r\n"));
            var rssFolder = Strings.Concat(this.context.RssFolderRoot);
            this.CleanCacheFolder(oLogger, rssFolder, ".xml");

            // Clean cached pages content
            oLogger.Output(CAT("Cleaning Cache Folder ", this.context.CacheFolderRoot,  "...<br/>\r\n"));
            var cacheFolder = Strings.Concat(this.context.CacheFolderRoot);
            this.CleanCacheFolder(oLogger, cacheFolder, ".cache");

            oLogger.Output("<br/>... Done.<br/>\r\n");
        }

    }
}