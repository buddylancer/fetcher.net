// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;
    using System.Collections;

    using Bula.Fetcher;
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
        private THashtable item = null;

        /// Link to external item 
        public String link = null;
        /// Original title 
        public String fullTitle = null;
        /// Original description 
        public String fullDescription = null;

        // Output fields
        /// Final (processed) title 
        public String title = null;
        /// Final (processed) description 
        public String description = null;
        /// Final (processed) date 
        public String date = null;

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
        public BOItem (String source, THashtable item) {
                this.Initialize(source, item);
        }

        /// <summary>
        /// Initialize this BOItem.
        /// </summary>
        /// <param name="source">Current processed source.</param>
        /// <param name="item">Current processed RSS-item from given source.</param>
        private void Initialize(String source, THashtable item) {
            this.source = source;
            this.item = item;

            this.link = (String)item["link"];

            // Pre-process full description & title
            // Trick to eliminate non-UTF-8 characters
            this.fullTitle = Regex.Replace((String)item["title"], "[\xF0-\xF7][\x80-\xBF]{3}", "");
            if (item.ContainsKey("description") && !BLANK(item["description"]))
                this.fullDescription = Regex.Replace((String)item["description"], "[\xF0-\xF7][\x80-\xBF]{3}", "");

            this.PreProcessLink();
        }

        /// <summary>
        /// Pre-process link (just placeholder for now)
        /// </summary>
        protected void PreProcessLink()
        {}

        /// <summary>
        /// Process description.
        /// </summary>
        public void ProcessDescription() {
            var BR = "\n";

            var title = Strings.RemoveTags(this.fullTitle);
            // Normalize \r\n to \n
            title = Regex.Replace(title, "\r\n", BR);
            title = Regex.Replace(title, "(^&)#", "1[sharp]");
            title = title.Replace("&amp;", "&");
            title = Regex.Replace(title, "&amp;laquo;", "&laquo;");
            title = Regex.Replace(title, "&amp;raquo;", "&raquo;");
            title = title.Trim();
            title = Regex.Replace(title, "[\n]+", " ");

            // Moved to Rules
            //int httpIndex = Strings.LastIndexOf("http", title);
            //if (httpIndex != -1)
            //    title = Strings.Substring(title, 0, httpIndex);

            this.title = title;

            if (this.fullDescription == null)
                return;
            // Normalize \r\n to \n
            var description = Regex.Replace(this.fullDescription, "\r\n", BR);

            //TODO -- Fixes for FetchRSS feeds (parsed from Twitter) here...
            description = description.Replace("&#160;", " ");
            description = description.Replace("&nbsp;", " ");

            // Start -- Fixes and workarounds for some sources here...
            // End

            var hasP = Regex.IsMatch(description, "<p[^>]*>");
            var hasBr = description.IndexOf("<br") != -1;
            var hasLi = description.IndexOf("<li") != -1;
            var hasDiv = description.IndexOf("<div") != -1;
            var includeTags = Strings.Concat(
                "<br>",
                (hasP ? "<p>" : null),
                (hasLi ? "<ul><ol><li>" : null),
                (hasDiv ? "<div>" : null)
            );

            description = Strings.RemoveTags(description, includeTags);

            if (hasBr)
                description = Regex.Replace(description, "[ \t\r\n]*<br[ ]*[/]*>[ \t\r\n]*", BR, RegexOptions.IgnoreCase);
            if (hasLi) {
                description = Regex.Replace(description, "<ul[^>]*>", BR, RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "<ol[^>]*>", "* ", RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "<li[^>]*>", "* ", RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "</ul>", BR, RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "</ol>", BR, RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "</li>", BR, RegexOptions.IgnoreCase);
            }
            if (hasP) {
                description = Regex.Replace(description, "<p[^>]*>", BR, RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "</p>", BR, RegexOptions.IgnoreCase);
            }
            if (hasDiv) {
                description = Regex.Replace(description, "<div[^>]*>", BR, RegexOptions.IgnoreCase);
                description = Regex.Replace(description, "</div>", BR, RegexOptions.IgnoreCase);
            }

            // Process end-of-lines...
            //description = Regex.Replace(description, "[\n]+$", "");
            description = Regex.Replace(description, "[ \t]+\n", "\n");
            description = Regex.Replace(description, "[\n]+\n\n", "\n\n");
            description = Regex.Replace(description, "\n\n[ \t]*[\\+\\-\\*][^\\+\\-\\*][ \t]*", "\n* ");
            description = Regex.Replace(description, "[ \t]+", " ");

            description = Regex.Replace(description, "&amp;laquo;", "&laquo;");
            description = Regex.Replace(description, "&amp;raquo;", "&raquo;");

            // Normalize back to \r\n
            this.description = Regex.Replace(description.Trim(), BR, EOL);
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
        /// <param name="categoryItem">Input category.</param>
        /// <returns>Pre-processed category.</returns>
        private String PreProcessCategory(String categoryItem) {
            // Pre-process category from item["category"]

            // This is just sample - implement your own logic
            if (EQ(this.source, "something.com")) {
                // Fix categories from something.com
            }

            var category = (String)null;
            if (categoryItem.Length != 0) {
                String[] categoriesArr = Strings.Split(",", categoryItem.Replace(",&,", " & "));
                var categoriesNew = new TArrayList();
                for (int c = 0; c < SIZE(categoriesArr); c++) {
                    var temp = categoriesArr[c];
                    temp = Strings.Trim(temp);
                    if (BLANK(temp))
                        continue;
                    temp = Strings.FirstCharToUpper(temp);
                    if (category == null)
                        category = temp;
                    else
                        category += CAT(", ", temp);
                }
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
        /// <returns>Number of added categories.</returns>
        public int AddStandardCategories(DataSet dsCategories, String lang) {
            //if (BLANK(this.description))
            //    return;

            var categoryTags = new TArrayList();
            if (!BLANK(this.category))
                categoryTags.AddAll(Strings.Split(", ", this.category));
            for (int n1 = 0; n1 < dsCategories.GetSize(); n1++) {
                var oCategory = dsCategories.GetRow(n1);
                var rssAllowedKey = STR(oCategory["s_CatId"]);
                var name = STR(oCategory["s_Name"]);

                var filterValue = STR(oCategory["s_Filter"]);
                String[] filterChunks = Strings.Split("~", filterValue);
                String[] includeChunks = SIZE(filterChunks) > 0 ?
                    Strings.Split("\\|", filterChunks[0]) : Strings.EmptyArray();
                String[] excludeChunks = SIZE(filterChunks) > 1 ?
                    Strings.Split("\\|", filterChunks[1]) : Strings.EmptyArray();

                var includeFlag = false;
                for (int n2 = 0; n2 < SIZE(includeChunks); n2++) {
                    var includeChunk = includeChunks[n2]; //Regex.Escape(includeChunks[n2]);
                    if (Regex.IsMatch(this.title, includeChunk, RegexOptions.IgnoreCase)) {
                        includeFlag |= true;
                        break;
                    }
                    if (!BLANK(this.description) && Regex.IsMatch(this.description, includeChunk, RegexOptions.IgnoreCase)) {
                        includeFlag |= true;
                        break;
                    }
                }
                for (int n3 = 0; n3 < SIZE(excludeChunks); n3++) {
                    var excludeChunk = excludeChunks[n3]; //Regex.Escape(excludeChunks[n3]);
                    if (Regex.IsMatch(this.title, excludeChunk, RegexOptions.IgnoreCase)) {
                        includeFlag &= false;
                        break;
                    }
                    if (!BLANK(this.description) && Regex.IsMatch(this.description, excludeChunk, RegexOptions.IgnoreCase)) {
                        includeFlag &= false;
                        break;
                    }
               }
                if (includeFlag)
                    categoryTags.Add(name);
            }
            if (categoryTags.Size() == 0)
                return 0;

            //TODO
            //TArrayList uniqueCategories = this.NormalizeList(categoryTags, lang);
            //category = String.Join(", ", uniqueCategories);

            this.category = Strings.Join(", ", (String[])categoryTags.ToArray(
                typeof(String)
            ));

            return categoryTags.Size();
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
                    var temp = (THashtable)this.item["dc"];
                    if (!BLANK(temp["creator"]))
                        this.creator = STR(temp["creator"]);
                }
            }
            if (this.creator != null)
                this.creator = Regex.Replace(this.creator, "[ \t\r\n]+", " ");

            //TODO -- Implement your own logic for extracting creator here
        }

        /// <summary>
        /// Process rules.
        /// </summary>
        /// <param name="rules">The list of rules to process.</param>
        public int ProcessRules(DataSet rules) {
            var counter = 0;
            for (int n = 0; n < rules.GetSize(); n++) {
                var rule = rules.GetRow(n);
                var sourceName = STR(rule["s_SourceName"]);
                if (EQ(sourceName, "*") || EQ(sourceName, this.source))
                    counter += this.ProcessRule(rule);
            }
            return counter;
        }

        private int ProcessRule(THashtable rule) {
            var counter = 0;
            var nameTo = STR(rule["s_To"]);
            var valueTo = (String)null;
            var nameFrom = NUL(rule["s_From"]) ? nameTo : STR(rule["s_From"]);
            var valueFrom = STR(this.GetString(nameFrom));
            var operation = STR(rule["s_Operation"]);
            var intValue = INT(rule["i_Value"]);
            var pattern = STR(rule["s_Pattern"]);
            var stringValue = STR(rule["s_Value"]);
            if (EQ(operation, "shrink") && !NUL(valueFrom) && LEN(pattern) > 0) {
                var shrinkIndex = valueFrom.IndexOf(pattern);
                if (shrinkIndex != -1) 
                    valueTo = valueFrom.Substring(0, shrinkIndex).Trim();
            }
            if (EQ(operation, "cut") && !NUL(valueFrom) && LEN(pattern) > 0) {
                var cutIndex = valueFrom.IndexOf(pattern);
                if (cutIndex == 0) 
                    valueTo = valueFrom.Substring(cutIndex + LEN(pattern));
            }
            if (EQ(operation, "replace") && !NUL(valueFrom) && LEN(pattern) > 0) {
                valueTo = Regex.Replace(valueFrom, pattern, stringValue, RegexOptions.IgnoreCase);
                var replaceIndex = valueFrom.IndexOf(pattern);
                if (replaceIndex != -1) 
                    valueTo = valueFrom.Replace(pattern, stringValue);
            }
            if (EQ(operation, "remove") && !NUL(valueFrom) && LEN(pattern) > 0) {
                var matches =
                    Regex.Matches(valueFrom, pattern, RegexOptions.IgnoreCase);
                if (SIZE(matches) > 0)
                    valueTo = valueFrom.Replace(matches[0].Value, "");
            }
            else if (EQ(operation, "truncate") && !NUL(valueFrom) && intValue > 0) {
                if (LEN(valueFrom) > intValue) {
                    valueTo = valueFrom.Substring(0, intValue);
                    while (!valueTo.EndsWith(" "))
                        valueTo = valueTo.Substring(0, LEN(valueTo) - 1);
                    valueTo = valueTo += "...";
                }
                //print "valueTo: '" . valueTo . "'<br/>\r\n";
            }
            else if (EQ(operation, "extract") && !NUL(valueFrom)) {
                var matches =
                    Regex.Matches(valueFrom, pattern, RegexOptions.IgnoreCase);
                if (SIZE(matches) > intValue) {
                    if (BLANK(stringValue))
                        valueTo = matches[intValue].Value;
                    else {
                        valueTo = stringValue;
                        for (int n = 0; n < SIZE(matches); n++) {
                            if (valueTo.IndexOf(CAT("$", n)) != -1)
                                valueTo = valueTo.Replace(CAT("$", n), matches[n].Value);
                        }
                    }
                }
            }
            if (!NUL(valueTo))
                this.SetString(nameTo, valueTo);
            return counter;
        }

        private void SetString(String name, String value) {
            if (EQ(name, "link"))
                this.link = value;
            else if (EQ(name, "title"))
                this.title = value;
            else if (EQ(name, "description"))
                this.description = value;
            else if (EQ(name, "date"))
                this.date = value;
            else if (EQ(name, "category")) {
                if (BLANK(this.category))
                    this.category = value;
                else
                    this.category = CAT(value, ", ", this.category);
            }
            else if (EQ(name, "creator"))
                this.creator = value;
            else if (EQ(name, "custom1"))
                this.custom1 = value;
            else if (EQ(name, "custom2"))
                this.custom2 = value;
        }

        private String GetString(String name) {
            if (EQ(name, "link"))
                return this.link;
            else if (EQ(name, "title"))
                return this.title;
            else if (EQ(name, "description"))
                return this.description;
            else if (EQ(name, "date"))
                return this.date;
            else if (EQ(name, "creator"))
                return this.creator;
            else if (EQ(name, "custom1"))
                return this.custom1;
            else if (EQ(name, "custom2"))
                return this.custom2;
            else if (this.item.ContainsKey(name))
                return STR(this.item[name]);
            return null;
        }

        /// <summary>
        /// Generate URL title from item title.
        /// </summary>
        /// <returns>Resulting URL title.</returns>
        public String GetUrlTitle() {
            return GetUrlTitle(false);
        }

        /// <summary>
        /// Generate URL title from item title.
        /// </summary>
        /// <param name="translit">Whether to apply transliteration or not.</param>
        /// <returns>Resulting URL title.</returns>
        ///
        /// For example:
        /// "Officials: Fireworks Spark Utah Wildfire, Evacuations"
        ///    will become
        /// "officials-fireworks-spark-utah-wildfire-evacuations"
        public String GetUrlTitle(Boolean translit) {
            var title = Strings.AddSlashes(this.title);

            if (translit)
                title = Util.TransliterateRusToLat(title);

            title = Regex.Replace(title, "\\&amp\\;", " and ");
            title = Regex.Replace(title, "[^A-Za-z0-9]", "-");
            title = Regex.Replace(title, "[\\-]+", "-");
            title = title.Trim(new char[] {'-'}).ToLower();
            return title;
        }
    }
}