using System;
using System.Web;
using Bula.Fetcher.Controller;

public partial class _Index : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        (new Index(new Bula.Fetcher.Context())).Execute();
    }
}