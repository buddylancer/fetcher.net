namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using System.Text.RegularExpressions;
    using Bula.Objects;
    using Bula.Model;

    /// <summary>
    /// Manipulating with items.
    /// </summary>
    public class BOItem : Bula.Meta {
        // Input fields
        /// Source name 
        private String source = null;
        /// RSS-item 
        private Hashtable item = null;

        /// Link to external item 
        public String link = null;
        /// Original title 
        public String full_title = null;
        /// Original description 
        public String full_description = null;

        // Output fields
        /// Final (processed) title 
        public String title = null;
        /// Final (processed) description 
        public String description = null;

        // Custom output fields
        /// Extracted creator (publisher) 
        public String creator = null;
        /// Extracted category 
        public String category = null;
        /// Extracted custom field 1 
        public String custom1 = null;
        /// Extracted custom field 2 
        public String custom2 = null;

        /// <summary>
        /// Instantiate BOItem from given source and RSS-item.
        /// </summary>
        /// <param name="source">Current processed source.</param>
        /// <param name="item">Current processed RSS-item from given source.</param>
        public BOItem (String source, Hashtable item) {
            this.Initialize(source, item);
        }

        /// <summary>
        /// Initialize this BOItem.
        /// </summary>
        /// <param name="source">Current processed source.</param>
        /// <param name="item">Current processed RSS-item from given source.</param>
        private void Initialize(String source, Hashtable item) {
            this.source = source;
            this.item = item;

            this.link = (String)item["link"];

            // Pre-process full description & title
            // Trick to eliminate non-UTF-8 characters
            this.full_title = Regex.Replace((String)item["title"], "[\xF0-\xF7][\x80-\xBF]{3}", "");
            if (item.ContainsKey("description") && !BLANK(item["description"]))
                this.full_description = Regex.Replace((String)item["description"], "[\xF0-\xF7][\x80-\xBF]{3}", "");

            this.PreProcessLink();
        }

        /// <summary>
        /// Pre-process link (just placeholder for now)
        /// </summary>
        protected void PreProcessLink() {}

        /// <summary>
        /// Process description.
        /// </summary>
        public void ProcessDescription() {
            var BR = "\n";
            var title = Strings.RemoveTags(this.full_title);
            title = title.Replace("&#", "[--amp--]");
            title = title.Replace("#", "[sharp]");
            title = title.Replace("[--amp--]", "&#");
            title = title.Replace("&amp;", "&");
            this.title = title;

            if (this.full_description == null)
                return;
            var description = this.full_description;

            //TODO -- Fixes for FetchRSS feeds (parsed from Twitter) here...
            description = description.Replace("&#160;", "");
            description = description.Replace("&nbsp;", "");

            // Start -- Fixes and workarounds for some sources here...
            // End

            var has_p = Regex.IsMatch(description, "<p[^>]*>");
            var has_br = description.IndexOf("<br") != -1;
            var has_li = description.IndexOf("<li") != -1;
            var has_div = description.IndexOf("<div") != -1;
            var include_tags = Strings.Concat(
                "<br>",
                (has_p ? "<p>" : null),
                (has_li ? "<ul><ol><li>" : null),
                (has_div ? "<div>" : null)
            );

            description = Strings.RemoveTags(description, include_tags);

            if (has_br)
                description = Regex.Replace(description, "[ \t\r\n]*<br[ ]*[/]*>[ \t\r\n]*", BR, RegexOptions.IgnoreCase);
            if (has_li) {
                description = Regex.Replace(description, "<ul[^>]*>", BR, RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "<ol[^>]*>", "* ", RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "<li[^>]*>", "* ", RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "</ul>", BR, RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "</ol>", BR, RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "</li>", BR, RegexOptions.IgnoreCase);
            }
            if (has_p) {
                description = Regex.Replace(description, "<p[^>]*>", BR, RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "</p>", BR, RegexOptions.IgnoreCase);
            }
            if (has_div) {
                description = Regex.Replace(description, "<div[^>]*>", BR, RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "</div>", BR, RegexOptions.IgnoreCase);
            }

            // Process end-of-lines...
            while (description.IndexOf(" \n") != -1)
                description = description.Replace(" \n", "\n");
            while (description.IndexOf("\n\n\n") != -1)
                description = description.Replace("\n\n\n", "\n\n");
            description = Regex.Replace(description, "\n\n[ \t]*[+\\-\\*][^+\\-\\*][ \t]*", "\n* ");
            description = Regex.Replace(description, "[ \t]+", " ");

            this.description = description.Trim();
        }

        /// <summary>
        /// Process category (if any).
        /// </summary>
        public void ProcessCategory() {
            // Set or fix category from item
            var category = (String)null;
            if (!BLANK(this.item["category"]))
                category = this.PreProcessCategory(STR(this.item["category"]));
            else {
                if (!BLANK(this.item["tags"]))
                    category = this.PreProcessCategory(STR(this.item["tags"]));
                else
                    category = this.ExtractCategory();
            }
            this.category = category;
        }

        /// <summary>
        /// Pre-process category.
        /// </summary>
        /// <param name="category_item">Input category.</param>
        /// <returns>Pre-processed category.</returns>
        private String PreProcessCategory(String category_item) {
            // Pre-process category from item["category"]

            // This is just sample - implement your own logic
            if (EQ(this.source, "something.com")) {
                // Fix categories from something.com
            }

            var category = (String)null;
            if (category_item.Length != 0) {
                String[] categories_arr = category_item.Replace(",&,", " & ").Split(new char[] {','});
                var categories_new = new ArrayList();
                for (int c = 0; c < SIZE(categories_arr); c++) {
                    var temp = categories_arr[c];
                    if (!BLANK(temp))
                        categories_new.Add(temp);
                }
                category = Strings.Join(", ", (String[])categories_new.ToArray());
            }

            return category;
        }

        /// <summary>
        /// Extract category.
        /// </summary>
        /// <returns>Resulting category.</returns>
        private String ExtractCategory() {
            // Try to extract category from description body (if no item["category"])

            var category = (String)null;

            //TODO -- This is just sample - implement your own logic for extracting category
            //if (Config.RssAllowed == null)
            //    category = this.source;

            return category;
        }

        /// <summary>
        /// Add standard categories (from DB) to current item.
        /// </summary>
        /// <param name="dsCategories">DataSet with categories (pre-loaded from DB).</param>
        /// <param name="lang">Input language.</param>
        public void AddStandardCategories(DataSet dsCategories, String lang) {
            //if (BLANK(this.description))
            //    return;

            String[] category_tags = BLANK(this.category) ?
                Strings.EmptyArray() : this.category.Split(new char[] {','});
            for (int n1 = 0; n1 < dsCategories.GetSize(); n1++) {
                var oCategory = dsCategories.GetRow(n1);
                var rss_allowed_key = STR(oCategory["s_CatId"]);
                var name = STR(oCategory["s_Name"]);

                var filter_value = STR(oCategory["s_Filter"]);
                String[] filter_chunks = Strings.Split("~", filter_value);
                String[] include_chunks = SIZE(filter_chunks) > 0 ?
                    Strings.Split("|", filter_chunks[0]) : Strings.EmptyArray();
                String[] exclude_chunks = SIZE(filter_chunks) > 1 ?
                    Strings.Split("|", filter_chunks[1]) : Strings.EmptyArray();

                var include_flag = false;
                for (int n2 = 0; n2 < SIZE(include_chunks); n2++) {
                    var include_chunk = Regex.Escape(include_chunks[n2]);
                    if (!BLANK(this.description) && Regex.IsMatch(this.description, include_chunk, RegexOptions.IgnoreCase))
                        include_flag |= true;
                    if (Regex.IsMatch(this.title, include_chunk, RegexOptions.IgnoreCase))
                        include_flag |= true;
                }
                for (int n3 = 0; n3 < SIZE(exclude_chunks); n3++) {
                    var exclude_chunk = Regex.Escape(exclude_chunks[n3]);
                    if (!BLANK(this.description) && Regex.IsMatch(this.description, exclude_chunk, RegexOptions.IgnoreCase))
                        include_flag &= false;
                    if (Regex.IsMatch(this.title, exclude_chunk, RegexOptions.IgnoreCase))
                        include_flag |= true;
                }
                if (include_flag) {
                    ArrayList arrayList = Arrays.CreateArrayList(category_tags); arrayList.Add(name);
                    category_tags = (String[])arrayList.ToArray(typeof(String));
                }
            }
            if (SIZE(category_tags) == 0)
                return;

            //TODO
            //ArrayList unique_categories = this.NormalizeList(category_tags, lang);
            //category = String.Join(", ", unique_categories);

            this.category = Strings.Join(", ", category_tags);
        }

        /// <summary>
        /// Process creator (publisher, company etc).
        /// </summary>
        public void ProcessCreator() {
            // Extract creator from item (if it is not set yet)
            if (this.creator == null) {
                if (!BLANK(this.item["company"]))
                    this.creator = STR(this.item["company"]);
                else if (!BLANK(this.item["source"]))
                    this.creator = STR(this.item["source"]);
                else if (!BLANK(this.item["dc"])) { //TODO implement [dc][creator]
                    var temp = (Hashtable)this.item["dc"];
                    if (!BLANK(temp["creator"]))
                        this.creator = STR(temp["creator"]);
                }
            }
            if (this.creator != null)
                this.creator = Regex.Replace(this.creator, "[ \t\r\n]+", " ");

            //TODO -- Implement your own logic for extracting creator here
        }

        /// <summary>
        /// Generate URL title from item title.
        /// </summary>
        /// <param name="translit">Whether to apply transliteration or not.</param>
        /// <returns>Resulting URL title.</returns>
        ///
        /// For example:
        /// "Officials: Fireworks Spark Utah Wildfire, Evacuations"
        ///    will became
        /// "officials-fireworks-spark-utah-wildfire-evacuations"
        public String GetUrlTitle(Boolean translit = false) {
            var title = Strings.AddSlashes(this.title);

            if (translit)
                title = Util.TransliterateRusToLat(title);

            title = Regex.Replace(title, "\\&amp\\;", " and ");
            title = Regex.Replace(title, "[^A-Za-z0-9\\-\\. ]", " ");
            title = Regex.Replace(title, " +", " ");
            title = title.Trim();
            title = Regex.Replace(title, "\\.+", "-");
            title = Regex.Replace(title, " \\- ", "-");
            title = Regex.Replace(title, " \\. ", ".");
            title = Regex.Replace(title, "[ ]+", "-");
            title = Regex.Replace(title, "\\-+", "-");
            title = title.Trim(new char[] {'-'}).ToLower();
            return title;
        }
    }
}