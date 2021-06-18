using System;
using System.Collections;

namespace Bula.Objects
{
    public class DataRange : SortedList
    {
        public DataRange() : base() { }

        protected DataRange(DataRange from) : base(from) { }

        public override Object Clone() { return new DataRange(this); }

        public override void Add(object key, object value)
        {
            if (this.Contains(key))
                this.Remove(key);
            base.Add(key, value);
        }
    }
}
