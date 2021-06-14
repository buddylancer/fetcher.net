// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Objects {
    using System;
    using System.Collections;

    using System.Globalization;

    using Bula.Objects;

    /// <summary>
    /// Helper class to manipulate with Date and Times.
    /// </summary>
    public class DateTimes : Bula.Meta {
        /// Date/time format for processing GMT date/times 
        public const String GMT_DTS = "dd-MMM-yyyy HH:mm \\G\\M\\T";
        /// Date/time format for RSS operations 
        public const String XML_DTS = "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T";
        /// Date/time format for DB operations 
        public const String SQL_DTS = "yyyy-MM-dd HH:mm:ss";
        /// Format of log-file name. 
        public const String LOG_DTS = "yyyy-MM-dd_HH-mm-ss";
        /// Format of date/time in RSS-feeds. 
        public const String RSS_DTS = "ddd, dd MMM yyyy HH:mm:ss zzz";
        private static DateTime unix = new DateTime(1970, 1, 1);

        /// <summary>
        /// Get current time as Unix timestamp.
        /// </summary>
        /// <returns>Resulting time (Unix timestamp).</returns>
        public static long GetTime() {
            return (int)DateTime.Now.Subtract(unix).TotalSeconds;
        }

        /// <summary>
        /// Get time as Unix timestamp.
        /// </summary>
        /// <param name="timeString">Input string.</param>
        /// <returns>Resulting time (Unix timestamp).</returns>
        public static long GetTime(String timeString) {
            return (long)DateTime.Parse(timeString).Subtract(unix).TotalSeconds;
        }

        /// <summary>
        /// Get Unix timestamp from date/time extracted from RSS-feed.
        /// </summary>
        /// <param name="timeString">Input string.</param>
        /// <returns>Resulting timestamp.</returns>
        public static long FromRss(String timeString) {
            timeString = timeString.Replace("PDT", "-07:00");
            timeString = timeString.Replace("PST", "-08:00");
            return (int)DateTime.ParseExact(timeString, RSS_DTS,
                DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None).ToUniversalTime().Subtract(unix).TotalSeconds;
        }

        /// <summary>
        /// Format to string presentation.
        /// </summary>
        /// <param name="formatString">Format to apply.</param>
        /// <returns>Resulting string.</returns>
        public static String Format(String formatString) {
            return Format(formatString, 0);
        }

        /// <summary>
        /// Format time from Unix timestamp to string presentation.
        /// </summary>
        /// <param name="formatString">Format to apply.</param>
        /// <param name="timeValue">Input time value (Unix timestamp).</param>
        /// <returns>Resulting string.</returns>
        public static String Format(String formatString, long timeValue) {
            return (timeValue == 0 ? DateTime.Now : unix.AddSeconds((double)timeValue)).ToString(formatString);
        }

        /// <summary>
        /// Format current time to GMT string presentation.
        /// </summary>
        /// <param name="formatString">Format to apply.</param>
        /// <returns>Resulting string.</returns>
        public static String GmtFormat(String formatString) {
            return GmtFormat(formatString, 0);
        }

        /// <summary>
        /// Format time from timestamp to GMT string presentation.
        /// </summary>
        /// <param name="formatString">Format to apply.</param>
        /// <param name="timeValue">Input time value (Unix timestamp).</param>
        /// <returns>Resulting string.</returns>
        public static String GmtFormat(String formatString, long timeValue) {
            return (timeValue == 0 ? DateTime.UtcNow : unix.AddSeconds((double)timeValue)).ToString(formatString);
        }
    }
}