namespace Bula.Fetcher.Controller.Actions {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using Bula.Objects;
    using Bula.Model;
    using Bula.Fetcher.Model;

    /**
     * Redirecting to the external item.
     */
    public class DoRedirectItem : Bula.Meta {
        public static void Execute() {
            String error_message = null;
            String link_to_redirect = null;
            if (!Request.Contains("id"))
                error_message = "Item ID is required!";
            else {
                String id = Request.Get("id");
                if (!Request.IsInteger(id) || INT(id) <= 0)
                    error_message = "Incorrect item ID!";
                else {
                    DOItem doItem = new DOItem();
                    DataSet dsItems = doItem.GetById(INT(id));
                    if (dsItems.GetSize() == 0)
                        error_message = "No item with such ID!";
                    else {
                        Hashtable oItem = dsItems.GetRow(0);
                        link_to_redirect = (String) STR(oItem["s_Link"]);
                    }
                }
            }
            DoRedirect.Execute(link_to_redirect, error_message);
        }
    }
}