using System;
using System.Web;
using Bula.Fetcher.Controller;

public partial class _Rss : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Bula.Fetcher.Common.Initialize();
        Rss.Execute();
    }
}