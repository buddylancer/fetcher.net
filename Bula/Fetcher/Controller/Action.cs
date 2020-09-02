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
        private static Object[] actions_array = null;

        private static void Initialize() {
            actions_array = ARR(
            //action name            page                   post      code
            "do_redirect_item",     "DoRedirectItem",       0,        0,
            "do_redirect_source",   "DoRedirectSource",     0,        0,
            "do_clean_cache",       "DoCleanCache",         0,        1,
            "do_test_items",        "DoTestItems",          0,        1
            );
        }

        public Action(Context context) : base(context) { }

        /// Execute main logic for required action. 
        public override void Execute() {
            if (actions_array == null)
                Initialize();

            var action_info = Request.TestPage(actions_array);

            // Test action name
            if (!action_info.ContainsKey("page"))
                Response.End("Error in parameters -- no page");

            // Test action context
            if (INT(action_info["post_required"]) == 1 && INT(action_info["from_post"]) == 0)
                Response.End("Error in parameters -- inconsistent pars");

            Request.Initialize();
            if (INT(action_info["post_required"]) == 1)
                Request.ExtractPostVars();
            else
                Request.ExtractAllVars();

            //TODO!!!
            //if (!Request.CheckReferer(Config.Site))
            //    err404();

            if (INT(action_info["code_required"]) == 1) {
                if (!Request.Contains("code") || !EQ(Request.Get("code"), Config.SECURITY_CODE)) //TODO -- hardcoded!!!
                    Response.End("No access.");
            }

            var action_class = CAT("Bula/Fetcher/Controller/Actions/", action_info["class"]);
            //Internal.CallStaticMethod(action_class, "Execute");
            var args0 = new ArrayList(); args0.Add(this.context);
            Internal.CallMethod(action_class, args0, "Execute", null);

            if (DBConfig.Connection != null) {
                DBConfig.Connection.Close();
                DBConfig.Connection = null;
            }
        }
    }
}