namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using System.Collections;

    using Bula.Objects;

    /**
     * Various helper methods.
     */
    public class Util : Bula.Meta {
        public static String Safe(String input) {
            String output = Strings.StripSlashes(input);
            output = output.Replace("<", "&lt;");
            output = output.Replace(">", "&gt;");
            output = output.Replace("\"", "&quot;");
            return output;
        }

        public static String Show(String input) {
            if (input == null)
                return null;
            String output = Safe(input);
            output = output.Replace("\n", "<br/>");
            return output;
        }

        public static String ShowTime(String input) {
            return DateTimes.Format(Config.GMT_DTS, DateTimes.GetTime(input));
        }

        public static String FormatString(String format, Object[] arr) {
            if (BLANK(format))
                return null;
            String output = format;
            int arr_size = SIZE(arr);
            for (int n = 0; n < arr_size; n++) {
                String match = CAT("{", n, "}");
                int ind = format.IndexOf(match);
                if (ind == -1)
                    continue;
                output = output.Replace(match, (String)arr[n]);
            }
            return output;
        }

        public static String ShowFromCache(String cache_folder, String page) {
            return ShowFromCache(cache_folder, page, null);
        }
        ///Main logic for getting/saving page from/into cache.
        /// <param name="cache_folder">Cache folder root</param>
        /// <param name="page">Page to process</param>
        /// <param name="query">Query to process</param>
        /// <returns>Resulting content</returns>
        public static String ShowFromCache(String cache_folder, String page, String query) {
            if (EQ(page, "bottom"))
                query = page;
            else {
                if (query == null)
                    query = Request.GetVar(Request.INPUT_SERVER, "QUERY_STRING");
                if (BLANK(query))
                    query = "p=home";
            }

            String content = null;

            // Clean twitter-added parameters -- &utm_source=twitterfeed&utm_medium=twitter
            if (EQ(page, "view_item")) {
                if (!Request.Contains("id") || !Request.IsInteger(Request.Get("id"))) {
                    Hashtable Prepare = new Hashtable();
                    Prepare["[#ErrMessage]"] = "Incorrect Item ID, or not set ID!";
                    content = Engine.ShowTemplate("Bula/Fetcher/View/index.html", Prepare);
                    return content;
                }
                //TODO -- cut off "&title=some-title-of-the-item
                int title_pos = query.IndexOf("&title=");
                if (title_pos != -1)
                    query = query.Substring(0, title_pos);
                //query = Str_replace("&utm_source=twitterfeed&utm_medium=twitter", "", query);
            }

            String hash = query;
            //hash = Str_replace("?", "_Q_", hash);
            hash = Strings.Replace("=", "_EQ_", hash);
            hash = Strings.Replace("&", "_AND_", hash);
            String file_name = Strings.Concat(cache_folder, "/", hash, ".cache");
            if (Helper.FileExists(file_name)) {
                content = Helper.ReadAllText(file_name);
                //content = CAT("*** Got from cache ", Str_replace("/", " /", file_name), "***<br/>", content);
            }
            else {
                String prefix = EQ(page, "bottom") ? null : "pages/";
                int bottom_flag = EQ(page, "bottom") ? 1 : 0;
                content = Engine.IncludeTemplate(CAT("Bula/Fetcher/Controller/", prefix, page));

                TestFileFolder(file_name);
                Helper.WriteText(file_name, content);
                //content = CAT("*** Cached to ", Str_replace("/", " /", file_name), "***<br/>", content);
            }
            return content;
        }

        public const int MAX_EXTRACT = 100;
        ///Extract info from a string.
        /// <param name="source">Input string</param>
        /// <param name="after">Substring to extract info "After"</param>
        /// <param name="to">Substring to extract info "To"</param>
        /// <returns>Resulting string</returns>
        public static String ExtractInfo(String source, String after, String to = null) {
            String result = null;
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
                int length = index2 - index1;
                if (length > MAX_EXTRACT)
                    length = MAX_EXTRACT;
                result = source.Substring(index1, length);
            }
            return result;
        }

        ///Remove info from string.
        /// <param name="source">Input string</param>
        /// <param name="from">Substring to remove "From"</param>
        /// <param name="to">Substring to remove "To"</param>
        /// <returns>Resulting string</returns>
        public static String RemoveInfo(String source, String from, String to = null) {
            String result = null;
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

        ///Test the chain of (sub)folder(s), create if necessary.
        /// <param name="folder">Folder's full path</param>
        public static void TestFolder(String folder) {
            String[] chunks = folder.Split(new char[] {'/'});
            String pathname = null;
            for (int n = 0; n < SIZE(chunks); n++) {
                pathname = CAT(pathname, chunks[n]);
                if (!Helper.DirExists(pathname))
                    Helper.CreateDir(pathname);
                pathname = CAT(pathname, "/");
            }
        }

        ///Test the chain of (sub)folder(s) and file, create if necessary.
        /// <param name="filename">Filename's full path</param>
        public static void TestFileFolder(String filename) {
            String[] chunks = filename.Split(new char[] {'/'});
            String pathname = null;
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
        public static String TransliterateRusToLat(String ru_text) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(ru_text);
            for (int n = 0; n < ru_chars.Length; n++)
                sb.Replace(ru_chars[n], en_chars[n]);
            return sb.ToString();
        }

        ///Call method of given class using provided arguments.
        /// <param name="class_name">Class name</param>
        /// <param name="method_name">Method name</param>
        /// <returns>Result of method execution</returns>
        public static Object CallStaticMethod(String class_name, String method_name) {
            return CallMethod(class_name, method_name, null); }

        ///Call static method of given class using provided arguments.
        /// <param name="class_name">Class name</param>
        /// <param name="method_name">Method name</param>
        /// <param name="args">List of arguments</param>
        /// <returns>Result of method execution</returns>
        public static Object CallStaticMethod(String class_name, String method_name, ArrayList args) {
            Type type = Type.GetType(class_name.Replace('/', '.'));
            System.Reflection.MethodInfo methodInfo = type.GetMethod(method_name);
            if (args != null && args.Count > 0)
                return methodInfo.Invoke(null, args.ToArray());
            else
                return methodInfo.Invoke(null, null);
        }

        ///Call method of given class using provided arguments.
        /// <param name="class_name">Class name</param>
        /// <param name="method_name">Method name</param>
        /// <param name="args">List of arguments</param>
        /// <returns>Result of method execution</returns>
        public static Object CallMethod(String class_name, String method_name, ArrayList args) {
            Type type = Type.GetType(class_name.Replace('/', '.'));
            System.Reflection.ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { });
            Object doObject = constructorInfo.Invoke(new Object[] { });

            Type[] types = args != null && args.Count > 0 ? new Type[args.Count] : new Type[0];
            if (types.Length > 0) {
                for (int n = 0; n < args.Count; n++)
                {
                    int result;
                    if (int.TryParse((String)args[n], out result))
                    {
                        types[n] = typeof(int);
                        args[n] = result;
                    }
                    else
                        types[n] = args[n].GetType();
                }
            }

            System.Reflection.MethodInfo methodInfo = type.GetMethod(method_name, types);
            if (methodInfo != null) {
                if (args != null && args.Count > 0)
                    return methodInfo.Invoke(doObject, args.ToArray());
                else
                    return methodInfo.Invoke(doObject, null);
            }
            else
                return null;
        }
    }
}