namespace Bula.Objects {
    using System;
    using System.IO;

    using Bula.Objects;
    using System.Collections;

    /**
     * Helper class for manipulation with Files & Directories.
     */
    public class Helper : Bula.Meta {
    	///Check whether file exists.
        /// <param name="path">File name.</param>
        public static Boolean FileExists(String path) {
            return File.Exists(path);
    	}

    	///Check whether file exists.
        /// <param name="path">File name.</param>
    	public static Boolean DirExists(String path) {
            return Directory.Exists(path);
    	}

        ///Create directory.
        /// @param String path
        /// <returns>True - created OK, False - error.</returns>
        public static Boolean CreateDir(String path) {
            try {
                DirectoryInfo dirInfo = Directory.CreateDirectory(path);
            }
            catch (Exception ex) {
                return false;
            }
            return true;
    	}

    	///Delete file.
        /// <param name="path">File name.</param>
        /// <returns>True - OK, False - error.</returns>
        public static Boolean DeleteFile(String path) {
            try {
                File.Delete(path);
            }
            catch (Exception ex) {
                return false;
            }
            return true;
    	}

    	///Delete directory.
        /// <param name="path">Directory name.</param>
        /// <returns>True - OK, False - error.</returns>
    	public static Boolean DeleteDir(String path) {

            if (!DirExists(path))
                return false;

            var entries = ListDirEntries(path);
            while (entries.MoveNext()) {
                var entry = CAT(entries.Current);

                if (IsFile(entry))
                    DeleteFile(entry);
                else if (IsDir(entry))
                    DeleteDir(entry);
            }
    		return RemoveDir(path);
    	}

        public static Boolean RemoveDir(String path) {
            try {
                Directory.Delete(path);
            }
            catch (Exception ex) {
                return false;
            }
            return true;
        }

        public static String ReadAllText(String filename) {
            return ReadAllText(filename, null); }

    	///Read all content of text file.
        /// <param name="filename">File name.</param>
        /// <param name="encoding">Encoding name [optional].</param>
        /// <returns>Resulting content.</returns>
        public static String ReadAllText(String filename, String encoding) {
            return File.ReadAllText(filename, System.Text.Encoding.GetEncoding(encoding));
    	}

        public static Object[] ReadAllLines(String filename) {
            return ReadAllLines(filename, null); }

      	///Read all content of text file as list of lines.
        /// <param name="filename">File name.</param>
        /// <param name="encoding">Encoding name [optional].</param>
        /// <returns>Resulting content.</returns>
        public static Object[] ReadAllLines(String filename, String encoding) {
            if (encoding == null)
                return File.ReadAllLines(filename);
            else
                return File.ReadAllLines(filename, System.Text.Encoding.GetEncoding(encoding));
    	}

        public static Boolean WriteText(String filename, String text) {
            File.WriteAllText(filename, text); /*, encoding); */ return true;
        }

        public static Boolean AppendText(String filename, String text) {
            File.AppendAllText(filename, text); /*, encoding); */ return true;
        }

        ///Check whether given path is a file.
        /// <param name="path">Path of an object.</param>
        /// <returns>True - is a file.</returns>
        public static Boolean IsFile(String path) {
            return File.Exists(path) && (File.GetAttributes(path) & FileAttributes.Directory) == 0;
        }

        ///Check whether given path is a directory.
        /// <param name="path">Path of an object.</param>
        /// <returns>True - is a directory.</returns>
        public static Boolean IsDir(String path) {
            return Directory.Exists(path) && (File.GetAttributes(path) & FileAttributes.Directory) != 0;
        }

        public static IEnumerator ListDirEntries(String path) {
            String[] entries = Directory.GetDirectories(path);
            String[] files = Directory.GetFiles(path);
            entries = (String[])Arrays.MergeArray(entries, files);
            return entries.GetEnumerator();
        }
    }
}