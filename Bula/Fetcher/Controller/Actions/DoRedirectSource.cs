// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Actions {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;
    using System.Collections;
    using Bula.Fetcher.Model;

    /// <summary>
    /// Redirection to external source.
    /// </summary>
    public class DoRedirectSource : DoRedirect {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public DoRedirectSource(Context context) : base(context) { }

        /// Execute main logic for DoRedirectSource action 
        public override void Execute() {
            var error_message = (String)null;
            var link_to_redirect = (String)null;
            if (!Request.Contains("source"))
                error_message = "Source name is required!";
            else {
                var source_name = Request.Get("source");
                if (!Request.IsDomainName(source_name))
                    error_message = "Incorrect source name!";
                else {
                    var doSource = new DOSource();
                    Hashtable[] oSource =
                        {new Hashtable()};
                    if (!doSource.CheckSourceName(source_name, oSource))
                        error_message = "No such source name!";
                    else
                        link_to_redirect = STR(oSource[0]["s_External"]);
                }
            }
            this.ExecuteRedirect(link_to_redirect, error_message);
        }
    }
}