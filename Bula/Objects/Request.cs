// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;

    using Bula.Objects;
    using System.Collections;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper class for processing query/form request.
    /// </summary>
    public class Request : RequestBase {
        /// Internal storage for GET/POST variables 
        private static Hashtable Vars = null;
        /// Internal storage for SERVER variables 
        private static Hashtable ServerVars = null;

        static Request() { Initialize(); }

        /// Initialize internal variables for new request. 
        public static void Initialize() {
            Vars = Arrays.NewHashtable();
            ServerVars = Arrays.NewHashtable();
        }

        /// <summary>
        /// Get private variables.
        /// </summary>
        public static Hashtable GetPrivateVars() {
            return Vars;
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
            //return (String)(Vars.ContainsKey(name) ? Vars[name] : null);
            if (!Vars.ContainsKey(name))
                return null;
            var value = (String)Vars[name];
            if (NUL(value))
                value = "";
            return value;
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
            var httpReferer = GetVar(INPUT_SERVER, "HTTP_REFERER");
            if (httpReferer == null)
                return false;
            return httpReferer.IndexOf(text) != -1;
        }

        /// <summary>
        /// Check that request was originated from test script.
        /// </summary>
        /// <returns>True - from test script, False - from ordinary user agent.</returns>
        public static Boolean CheckTester() {
            var httpTester = GetVar(INPUT_SERVER, "HTTP_USER_AGENT");
            if (httpTester == null)
                return false;
            return httpTester.IndexOf("Wget") != -1;
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
        /// <param name="defaultPage">Default page to use for testing.</param>
        /// <returns>Resulting page parameters.</returns>
        public static Hashtable TestPage(Object[] pages, String defaultPage) {
            var pageInfo = new Hashtable();

            // Get page name
            var page = (String)null;
            pageInfo["from_get"] = 0;
            pageInfo["from_post"] = 0;

            var apiValue = GetVar(INPUT_GET, "api");
            if (apiValue != null) {
                if (EQ(apiValue, "rest")) // Only Rest for now
                    pageInfo["api"] = apiValue;
            }

            var pValue = GetVar(INPUT_GET, "p");
            if (pValue != null) {
                page = pValue;
                pageInfo["from_get"] = 1;
            }
            pValue = GetVar(INPUT_POST, "p");
            if (pValue != null) {
                page = pValue;
                pageInfo["from_post"] = 1;
            }
            if (page == null)
                page = defaultPage;

            pageInfo.Remove("page");
            for (int n = 0; n < SIZE(pages); n += 4) {
                if (EQ(pages[n], page)) {
                    pageInfo["page"] = pages[n + 0];
                    pageInfo["class"] = pages[n + 1];
                    pageInfo["post_required"] = pages[n + 2];
                    pageInfo["code_required"] = pages[n + 3];
                    break;
                }
            }
            return pageInfo;
        }

        /// <summary>
        /// Check whether text is ordinary name.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>True - text matches name, False - not matches.</returns>
        public static Boolean IsName(String input) {
            return Regex.IsMatch(input, "^[A-Za-z_]+[A-Za-z0-9_]*$");
        }

        /// <summary>
        /// Check whether text is domain name.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>True - text matches domain name, False - not matches.</returns>
        public static Boolean IsDomainName(String input) {
            return Regex.IsMatch(input, "^[A-Za-z]+[A-Za-z0-9\\.]*$");
        }

        /// <summary>
        /// Check whether text is positive integer.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>True - text matches, False - not matches.</returns>
        public static Boolean IsInteger(String input) {
            return Regex.IsMatch(input, "^[1-9]+[0-9]*$");
        }
    }
}