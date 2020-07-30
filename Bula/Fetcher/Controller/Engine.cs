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
        private static ArrayList engine_instances = null;
        private static int current_index = -1;

        private Boolean print_flag = false;
        private String print_string = "";

        /// Public default constructor 
        public Engine () {
            if (engine_instances == null)
                engine_instances = new ArrayList();
            this.print_flag = false;
            this.print_string = "";
        }

        /// <summary>
        /// Push engine.
        /// </summary>
        /// <param name="print_flag">Whether to print content immediately (true) or save it for further processing (false).</param>
        public static void Push(Boolean print_flag) {
            var engine = new Engine();
            engine.print_flag = print_flag;
            current_index++;
            if (engine_instances == null)
                engine_instances = new ArrayList();
            if (engine_instances.Count <= current_index)
                engine_instances.Add(engine);
            else
                engine_instances[current_index] = engine;
        }

        /// Pop engine back. 
        public static void Pop() {
            SetPrintString(null);
            current_index--;
        }

        /// <summary>
        /// Set print string for current engine instance.
        /// </summary>
        /// <param name="val">Print string to set.</param>
        private static void SetPrintString(String val) {
            var engine = (Engine)engine_instances[current_index];
            engine.print_string = val;
        }

        /// <summary>
        /// Get print string for current engine instance.
        /// </summary>
        /// <returns>Current print string.</returns>
        private static String GetPrintString() {
            var engine = (Engine)engine_instances[current_index];
            return engine.print_string;
        }

        /// <summary>
        /// Write string.
        /// </summary>
        /// <param name="val">String to write.</param>
        public static void Write(String val) {
            var engine = (Engine)engine_instances[current_index];
            if (engine.print_flag)
                Response.Write(val);
            else
                engine.print_string += val;
        }

        //TODO!!!
        //public String ReadCachedFile(String id) {
        //    String content = GetFile("_cache", id);
        //}

        //public int WriteCachedFile(String id, String content) {
        //    String filename = CachedFileName(id);
        //}

        //public String IncludeCashedTemplate(String id, String filename, Hashtable args = null) {
        //    String content = ReadCachedFile(id);
        //    if (content == null) {
        //        content = IncludeTemplate(filename, args);
        //        WriteCachedFile(id, content);
        //    }
        //    return content;
        //}

        /// <summary>
        /// Include file with class and generate content by calling method Execute().
        /// </summary>
        /// <param name="class_name">Class name to include.</param>
        /// <returns>Resulting content.</returns>
        public static String IncludeTemplate(String class_name) {
            Push(false);
            var file_name = 
                CAT(class_name, ".cs");

            var content = (String)null;
            if (Helper.FileExists(CAT(Config.LocalRoot, file_name))) {
                //Config.IncludeFile(file_name);
                Internal.CallStaticMethod(class_name, "Execute");
                content = GetPrintString();
            }
            else
                content = CAT("No such file: ", file_name);
            Pop();
            return content;
        }

        /// <summary>
        /// Include file with class and check for errors by calling method Check().
        /// </summary>
        /// <param name="class_name">Class name to include.</param>
        /// <returns>Resulting content.</returns>
        public static String CheckForErrors(String class_name) {
            Push(false);
            var file_name = 
                CAT(class_name, ".cs");

            var content = (String)null;
            if (Helper.FileExists(CAT(Config.LocalRoot, file_name))) {
                //Config.IncludeFile(file_name);
                Internal.CallStaticMethod(class_name, "check");
                content = GetPrintString();
            }
            else
                content = CAT("No such file: ", file_name);
            Pop();
            return content;
        }

        /// <summary>
        /// Show template content.
        /// </summary>
        /// <param name="filename">Template file to use.</param>
        /// <returns>Resulting content.</returns>
        public static String ShowTemplate(String filename) {
            return ShowTemplate(filename, null); }

        /// <summary>
        /// Show template content by merging template and data.
        /// </summary>
        /// <param name="filename">Template file to use for merging.</param>
        /// <param name="hash">Data in the form of Hashtable to use for merging.</param>
        /// <returns>Resulting content.</returns>
        public static String ShowTemplate(String filename, Hashtable hash) {
            var template = GetTemplate(filename);

            var content = "";
            content += (CAT("\n<!-- BEGIN ", Strings.Replace("Bula/Fetcher/", "", filename), " -->\n"));
            content += (ProcessTemplate(template, hash));
            content += (CAT("<!-- END ", Strings.Replace("Bula/Fetcher/", "", filename), " -->\n"));
            return content;
        }

        /// <summary>
        /// Get template as the list of lines.
        /// </summary>
        /// <param name="filename">File name.</param>
        /// <returns>Resulting array with lines.</returns>
        private static ArrayList GetTemplate(String filename) {
            if (Helper.FileExists(CAT(Config.LocalRoot, filename))) {
                Object[] lines = Helper.ReadAllLines(CAT(Config.LocalRoot, filename));
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
        public static String FormatTemplate(String template, Hashtable hash) {
            if (hash == null)
                hash = new Hashtable();
            var content = Strings.ReplaceInTemplate(template, hash);
            return Strings.ReplaceInTemplate(content, Config.GlobalConstants);
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
        private static String ProcessTemplate(ArrayList template, Hashtable hash) {
            if (Config.IsMobile)
                hash["[#Is_Mobile]"] = "1";

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
                                    String if_what_1 = if_what_array[0].Trim();
                                    String if_what_2 = if_what_array[1].Trim();
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