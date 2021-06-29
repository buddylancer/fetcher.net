// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    /// <summary>
    /// Straight-forward implementation of ArrayList.
    /// </summary>
    public class TArrayList : TArrayListBase {
        /// <summary>
        /// Public constructors.
        /// </summary>
        public TArrayList() : base() { }
        public TArrayList(Object[] items) : base(items) { }

        /// Create new array list. 
        public static TArrayList Create() {
            return new TArrayList();
        }

        /// <summary>
        /// Add multiple objects.
        /// </summary>
        /// <param name="inputs">Array of objects.</param>
        /// <returns>Number of added objects,</returns>
        public int AddAll(Object[] inputs) {
            var counter = 0;
            foreach (Object input in inputs) {
                this.Add(input);
                counter++;
            }
            return counter;
        }

        /// <summary>
        /// Create array list from array of objects.
        /// </summary>
        /// <param name="input">Array of objects.</param>
        /// <returns>Resulting array list.</returns>
        public static TArrayList CreateFrom(Object[] input) {
            if (input == null)
                return null;
            var output = Create();
            if (SIZE(input) == 0)
                return output;
            foreach (Object obj in input)
                output.Add(obj);
            return output;
        }

        /// <summary>
        /// Merge array lists.
        /// </summary>
        /// <param name="input">Original array list.</param>
        /// <param name="extra">Array list to merge with original one.</param>
        /// <returns>Resulting array list.</returns>
        public TArrayList Merge(TArrayList extra) {
            var output = Create();
            for (int n1 = 0; n1 < this.Size(); n1++)
                output.Add(this[n1]);
            if (extra == null)
                return output;
            for (int n2 = 0; n2 < extra.Size(); n2++)
                output.Add(extra[n2]);
            return output;
        }

    }
}