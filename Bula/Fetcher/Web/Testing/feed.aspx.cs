using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bula.Objects;

public partial class _Feed : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Bula.Fetcher.Context context = new Bula.Fetcher.Context();

        Bula.Objects.Request.Initialize();
        Bula.Objects.Request.ExtractAllVars();

        string source = Bula.Objects.Request.Get("source");
        Bula.Objects.Response.WriteHeader("Content-type", "text/xml; charset=UTF-8");
        string xml_content = Helper.ReadAllText(Strings.Concat(context.LocalRoot, "local/tests/input/U.S. News - ", source, ".xml"));
        Bula.Objects.Response.Write(xml_content);
    }
}