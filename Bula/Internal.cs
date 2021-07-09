// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

// Note: this class is not ported (is fully specific for NET-version).

namespace Bula
{
    using System;
    using System.Xml;

    using System.Collections;
    using System.Text;
    using System.Text.RegularExpressions;

    using Bula.Objects;

    /// <summary>
    /// Various operations specific to C# version.
    /// </summary>
    public class Internal : Bula.Meta {

        /// <summary>
        /// Remove tags from a string.
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="except">Allowed tags</param>
        /// <returns>Resulting string</returns>
        public static String RemoveTags(String input, String except)
        {
            Boolean has_open = Regex.IsMatch(input, "<[a-z]+[^>]*>");
            Boolean has_close = Regex.IsMatch(input, "</[a-z]+>");
            Boolean has_twin = Regex.IsMatch(input, "<[a-z]+/>");

            if (!has_open && !has_close && !has_twin)
                return input;

            if (except == null)
                return RemoveTag(input, "[a-z]+");

            String output = input;
            output = DecorateTags(output, except);
            output = RemoveTags(output, null);
            output = UndecorateTags(output);
            return output;
        }

        private static String RemoveTag(String input, String tag)
        {
            return Regex.Replace(input, CAT("<[/]*", tag, "[^<>]*[/]*>"), "");
        }

        private static String DecorateTags(String input, String except)
        {
            String[] chunks = Regex.Replace(except, "[/]*>", "").Split(new char[] {'<'});
            String output = input;
            foreach (String chunk in chunks)
            {
                if (chunk.Length != 0)
                    output = DecorateTag(output, chunk);
            }
            return output;
        }

        private static String DecorateTag(String input, String tag)
        {
            return Regex.Replace(input, CAT("<([/]*", tag, "[^<>]*[/]*)>"), "~{$1}~");
        }

        private static String UndecorateTags(String input)
        {
            return Regex.Replace(input, CAT("~{([/]*[^}]+)}~"), "<$1>");
        }

        private static String AllowedChars = "€₹₽₴—•–‘’—№…"; //TODO!!! Hardcode Russian Ruble, Ukranian Hryvnia etc for now

