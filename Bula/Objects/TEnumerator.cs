// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    /// <summary>
    /// Very simple implementation of TEnumerator.
    /// </summary>
    public class TEnumerator : Bula.Meta {
        private Object[] collection = null;
        private int pointer = -1;

        private IEnumerator enumeration = null;
        private Object current = null;

        public TEnumerator  (Object[] elements) { this.collection = elements; }

        public TEnumerator (IEnumerator enumeration) { this.enumeration = enumeration; }
        public TEnumerator (ICollection collection) { this.enumeration = collection.GetEnumerator(); }

        public Boolean MoveNext() {
            if (this.enumeration != null) {
                if (this.enumeration.MoveNext()) {
                    this.current = this.enumeration.Current;
                    return true;
                }
                return false;
            }
            if (this.pointer < SIZE(this.collection) - 1) {
               this.current = this.collection[++this.pointer];
               return true;
            }
            return false;
        }

        public Object GetCurrent() {
            return this.current;
        }
    }
}