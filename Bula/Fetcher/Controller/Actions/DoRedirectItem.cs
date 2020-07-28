namespace Bula.Fetcher.Controller.Actions {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using Bula.Objects;
    using Bula.Model;
    using Bula.Fetcher.Model;

    /// <summary>
    /// Redirecting to the external item.
    /// </summary>
    public class DoRedirectItem : Bula.Meta {
        public static void Execute() {
            var error_message = (String)null;
            var link_to_redirect = (String)null;
            if (!Request.Contains("id"))
                error_message = "Item ID is required!";
            else {
                var id = Request.Get("id");
                if (!Request.IsInteger(id) || INT(id) <= 0)
                    error_message = "Incorrect item ID!";
                else {
                    var doItem = new DOItem();
                    var dsItems = doItem.GetById(INT(id));
                    if (dsItems.GetSize() == 0)
                        error_message = "No item with such ID!";
                    else {
                        var oItem = dsItems.GetRow(0);
                        link_to_redirect = STR(oItem["s_Link"]);
                    }
                }
            }
            DoRedirect.Execute(link_to_redirect, error_message);
        }
    }
}