        /// <summary>
        /// Clean out UTF-8 chars which are not accepted by MySQL.
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Resulting string</returns>
        public static String CleanChars(String input)
        {
            char[] inputChars = input.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int n = 0; n < inputChars.Length; n++) {
                if (inputChars[n] < 2048 || AllowedChars.IndexOf(inputChars[n]) != -1)
                    sb.Append(inputChars[n]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Call method of given class using provided arguments.
        /// </summary>
        /// <param name="class_name">Class name</param>
        /// <param name="method_name">Method name</param>
        /// <returns>Result of method execution</returns>
        public static Object CallStaticMethod(String class_name, String method_name)
        {
            return CallMethod(class_name, null, method_name, null);
        }

        /// <summary>
        /// Call static method of given class using provided arguments.
        /// </summary>
        /// <param name="class_name">Class name</param>
        /// <param name="method_name">Method name</param>
        /// <param name="args">List of arguments</param>
        /// <returns>Result of method execution</returns>
        public static Object CallStaticMethod(String class_name, String method_name, TArrayList args)
        {
            Type type = Type.GetType(class_name.Replace('/', '.'));
            System.Reflection.MethodInfo methodInfo = type.GetMethod(method_name);
            if (args != null && args.Size() > 0)
                return methodInfo.Invoke(null, args.ToArray());
            else
                return methodInfo.Invoke(null, null);
        }

        private static Type[] GetTypes(TArrayList args) {
            Type[] types = args != null && args.Size() > 0 ? new Type[args.Size()] : new Type[0];
            if (types.Length > 0)
            {
                for (int n = 0; n < args.Size(); n++)
                {
                    types[n] = args[n].GetType();
                    if (args[n] is String)
                    {
                        int result;
                        if (int.TryParse((String)args[n], out result))
                        {
                            types[n] = typeof(int);
                            args[n] = result;
                        }
                    }
                }
            }
            return types;
        }

        /// <summary>
        /// Call method of given class using provided arguments.
        /// </summary>
        /// <param name="class_name">Class name</param>
        /// <param name="args0">Constructor args</param>
        /// <param name="method_name">Method name</param>
        /// <param name="args">List of arguments</param>
        /// <returns>Result of method execution</returns>
        public static Object CallMethod(String class_name, TArrayList args0, String method_name, TArrayList args)
        {
            Type type = Type.GetType(class_name.Replace('/', '.'));

            Type[] types0 = GetTypes(args0);
            System.Reflection.ConstructorInfo constructorInfo = type.GetConstructor(types0);
            Object doObject = constructorInfo.Invoke(args0.ToArray());

            Type[] types = GetTypes(args);
            System.Reflection.MethodInfo methodInfo = type.GetMethod(method_name, types);
            if (methodInfo != null)
            {
                if (args != null && args.Size() > 0)
                    return methodInfo.Invoke(doObject, args.ToArray());
                else
                    return methodInfo.Invoke(doObject, null);
            }
            else
                return null;
        }

        /// <summary>
        /// Fetch info from RSS-feed.
        /// </summary>
        /// <param name="url">Feed url</param>
        /// <returns>Resulting array of items</returns>
        public static Object[] FetchRss(String url)
        {
            var items = new TArrayList();

            XmlDocument rssXmlDoc = new XmlDocument();

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(rssXmlDoc.NameTable);
            nsmgr.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");

            // Load the RSS file from the RSS URL
            try {
                rssXmlDoc.Load(url);
            }
            catch (Exception ex1) {
                var matchCollection = Regex.Matches(ex1.Message, "'([^']+)' is an undeclared prefix. Line [0-9]+, position [0-9]+.");
                if (matchCollection.Count > 0) {
                    var prefix = matchCollection[0].Groups[1].Value;
                    try
                    {
                        var client = new System.Net.WebClient();
                        var content = (new System.Net.WebClient()).DownloadString(url);
                        byte[] bytes = Encoding.Default.GetBytes(content);
                        content = Encoding.UTF8.GetString(bytes);
                        //content = System.Text.Encoding.UTF8.GetBytes(content).ToString();
                        var pattern = CAT("<", prefix, ":[^>]+>[^<]+</", prefix, ":[^>]+>");
                        content = Regex.Replace(content, pattern, "");
                        rssXmlDoc.LoadXml(content);
                    }
                    catch (Exception ex2) {
                        return null;
                    }
                }
                else
                    return null;
            }

            // Parse the Items in the RSS file
            XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");

            // Iterate through the items in the RSS file
            foreach (XmlNode rssNode in rssNodes)
            {
                var item = new THashtable();

                XmlNodeList itemNodes = rssNode.SelectNodes("*");
                foreach (XmlNode itemNode in itemNodes) {
                    String name = itemNode.Name;
                    String text = itemNode.InnerXml;
                    if (text.StartsWith("<![CDATA["))
                        text = text.Replace("<![CDATA[", "").Replace("]]>", "");

                    if (name == "category") {
                        if (item[name] == null)
                            item[name] = text;
                        else
                            item[name] += ", " + text;
                    }
                    else if (name == "dc:creator") {
                        THashtable dc = item.ContainsKey("dc") ? (THashtable)item["dc"] : new THashtable();
                        dc["creator"] = text;
                        item["dc"] = dc;
                    }
                    else if (name == "dc:date") {
                        THashtable dc = item.ContainsKey("dc") ? (THashtable)item["dc"] : new THashtable();
                        dc["date"] = text;
                        item["dc"] = dc;
                    }
                    else
                        item[name] = text;
                }
                items.Add(item);
            }
            return items.ToArray();
        }
    }
}