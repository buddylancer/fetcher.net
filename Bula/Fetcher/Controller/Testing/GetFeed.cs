// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Testing {
    using System;

    using Bula.Objects;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Logic for getting test feed.
    /// </summary>
    public class GetFeed : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public GetFeed(Context context) : base(context) { }

        /// Get test feed using parameters from request. 
        public override void Execute() {
            Request.Initialize();
            Request.ExtractAllVars();

            // Check source
            if (!Request.Contains("source")) {
                Response.End("Source is required!");
                return;
            }
            var source = Request.Get("source");
            if (BLANK(source)) {
                Response.End("Empty source!");
                return;
            }

            Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
            Response.Write(Helper.ReadAllText(CAT(this.context.LocalRoot, "local/tests/input/U.S. News - ", source, ".xml")));
            Response.End("");
        }
    }
}