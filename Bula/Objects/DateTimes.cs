namespace Bula.Objects {
    using System;

    using System.Globalization;
    using Bula.Objects;

    /**
     * Helper class to manipulate with Date & Times.
     */
    public class DateTimes : Bula.Meta {
        public const String RSS_DTS = "ddd, dd MMM yyyy HH:mm:ss zzz";
        private static DateTime unix = new DateTime(1970, 1, 1);

        public static int GetTime() { return (int)DateTime.Now.Subtract(unix).TotalSeconds; }

        ///Get unix timestamp.
        /// <param name="time_string">Input string.</param>
        /// <returns>Resulting timestamp.</returns>
        public static int GetTime(String time_string) {
            return (int)DateTime.Parse(time_string).Subtract(unix).TotalSeconds;
        }

        public static int FromRss(String time_string) {
            time_string = time_string.Replace("PDT", "-07:00");
            time_string = time_string.Replace("PST", "-08:00");
            return (int)DateTime.ParseExact(time_string, RSS_DTS,
                DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None).ToUniversalTime().Subtract(unix).TotalSeconds;

        }

        ///Format to string presentation.
        /// <param name="format_string">Format to apply.</param>
        /// <returns>Resulting string.</returns>
        public static String Format(String format_string) {
            return Format(format_string, 0); }

        ///Format from unix timestamp to string presentation.
        /// <param name="format_string">Format to apply.</param>
        /// <param name="time_value">Input unix timestamp.</param>
        /// <returns>Resulting string.</returns>
        public static String Format(String format_string, int time_value) {
            return (time_value == 0 ? DateTime.Now : unix.AddSeconds((double)time_value)).ToString(format_string);
        }

        public static String GmtFormat(String format_string) {
            return GmtFormat(format_string, 0); }

        ///Format from timestamp to GMT string presentation.
        /// <param name="format_string">Format to apply.</param>
        /// <param name="time_value">Input timestamp.</param>
        /// <returns>Resulting string.</returns>
        public static String GmtFormat(String format_string, int time_value) {
            return (time_value == 0 ? DateTime.UtcNow : unix.AddSeconds((double)time_value)).ToString(format_string);
        }
    }
}