namespace Bula.Objects {
    using System;

    using System.Web;

    public class Response : Bula.Meta {
        public static void Write(String input) {
            HttpContext.Current.Response.Write(input);
        }

        public static void WriteHeader(String name, String value) {
            HttpContext.Current.Response.Headers.Add(name, value);
        }

        public static void End(String input) {
            Write(input);
            HttpContext.Current.Response.End();
        }
    }

}