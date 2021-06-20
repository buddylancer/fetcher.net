// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    using Bula.Objects;

    /// <summary>
    /// Base helper class for processing query/form request.
    /// </summary>
    public class TRequestBase : Bula.Meta {
        /// Current Http request 
        public System.Web.HttpRequest HttpRequest = null;
        /// Current response 
        public TResponse response = null;

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

        public TRequestBase () { }

        public TRequestBase (Object currentRequest) {
            if (NUL(currentRequest))
                return;
            HttpRequest = (System.Web.HttpRequest)currentRequest;
        }

        /// <summary>
        /// Get all variables of given type.
        /// </summary>
        /// <param name="type">Required type.</param>
        /// <returns>Requested variables.</returns>
        public THashtable GetVars(int type) {
            THashtable hash = new THashtable();
            System.Collections.Specialized.NameValueCollection vars = null;
            switch (type) {
                case TRequest.INPUT_GET:
                default:
                    vars = HttpRequest.QueryString;
                    break;
                case TRequest.INPUT_POST:
                    vars = HttpRequest.Form;
                    break;
                case TRequest.INPUT_SERVER:
                    vars = HttpRequest.ServerVariables;
                    break;
            }
            IEnumerator keys = vars.AllKeys.GetEnumerator();
            for (int n = 0; n < vars.Count; n++) {
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
        public String GetVar(int type, String name) {
            var vars = GetVars(type);
            return (String)vars[name];
        }
    }

}