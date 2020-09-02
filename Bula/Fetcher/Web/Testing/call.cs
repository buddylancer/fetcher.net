// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Web\Testing {
    using System;

    use Bula.Fetcher.Controller.Testing\CallMethod;

    error_reporting(E_ALL);
    date_default_timezone_set("UTC");
    CallMethod.Execute();
}