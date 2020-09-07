// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;

    using Bula;
    using Bula.Fetcher;
    using Bula.Objects;
    using System.Collections;
    using Bula.Model;

    /// <summary>
    /// Logic for executing actions.
    /// </summary>
    public class Action : Page {
        private static Object[] actionsArray = null;

        private static void Initialize() {
            actionsArray = ARR(
            //action name            page                   post      code
            "do_redirect_item",     "DoRedirectItem",       0,        0,
            "do_redirect_source",   "DoRedirectSource",     0,        0,
            "do_clean_cache",       "DoCleanCache",         0,        1,
            "do_test_items",        "DoTestItems",          0,        1
            );
        }

        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public Action(Context context) : base(context) { }

        /// Execute main logic for required action. 
        public override void Execute() {
            if (actionsArray == null)
                Initialize();

            var actionInfo = Request.TestPage(actionsArray);

            // Test action name
            if (!actionInfo.ContainsKey("page"))
                Response.End("Error in parameters -- no page");

            // Test action context
            if (INT(actionInfo["post_required"]) == 1 && INT(actionInfo["from_post"]) == 0)
                Response.End("Error in parameters -- inconsistent pars");

            Request.Initialize();
            if (INT(actionInfo["post_required"]) == 1)
                Request.ExtractPostVars();
            else
                Request.ExtractAllVars();

            //TODO!!!
            //if (!Request.CheckReferer(Config.Site))
            //    err404();

            if (INT(actionInfo["code_required"]) == 1) {
                if (!Request.Contains("code") || !EQ(Request.Get("code"), Config.SECURITY_CODE)) //TODO -- hardcoded!!!
                    Response.End("No access.");
            }

            var actionClass = CAT("Bula/Fetcher/Controller/Actions/", actionInfo["class"]);
            ArrayList args0 = new ArrayList(); args0.Add(this.context);
            Internal.CallMethod(actionClass, args0, "Execute", null);

            if (DBConfig.Connection != null) {
                DBConfig.Connection.Close();
                DBConfig.Connection = null;
            }
        }
    }
}