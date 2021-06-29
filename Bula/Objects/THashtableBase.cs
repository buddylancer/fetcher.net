using System;
using System.Collections;

namespace Bula.Objects
{
    /** Base class for Hashtable wrapper */
    public class THashtableBase : Bula.Meta {
        private SortedList content = new SortedList();

        public void Add(String key, Object value) { content.Add(key, value); }

        public Object Get(String key) { return content[key]; }

        public void Put(String key, Object value) { content[key] = value; }

        public void Remove(String key) { content.Remove(key); }

        public int Size() { return content.Count; }

        public Object this[String key] { get { return Get(key); } set { Put(key, value); } }

        public bool ContainsKey(String key) { return content.ContainsKey(key); }

        public ICollection Keys { get { return content.Keys; } }
    }
}
