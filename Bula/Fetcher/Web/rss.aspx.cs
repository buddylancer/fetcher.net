using System;
using System.Web;
using Bula.Fetcher.Controller;

public partial class _Rss : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        (new Rss(new Bula.Fetcher.Context())).Execute();
    }
}