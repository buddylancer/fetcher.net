namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using System.Text.RegularExpressions;
    using Bula.Objects;
    using Bula.Model;

    /**
     * Manipulating with items.
     */
    public class BOItem : Bula.Meta {
        // Input fields
        protected String source = null; // Source name
        protected Hashtable item = null; // RSS-item info

        public String link = null; // Link to external item.
        public String full_title = null; // Original title
        public String full_description = null; // Original description

        // Output fields
        public String title = null; // Final title
        public String description = null; // Final description.
        // Custom output fields
        public String creator = null; // Extracted creator (publisher)
        public String category = null; // Extracted category
        public String custom1 = null; // Extracted custom field #1
        public String custom2 = null; // Extracted custom field #2

        public BOItem (String Source, Hashtable Item) {
            this.Initialize(Source, Item);
        }

        ///Initialize the item.
        /// <param name="Source">Source name.</param>
        /// <param name="Item">Item object.</param>
        private void Initialize(String Source, Hashtable Item) {
            this.source = Source;
            this.item = Item;

            this.link = (String)Item["link"];

            // Pre-process full description & title
            // Trick to eliminate non-UTF-8 characters
            this.full_title = Regex.Replace((String)Item["title"], "[\xF0-\xF7]...", "");
            if (Item.ContainsKey("description") && !BLANK(Item["description"]))
                this.full_description = Regex.Replace((String)Item["description"], "[\xF0-\xF7]...", "");

            this.PreProcessLink();
        }

        protected void PreProcessLink() {}

        ///Process description.
        public void ProcessDescription() {
            String BR = "\n";
            String title = Strings.RemoveTags(this.full_title);
            title = title.Replace("&#", "[--amp--]");
            title = title.Replace("#", "[sharp]");
            title = title.Replace("[--amp--]", "&#");
            title = title.Replace("&amp;", "&");
            this.title = title;

            if (this.full_description == null)
                return;
            String description = this.full_description;

            //TODO -- Fixes for FetchRSS feeds (parsed from Twitter) here...
            description = description.Replace("&#160;", "");
            description = description.Replace("&nbsp;", "");

            // Start -- Fixes and workarounds for some sources here...
            // End

            Boolean has_p = Regex.IsMatch(description, "<p[^>]*>");
            Boolean has_br = description.IndexOf("<br") != -1;
            Boolean has_li = description.IndexOf("<li") != -1;
            Boolean has_div = description.IndexOf("<div") != -1;
            String include_tags = Strings.Concat(
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
            description = Regex.Replace(description, "\n\n[ \t]*[+-*][^+-*][ \t]*", "\n* ");
            description = Regex.Replace(description, "[ \t]+", " ");

            this.description = description.Trim();
        }

        ///Process category (if any).
        public void ProcessCategory() {
            // Set or fix category from item
            String category = null;
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

        ///Pre-process category.
        /// <param name="category_item">Input category.</param>
        /// <returns>Pre-processed category.</returns>
        private String PreProcessCategory(String category_item) {
            // Pre-process category from item["category"]

            // This is just sample - implement your own logic
            if (EQ(this.source, "something.com")) {
                // Fix categories from something.com
            }

            String category = null;
            if (category_item.Length != 0) {
                String[] categories_arr = category_item.Replace(",&,", " & ").Split(new char[] {','});
                ArrayList categories_new = new ArrayList();
                for (int c = 0; c < SIZE(categories_arr); c++) {
                    String temp = categories_arr[c];
                    if (!BLANK(temp))
                        categories_new.Add(temp);
                }
                category = Strings.Join(", ", (String[])categories_new.ToArray());
            }

            return category;
        }

        ///Extract category.
        /// <returns>Resulting category.</returns>
        private String ExtractCategory() {
            // Try to extract category from description body (if no item["category"])

            String category = null;

            //TODO -- This is just sample - implement your own logic for extracting category
            //if (Config.RssAllowed == null)
            //    category = this.source;

            return category;
        }

        ///Quote pattern for Regex operations.
        /// <param name="pattern">Input pattern (not-quoted).</param>
        /// <returns>Resulting pattern (quoted).</returns>
        private String QuotePattern(String pattern) {
            pattern = pattern.Replace(".", "\\.");
            pattern = pattern.Replace("#", "\\#");
            pattern = pattern.Replace("+", "\\+");
            pattern = pattern.Replace("[", "\\[");
            pattern = pattern.Replace("]", "\\]");
            return pattern;
        }

        ///Add standard categories (from DB).
        /// <param name="dsCategories">Data set with categories (from DB).</param>
        /// <param name="lang">Input language.</param>
        public void AddStandardCategories(DataSet dsCategories, String lang) {
            //if (BLANK(this.description))
            //    return;

            String[] category_tags = BLANK(this.category) ?
                Strings.EmptyArray() : this.category.Split(new char[] {','});
            for (int n1 = 0; n1 < dsCategories.GetSize(); n1++) {
                Hashtable oCategory = dsCategories.GetRow(n1);
                String rss_allowed_key = STR(oCategory["s_CatId"]);
                String name = STR(oCategory["s_Name"]);

                String filter_value = STR(oCategory["s_Filter"]);
                String[] filter_chunks = Regex.Split(filter_value, "~");
                String[] include_chunks = SIZE(filter_chunks) > 0 ?
                    Regex.Split(filter_chunks[0], "|") : Strings.EmptyArray();
                String[] exclude_chunks = SIZE(filter_chunks) > 1 ?
                    Regex.Split(filter_chunks[1], "|") : Strings.EmptyArray();

                Boolean include_flag = false;
                for (int n2 = 0; n2 < SIZE(include_chunks); n2++) {
                    String include_chunk = QuotePattern(include_chunks[n2]);
                    if (!BLANK(this.description) && Regex.IsMatch(this.description, include_chunk, RegexOptions.IgnoreCase))
                        include_flag |= true;
                    if (Regex.IsMatch(this.title, include_chunk, RegexOptions.IgnoreCase))
                        include_flag |= true;
                }
                for (int n3 = 0; n3 < SIZE(exclude_chunks); n3++) {
                    String exclude_chunk = QuotePattern(exclude_chunks[n3]);
                    if (!BLANK(this.description) && Regex.IsMatch(this.description, exclude_chunk, RegexOptions.IgnoreCase))
                        include_flag &= false;
                    if (Regex.IsMatch(this.title, exclude_chunk, RegexOptions.IgnoreCase))
                        include_flag |= true;
                }
                if (include_flag) {
                    category_tags = (String[])ADD(category_tags, name);
                }
            }
            if (SIZE(category_tags) == 0)
                return;

            //TODO
            //ArrayList unique_categories = this.NormalizeList(category_tags, lang);
            //category = String.Join(", ", unique_categories);

            this.category = Strings.Join(", ", category_tags);
        }

        ///Process creator (publisher, company etc).
        public void ProcessCreator() {
            // Extract creator from item (if it is not set yet)
            if (this.creator == null) {
                if (!BLANK(this.item["company"]))
                    this.creator = STR(this.item["company"]);
                else if (!BLANK(this.item["source"]))
                    this.creator = STR(this.item["source"]);
                else if (!BLANK(this.item["dc"])) { //TODO implement [dc][creator]
                    Hashtable temp = (Hashtable)this.item["dc"];
                    if (!BLANK(temp["creator"]))
                        this.creator = STR(temp["creator"]);
                }
            }
            if (this.creator != null)
                this.creator = Regex.Replace(this.creator, "[ \t\r\n]+", " ");

            //TODO -- Implement your own logic for extracting creator here
        }

        ///Generate URL title from item title.
        /// <param name="translit">Whether to apply transliteration or not.</param>
        /// <returns>Resulting URL title.</returns>
        ///
        /// For example:
        /// "Officials: Fireworks Spark Utah Wildfire, Evacuations"
        ///    will became
        /// "officials-fireworks-spark-utah-wildfire-evacuations"
        public String GetUrlTitle(Boolean translit = false) {
            String title = Strings.AddSlashes(this.title);

            if (translit)
                title = Util.TransliterateRusToLat(title);

            title = Regex.Replace(title, "&amp;", " and ");
            title = Regex.Replace(title, "[^A-Za-z0-9-. ]", " ");
            title = Regex.Replace(title, " +", " ");
            title = title.Trim();
            title = Regex.Replace(title, ".+", "-");
            title = Regex.Replace(title, " - ", "-");
            title = Regex.Replace(title, " . ", ".");
            title = Regex.Replace(title, "[ ]+", "-");
            title = Regex.Replace(title, "-+", "-");
            title = title.Trim(new char[] {'-'}).ToLower();
            return title;
        }
    }
}