// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    using Bula.Objects;

    /// <summary>
    /// Helper class for manipulating with arrays.
    /// </summary>
    public class Arrays : Bula.Meta {
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

            var inputSize = SIZE(input);
            var newSize = inputSize + 1;
            Object[] output = NewArray(newSize);
            for (int n = 0; n < inputSize; n++)
                output[n] = input[n];
            output[inputSize] = element;
            return output;
        }

    }
}