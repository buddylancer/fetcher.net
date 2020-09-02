// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Web\Testing {
    using System;

    using Bula.Objects;
    header ("Content-type: text/xml; charset=UTF-8");
    print Helper.ReadAllText("../local/tests/input/U.S. News - " . _GET["source"] . ".xml");
}