namespace Bula {
    using System;
    using System.Xml;

    using System.Collections;
    using System.Text.RegularExpressions;

    public class Internal : Bula.Meta {

        public static String RemoveTags(String input, String except)
        {
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
            return Regex.Replace(Regex.Escape(input), CAT("<[/]*", tag, "[/]*>"), "");
        }

        private static String DecorateTags(String input, String except)
        {
            String[] chunks = Regex.Replace(Regex.Escape(input), "[/]*>", "").Split(new char[] {'<'});
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
            return Regex.Replace(input, CAT("<([/]*", tag, "[^>]+)>"), "~{$1}~");
        }

        private static String UndecorateTags(String input)
        {
            return Regex.Replace(input, CAT("~{([/]*[^}]+)}~"), "<$1>");
        }

        
        ///Call method of given class using provided arguments.
        /// <param name="class_name">Class name</param>
        /// <param name="method_name">Method name</param>
        /// <returns>Result of method execution</returns>
        public static Object CallStaticMethod(String class_name, String method_name)
        {
            return CallMethod(class_name, method_name, null);
        }

        ///Call static method of given class using provided arguments.
        /// <param name="class_name">Class name</param>
        /// <param name="method_name">Method name</param>
        /// <param name="args">List of arguments</param>
        /// <returns>Result of method execution</returns>
        public static Object CallStaticMethod(String class_name, String method_name, ArrayList args)
        {
            Type type = Type.GetType(class_name.Replace('/', '.'));
            System.Reflection.MethodInfo methodInfo = type.GetMethod(method_name);
            if (args != null && args.Count > 0)
                return methodInfo.Invoke(null, args.ToArray());
            else
                return methodInfo.Invoke(null, null);
        }

        ///Call method of given class using provided arguments.
        /// <param name="class_name">Class name</param>
        /// <param name="method_name">Method name</param>
        /// <param name="args">List of arguments</param>
        /// <returns>Result of method execution</returns>
        public static Object CallMethod(String class_name, String method_name, ArrayList args)
        {
            Type type = Type.GetType(class_name.Replace('/', '.'));
            System.Reflection.ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { });
            Object doObject = constructorInfo.Invoke(new Object[] { });

            Type[] types = args != null && args.Count > 0 ? new Type[args.Count] : new Type[0];
            if (types.Length > 0)
            {
                for (int n = 0; n < args.Count; n++)
                {
                    int result;
                    if (int.TryParse((String)args[n], out result))
                    {
                        types[n] = typeof(int);
                        args[n] = result;
                    }
                    else
                        types[n] = args[n].GetType();
                }
            }

            System.Reflection.MethodInfo methodInfo = type.GetMethod(method_name, types);
            if (methodInfo != null)
            {
                if (args != null && args.Count > 0)
                    return methodInfo.Invoke(doObject, args.ToArray());
                else
                    return methodInfo.Invoke(doObject, null);
            }
            else
                return null;
        }

        public static Object[] FetchRss(String url)
        {
            var items = new ArrayList();

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
                var item = new Hashtable();

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
                    item["dc"] = new Hashtable();
                    ((Hashtable)item["dc"])["creator"] = rssSubNode.InnerText;
                }
                items.Add(item);
            }
            return items.ToArray();
        }    
    }
}