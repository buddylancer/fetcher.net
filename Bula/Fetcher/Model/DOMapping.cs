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
    public class DOMapping : DOBase {
        /// Public constructor (overrides base constructor) 
        public DOMapping (): base() {
            this.tableName = "mappings";
            this.idField = "i_MappingId";
        }
    }
}