// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Testing {
    using System;
    using System.Collections;

    using Bula.Fetcher;
    using Bula.Fetcher.Controller;
    using Bula.Objects;

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
            //this.context.Request.Initialize();
            this.context.Request.ExtractAllVars();

            // Check source
            if (!this.context.Request.Contains("source")) {
                this.context.Response.End("Source is required!");
                return;
            }
            var source = this.context.Request["source"];
            if (BLANK(source)) {
                this.context.Response.End("Empty source!");
                return;
            }

            this.context.Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
            this.context.Response.Write(Helper.ReadAllText(CAT(this.context.LocalRoot, "local/tests/input/U.S. News - ", source, ".xml"), "UTF-8"));
            this.context.Response.End();
        }
    }
}