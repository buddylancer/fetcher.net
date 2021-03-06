// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller {
    using System;
    using System.Collections;

    using Bula;
    using Bula.Fetcher;
    using Bula.Objects;

    /// <summary>
    /// Engine for processing templates.
    /// </summary>
    public class Engine : Bula.Meta {
        /// Instance of Context 
        public Context context = null;
        private Boolean printFlag = false;
        private String printString = "";

        /// Public default constructor 
        public Engine (Context context) {
            this.context = context;
            this.printFlag = false;
            this.printString = "";
        }

        /// <summary>
        /// Set print string for current engine instance.
        /// </summary>
        /// <param name="val">Print string to set.</param>
        public void SetPrintString(String val) {
            this.printString = val;
        }

        /// <summary>
        /// Get print string for current engine instance.
        /// </summary>
        /// <returns>Current print string.</returns>
        public String GetPrintString() {
            return this.printString;
        }

        /// <summary>
        /// Set print flag for current engine instance.
        /// </summary>
        /// <param name="val">Print flag to set.</param>
        public void SetPrintFlag(Boolean val) {
            this.printFlag = val;
        }

        /// <summary>
        /// Get print flag for current engine instance.
        /// </summary>
        /// <returns>Current print flag.</returns>
        public Boolean GetPrintFlag() {
            return this.printFlag;
        }

        /// <summary>
        /// Write string.
        /// </summary>
        /// <param name="val">String to write.</param>
        public void Write(String val) {
            if (this.printFlag) {
                var langFile = (String)null;
                if (BLANK(this.context.Api) && Config.SITE_LANGUAGE != null)
                    langFile = CAT(this.context.LocalRoot, "local/", Config.SITE_LANGUAGE, ".txt");
                this.context.Response.Write(val, langFile);
            }
            else
                this.printString += val;
        }

        /// <summary>
        /// Include file with class and generate content by calling method Execute().
        /// </summary>
        /// <param name="className">Class name to include.</param>
        public String IncludeTemplate(String className) {
            return IncludeTemplate(className, "Execute");
        }

        /// <summary>
        /// Include file with class and generate content by calling method.
        /// </summary>
        /// <param name="className">Class name to include.</param>
        /// <param name="defaultMethod">Default method to call.</param>
        /// <returns>Resulting content.</returns>
        public String IncludeTemplate(String className, String defaultMethod) {
            var engine = this.context.PushEngine(false);
            var prefix = CAT(Config.FILE_PREFIX, "Bula/Fetcher/Controller/");
            var fileName =
                CAT(prefix, className, ".cs");

            var content = (String)null;
            if (Helper.FileExists(CAT(this.context.LocalRoot, fileName))) {
                TArrayList args0 = new TArrayList(); args0.Add(this.context);
                Internal.CallMethod(CAT(prefix, className), args0, defaultMethod, null);
                content = engine.GetPrintString();
            }
            else
                content = CAT("No such file: ", fileName);
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
        /// <param name="id">Template ID to use for merging.</param>
        /// <param name="hash">Data in the form of THashtable to use for merging.</param>
        /// <returns>Resulting content.</returns>
        public String ShowTemplate(String id, THashtable hash) {
            var ext = BLANK(this.context.Api) ? ".html" : (Config.API_FORMAT == "Xml"? ".xml" : ".txt");
            var prefix = CAT(Config.FILE_PREFIX, "Bula/Fetcher/View/");

            var filename =
                    CAT(prefix, (BLANK(this.context.Api) ? "Html/" : (Config.API_FORMAT == "Xml"? "Xml/" : "Rest/")), id, ext);
            var template = this.GetTemplate(filename);

            var content = "";
            var short_name = Strings.Replace("Bula/Fetcher/View/Html", "View", filename);
            if (!BLANK(Config.FILE_PREFIX))
                short_name = Strings.Replace(Config.FILE_PREFIX, "", short_name);
            if (BLANK(this.context.Api))
                content += CAT(EOL, "<!-- BEGIN ", short_name, " -->", EOL);
            if (!BLANK(template))
                content += this.ProcessTemplate(template, hash);
            if (BLANK(this.context.Api))
                content += CAT("<!-- END ", short_name, " -->", EOL);
            return content;
        }

        /// <summary>
        /// Get template as the list of lines.
        /// </summary>
        /// <param name="filename">File name.</param>
        /// <returns>Resulting array with lines.</returns>
        private TArrayList GetTemplate(String filename) {
            if (Helper.FileExists(CAT(this.context.LocalRoot, filename))) {
                Object[] lines = Helper.ReadAllLines(CAT(this.context.LocalRoot, filename));
                return TArrayList.CreateFrom(lines);
            }
            else {
                var temp = new TArrayList();
                temp.Add(CAT("File not found -- '", filename, "'<hr/>"));
                return temp;
            }
        }

        /// <summary>
        /// Do actual merging of template and data.
        /// </summary>
        /// <param name="template">Template content.</param>
        /// <param name="hash">Data for merging with template.</param>
        /// <returns>Resulting content.</returns>
        public String FormatTemplate(String template, THashtable hash) {
            if (hash == null)
                hash = new THashtable();
            String content1 = Strings.ReplaceInTemplate(template, hash);
            String content2 = Strings.ReplaceInTemplate(content1, this.context.GlobalConstants);
            return content2;
        }

        /// <summary>
        /// Trim comments from input string.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <returns>Resulting string.</returns>
        private static String TrimComments(String str) {
            return TrimComments(str, true);
        }

        /// <summary>
        /// Trim comments from input string.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <param name="trim">Whether to trim spaces in resulting string.</param>
        /// <returns>Resulting string.</returns>
        private static String TrimComments(String str, Boolean trim) {
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
            if (trim)
                line = line.Trim();
            return line;
        }

        /// <summary>
        /// Execute template processing.
        /// </summary>
        /// <param name="template">Template in form of the list of lines.</param>
        /// <param name="hash">Data for merging with template.</param>
        /// <returns>Resulting content.</returns>
        private String ProcessTemplate(TArrayList template, THashtable hash) {
            if (this.context.IsMobile) {
                if (hash == null)
                    hash = new THashtable();
                hash["[#Is_Mobile]"] = 1;
            }
            var trimLine = true;
            var trimEnd = EOL;
            var ifMode = 0;
            var repeatMode = 0;
            var ifBuf = new TArrayList();
            var repeatBuf = new TArrayList();
            var ifWhat = "";
            var repeatWhat = "";
            var content = "";
            for (int n = 0; n < template.Size(); n++) {
                var line = (String)template[n];
                var lineNoComments = TrimComments(line); //, BLANK(this.context.Api)); //TODO
                if (ifMode > 0) {
                    if (lineNoComments.IndexOf("#if") == 0)
                        ifMode++;
                    if (lineNoComments.IndexOf("#end if") == 0) {
                        if (ifMode == 1) {
                            var not = (ifWhat.IndexOf("!") == 0);
                            var eq = (ifWhat.IndexOf("==") != -1);
                            var neq = (ifWhat.IndexOf("!=") != -1);
                            var processFlag = false;
                            if (not == true) {
                                if (!hash.ContainsKey(ifWhat.Substring(1))) //TODO
                                    processFlag = true;
                            }
                            else {
                                if (eq) {
                                    String[] ifWhatArray = Strings.Split("==", ifWhat);
                                    String ifWhat1 = ifWhatArray[0];
                                    String ifWhat2 = ifWhatArray[1];
                                    if (hash.ContainsKey(ifWhat1) && EQ(hash[ifWhat1], ifWhat2))
                                        processFlag = true;
                                }
                                else if (neq) {
                                    String[] ifWhatArray = Strings.Split("!=", ifWhat);
                                    String ifWhat1 = ifWhatArray[0];
                                    String ifWhat2 = ifWhatArray[1];
                                    if (hash.ContainsKey(ifWhat1) && !EQ(hash[ifWhat1], ifWhat2))
                                        processFlag = true;
                                }
                                else if (hash.ContainsKey(ifWhat))
                                    processFlag = true;
                            }

                            if (processFlag)
                                content += ProcessTemplate(ifBuf, hash);
                            ifBuf = new TArrayList();
                        }
                        else
                            ifBuf.Add(line);
                        ifMode--;
                    }
                    else
                        ifBuf.Add(line);
                }
                else if (repeatMode > 0) {
                    if (lineNoComments.IndexOf("#repeat") == 0)
                        repeatMode++;
                    if (lineNoComments.IndexOf("#end repeat") == 0) {
                        if (repeatMode == 1) {
                            if (hash.ContainsKey(repeatWhat)) {
                                var rows = (TArrayList)hash[repeatWhat];
                                for (int r = 0; r < rows.Size(); r++)
                                    content += ProcessTemplate(repeatBuf, (THashtable)rows[r]);
                                hash.Remove(repeatWhat);
                            }
                            repeatBuf = new TArrayList();
                        }
                        else
                            repeatBuf.Add(line);
                        repeatMode--;
                    }
                    else
                        repeatBuf.Add(line);
                }
                else {
                    if (lineNoComments.IndexOf("#if") == 0) {
                        ifMode = repeatMode > 0 ? 2 : 1;
                        ifWhat = lineNoComments.Substring(4).Trim();
                    }
                    else if (lineNoComments.IndexOf("#repeat") == 0) {
                        repeatMode++;
                        repeatWhat = lineNoComments.Substring(8).Trim();
                        repeatBuf = new TArrayList();
                    }
                    else {
                        if (trimLine) {
                            line = line.Trim();
                            line += trimEnd;
                        }
                        content += line;
                    }
                }
            }
            var result = FormatTemplate(content, hash);
            return result;
        }
    }
}