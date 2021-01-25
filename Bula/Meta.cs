// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

// Note: this class is not ported (is fully specific for NET-version).

namespace Bula
{
    using System;

    using Bula.Objects;
    using System.Collections;

    /// <summary>
    /// Meta functions, that can be replaced when converting to other language (Java, C#).
    /// </summary>
    public class Meta {

        /// <summary>
        /// Stop executiong.
        /// </summary>
        /// <param name="str">Error message</param>
        public static void STOP(Object str) {
            Response.Write(str.ToString());
            Response.End("");
        }

        // Common functions

        /// <summary>
        /// Check whether an object is null.
        /// </summary>
        /// <param name="value">Input object</param>
        /// <returns></returns>
        public static bool NUL(object value) {
            return value == null;
        }

        /// <summary>
        /// Get integer value of any object.
        /// </summary>
        /// <param name="value">Input object</param>
        /// <returns>Integer result</returns>
        public static int INT(object value) {
            if (NUL(value))
                return 0;
	        if (value is string)
               return int.Parse((string)value);
            return int.Parse(value.ToString());
        }

        /// <summary>
        /// Get float value of any object.
        /// </summary>
        /// <param name="value">Input object</param>
        /// <returns>Float result</returns>
        public static float FLOAT(object value)
        {
            if (NUL(value))
                return 0;
	        if (value is string)
                return float.Parse((string)value);
            return float.Parse(value.ToString());
        }

        /// <summary>
        /// Get string value of any object.
        /// </summary>
        /// <param name="value">Input object</param>
        /// <returns>String result</returns>
        public static String STR(object value)
        {
            if (NUL(value))
                return null;
	        if (value is String)
                return (String)value;
            return value.ToString();
        }

        /// <summary>
        /// Check whether 2 object are equal.
        /// </summary>
        /// <param name="value1">First object</param>
        /// <param name="value2">Second object</param>
        /// <returns></returns>
        public static bool EQ(Object value1, Object value2) {
            if (value1 == null || value2 == null)
                return false;
	        return value1.ToString() == value2.ToString();
        }

        // String functions

        /// <summary>
        /// Check whether an object is empty.
        /// </summary>
        /// <param name="arg">Input object</param>
        /// <returns></returns>
        public static bool BLANK(object arg) {
	        if (arg == null)
		        return true;
	        if (arg is string)
		        return (arg as string).Length == 0;
	        return (arg.ToString() == "");
        }

        /// <summary>
        /// Get the length of an object (processed as string).
        /// </summary>
        /// <param name="str">Input object</param>
        /// <returns>Length of resulting string</returns>
        public static int LEN(object str) {
            return BLANK(str) ? 0 : str.ToString().Length;
        }

        /// <summary>
        /// Concatenate any number of objects as string.
        /// </summary>
        /// <param name="args">Array of objects</param>
        /// <returns>Resulting string</returns>
        public static string CAT(params object[] args) {
	        string result = "";
            foreach (object arg in args)
                result += STR(arg);
	        return result;
        }

        /// <summary>
        /// Get index of first substring occurence.
        /// </summary>
        /// <param name="str">Input string to search in</param>
        /// <param name="what">Substring to search for</param>
        /// <param name="off">Optional offset from input string beginning</param>
        /// <returns>Index of the substring (or -1)</returns>
        public static int IXOF(string str, string what, int off = 0) {
	        return str.IndexOf(what, off);
        }

        /// <summary>
        /// Instantiate array of objects.
        /// </summary>
        /// <param name="args">Variable length array of parameters</param>
        /// <returns>Resulting array</returns>
        public static Object[] ARR(params Object[] args) {
            return (Object[])args.Clone();
        }

        /// <summary>
        /// Instantiate empty array of required size.
        /// </summary>
        /// <param name="size">Size of array</param>
        /// <returns>Resulting empty array</returns>
        public static object[] ARR(int size)
        {
            return new Object[size];
        }

        /// <summary>
        /// Merge arrays.
        /// </summary>
        /// <param name="input">Input array</param>
        /// <param name="args">Variable length array of parameters</param>
        /// <returns>Merged array</returns>
        public static object[] ADD(object[] input, params object[] args) {
            object[] output = new object[input.Length + args.Length];
            input.CopyTo(output, 0);
            for (int n = 0; n < args.Length; n++)
                output[input.Length + n] = args[n];
            return output;
        }

        /// <summary>
        /// Identify the size of any object.
        /// </summary>
        /// <param name="val">Input object</param>
        /// <returns>Resulting size</returns>
        public static int SIZE(object val) {
            if (val == null) return 0;
            else if (val is Object[]) return ((Object[])val).Length;
            else if (val is ArrayList) return ((ArrayList)val).Count;
            else if (val is Hashtable) return ((Hashtable)val).Count;
            else if (val is String) return ((String)val).Length;
            return 0;
        }

        /// <summary>
        /// Call obj.method(args) and return its result.
        /// </summary>
        /// <param name="obj">Object instance</param>
        /// <param name="method">Method to call</param>
        /// <param name="args">Arguments</param>
        /// <returns>Result of method calling</returns>
        public object CALL(Object obj, String method, object[] args) {
            System.Reflection.MethodInfo methodInfo = obj.GetType().GetMethod(method);
            return methodInfo.Invoke(obj, args);
        }
    }
}
