// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;

    using Bula;
    using Bula.Fetcher;

    using System.Collections;
    using Bula.Objects;

    /// <summary>
    /// Engine for processing templates.
    /// </summary>
    public class Engine : Bula.Meta {
        private Context context = null;
        private Boolean print_flag = false;
        private String print_string = "";

        /// Public default constructor 
        public Engine (Context context) {
            this.context = context;
            this.print_flag = false;
            this.print_string = "";
        }

        /// <summary>
        /// Set print string for current engine instance.
        /// </summary>
        /// <param name="val">Print string to set.</param>
        public void SetPrintString(String val) {
            this.print_string = val;
        }

        /// <summary>
        /// Get print string for current engine instance.
        /// </summary>
        /// <returns>Current print string.</returns>
        public String GetPrintString() {
            return this.print_string;
        }

        /// <summary>
        /// Set print flag for current engine instance.
        /// </summary>
        /// <param name="val">Print flag to set.</param>
        public void SetPrintFlag(Boolean val) {
            this.print_flag = val;
        }

        /// <summary>
        /// Get print flag for current engine instance.
        /// </summary>
        /// <returns>Current print flag.</returns>
        public Boolean GetPrintFlag() {
            return this.print_flag;
        }

        /// <summary>
        /// Write string.
        /// </summary>
        /// <param name="val">String to write.</param>
        public void Write(String val) {
            if (this.print_flag)
                Response.Write(val);
            else
                this.print_string += val;
        }

        /// <summary>
        /// Include file with class and generate content by calling method Execute().
        /// </summary>
        /// <param name="class_name">Class name to include.</param>
        /// <returns>Resulting content.</returns>
        public String IncludeTemplate(String class_name, String default_method = "Execute") {
            var engine = this.context.PushEngine(false);
            var file_name = 
                CAT(class_name, ".cs");

            var content = (String)null;
            if (Helper.FileExists(CAT(this.context.LocalRoot, file_name))) {
                ArrayList args0 = new ArrayList(); args0.Add(this.context);
                Internal.CallMethod(class_name, args0, "Execute", null);
                content = engine.GetPrintString();
            }
            else
                content = CAT("No such file: ", file_name);
            this.context.PopEngine();
            return content;
        }

        /// <summary>
        /// Show template content.
        /// </summary>
        /// <param name="filename">Template file to use.</param>
        /// <returns>Resulting content.</returns>
        public String ShowTemplate(String filename) {
            return ShowTemplate(filename, null); }

        /// <summary>
        /// Show template content by merging template and data.
        /// </summary>
        /// <param name="filename">Template file to use for merging.</param>
        /// <param name="hash">Data in the form of Hashtable to use for merging.</param>
        /// <returns>Resulting content.</returns>
        public String ShowTemplate(String filename, Hashtable hash) {
            var template = this.GetTemplate(filename);

            var content = "";
            content += (CAT("\n<!-- BEGIN ", Strings.Replace("Bula/Fetcher/", "", filename), " -->\n"));
            content += (this.ProcessTemplate(template, hash));
            content += (CAT("<!-- END ", Strings.Replace("Bula/Fetcher/", "", filename), " -->\n"));
            return content;
        }

        /// <summary>
        /// Get template as the list of lines.
        /// </summary>
        /// <param name="filename">File name.</param>
        /// <returns>Resulting array with lines.</returns>
        private ArrayList GetTemplate(String filename) {
            if (Helper.FileExists(CAT(this.context.LocalRoot, filename))) {
                Object[] lines = Helper.ReadAllLines(CAT(this.context.LocalRoot, filename));
                return Arrays.CreateArrayList(lines);
            }
            else {
                var temp = new ArrayList();
                temp.Add(CAT("File nor found -- '", filename, "'<hr/>"));
                return temp;
            }
        }

        /// <summary>
        /// Do actual merging of template and data.
        /// </summary>
        /// <param name="template">Template content.</param>
        /// <param name="hash">Data for merging with template.</param>
        /// <returns>Resulting content.</returns>
        public String FormatTemplate(String template, Hashtable hash) {
            if (hash == null)
                hash = new Hashtable();
            var content = Strings.ReplaceInTemplate(template, hash);
            return Strings.ReplaceInTemplate(content, this.context.GlobalConstants);
        }

        /// <summary>
        /// Trim comments from input string.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <returns>Resulting string.</returns>
        private static String TrimComments(String str) {
            var line = (String)str;
            var trimmed = false;
            if (line.IndexOf("<!--#") != -1) {
                line = line.Replace("<!--", "");
                line = line.Replace("-->", "");
                trimmed = true;
            }
            else if (line.IndexOf("//#") != -1) {
                line = line.Replace("//#", "#");
                trimmed = true;
            }
            line = line.Trim();
            return line;
        }

        /// <summary>
        /// Execute template processing.
        /// </summary>
        /// <param name="template">Template in form of the list of lines.</param>
        /// <param name="hash">Data for merging with template.</param>
        /// <returns>Resulting content.</returns>
        private String ProcessTemplate(ArrayList template, Hashtable hash) {
            if (this.context.IsMobile) {
                if (hash == null)
                    hash = new Hashtable();
                hash["[#Is_Mobile]"] = 1;
            }
            var trim_line = true;
            var trim_end = "\n";
            var if_mode = 0;
            var repeat_mode = 0;
            var if_buf = new ArrayList();
            var repeat_buf = new ArrayList();
            var if_what = "";
            var repeat_what = "";
            var content = "";
            for (int n = 0; n < template.Count; n++) {
                var line = (String)template[n];
                var line_no_comments = TrimComments(line);
                if (if_mode > 0) {
                    if (line_no_comments.IndexOf("#if") == 0)
                        if_mode++;
                    if (line_no_comments.IndexOf("#end if") == 0) {
                        if (if_mode == 1) {
                            var not = (if_what.IndexOf("!") == 0);
                            var eq = (if_what.IndexOf("==") != -1);
                            var neq = (if_what.IndexOf("!=") != -1);
                            var process_flag = false;
                            if (not == true) {
                                if (!hash.ContainsKey(if_what.Substring(1))) //TODO
                                    process_flag = true;
                            }
                            else {
                                if (eq) {
                                    String[] if_what_array = Strings.Split("==", if_what);
                                    String if_what_1 = if_what_array[0];
                                    String if_what_2 = if_what_array[1];
                                    if (hash.ContainsKey(if_what_1) && EQ(hash[if_what_1], if_what_2))
                                        process_flag = true;
                                }
                                else if (neq) {
                                    String[] if_what_array = Strings.Split("!=", if_what);
                                    String if_what_1 = if_what_array[0];
                                    String if_what_2 = if_what_array[1];
                                    if (hash.ContainsKey(if_what_1) && !EQ(hash[if_what_1], if_what_2))
                                        process_flag = true;
                                }
                                else if (hash.ContainsKey(if_what))
                                    process_flag = true;
                            }

                            if (process_flag)
                                content += (ProcessTemplate(if_buf, hash));
                            if_buf = new ArrayList();
                        }
                        else
                            if_buf.Add(line);
                        if_mode--;
                    }
                    else
                        if_buf.Add(line);
                }
                else if (repeat_mode > 0) {
                    if (line_no_comments.IndexOf("#repeat") == 0)
                        repeat_mode++;
                    if (line_no_comments.IndexOf("#end repeat") == 0) {
                        if (repeat_mode == 1) {
                            if (hash.ContainsKey(repeat_what)) {
                                var rows = (ArrayList)hash[repeat_what];
                                for (int r = 0; r < rows.Count; r++)
                                    content += (ProcessTemplate(repeat_buf, (Hashtable)rows[r]));
                                hash.Remove(repeat_what);
                            }
                            repeat_buf = new ArrayList();
                        }
                        else
                            repeat_buf.Add(line);
                        repeat_mode--;
                    }
                    else
                        repeat_buf.Add(line);
                }
                else {
                    if (line_no_comments.IndexOf("#if") == 0) {
                        if_mode = repeat_mode > 0 ? 2 : 1;
                        if_what = line_no_comments.Substring(4).Trim();
                    }
                    else if (line_no_comments.IndexOf("#repeat") == 0) {
                        repeat_mode++;
                        repeat_what = line_no_comments.Substring(8).Trim();
                        repeat_buf = new ArrayList();
                    }
                    else {
                        if (trim_line) {
                            line = line.Trim();
                            line += (trim_end);
                        }
                        content += (line);
                    }
                }
            }
            var result = FormatTemplate(content, hash);
            return result;
        }
    }
}