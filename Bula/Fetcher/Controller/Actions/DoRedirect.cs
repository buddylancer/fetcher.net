namespace Bula.Fetcher.Controller.Actions {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;
    using System.Collections;
    using Bula.Fetcher.Controller;

    /// <summary>
    /// Base class for redirecting from the web-site.
    /// </summary>
    public class DoRedirect : Bula.Meta {
        public static void Execute(String link_to_redirect, String error_message) {
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

            Engine.Push(true);
            Response.Write(Engine.ShowTemplate(template_name, Prepare));
        }
    }
}