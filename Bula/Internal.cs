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
            return Regex.Replace(input, CAT("<[/]*", tag, "[^>]*[/]*>"), "");
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
            return Regex.Replace(input, CAT("<([/]*", tag, "[^>]*[/]*)>"), "~{$1}~");
        }

        private static String UndecorateTags(String input)
        {
            return Regex.Replace(input, CAT("~{([/]*[^}]+)}~"), "<$1>");
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
            rssXmlDoc.Load(url);

            // Parse the Items in the RSS file
            XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");

            // Iterate through the items in the RSS file
            foreach (XmlNode rssNode in rssNodes)
            {
                var item = new THashtable();

                XmlNode rssSubNode = rssNode.SelectSingleNode("title");
                if (rssSubNode != null)
                    item["title"] = rssSubNode.InnerText;

                rssSubNode = rssNode.SelectSingleNode("link");
                if (rssSubNode != null)
                    item["link"] = rssSubNode.InnerText;

                rssSubNode = rssNode.SelectSingleNode("description");
                if (rssSubNode != null)
                    item["description"] = rssSubNode.InnerText;

                rssSubNode = rssNode.SelectSingleNode("pubDate");
                if (rssSubNode != null)
                    item["pubdate"] = rssSubNode.InnerText; //Yes, lower case

                rssSubNode = rssNode.SelectSingleNode("dc:creator", nsmgr);
                if (rssSubNode != null)
                {
                    item["dc"] = new THashtable();
                    ((THashtable)item["dc"])["creator"] = rssSubNode.InnerText;
                }
                items.Add(item);
            }
            return items.ToArray();
        }    
    }
}