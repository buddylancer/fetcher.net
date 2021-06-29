// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    /// <summary>
    /// Implementation of DB NULL object.
    /// </summary>
    public class TNull : Bula.Meta {
        private static TNull value;

        private TNull () {
            value = null;
        }

        /// <summary>
        /// Get NULL value.
        /// </summary>
        /// <returns>NULL value.</returns>
        public static TNull GetValue() {
            if (value == null)
                value = new TNull();
            return value;
        }
    }
}