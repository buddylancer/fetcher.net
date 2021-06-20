// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    /// <summary>
    /// Helper class for processing server response.
    /// </summary>
    public class TResponse : Bula.Meta {
        /// Current response 
        private System.Web.HttpResponse httpResponse = null;

        public TResponse (Object response) {
            httpResponse = (System.Web.HttpResponse)response;
        }

        /// <summary>
        /// Write text to current response.
        /// </summary>
        /// <param name="input">Text to write.</param>
        public void Write(String input) {
            httpResponse.Write(input);
        }

        /// <summary>
        /// Write header to current response.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value.</param>
        public void WriteHeader(String name, String value) {
            httpResponse.AppendHeader(name, value);
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