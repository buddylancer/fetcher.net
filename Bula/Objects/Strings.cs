// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    using System.Text.RegularExpressions;

    using Bula;
    using Bula.Objects;

    /// <summary>
    /// Helper class for manipulations with strings.
    /// </summary>
    public class Strings : Bula.Meta {
        /// <summary>
        /// Provide empty array.
        /// </summary>
        /// <returns>Empty array of strings.</returns>
        public static String[] EmptyArray() {
            return new String[0];
        }

        /// <summary>
        /// Convert first char of a string to upper case.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Resulting string.</returns>
        public static String FirstCharToUpper(String input) {
            return Concat(input.Substring(0, 1).ToUpper(), input.Substring(1));
        }

        /// <summary>
        /// Join an array of strings using divider,
        /// </summary>
        /// <param name="divider">Divider (yes, may be empty).</param>
        /// <param name="strings">Array of strings.</param>
        /// <returns>Resulting string.</returns>
        public static String Join(String divider, String[] strings) {
            var output = "";
            var count = 0;
            foreach (String string1 in strings) {
                if (count > 0)
                    output += divider;
                output += string1;
                count++;
            }
            return output;
        }

        /// <summary>
        /// Remove all HTML tags from string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Resulting string.</returns>
        public static String RemoveTags(String input) {
            return RemoveTags(input, null);
        }

        /// <summary>
        /// Remove HTML tags from string except allowed ones.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="except">List of allowed tags (do not remove).</param>
        /// <returns>Resulting string.</returns>
        public static String RemoveTags(String input, String except) {
            return Internal.RemoveTags(input, except);
        }

        /// <summary>
        /// Add slashes to the string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Resulting string.</returns>
        public static String AddSlashes(String input) {
            return input.Replace("'", "\\'"); //TODO!!!
        }

        /// <summary>
        /// remove slashes from the string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Resulting string.</returns>
        public static String StripSlashes(String input) {
            return input.Replace("\\'", "'"); //TODO!!!
        }

        /// <summary>
        /// Count substrings in the string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="chunk">String to count.</param>
        /// <returns>Number of substrings.</returns>
        public static int CountSubstrings(String input, String chunk) {
            if (input.Length == 0)
                return 0;
            var replaced = input.Replace(chunk, "");
            return input.Length - replaced.Length;
        }

        /// <summary>
        /// Concatenate a number of strings to a single one.
        /// </summary>
        /// <param name="args">Array of strings.</param>
        /// <returns>Resulting string.</returns>
        public static String Concat(params object[] args) {
            var output = "";
            if (SIZE(args) != 0) {
                foreach (object arg in args) {
                    if (arg == null)
                        continue;
                    output += (String)arg;
                }
            }
            return output;
        }

        /// <summary>
        /// Split a string using divider/separator.
        /// </summary>
        /// <param name="divider">Divider/separator.</param>
        /// <param name="input">Input string.</param>
        /// <returns>Array of resulting strings.</returns>
        public static String[] Split(String divider, String input) {
            String[] chunks =
                Regex.Split(input, divider);
            var result = new TArrayList();
            for (int n = 0; n < SIZE(chunks); n++)
                result.Add(chunks[n]);
            return (String[])result.ToArray(typeof(String));
        }

        /// <summary>
        /// Replace all Substring(s) from a string.
        /// </summary>
        /// <param name="from">Substring to replace.</param>
        /// <param name="to">Replacement string.</param>
        /// <param name="input">Input string.</param>
        /// <returns>Resulting string.</returns>
        public static String Replace(String from, String to, String input) {
            return Replace(from, to, input, 0);
        }

        /// <summary>
        /// Replace a number of Substring(s) from a string.
        /// </summary>
        /// <param name="from">Substring to replace.</param>
        /// <param name="to">Replacement string.</param>
        /// <param name="input">Input string.</param>
        /// <param name="limit">Max number of replacements [optional].</param>
        /// <returns>Resulting string.</returns>
        public static String Replace(String from, String to, String input, int limit) {
            return limit != 0 ? (new Regex(from)).Replace(input, to, limit) : input.Replace(from, to);
        }

        /// <summary>
        /// Replace all substrings using regular expressions.
        /// </summary>
        /// <param name="regex">Regular expression to match Substring(s).</param>
        /// <param name="to">Replacement string.</param>
        /// <param name="input">Input string.</param>
        /// <returns>Resulting string.</returns>
        public static String ReplaceAll(String regex, String to, String input) {
            return Replace(regex, to, input);
        }

        /// <summary>
        /// Replace first substring using regular expressions.
        /// </summary>
        /// <param name="regex">Regular expression to match substring.</param>
        /// <param name="to">Replacement string.</param>
        /// <param name="input">Input string.</param>
        /// <returns>Resulting string.</returns>
        public static String ReplaceFirst(String regex , String to, String input) {
            return Replace(regex, to, input, 1);
        }

        /// <summary>
        /// Replace "keys by values" in a string.
        /// </summary>
        /// <param name="template">Input template.</param>
        /// <param name="hash">Set of key/value pairs.</param>
        /// <returns>Resulting string.</returns>
        public static String ReplaceInTemplate(String template, THashtable hash) {
            var keys = new TEnumerator(hash.Keys.GetEnumerator());
            while (keys.MoveNext()) {
                var key = STR(keys.GetCurrent());
                if (template.IndexOf(key) != -1)
                    template = template.Replace(key, STR(hash[key]));
            }
            return template;
        }

        /// <summary>
        /// Trim this string.
        /// </summary>
        /// <param name="input">String to trim.</param>
        /// <returns>Resulting string.</returns>
        public static String Trim(String input) {
            return Trim(input, null);
        }

        /// <summary>
        /// Trim this string.
        /// </summary>
        /// <param name="input">String to trim.</param>
        /// <param name="chars">Which chars to trim [optional].</param>
        /// <returns>Resulting string.</returns>
        public static String Trim(String input, String chars) {
            if (chars == null)
                chars = " \\n\\r\\t\\v\\0";
            input = Regex.Replace(input, CAT("^", "[", chars, "]*"), "");
            input = Regex.Replace(input, CAT("[", chars, "]*$"), "");
            return input;
        }

    }
}