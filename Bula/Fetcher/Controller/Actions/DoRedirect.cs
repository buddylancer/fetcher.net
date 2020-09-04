// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
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
        /// <param name="link_to_redirect">Link to redirect (or null if there were some errors).</param>
        /// <param name="error_message">Error to show (or null if no errors).</param>
        public void ExecuteRedirect(String link_to_redirect, String error_message) {
            var Prepare = new Hashtable();
            var template_name = (String)null;
            if (!NUL(error_message)) {
                Prepare["[#Title]"] = "Error";
                Prepare["[#ErrMessage]"] = error_message;
                template_name = "Bula/Fetcher/View/error_alone.html";
            }
            else if (!BLANK(link_to_redirect)) {
                Prepare["[#Link]"] = link_to_redirect;
                template_name = "Bula/Fetcher/View/redirect.html";
            }

            var engine = this.context.PushEngine(true);
            Response.Write(engine.ShowTemplate(template_name, Prepare));
        }
    }
}