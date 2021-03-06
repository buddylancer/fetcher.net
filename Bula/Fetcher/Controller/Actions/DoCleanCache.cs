// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Actions {
    using System;
    using System.Collections;

    using Bula.Objects;
    using Bula.Fetcher;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Action for cleaning cache.
    /// </summary>
    public class DoCleanCache : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public DoCleanCache(Context context) : base(context) { }

        /// Execute main logic for DoCleanCache action 
        public override void Execute() {
            var oLogger = new Logger();
            var log = this.context.Request.GetOptionalInteger("log");
            if (!NUL(log) && log != -99999) {
                var filenameTemplate = (String)"C:/Temp/Log_{0}_{1}.html";
                var filename = Util.FormatString(filenameTemplate, ARR("do_clean_cache", DateTimes.Format(DateTimes.SQL_DTS)));
                oLogger.InitFile(filename);
            }
            else
                oLogger.InitResponse(this.context.Response);
            this.CleanCache(oLogger);
        }

        /// <summary>
        /// Actual cleaning of cache folder.
        /// </summary>
        /// <param name="oLogger">Logger instance.</param>
        /// <param name="pathName">Cache folder name (path).</param>
        /// <param name="ext">Files extension to clean.</param>
        private void CleanCacheFolder(Logger oLogger, String pathName, String ext) {
            if (!Helper.DirExists(pathName))
                return;

            var entries = Helper.ListDirEntries(pathName);
            while (entries.MoveNext()) {
                var entry = (String)entries.GetCurrent();

                if (Helper.IsFile(entry) && entry.EndsWith(ext)) {
                    oLogger.Output(CAT("Deleting of ", entry, " ...<br/>", EOL));
                    Helper.DeleteFile(entry);
                }
                else if (Helper.IsDir(entry)) {
                    oLogger.Output(CAT("Drilling to ", entry, " ...<br/>", EOL));
                    CleanCacheFolder(oLogger, entry, ext);
                }
                //unlink(pathName); //Comment for now -- dangerous operation!!!
            }
        }

        /// <summary>
        /// Clean all cached info (both for Web and RSS).
        /// </summary>
        public void CleanCache(Logger oLogger) {
            // Clean cached rss content
            oLogger.Output(CAT("Cleaning Rss Folder ", this.context.RssFolderRoot, " ...<br/>", EOL));
            var rssFolder = Strings.Concat(this.context.RssFolderRoot);
            this.CleanCacheFolder(oLogger, rssFolder, ".xml");

            // Clean cached pages content
            oLogger.Output(CAT("Cleaning Cache Folder ", this.context.CacheFolderRoot,  "...<br/>", EOL));
            var cacheFolder = Strings.Concat(this.context.CacheFolderRoot);
            this.CleanCacheFolder(oLogger, cacheFolder, ".cache");

            oLogger.Output(CAT("<br/>... Done.<br/>", EOL));
        }

    }
}