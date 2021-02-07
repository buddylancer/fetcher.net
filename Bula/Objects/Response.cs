// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;

    /// <summary>
    /// Helper class for processing server response.
    /// </summary>
    public class Response : Bula.Meta {

        private static System.Web.HttpResponse CurrentResponse() {
            return System.Web.HttpContext.Current.Response;
        }

        /// <summary>
        /// Write text to current response.
        /// </summary>
        /// <param name="input">Text to write.</param>
        public static void Write(String input) {
            CurrentResponse().Write(input);
        }

        /// <summary>
        /// Write header to current response.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value.</param>
        public static void WriteHeader(String name, String value) {
            CurrentResponse().AppendHeader(name, value);
        }

        /// <summary>
        /// End current response.
        /// </summary>
        /// <param name="input">Text to write before ending response.</param>
        public static void End(String input) {
            Write(input);
            CurrentResponse().Flush();
            CurrentResponse().End();
        }
    }

}