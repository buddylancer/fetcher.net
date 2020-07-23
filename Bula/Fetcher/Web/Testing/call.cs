namespace Bula.Fetcher.Web\Testing {
    using System;

    use Bula.Fetcher.Controller.Testing\CallMethod;

    error_reporting(E_ALL);
    date_default_timezone_set("UTC");
    CallMethod.Execute();
}