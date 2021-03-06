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
    /// Helper class for processing query/form request.
    /// </summary>
    public class TRequest : TRequestBase {
        /// Internal storage for GET/POST variables 
        private THashtable Vars = null;
        /// Internal storage for SERVER variables 
        private THashtable ServerVars = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="currentRequest">Current http request object.</param>
        public TRequest(Object currentRequest) : base(currentRequest) { Initialize(); }

        /// Initialize internal variables for new request. 
        private void Initialize() {
            this.Vars = THashtable.Create();
            this.ServerVars = THashtable.Create();
        }

        /// <summary>
        /// Get private variables.
        /// </summary>
        public THashtable GetPrivateVars() {
            return this.Vars;
        }

        /// <summary>
        /// Check whether request contains variable.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <returns>True - variable exists, False - not exists.</returns>
        public Boolean Contains(String name) {
            return this.Vars.ContainsKey(name);
        }

        /// <summary>
        /// Get variable from internal storage.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <returns>Variable value.</returns>
        public String Get(String name) {
            //return (String)(Vars.ContainsKey(name) ? Vars[name] : null);
            if (!this.Vars.ContainsKey(name))
                return null;
            var value = (String)this.Vars[name];
            if (NUL(value))
                value = "";
            return value;
        }

        public String this[String name] {
            get { return Get(name); }
            set { Set(name, value); }
        }

        /// <summary>
        /// Set variable into internal storage.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="value">Variable value.</param>
        public void Set(String name, String value) {
            this.Vars[name] = value;
        }

        /// <summary>
        /// Get all variable keys from request.
        /// </summary>
        /// <returns>All keys enumeration.</returns>
        public TEnumerator GetKeys() {
            return new TEnumerator(this.Vars.Keys.GetEnumerator());
        }

        /// Extract all POST variables into internal variables. 
        public void ExtractPostVars() {
            var vars = this.GetVars(INPUT_POST);
            this.Vars = this.Vars.Merge(vars);
        }

        /// Extract all SERVER variables into internal storage. 
        public void ExtractServerVars() {
            var vars = this.GetVars(INPUT_SERVER);
            this.Vars = this.ServerVars.Merge(vars);
        }

        /// Extract all GET and POST variables into internal storage. 
        public void ExtractAllVars() {
            var vars = this.GetVars(INPUT_GET);
            this.Vars = this.Vars.Merge(vars);
            this.ExtractPostVars();
        }

        /// <summary>
        /// Check that referer contains text.
        /// </summary>
        /// <param name="text">Text to check.</param>
        /// <returns>True - referer contains provided text, False - not contains.</returns>
        public Boolean CheckReferer(String text) {
            //return true; //TODO
            var httpReferer = this.GetVar(INPUT_SERVER, "HTTP_REFERER");
            if (httpReferer == null)
                return false;
            return httpReferer.IndexOf(text) != -1;
        }

        /// <summary>
        /// Check that request was originated from test script.
        /// </summary>
        /// <returns>True - from test script, False - from ordinary user agent.</returns>
        public Boolean CheckTester() {
            var httpTester = this.GetVar(INPUT_SERVER, "HTTP_USER_AGENT");
            if (httpTester == null)
                return false;
            return httpTester.IndexOf("Wget") != -1;
        }

        /// <summary>
        /// Get required parameter by name (or stop execution).
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Resulting value.</returns>
        public String GetRequiredParameter(String name) {
            var val = (String)null;
            if (this.Contains(name))
                val = this[name];
            else {
                var error = CAT("Parameter '", name, "' is required!");
                this.response.End(error);
            }
            return val;
        }

        /// <summary>
        /// Get optional parameter by name.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Resulting value or null.</returns>
        public String GetOptionalParameter(String name) {
            var val = (String)null;
            if (this.Contains(name))
                val = this[name];
            return val;
        }

        /// <summary>
        /// Get required integer parameter by name (or stop execution).
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Resulting value.</returns>
        public int GetRequiredInteger(String name) {
            var str = this.GetRequiredParameter(name);
            if (str == "" || !IsInteger(str)) {
                var error = CAT("Error in parameter '", name, "'!");
                this.response.End(error);
            }
            return INT(str);
        }

        /// <summary>
        /// Get optional integer parameter by name.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Resulting value or null.</returns>
        public int GetOptionalInteger(String name) {
            var val = this.GetOptionalParameter(name);
            if (val == null)
                return -99999; //TODO

            var str = STR(val);
            if (str == "" || !IsInteger(str)) {
                var error = CAT("Error in parameter '", name, "'!");
                this.response.End(error);
            }
            return INT(val);
        }

        /// <summary>
        /// Get required string parameter by name (or stop execution).
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Resulting value.</returns>
        public String GetRequiredString(String name) {
            var val = this.GetRequiredParameter(name);
            return val;
        }

        /// <summary>
        /// Get optional string parameter by name.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Resulting value or null.</returns>
        public String GetOptionalString(String name) {
            var val = this.GetOptionalParameter(name);
            return val;
        }

        /// <summary>
        /// Test (match) a page request with array of allowed pages.
        /// </summary>
        /// <param name="pages">Array of allowed pages (and their parameters).</param>
        /// <returns>Resulting page parameters.</returns>
        public THashtable TestPage(Object[] pages) {
            return TestPage(pages, null); }

        /// <summary>
        /// Test (match) a page request with array of allowed pages.
        /// </summary>
        /// <param name="pages">Array of allowed pages (and their parameters).</param>
        /// <param name="defaultPage">Default page to use for testing.</param>
        /// <returns>Resulting page parameters.</returns>
        public THashtable TestPage(Object[] pages, String defaultPage) {
            var pageInfo = new THashtable();

            // Get page name
            var page = (String)null;
            pageInfo["from_get"] = 0;
            pageInfo["from_post"] = 0;

            var apiValue = this.GetVar(INPUT_GET, "api");
            if (apiValue != null) {
                if (EQ(apiValue, "rest")) // Only Rest for now
                    pageInfo["api"] = apiValue;
            }

            var pValue = this.GetVar(INPUT_GET, "p");
            if (pValue != null) {
                page = pValue;
                pageInfo["from_get"] = 1;
            }
            pValue = this.GetVar(INPUT_POST, "p");
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
            return Regex.IsMatch(input, "^[A-Za-z0-9]+[A-Za-z0-9\\.\\-]*$");
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