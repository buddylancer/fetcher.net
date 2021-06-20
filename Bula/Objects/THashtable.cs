// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    using Bula.Objects;

    /// <summary>
    /// Straight-forward implementation of Java THashtable object.
    /// </summary>
    public class THashtable : THashtableBase {
        public THashtable () {
        }

        /// <summary>
        /// Create new hash table.
        /// </summary>
        /// <returns>New hash table.</returns>
        public static THashtable Create() {
            return new THashtable();
        }

        /// <summary>
        /// Merge hash tables.
        /// </summary>
        /// <param name="extra">Hash table to merge with original one.</param>
        /// <returns>Merged hash table.</returns>
        public THashtable Merge(THashtable extra) {
            if (extra == null)
                return this;

            var output = Create();

            TEnumerator keys1 = new TEnumerator(this.Keys.GetEnumerator());
            while (keys1.MoveNext()) {
                String key1 = (String)keys1.GetCurrent();
                output[key1] = this[key1];
            }

            TEnumerator keys2 = new TEnumerator(extra.Keys.GetEnumerator());
            while (keys2.MoveNext()) {
                String key2 = (String)keys2.GetCurrent();
                output[key2] = extra[key2];
            }
            return output;
        }

    }
}