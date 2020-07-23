namespace Bula.Fetcher.Web\Testing {
    using System;

    using Bula.Fetcher;

    header ("Content-type: text/xml; charset=UTF-8");
    print Join("", Config.GetFile("local/tests/input/U.S. News - " . _GET["source"] . ".xml"));
}