namespace Bula.Objects {
    using System;

    using System.Text.RegularExpressions;
    using System.Collections;

    using Bula.Fetcher;
    using Bula.Objects;

    /**
     * Helper class for manipulations with strings.
     */
    public class Strings : Bula.Meta {
        public static String[] EmptyArray() {
            return new String[0];
        }

    	///Convert first char of a string to upper case.
        /// <param name="input">Input string.</param>
        /// <returns>Resulting string.</returns>
        public static String FirstCharToUpper(String input) {
            return Concat(input.Substring(0, 1).ToUpper(), input.Substring(1));
    	}

    	///Join an array of strings using divider,
        /// <param name="divider">Divider (yes, may be empty).</param>
        /// <param name="strings">Array of strings.</param>
        /// <returns>Resulting string.</returns>
        public static String Join(String divider, String[] strings) {
            var output = "";
            var count = 0;
            foreach (String string1 in strings) {
                if (count > 0)
                    output += (divider);
                output += (string1);
                count++;
            }
            return output;
    	}

    	public static String RemoveTags(String input) {
            return RemoveTags(input, null);
        }

        public static String RemoveTags(String input, String except) {
            return Internal.RemoveTags(input, except);
    	}

    	public static String AddSlashes(String input) {
            return input.Replace("'", "\\'"); //TODO!!!
    	}

    	public static String StripSlashes(String input) {
            return input.Replace("\\'", "'"); //TODO!!!
    	}

    	public static int CountSubstrings(String input, String chunk) {
    		if (input.Length == 0)
    			return 0;
    		var replaced = input.Replace(chunk, "");
    		return input.Length - replaced.Length;
    	}

        public static String Concat(params object[] args) {
    		var output = "";
    		if (SIZE(args) != 0) {
                foreach (object arg in args) {
                    if (arg == null)
                        continue;
    				output = output += (arg);
    			}
    		}
    		return output;
    	}

        public static String GetSqlDate(String pubdate) {
            return DateTimes.GmtFormat(Config.SQL_DTS, BLANK(pubdate) ? 0 : DateTimes.GetTime(pubdate));
        }

    	public static String[] Split(String divider, String input) {
    		String[] chunks = 
                Regex.Split(input, Regex.Escape(divider));
    		var result = new ArrayList();
            for (int n = 0; n < SIZE(chunks); n++)
    			result.Add(chunks[n]);
    		return (String[])result.ToArray(typeof(String));
    	}

    	public static String Replace(String from, String to, String input) {
            return Replace(from, to, input, 0);
        }

    	///Replace Substring(s).
        /// <param name="from">Substring to replace.</param>
        /// <param name="to">Replacement string.</param>
        /// <param name="input">Input string.</param>
        /// <param name="limit">Max number of replacements [optional].</param>
        /// <returns>Resulting string.</returns>
        public static String Replace(String from, String to, String input, int limit) {
            return limit != 0 ? (new Regex(Regex.Escape(from))).Replace(input, to, limit) : input.Replace(from, to);
    	}

        ///Replace all substrings with another string using regular expressions.
        /// <param name="regex">Regular expression to match Substring(s).</param>
        /// <param name="to">Replacement string.</param>
        /// <returns>Resulting string.</returns>
        public static String ReplaceAll(String regex, String to, String input) {
            return Replace(regex, to, input);
        }

        ///Replace first substring with another string using regular expressions.
        /// <param name="regex">Regular expression to match substring.</param>
        /// <param name="to">Replacement string.</param>
        /// <returns>Resulting string.</returns>
        public static String ReplaceFirst(String regex , String to, String input) {
            return Replace(regex, to, input, 1);
        }

        public static String ReplaceInTemplate(String template, Hashtable hash){
            System.Text.StringBuilder sb = new System.Text.StringBuilder(template);
            IEnumerator keys = hash.Keys.GetEnumerator();
            while (keys.MoveNext()) {
                String key = STR(keys.Current);
                sb.Replace(key, STR(hash[key]));
            }
            return sb.ToString();
        }

    }
}