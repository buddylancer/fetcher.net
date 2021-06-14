// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Actions {
    using System;
    using System.Collections;

    using Bula.Fetcher;
    using Bula.Objects;
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
            var errorMessage = (String)null;
            var linkToRedirect = (String)null;
            if (!this.context.Request.Contains("source"))
                errorMessage = "Source name is required!";
            else {
                var sourceName = this.context.Request["source"];
                if (!Request.IsDomainName(sourceName))
                    errorMessage = "Incorrect source name!";
                else {
                    var doSource = new DOSource();
                    Hashtable[] oSource =
                        {new Hashtable()};
                    if (!doSource.CheckSourceName(sourceName, oSource))
                        errorMessage = "No such source name!";
                    else
                        linkToRedirect = STR(oSource[0]["s_External"]);
                }
            }
            this.ExecuteRedirect(linkToRedirect, errorMessage);
        }
    }
}