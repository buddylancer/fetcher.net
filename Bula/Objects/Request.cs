namespace Bula.Objects {
    using System;

    using Bula.Objects;
    using System.Collections;
    using System.Text.RegularExpressions;

    public class Request : Bula.Meta {
        private static Hashtable Vars = null;
        private static Hashtable ServerVars = null;
        public const int INPUT_POST = 0;
        public const int INPUT_GET = 1;
        public const int INPUT_COOKIE = 2;
        public const int INPUT_ENV = 4;
        public const int INPUT_SERVER = 5;

        static Request() { Initialize(); }

        public static void Initialize() {
            Vars = Arrays.NewHashtable();
            ServerVars = Arrays.NewHashtable();
        }

        public static Boolean Contains(String name) {
            return Vars.ContainsKey(name);
        }

        public static String Get(String name) {
            return (String)(Vars.ContainsKey(name) ? Vars[name] : null);
        }

        public static void Set(String name, String value) {
            Vars[name] = value;
        }

        public static IEnumerator GetKeys() {
            return Vars.Keys.GetEnumerator();
        }

        public static void ExtractPostVars() {
            var vars = GetVars(INPUT_POST);
            Vars = Arrays.MergeHashtable(Vars, vars);
        }

        public static void ExtractServerVars() {
            var vars = GetVars(INPUT_SERVER);
            Vars = Arrays.MergeHashtable(ServerVars, vars);
        }

        public static void ExtractAllVars() {
            var vars = GetVars(INPUT_GET);
            Vars = Arrays.MergeHashtable(Vars, vars);
            ExtractPostVars();
        }

        public static Boolean CheckReferer(String text) {
            //return true; //TODO
            var http_referer = GetVar(INPUT_SERVER, "HTTP_REFERER");
            if (http_referer == null)
                return false;
            return http_referer.IndexOf(text) != -1;
        }

        public static Boolean CheckTester() {
            var http_tester = GetVar(INPUT_SERVER, "HTTP_USER_AGENT");
            if (http_tester == null)
                return false;
            return http_tester.IndexOf("Wget") != -1;
        }

        public static String GetRequiredParameter(String name) {
            var val = (String)null;
            if (Contains(name))
                val = Get(name);
            else
                STOP(CAT("Parameter '", name, "' is required!"));
            return val;
        }

        public static String GetOptionalParameter(String name) {
            var val = (String)null;
            if (Contains(name))
                val = Get(name);
            return val;
        }

        public static int GetRequiredInteger(String name) {
            var str = GetRequiredParameter(name);
            if (str == "" || !IsInteger(str))
                STOP(CAT("Error in parameter '", name, "'!"));
            return INT(str);
        }

        public static int GetOptionalInteger(String name) {
            var val = GetOptionalParameter(name);
            if (val == null)
                return -99999; //TODO

            var str = STR(val);
            if (str == "" || !IsInteger(str))
                STOP(CAT("Error in parameter '", name, "'!"));
            return INT(val);
        }

        public static String GetRequiredString(String name) {
            var val = GetRequiredParameter(name);
            return val;
        }

        public static String GetOptionalString(String name) {
            var val = GetOptionalParameter(name);
            return val;
        }

        public static Hashtable TestPage(Object[] pages) {
            return TestPage(pages, null); }

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

        public static Boolean IsDomainName(String input) {
            return Regex.IsMatch(input, "^[A-Za-z0-9\\.]+$");
        }

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
            //while (keys.MoveNext()) {
            //    String key = (String)keys.Current;
            //    if (key != null)
            //        hash.Add(key, vars[key]);
            //}
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