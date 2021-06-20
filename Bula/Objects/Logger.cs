// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    using Bula.Objects;

    /// <summary>
    /// Simple logger.
    /// </summary>
    public class Logger : Bula.Meta {
        private String fileName = null;
        private TResponse response = null;

        /// <summary>
        /// Initialize logging into file.
        /// </summary>
        /// <param name="filename">Log file name.</param>
        public void InitFile(String filename) {
            this.response = null;
            this.fileName = filename;
            if (filename.Length != 0) {
                if (Helper.FileExists(filename))
                    Helper.DeleteFile(filename);
            }
        }

        /// <summary>
        /// Initialize logging into file.
        /// </summary>
        /// <param name="filename">Log file name.</param>
        public void InitTResponse(TResponse response) {
            this.fileName = null;
            if (!NUL(response))
                this.response = response;
        }

        /// <summary>
        /// Log text string.
        /// </summary>
        /// <param name="text">Content to log.</param>
        public void Output(String text) {
            if (this.fileName == null) {
                this.response.Write(text);
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
            this.Output(CAT(text, " -- ", DateTimes.Format("H:i:s"), "<br/>", EOL));
        }
    }
}