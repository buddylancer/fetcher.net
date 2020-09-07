// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using System.Collections;

    using Bula.Objects;

    /// <summary>
    /// Various helper methods.
    /// </summary>
    public class Util : Bula.Meta {
        /// <summary>
        /// Output text safely.
        /// </summary>
        /// <param name="input">Text to output.</param>
        /// <returns>Converted text.</returns>
        public static String Safe(String input) {
            var output = Strings.StripSlashes(input);
            output = output.Replace("<", "&lt;");
            output = output.Replace(">", "&gt;");
            output = output.Replace("\"", "&quot;");
            return output;
        }

        /// <summary>
        /// Output text safely with line breaks.
        /// </summary>
        /// <param name="input">Text to output.</param>
        /// <returns>Converted text.</returns>
        public static String Show(String input) {
            if (input == null)
                return null;
            var output = Safe(input);
            output = output.Replace("\n", "<br/>");
            return output;
        }

        /// <summary>
        /// Format date/time to GMT presentation.
        /// </summary>
        /// <param name="input">Input date/time.</param>
        /// <returns>Resulting date/time.</returns>
        public static String ShowTime(String input) {
            return DateTimes.Format(Config.GMT_DTS, DateTimes.GetTime(input));
        }

        /// <summary>
        /// Format string.
        /// </summary>
        /// <param name="format">Format (template).</param>
        /// <param name="arr">Parameters.</param>
        /// <returns>Resulting string.</returns>
        public static String FormatString(String format, Object[] arr) {
            if (BLANK(format))
                return null;
            var output = format;
            var arrSize = SIZE(arr);
            for (int n = 0; n < arrSize; n++) {
                var match = CAT("{", n, "}");
                var ind = format.IndexOf(match);
                if (ind == -1)
                    continue;
                output = output.Replace(match, (String)arr[n]);
            }
            return output;
        }

        /// <summary>
        /// Logic for getting/saving page from/into cache.
        /// </summary>
        /// <param name="engine">Engine instance.</param>
        /// <param name="cacheFolder">Cache folder root.</param>
        /// <param name="pageName">Page to process.</param>
        /// <param name="className">Appropriate class name.</param>
        /// <returns>Resulting content.</returns>
        public static String ShowFromCache(Engine engine, String cacheFolder, String pageName, String className) {
            return ShowFromCache(engine, cacheFolder, pageName, className, null);
        }

        /// <summary>
        /// Main logic for getting/saving page from/into cache.
        /// </summary>
        /// <param name="engine">Engine instance.</param>
        /// <param name="cacheFolder">Cache folder root.</param>
        /// <param name="pageName">Page to process.</param>
        /// <param name="className">Appropriate class name.</param>
        /// <param name="query">Query to process.</param>
        /// <returns>Resulting content.</returns>
        public static String ShowFromCache(Engine engine, String cacheFolder, String pageName, String className, String query) {
            if (EQ(pageName, "bottom"))
                query = pageName;
            else {
                if (query == null)
                    query = Request.GetVar(Request.INPUT_SERVER, "QUERY_STRING");
                if (BLANK(query))
                    query = "p=home";
            }

            var content = (String)null;

            if (EQ(pageName, "view_item")) {
                var titlePos = query.IndexOf("&title=");
                if (titlePos != -1)
                    query = query.Substring(0, titlePos);
            }

            var hash = query;
            //hash = Str_replace("?", "_Q_", hash);
            hash = Strings.Replace("=", "_EQ_", hash);
            hash = Strings.Replace("&", "_AND_", hash);
            var fileName = Strings.Concat(cacheFolder, "/", hash, ".cache");
            if (Helper.FileExists(fileName)) {
                content = Helper.ReadAllText(fileName);
                //content = CAT("*** Got from cache ", Str_replace("/", " /", fileName), "***<br/>", content);
            }
            else {
                var prefix = EQ(pageName, "bottom") ? null : "Pages/";
                content = engine.IncludeTemplate(CAT("Bula/Fetcher/Controller/", prefix, className));

                Helper.TestFileFolder(fileName);
                Helper.WriteText(fileName, content);
                //content = CAT("*** Cached to ", Str_replace("/", " /", fileName), "***<br/>", content);
            }
            return content;
        }

        /// <summary>
        /// Max length to extract from string.
        /// </summary>
        public const int MAX_EXTRACT = 100;

        /// <summary>
        /// Extract info from a string.
        /// </summary>
        /// <param name="source">Input string.</param>
        /// <param name="after">Substring to extract info "After".</param>
        /// <param name="to">Substring to extract info "To".</param>
        /// <returns>Resulting string.</returns>
        public static String ExtractInfo(String source, String after, String to = null) {
            var result = (String)null;
            if (!NUL(source)) {
                int index1 = 0;
                if (!NUL(after)) {
                    index1 = source.IndexOf(after);
                    if (index1 == -1)
                        return null;
                    index1 += LEN(after);
                }
                int index2 = source.Length;
                if (!NUL(to)) {
                    index2 = source.IndexOf(to, index1);
                    if (index2 == -1)
                        index2 = source.Length;
                }
                var length = index2 - index1;
                if (length > MAX_EXTRACT)
                    length = MAX_EXTRACT;
                result = source.Substring(index1, length);
            }
            return result;
        }

        /// <summary>
        /// Remove some content from a string.
        /// </summary>
        /// <param name="source">Input string.</param>
        /// <param name="from">Substring to remove "From".</param>
        /// <param name="to">Substring to remove "To".</param>
        /// <returns>Resulting string.</returns>
        public static String RemoveInfo(String source, String from, String to = null) {
            var result = (String)null;
            int index1 = from == null ? 0 : source.IndexOf(from);
            if (index1 != -1) {
                if (to == null)
                    result = source.Substring(index1);
                else {
                    int index2 = source.IndexOf(to, index1);
                    if (index2 == -1)
                        result = source;
                    else {
                        index2 += to.Length;
                        result = Strings.Concat(
                            source.Substring(0, index1),
                            source.Substring(index2));
                    }
                }
            }
            return result.Trim();
        }

        private static String[] ruChars =
        {
            "а","б","в","г","д","е","ё","ж","з","и","й","к","л","м","н","о","п",
            "р","с","т","у","ф","х","ц","ч","ш","щ","ъ","ы","ь","э","ю","я",
            "А","Б","В","Г","Д","Е","Ё","Ж","З","И","Й","К","Л","М","Н","О","П",
            "Р","С","Т","У","Ф","Х","Ц","Ч","Ш","Щ","Ъ","Ы","Ь","Э","Ю","Я",
            "á", "ą", "ä", "ę", "ó", "ś",
            "Á", "Ą", "Ä", "Ę", "Ó", "Ś"
        };

        private static String[] enChars =
        {
            "a","b","v","g","d","e","io","zh","z","i","y","k","l","m","n","o","p",
            "r","s","t","u","f","h","ts","ch","sh","shch","\"","i","\"","e","yu","ya",
            "A","B","V","G","D","E","IO","ZH","Z","I","Y","K","L","M","N","O","P",
            "R","S","T","U","F","H","TS","CH","SH","SHCH","\"","I","\"","E","YU","YA",
            "a", "a", "ae", "e", "o", "s",
            "A", "a", "AE", "E", "O", "S"
        };

        /// <summary>
        /// Transliterate Russian text.
        /// </summary>
        /// <param name="ruText">Original Russian text.</param>
        /// <returns>Transliterated text.</returns>
        public static String TransliterateRusToLat(String ruText) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(ruText);
            for (int n = 0; n < ruChars.Length; n++)
                sb.Replace(ruChars[n], enChars[n]);
            return sb.ToString();
        }
    }
}