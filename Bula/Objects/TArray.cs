// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    /// <summary>
    /// Straight-forward implementation of Array.
    /// </summary>
    public class TArray : Bula.Meta {
        Object[] content;

        /// Default constructor. 
        public TArray (int size) {
            this.Instantiate(size);
        }

        private void Instantiate(int size) {
            content = new Object[size];
        }

        public int Size() {
            return content.Length;
        }

        public Boolean Set(int pos, Object value) {
            if (pos >= this.Size())
                return false;
            content[pos] = value;
            return true;
        }

        public Object Get(int pos) {
            if (pos >= this.Size())
                return false;
            return content[pos];
        }

        public void Add(Object value) {
            var cloned = this.Clone();
            this.Instantiate(this.Size() + 1);
            for (int n = 0; n < cloned.Size(); n++)
                this[n] = cloned[n];
            this[cloned.Size() + 1] = value;
        }

        public TArray Clone() {
            var cloned = new TArray(this.Size());
            for (int n = 0; n < this.Size(); n++)
                cloned[n] = this[n];
            return cloned;
        }

        public Object this[int pos] {
            get { return content[pos]; }
            set { content[pos] = value; }
        }

        public Object[] ToArray() {
            return content;
        }
    }
}