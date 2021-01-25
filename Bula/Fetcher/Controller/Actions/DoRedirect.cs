// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Actions {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;
    using System.Collections;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Base class for redirecting from the web-site.
    /// </summary>
    public abstract class DoRedirect : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public DoRedirect(Context context) : base(context) { }

        /// <summary>
        /// Execute main logic for this action.
        /// </summary>
        /// <param name="linkToRedirect">Link to redirect (or null if there were some errors).</param>
        /// <param name="errorMessage">Error to show (or null if no errors).</param>
        public void ExecuteRedirect(String linkToRedirect, String errorMessage) {
            var prepare = new Hashtable();
            var templateName = (String)null;
            if (!NUL(errorMessage)) {
                prepare["[#Title]"] = "Error";
                prepare["[#ErrMessage]"] = errorMessage;
                templateName = "Bula/Fetcher/View/error_alone.html";
            }
            else if (!BLANK(linkToRedirect)) {
                prepare["[#Link]"] = linkToRedirect;
                templateName = "Bula/Fetcher/View/redirect.html";
            }

            var engine = this.context.PushEngine(true);
            Response.Write(engine.ShowTemplate(templateName, prepare));
        }
    }
}