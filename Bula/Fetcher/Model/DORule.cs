// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Model {
    using System;
    using System.Collections;

    using Bula.Model;

    /// <summary>
    /// Manipulating with rules.
    /// </summary>
    public class DORule : DOBase {
        /// Public constructor (overrides base constructor) 
        public DORule (): base() {
            this.tableName = "rules";
            this.idField = "i_RuleId";
        }
    }
}