namespace Bula.Fetcher.Controller {
    using System;

    using Bula.Objects;

    /**
     * Simple logger.
     */
    public class Logger : Bula.Meta {
        private String file_name = null;

        public void Init(String filename) {
            this.file_name = filename;
            if (filename.Length != 0) {
                if (Helper.FileExists(filename))
                    Helper.DeleteFile(filename);
            }
        }

        public void Output(String buffer) {
            if (this.file_name == null) {
                Response.Write(buffer);
                return;
            }
            if (Helper.FileExists(this.file_name))
                Helper.AppendText(this.file_name, buffer);
            else
                Helper.WriteText(this.file_name, buffer);

        }

        public void Time(String text) {
            this.Output(CAT(text, " -- ", DateTimes.Format("H:i:s"), "<br/>\r\n"));
        }
    }
}