using System;
using System.Drawing;
using System.Globalization;

namespace Templar.Helpers {
    public class Conversion {

        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        #region To days
        public static double MillisecondsToDays(double milliseconds) {
            return TimeSpan.FromMilliseconds(milliseconds).TotalDays;
        }

        public static double SecondsToDays(double seconds) {
            return TimeSpan.FromSeconds(seconds).TotalDays;
        }

        public static double MinutesToDays(double minutes) {
            return TimeSpan.FromMinutes(minutes).TotalDays;
        }

        public static double HoursToDays(double hours) {
            return TimeSpan.FromHours(hours).TotalDays;
        }
        #endregion

        #region To hours
        public static double MillisecondsToHours(double milliseconds) {
            return TimeSpan.FromMilliseconds(milliseconds).TotalHours;
        }

        public static double SecondsToHours(double seconds) {
            return TimeSpan.FromSeconds(seconds).TotalHours;
        }

        public static double MinutesToHours(double minutes) {
            return TimeSpan.FromMinutes(minutes).TotalHours;
        }

        public static double DaysToHours(double days) {
            return TimeSpan.FromHours(days).TotalHours;
        }
        #endregion

        #region To minutes
        public static double MillisecondsToMinutes(double milliseconds) {
            return TimeSpan.FromMilliseconds(milliseconds).TotalMinutes;
        }

        public static double SecondsToMinutes(double seconds) {
            return TimeSpan.FromSeconds(seconds).TotalMinutes;
        }

        public static double HoursToMinutes(double hours) {
            return TimeSpan.FromHours(hours).TotalMinutes;
        }

        public static double DaysToMinutes(double days) {
            return TimeSpan.FromDays(days).TotalMinutes;
        }
        #endregion

        #region To seconds
        public static double MillisecondsToSeconds(double milliseconds) {
            return TimeSpan.FromMilliseconds(milliseconds).TotalSeconds;
        }

        public static double MinutesToSeconds(double minutes) {
            return TimeSpan.FromMinutes(minutes).TotalSeconds;
        }

        public static double HoursToSeconds(double hours) {
            return TimeSpan.FromHours(hours).TotalSeconds;
        }

        public static double DaysToSeconds(double days) {
            return TimeSpan.FromDays(days).TotalSeconds;
        }
        #endregion

        #region To milliseconds
        public static double SecondsToMilliseconds(double seconds) {
            return TimeSpan.FromSeconds(seconds).TotalMilliseconds;
        }

        public static double MinutesToMilliseconds(double minutes) {
            return TimeSpan.FromMinutes(minutes).TotalMilliseconds;
        }

        public static double HoursToMilliseconds(double hours) {
            return TimeSpan.FromHours(hours).TotalMilliseconds;
        }

        public static double DaysToMilliseconds(double days) {
            return TimeSpan.FromDays(days).TotalMilliseconds;
        }
        #endregion

        public static string PrettyTime(TimeSpan duration, bool displayDays, bool displayHours, bool displayMinutes, bool displaySeconds, bool displayMilliseconds, bool isLongLength) {
            var output = "";

            if(displayDays) {
                output += "{0}";

                if(isLongLength) {
                    output += "d";
                }
            }

            if(displayHours) {
                if(!isLongLength && displayDays) {
                    output += ":";
                }

                output += "{1:D2}";

                if(isLongLength) {
                    output += "h";
                }
            }

            if(displayMinutes) {
                if(!isLongLength && displayHours) {
                    output += ":";
                }

                output += "{2:D2}";

                if(isLongLength) {
                    output += "m";
                }
            }

            if(displaySeconds) {
                if(!isLongLength && displayMinutes) {
                    output += ":";
                }

                output += "{3:D2}";

                if(isLongLength) {
                    output += "s";
                }
            }

            if(displayMilliseconds) {
                if(!isLongLength && displaySeconds) {
                    output += ".";
                }

                output += "{4:D3}";

                if(isLongLength) {
                    output += "ms";
                }
            }

            return string.Format(output, duration.Days, duration.Hours, duration.Minutes, duration.Seconds, duration.Milliseconds);
        }

        public static Color ConvertHexToRGB(string hexColor) {
            // Remove # if present
            if(hexColor.IndexOf("#", StringComparison.Ordinal) != -1) {
                hexColor = hexColor.Replace("#", "");
            }

            var red = 0;
            var green = 0;
            var blue = 0;

            // #RRGGBB
            if(hexColor.Length == 6) {
                red = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                green = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                blue = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
            }

            // #RGB
            if(hexColor.Length == 3) {
                red = int.Parse(hexColor[0].ToString(CultureInfo.InvariantCulture) + hexColor[0], NumberStyles.AllowHexSpecifier);
                green = int.Parse(hexColor[1].ToString(CultureInfo.InvariantCulture) + hexColor[1], NumberStyles.AllowHexSpecifier);
                blue = int.Parse(hexColor[2].ToString(CultureInfo.InvariantCulture) + hexColor[2], NumberStyles.AllowHexSpecifier);
            }

            return Color.FromArgb(red, green, blue);
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

    }
}
