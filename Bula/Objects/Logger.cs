// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;

    using Bula.Objects;

    /// <summary>
    /// Simple logger.
    /// </summary>
    public class Logger : Bula.Meta {
        private String fileName = null;

        /// <summary>
        /// Initialize logging into file.
        /// </summary>
        /// <param name="filename">Log file name.</param>
        public void Init(String filename) {
            this.fileName = filename;
            if (filename.Length != 0) {
                if (Helper.FileExists(filename))
                    Helper.DeleteFile(filename);
            }
        }

        /// <summary>
        /// Log text string.
        /// </summary>
        /// <param name="text">Content to log.</param>
        public void Output(String text) {
            if (this.fileName == null) {
                Response.Write(text);
                return;
            }
            if (Helper.FileExists(this.fileName))
                Helper.AppendText(this.fileName, text);
            else {
                Helper.TestFileFolder(this.fileName);
                Helper.WriteText(this.fileName, text);
            }

        }

        /// <summary>
        /// Log text string + current time.
        /// </summary>
        /// <param name="text">Content to log.</param>
        public void Time(String text) {
            this.Output(CAT(text, " -- ", DateTimes.Format("H:i:s"), "<br/>\r\n"));
        }
    }
}