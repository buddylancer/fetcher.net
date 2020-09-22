// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

// Note: this class is not ported (is fully specific for NET-version).

namespace Bula.Objects
{
    using System;
    using System.IO;

    using Bula.Objects;
    using System.Collections;

    /// <summary>
    /// Helper class for manipulation with Files and Directories.
    /// </summary>
    public class Helper : Bula.Meta {
        private static String last_error = null;

    	/// <summary>
    	/// Get last error (if any).
    	/// </summary>
        /// <returns>Last error message.</returns>
        public static String LastError() {
            return last_error;
        }

        /// <summary>
        /// Check whether file exists.
        /// </summary>
        /// <param name="path">File name.</param>
        public static Boolean FileExists(String path) {
            return File.Exists(path);
    	}

    	/// <summary>
    	/// Check whether file exists.
    	/// </summary>
        /// <param name="path">File name.</param>
    	public static Boolean DirExists(String path) {
            return Directory.Exists(path);
    	}

        /// <summary>
        /// Create directory.
        /// </summary>
        /// <param name="path">Directory path to create.</param>
        /// <returns>True - created OK, False - error.</returns>
        public static Boolean CreateDir(String path) {
            try {
                DirectoryInfo dirInfo = Directory.CreateDirectory(path);
            }
            catch (Exception ex) {
                last_error = ex.Message.ToString();
                return false;
            }
            return true;
    	}

    	/// <summary>
    	/// Delete file.
    	/// </summary>
        /// <param name="path">File name.</param>
        /// <returns>True - OK, False - error.</returns>
        public static Boolean DeleteFile(String path) {
            try {
                File.Delete(path);
            }
            catch (Exception ex) {
                last_error = ex.Message.ToString();
                return false;
            }
            return true;
    	}

    	/// <summary>
    	/// Delete directory (recursively).
    	/// </summary>
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

    	/// <summary>
    	/// Remove directory.
    	/// </summary>
        /// <param name="path">Directory name.</param>
        /// <returns>True - OK, False - error.</returns>
        public static Boolean RemoveDir(String path) {
            try {
                Directory.Delete(path);
            }
            catch (Exception ex) {
                last_error = ex.Message.ToString();
                return false;
            }
            return true;
        }

    	/// <summary>
    	/// Read all content of text file.
    	/// </summary>
        /// <param name="filename">File name.</param>
        /// <returns>Resulting content.</returns>
        public static String ReadAllText(String filename) {
            return ReadAllText(filename, null); }

    	/// <summary>
    	/// Read all content of text file.
    	/// </summary>
        /// <param name="filename">File name.</param>
        /// <param name="encoding">Encoding name [optional].</param>
        /// <returns>Resulting content.</returns>
        public static String ReadAllText(String filename, String encoding) {
            if (encoding == null)
                return File.ReadAllText(filename);
            else
                return File.ReadAllText(filename, System.Text.Encoding.GetEncoding(encoding));
        }

      	/// <summary>
      	/// Read all content of text file as list of lines.
      	/// </summary>
        /// <param name="filename">File name.</param>
        /// <returns>Resulting content (lines).</returns>
        public static Object[] ReadAllLines(String filename) {
            return ReadAllLines(filename, null); }

      	/// <summary>
      	/// Read all content of text file as list of lines.
      	/// </summary>
        /// <param name="filename">File name.</param>
        /// <param name="encoding">Encoding name [optional].</param>
        /// <returns>Resulting content (lines).</returns>
        public static Object[] ReadAllLines(String filename, String encoding) {
            if (encoding == null)
                return File.ReadAllLines(filename);
            else
                return File.ReadAllLines(filename, System.Text.Encoding.GetEncoding(encoding));
    	}

    	/// <summary>
    	/// Write content to text file.
    	/// </summary>
        /// <param name="filename">File name.</param>
        /// <param name="text">Content to write.</param>
        /// <returns>Result of operation (true - OK, false - error).</returns>
        public static Boolean WriteText(String filename, String text) {
            File.WriteAllText(filename, text); /*, encoding); */ return true;
        }

    	/// <summary>
    	/// Append content to text file.
    	/// </summary>
        /// <param name="filename">File name.</param>
        /// <param name="text">Content to append.</param>
        /// <returns>Result of operation (true - OK, false - error).</returns>
        public static Boolean AppendText(String filename, String text) {
            File.AppendAllText(filename, text); /*, encoding); */ return true;
        }

        /// <summary>
        /// Check whether given path is a file.
        /// </summary>
        /// <param name="path">Path of an object.</param>
        /// <returns>True - is a file.</returns>
        public static Boolean IsFile(String path) {
            return File.Exists(path) && (File.GetAttributes(path) & FileAttributes.Directory) == 0;
        }

        /// <summary>
        /// Check whether given path is a directory.
        /// </summary>
        /// <param name="path">Path of an object.</param>
        /// <returns>True - is a directory.</returns>
        public static Boolean IsDir(String path) {
            return Directory.Exists(path) && (File.GetAttributes(path) & FileAttributes.Directory) != 0;
        }

        /// <summary>
        /// Test the chain of (sub)folder(s), create them if necessary.
        /// </summary>
        /// <param name="folder">Folder's full path.</param>
        public static void TestFolder(String folder)
        {
            String[] chunks = folder.Split(new char[] { '/' });
            var pathname = (String)null;
            for (int n = 0; n < SIZE(chunks); n++)
            {
                pathname = CAT(pathname, chunks[n]);
                if (!Helper.DirExists(pathname))
                    Helper.CreateDir(pathname);
                pathname = CAT(pathname, "/");
            }
        }

        /// <summary>
        /// Test the chain of (sub)folder(s) and file, create if necessary.
        /// </summary>
        /// <param name="filename">Filename's full path</param>
        public static void TestFileFolder(String filename)
        {
            String[] chunks = filename.Split(new char[] { '/' });
            var pathname = (String)null;
            for (int n = 0; n < SIZE(chunks) - 1; n++)
            {
                pathname = CAT(pathname, chunks[n]);
                if (!Helper.DirExists(pathname))
                    Helper.CreateDir(pathname);
                pathname = CAT(pathname, "/");
            }
        }

        /// <summary>
        /// List (enumerate) entries of a given path.
        /// </summary>
        /// <param name="path">Path of a directory.</param>
        /// <returns>Enumerated entries.</returns>
        public static IEnumerator ListDirEntries(String path) {
            String[] entries = Directory.GetDirectories(path);
            String[] files = Directory.GetFiles(path);

            ArrayList allEntries = Arrays.CreateArrayList(entries);
            ArrayList fileEntries = Arrays.CreateArrayList(files);
            allEntries = Arrays.MergeArrayList(allEntries, fileEntries);

            return allEntries.ToArray(typeof(String)).GetEnumerator();
        }
    }
}