namespace Bula.Objects {
    using System;

    using System.Collections;

    /// <summary>
    /// Helper class for manipulating with arrays.
    /// </summary>
    public class Arrays : Bula.Meta {
        /// Create new array list. 
        public static ArrayList NewArrayList() {
            return new ArrayList();
        }

        /// <summary>
        /// Create new hash table.
        /// </summary>
        /// <returns>New hash table.</returns>
        public static Hashtable NewHashtable() {
            return new Hashtable();
        }

        /// <summary>
        /// Create new empty array.
        /// </summary>
        public static Object[] NewArray() {
            return NewArray(0); }

        /// <summary>
        /// Create new array of objects.
        /// </summary>
        /// <param name="size">Size of array.</param>
        /// <returns>Resulting array.</returns>
        public static Object[] NewArray(int size) {
            return new Object[size];
        }

        /// <summary>
        /// Merge hash tables.
        /// </summary>
        /// <param name="input">Original hash table.</param>
        /// <param name="extra">Hash table to merge with original one.</param>
        /// <returns>Merged hash table.</returns>
        public static Hashtable MergeHashtable(Hashtable input, Hashtable extra) {
            if (input == null)
                return null;
            if (extra == null)
                return input;

            var output = (Hashtable)input.Clone();
            var keys = extra.Keys.GetEnumerator();
            while (keys.MoveNext()) {
                var key = (String)keys.Current;
                output[key] = extra[key];
            }
            return output;
        }

        /// <summary>
        /// Merge array lists.
        /// </summary>
        /// <param name="input">Original array list.</param>
        /// <param name="extra">Array list to merge with original one.</param>
        /// <returns>Resulting array list.</returns>
        public static ArrayList MergeArrayList(ArrayList input, ArrayList extra) {
            if (input == null)
                return null;
            if (extra == null)
                return input;

            var output = NewArrayList();
            for (int n = 0; n < SIZE(input); n++)
                output.Add(input[n]);
            for (int n = 0; n < SIZE(extra); n++)
                output.Add(extra[n]);
            return output;
        }

        /// <summary>
        /// Merge arrays.
        /// </summary>
        /// <param name="input">Original array.</param>
        /// <param name="extra">Array to merge with original one.</param>
        /// <returns>Resulting array.</returns>
        public static Object[] MergeArray(Object[] input, Object[] extra) {
            if (input == null)
                return null;
            if (extra == null)
                return input;

            var input_size = SIZE(input);
            var extra_size = SIZE(extra);
            var new_size = input_size + extra_size;
            Object[] output = NewArray(new_size);
            for (int n = 0; n < input_size; n++)
                output[n] = input[n];
            for (int n = 0; n < extra_size; n++)
                output[input_size + n] = extra[n];
            return output;
        }

        /// <summary>
        /// Extend array with additional element.
        /// </summary>
        /// <param name="input">Original array.</param>
        /// <param name="element">Object to add to original array.</param>
        /// <returns>Resulting array.</returns>
        public static Object[] ExtendArray(Object[] input, Object element) {
            if (input == null)
                return null;
            if (element == null)
                return input;

            var input_size = SIZE(input);
            var new_size = input_size + 1;
            Object[] output = NewArray(new_size);
            for (int n = 0; n < input_size; n++)
                output[n] = input[n];
            output[input_size] = element;
            return output;
        }

        /// <summary>
        /// Create array list from array of objects.
        /// </summary>
        /// <param name="input">Array of objects.</param>
        /// <returns>Resulting array list.</returns>
        public static ArrayList CreateArrayList(Object[] input) {
    		if (input == null)
                return null;
            var output = new ArrayList();
            if (SIZE(input) == 0)
                return output;
            foreach (Object obj in input)
                output.Add(obj);
            return output;
        }

    }
}