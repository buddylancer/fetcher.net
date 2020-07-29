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
            var arr_size = SIZE(arr);
            for (int n = 0; n < arr_size; n++) {
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
        /// <param name="cache_folder">Cache folder root.</param>
        /// <param name="page">Page to process.</param>
        /// <returns>Resulting content.</returns>
        public static String ShowFromCache(String cache_folder, String page) {
            return ShowFromCache(cache_folder, page, null);
        }

        /// <summary>
        /// Main logic for getting/saving page from/into cache.
        /// </summary>
        /// <param name="cache_folder">Cache folder root.</param>
        /// <param name="page">Page to process.</param>
        /// <param name="query">Query to process.</param>
        /// <returns>Resulting content.</returns>
        public static String ShowFromCache(String cache_folder, String page, String query) {
            if (EQ(page, "bottom"))
                query = page;
            else {
                if (query == null)
                    query = Request.GetVar(Request.INPUT_SERVER, "QUERY_STRING");
                if (BLANK(query))
                    query = "p=home";
            }

            var content = (String)null;

            // Clean twitter-added parameters -- &utm_source=twitterfeed&utm_medium=twitter
            if (EQ(page, "view_item")) {
                if (!Request.Contains("id") || !Request.IsInteger(Request.Get("id"))) {
                    var Prepare = new Hashtable();
                    Prepare["[#ErrMessage]"] = "Incorrect Item ID, or not set ID!";
                    content = Engine.ShowTemplate("Bula/Fetcher/View/index.html", Prepare);
                    return content;
                }
                //TODO -- cut off "&title=some-title-of-the-item
                var title_pos = query.IndexOf("&title=");
                if (title_pos != -1)
                    query = query.Substring(0, title_pos);
                //query = Str_replace("&utm_source=twitterfeed&utm_medium=twitter", "", query);
            }

            var hash = query;
            //hash = Str_replace("?", "_Q_", hash);
            hash = Strings.Replace("=", "_EQ_", hash);
            hash = Strings.Replace("&", "_AND_", hash);
            var file_name = Strings.Concat(cache_folder, "/", hash, ".cache");
            if (Helper.FileExists(file_name)) {
                content = Helper.ReadAllText(file_name);
                //content = CAT("*** Got from cache ", Str_replace("/", " /", file_name), "***<br/>", content);
            }
            else {
                var prefix = EQ(page, "bottom") ? null : "pages/";
                var bottom_flag = EQ(page, "bottom") ? 1 : 0;
                content = Engine.IncludeTemplate(CAT("Bula/Fetcher/Controller/", prefix, page));

                TestFileFolder(file_name);
                Helper.WriteText(file_name, content);
                //content = CAT("*** Cached to ", Str_replace("/", " /", file_name), "***<br/>", content);
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
            int index1 = from == null ? 0 : IXOF(source, from);
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

        /// <summary>
        /// Test the chain of (sub)folder(s), create them if necessary.
        /// </summary>
        /// <param name="folder">Folder's full path.</param>
        public static void TestFolder(String folder) {
            String[] chunks = folder.Split(new char[] {'/'});
            var pathname = (String)null;
            for (int n = 0; n < SIZE(chunks); n++) {
                pathname = CAT(pathname, chunks[n]);
                if (!Helper.DirExists(pathname))
                    Helper.CreateDir(pathname);
                pathname = CAT(pathname, "/");
            }
        }

        /// <summary>
        /// Test the chain of (sub)folder(s) and file, create if necessary.
        /// </summary>
        /// <param name="filename">Filename's full path</param>
        public static void TestFileFolder(String filename) {
            String[] chunks = filename.Split(new char[] {'/'});
            var pathname = (String)null;
            for (int n = 0; n < SIZE(chunks) - 1; n++) {
                pathname = CAT(pathname, chunks[n]);
                if (!Helper.DirExists(pathname))
                    Helper.CreateDir(pathname);
                pathname = CAT(pathname, "/");
            }
        }

        private static String[] ru_chars =
        {
            "а","б","в","г","д","е","ё","ж","з","и","й","к","л","м","н","о","п",
            "р","с","т","у","ф","х","ц","ч","ш","щ","ъ","ы","ь","э","ю","я",
            "А","Б","В","Г","Д","Е","Ё","Ж","З","И","Й","К","Л","М","Н","О","П",
            "Р","С","Т","У","Ф","Х","Ц","Ч","Ш","Щ","Ъ","Ы","Ь","Э","Ю","Я",
            "á", "ą", "ä", "ę", "ó", "ś",
            "Á", "Ą", "Ä", "Ę", "Ó", "Ś"
        };

        private static String[] en_chars =
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
        /// <param name="ru_text">Original Russian text.</param>
        /// <returns>Transliterated text.</returns>
        public static String TransliterateRusToLat(String ru_text) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(ru_text);
            for (int n = 0; n < ru_chars.Length; n++)
                sb.Replace(ru_chars[n], en_chars[n]);
            return sb.ToString();
        }
    }
}