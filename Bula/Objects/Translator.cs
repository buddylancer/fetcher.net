// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    using Bula.Objects;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper class for manipulation with text translations.
    /// </summary>
    public class Translator : Bula.Meta {
        private static TArrayList pairs = null;

        /// <summary>
        /// Initialize translation table.
        /// </summary>
        /// @param String @fileName Filename to load translation table from.
        /// <returns>Number of actual pairs in translation table.</returns>
        public static int Initialize(String fileName) {
            pairs = new TArrayList(Helper.ReadAllLines(fileName));
            return pairs.Size();
        }

        /// <summary>
        /// Translate content.
        /// </summary>
        /// <param name="input">Input content to translate.</param>
        /// <returns>Translated content.</returns>
        public static String Translate(String input) {
            var output = input;
            for (int n = 0; n < pairs.Size(); n++) {
                var line = Strings.Trim((String)pairs[n], "\r\n");
                if (BLANK(line) || line.IndexOf("#") == 0)
                    continue;
                if (line.IndexOf("|") == -1)
                    continue;

                String[] chunks = (String[])null;
                var needRegex = false;
                if (line.IndexOf("/") == 0) {
                    chunks = Strings.Split("\\|", line.Substring(1));
                    needRegex = true;
                }
                else {
                    chunks = Strings.Split("\\|", line);
                }
                var to = SIZE(chunks) > 1 ? chunks[1] : "";
                output = needRegex ?
                    Regex.Replace(output, chunks[0], to) :
                    Strings.Replace(chunks[0], to, output);
            }
            return output;
        }

        /// <summary>
        /// Check whether translation table is initialized (loaded).
        /// </summary>
        /// <returns>True if the table is initialized, False otherwise.</returns>
        public static Boolean IsInitialized() {
            return pairs != null;
        }
    }
}