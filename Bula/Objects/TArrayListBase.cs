using System;
using System.Collections;

namespace Bula.Objects
{
    public class TArrayListBase : Bula.Meta
    {
        public TArrayListBase() {
        }

        public TArrayListBase(Object[] items) {
            foreach (Object item in items)
                content.Add(item);
        }

        private ArrayList content = new ArrayList();

        public int Add(Object value) { return content.Add(value); }

        public Object Get(int pos) { return content[pos]; }

        public void Set(int pos, Object value) { content[pos] = value; }

        public int Size() { return content.Count; }

        public Object this[int pos] { get { return Get(pos); } set { Set(pos, value); } }

        public Object[] ToArray() { return content.ToArray(); }

        public Array ToArray(Type type) { return content.ToArray(type); }

    }
}
