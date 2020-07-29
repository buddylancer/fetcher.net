namespace Bula.Objects {
    using System;

    using Bula.Objects;
    using System.Collections;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper class for processing query/form request.
    /// </summary>
    public class Request : Bula.Meta {
        /// Internal storage for GET/POST variables 
        private static Hashtable Vars = null;
        /// Internal storage for SERVER variables 
        private static Hashtable ServerVars = null;

        /// Enum value (type) for getting POST parameters 
        public const int INPUT_POST = 0;
        /// Enum value (type) for getting GET parameters 
        public const int INPUT_GET = 1;
        /// Enum value (type) for getting COOKIE parameters 
        public const int INPUT_COOKIE = 2;
        /// Enum value (type) for getting ENV parameters 
        public const int INPUT_ENV = 4;
        /// Enum value (type) for getting SERVER parameters 
        public const int INPUT_SERVER = 5;

        static Request() { Initialize(); }

        /// <summary>
        /// Initialize internal variables for new request.
        /// </summary>
        public static void Initialize() {
            Vars = Arrays.NewHashtable();
            ServerVars = Arrays.NewHashtable();
        }

        /// <summary>
        /// Check whether request contains variable.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <returns>True - variable exists, False - not exists.</returns>
        public static Boolean Contains(String name) {
            return Vars.ContainsKey(name);
        }

        /// <summary>
        /// Get variable from internal storage.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <returns>Variable value.</returns>
        public static String Get(String name) {
            return (String)(Vars.ContainsKey(name) ? Vars[name] : null);
        }

        /// <summary>
        /// Set variable into internal storage.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="value">Variable value.</param>
        public static void Set(String name, String value) {
            Vars[name] = value;
        }

        /// <summary>
        /// Get all variable keys from request.
        /// </summary>
        /// <returns>All keys enumeration.</returns>
        public static IEnumerator GetKeys() {
            return Vars.Keys.GetEnumerator();
        }

        /// Extract all POST variables into internal variables. 
        public static void ExtractPostVars() {
            var vars = GetVars(INPUT_POST);
            Vars = Arrays.MergeHashtable(Vars, vars);
        }

        /// Extract all SERVER variables into internal storage. 
        public static void ExtractServerVars() {
            var vars = GetVars(INPUT_SERVER);
            Vars = Arrays.MergeHashtable(ServerVars, vars);
        }

        /// Extract all GET and POST variables into internal storage. 
        public static void ExtractAllVars() {
            var vars = GetVars(INPUT_GET);
            Vars = Arrays.MergeHashtable(Vars, vars);
            ExtractPostVars();
        }

        /// <summary>
        /// Check that referer contains text.
        /// </summary>
        /// <param name="text">Text to check.</param>
        /// <returns>True - referer contains provided text, False - not contains.</returns>
        public static Boolean CheckReferer(String text) {
            //return true; //TODO
            var http_referer = GetVar(INPUT_SERVER, "HTTP_REFERER");
            if (http_referer == null)
                return false;
            return http_referer.IndexOf(text) != -1;
        }

        /// <summary>
        /// Check that request was originated from test script.
        /// </summary>
        /// <returns>True - from test script, False - from ordinary user agent.</returns>
        public static Boolean CheckTester() {
            var http_tester = GetVar(INPUT_SERVER, "HTTP_USER_AGENT");
            if (http_tester == null)
                return false;
            return http_tester.IndexOf("Wget") != -1;
        }

        /// <summary>
        /// Get required parameter by name (or stop execution).
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Resulting value.</returns>
        public static String GetRequiredParameter(String name) {
            var val = (String)null;
            if (Contains(name))
                val = Get(name);
            else
                STOP(CAT("Parameter '", name, "' is required!"));
            return val;
        }

        /// <summary>
        /// Get optional parameter by name.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Resulting value or null.</returns>
        public static String GetOptionalParameter(String name) {
            var val = (String)null;
            if (Contains(name))
                val = Get(name);
            return val;
        }

        /// <summary>
        /// Get required integer parameter by name (or stop execution).
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Resulting value.</returns>
        public static int GetRequiredInteger(String name) {
            var str = GetRequiredParameter(name);
            if (str == "" || !IsInteger(str))
                STOP(CAT("Error in parameter '", name, "'!"));
            return INT(str);
        }

        /// <summary>
        /// Get optional integer parameter by name.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Resulting value or null.</returns>
        public static int GetOptionalInteger(String name) {
            var val = GetOptionalParameter(name);
            if (val == null)
                return -99999; //TODO

            var str = STR(val);
            if (str == "" || !IsInteger(str))
                STOP(CAT("Error in parameter '", name, "'!"));
            return INT(val);
        }

        /// <summary>
        /// Get required string parameter by name (or stop execution).
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Resulting value.</returns>
        public static String GetRequiredString(String name) {
            var val = GetRequiredParameter(name);
            return val;
        }

        /// <summary>
        /// Get optional string parameter by name.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Resulting value or null.</returns>
        public static String GetOptionalString(String name) {
            var val = GetOptionalParameter(name);
            return val;
        }

        /// <summary>
        /// Test (match) a page request with array of allowed pages.
        /// </summary>
        /// <param name="pages">Array of allowed pages (and their parameters).</param>
        /// <returns>Resulting page parameters.</returns>
        public static Hashtable TestPage(Object[] pages) {
            return TestPage(pages, null); }

        /// <summary>
        /// Test (match) a page request with array of allowed pages.
        /// </summary>
        /// <param name="pages">Array of allowed pages (and their parameters).</param>
        /// <param name="default_page">Default page to use for testing.</param>
        /// <returns>Resulting page parameters.</returns>
        public static Hashtable TestPage(Object[] pages, String default_page) {
            var page_info = new Hashtable();

            // Get page name
            var page = (String)null;
            page_info["from_get"] = 0;
            page_info["from_post"] = 0;
            var p_value = GetVar(INPUT_GET, "p");
            if (p_value != null) {
                page = p_value;
                page_info["from_get"] = 1;
            }
            p_value = GetVar(INPUT_POST, "p");
            if (p_value != null) {
                page = p_value;
                page_info["from_post"] = 1;
            }
            if (page == null)
                page = default_page;

            page_info.Remove("page");
            for (int n = 0; n < SIZE(pages); n += 4) {
                if (EQ(pages[n], page)) {
                    page_info["page"] = pages[n+1];
                    page_info["post_required"] = pages[n+2];
                    page_info["code_required"] = pages[n+3];
                    break;
                }
            }
            return page_info;
        }

        /// <summary>
        /// Check whether text is domain name.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>True - text matches domain name, False - not matches.</returns>
        public static Boolean IsDomainName(String input) {
            return Regex.IsMatch(input, "^[A-Za-z0-9\\.]+$");
        }

        /// <summary>
        /// Check whether text is positive integer.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>True - text matches, False - not matches.</returns>
        public static Boolean IsInteger(String input) {
            return Regex.IsMatch(input, "^[1-9]+[0-9]*$");
        }

        public static Hashtable GetVars(int type) {
            Hashtable hash = new Hashtable();
            System.Collections.Specialized.NameValueCollection vars = null;
            switch (type)
            {
                case Request.INPUT_GET:
                default:
                    vars = System.Web.HttpContext.Current.Request.QueryString;
                    break;
                case Request.INPUT_POST:
                    vars = System.Web.HttpContext.Current.Request.Form;
                    break;
                case Request.INPUT_SERVER:
                    vars = System.Web.HttpContext.Current.Request.ServerVariables;
                    break;
            }
            IEnumerator keys = vars.AllKeys.GetEnumerator();
            for (int n = 0; n < vars.Count; n++)
            {
                String key = vars.GetKey(n);
                String[] values = vars.GetValues(n);
                if (key == null) {
                    for (int v = 0; v < values.Length; v++)
                        hash.Add(values[v], null);
                }
                else
                    hash.Add(key, values[0]);
            }
            return hash;
        }

        /// <summary>
        /// Get a single variable of given type.
        /// </summary>
        /// <param name="type">Required type.</param>
        /// <param name="name">Variable name.</param>
        /// <returns>Requested variable.</returns>

        public static String GetVar(int type, String name) {
            System.Collections.Specialized.NameValueCollection vars = null;
            switch (type)
            {
                case Request.INPUT_GET:
                default:
                    vars = System.Web.HttpContext.Current.Request.QueryString;
                    break;
                case Request.INPUT_POST:
                    vars = System.Web.HttpContext.Current.Request.Form;
                    break;
                case Request.INPUT_SERVER:
                    vars = System.Web.HttpContext.Current.Request.ServerVariables;
                    break;
            }
            return vars[name];    
        }
    }
}