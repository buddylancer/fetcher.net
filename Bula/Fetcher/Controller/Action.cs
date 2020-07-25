namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;
    using System.Collections;
    using Bula.Model;

    /**
     * Logic for executing actions.
     */
    public class Action : Bula.Meta {
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

        public static void Execute() {
            if (actions_array == null)
                Initialize();

            Hashtable action_info = Request.TestPage(actions_array);

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

            String action_class = CAT("Bula/Fetcher/Controller/Actions/", action_info["page"]);
            Util.CallStaticMethod(action_class, "Execute");

            if (DBConfig.Connection != null) {
                DBConfig.Connection.Close();
                DBConfig.Connection = null;
            }
        }
    }
}