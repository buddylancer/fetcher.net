namespace Bula.Fetcher.Controller.Actions {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;
    using System.Collections;
    using Bula.Fetcher.Model;

    /**
     * Redirection to external source.
     */
    public class DoRedirectSource : Bula.Meta {
        public static void Execute() {
            String error_message = null;
            String link_to_redirect = null;
            if (!Request.Contains("source"))
                error_message = "Source name is required!";
            else {
                String source_name = Request.Get("source");
                if (!Request.IsDomainName(source_name))
                    error_message = "Incorrect source name!";
                else {
                    DOSource doSource = new DOSource();
                    Hashtable[] oSource =
                        {new Hashtable()};
                    if (!doSource.CheckSourceName(source_name, oSource))
                        error_message = "No such source name!";
                    else
                        link_to_redirect = STR(oSource[0]["s_External"]);
                }
            }

            DoRedirect.Execute(link_to_redirect, error_message);
        }
    }
}