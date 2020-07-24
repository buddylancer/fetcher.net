namespace Bula {
    using System;
    // Below are meta functions, that can be replaced when converting to other language (Java, C#)

    using Bula.Objects;
    using System.Collections;

    public class Meta {

        public static void STOP(Object str) {
            Response.Write(str.ToString());
            System.Web.HttpContext.Current.Response.End();
        }

        // Common functions
        public static bool NUL(object value) { return value == null; }

        /**
         * Convert object to Integer
         * @param object value
         * @return Integer
         */
        public static int INT(object value) {
            if (NUL(value))
                return 0;
	        if (value is string)
               return int.Parse((string)value);
            return int.Parse(value.ToString());
        }

        public static float FLOAT(object value) {
            if (NUL(value))
                return 0;
	        if (value is string)
                return float.Parse((string)value);
            return float.Parse(value.ToString());
        }

        public static String STR(object value) {
            if (NUL(value))
                return null;
	        if (value is String)
                return (String)value;
            return value.ToString();
        }

        public static bool EQ(Object value1, Object value2) {
            if (value1 == null || value2 == null)
                return false;
	        return value1.ToString() == value2.ToString();
        }

        // TString functions
        public static bool BLANK(object arg) {
	        if (arg == null)
		        return true;
	        if (arg is string)
		        return (arg as string).Length == 0;
	        return (arg.ToString() == "");
        }

        public static int LEN(object str) {
            return BLANK(str) ? 0 : str.ToString().Length;
        }

        public static string CAT(params object[] args) {
	        string result = "";
            foreach (object arg in args)
                result += STR(arg);
	        return result;
        }

        public static int IXOF(string str, string what, int off = 0) {
	        return str.IndexOf(what, off);
        }

        public static void PR(object str) {
        }

        public static object[] ARR(params object[] args) {
            return args;
        }

        public static object[] ARR(int size)
        {
            return new Object[size];
        }

        public static object[] ADD(object[] input, params object[] args) {
            object[] output = new object[input.Length + args.Length];
            input.CopyTo(output, 0);
            for (int n = 0; n < args.Length; n++)
                output[input.Length + n] = args[n];
            return output;
        }

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
