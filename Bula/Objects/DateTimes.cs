namespace Bula.Objects {
    using System;

    using Bula.Objects;

    /**
     * Helper class to manipulate with Date & Times.
     */
    public class DateTimes : Bula.Meta {
        private static System.DateTime unix = new System.DateTime(1970, 1, 1);

        ///Get unix timestamp.
        /// <param name="time_string">Input string.</param>
        /// <returns>Resulting timestamp.</returns>
        public static int GetTime()
        {
            return (int)System.DateTime.Now.Subtract(unix).TotalSeconds;
        }

        ///Get unix timestamp.
        /// <param name="time_string">Input string.</param>
        /// <returns>Resulting timestamp.</returns>
        public static int GetTime(String time_string) {
            return (int)System.DateTime.Parse(time_string).Subtract(unix).TotalSeconds;
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
            return (time_value == 0 ? System.DateTime.Now : unix.AddSeconds((double)time_value)).ToString(format_string);
        }

        public static String GmtFormat(String format_string) {
            return GmtFormat(format_string, 0); }

        ///Format from timestamp to GMT string presentation.
        /// <param name="format_string">Format to apply.</param>
        /// <param name="time_value">Input timestamp.</param>
        /// <returns>Resulting string.</returns>
        public static String GmtFormat(String format_string, int time_value) {
            return (time_value == 0 ? System.DateTime.UtcNow : unix.AddSeconds((double)time_value)).ToString(format_string);
        }
    }
}