// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    using Bula.Objects;

    /// <summary>
    /// Helper class for processing server response.
    /// </summary>
    public class TResponse : Bula.Meta {
        /// Current response 
        private System.Web.HttpResponse httpResponse = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="currentResponse">Current http response object.</param>
        public TResponse (Object currentResponse) {
            httpResponse = (System.Web.HttpResponse)currentResponse;
        }

        /// <summary>
        /// Write text to current response.
        /// </summary>
        /// <param name="input">Text to write.</param>
        public void Write(String input) {
            this.Write(input, null);
        }

        /// <summary>
        /// Write text to current response.
        /// </summary>
        /// <param name="input">Text to write.</param>
        /// <param name="lang">Language to tranlsate to (default - none).</param>
        public void Write(String input, String langFile) {
            if (langFile != null) {
                if (!Translator.IsInitialized())
                    Translator.Initialize(langFile);
                if (Translator.IsInitialized())
                    input = Translator.Translate(input);
            }
            httpResponse.Write(input);
        }

        /// <summary>
        /// Write header to current response.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value.</param>
        public void WriteHeader(String name, String value) {
            WriteHeader(name, value, "UTF-8");
        }

        /// <summary>
        /// Write header to current response.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value.</param>
        /// <param name="encoding">Response encoding.</param>
        public void WriteHeader(String name, String value, String encoding) {
            httpResponse.AppendHeader(name, value);
            if (encoding != null) httpResponse.ContentEncoding = System.Text.Encoding.GetEncoding(encoding);
        }

        /// <summary>
        /// End current response.
        /// </summary>
        public void End() {
            End(null);
        }

        /// <summary>
        /// End current response.
        /// </summary>
        /// <param name="input">Text to write before ending response.</param>
        public void End(String input) {
            if (!NUL(input))
                Write(input);
            httpResponse.Flush();
            httpResponse.End();
        }
    }

}