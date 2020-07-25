namespace Bula.Objects {
    using System;

    using System.Collections;

    /**
     * Helper class for manipulating with arrays.
     */
    public class Arrays : Bula.Meta {
        public static ArrayList NewArrayList() {
            return new ArrayList();
        }

        ///Create new Hashtable.
        /// <param name="input">Optional array to build Hashtable from.</param>
        /// <returns>Resulting Hashtable.</returns>
        public static Hashtable NewHashtable() {
            return new Hashtable();
        }

        public static Object[] NewArray() { return NewArray(0); }

        ///Create new array of objects.
        /// <param name="size">Size of array.</param>
        /// <returns>Resulting array.</returns>
        public static Object[] NewArray(int size) {
            return new Object[size];
        }

        ///Merge Hashtables.
        /// <param name="input">Original Hashtable.</param>
        /// <param name="extra">Hashtable to merge with original Hashtable.</param>
        /// <returns>Merged Hashtable.</returns>
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

        ///Merge ArrayLists.
        /// <param name="input">Original ArrayList.</param>
        /// <param name="extra">ArrayList to merge with original ArrayList.</param>
        /// <returns>Resulting ArrayList.</returns>
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

        ///Merge Arrays.
        /// <param name="input">Original Array.</param>
        /// <param name="extra">ArrayList to merge with original Array.</param>
        /// <returns>Resulting Array.</returns>
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

        ///Extend Array with additional element.
        /// <param name="input">Original Array.</param>
        /// <param name="extra">Object to add to original Array.</param>
        /// <returns>Resulting Array.</returns>
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