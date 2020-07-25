using System;
using System.Web;
using Bula.Fetcher.Controller.Testing;

public partial class _CallMethod : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Bula.Fetcher.Common.Initialize();
        CallMethod.Execute();
    }
}