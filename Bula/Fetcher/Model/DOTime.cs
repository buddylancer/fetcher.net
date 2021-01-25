// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Model {
    using System;

    using Bula.Model;

    /// <summary>
    /// Manipulating with times.
    /// </summary>
    public class DOTime : DOBase {
        /// Public constructor (overrides base constructor) 
        public DOTime (): base() {
            this.tableName = "as_of_time";
            this.idField = "i_Id";
        }
    }
}