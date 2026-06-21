using System.Windows.Media;
using Styx.Helpers;

namespace ObjectGatherer.Helpers {
    public class CustomLog {
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

        private static string PreviousNormal { get; set; }
        private static string PreviousDiagnostic { get; set; }
        private static object[] PreviousArgs { get; set; }
        public static string Name { get; set; }

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static void Normal(string message, params object[] args) {
            if(message == PreviousNormal) {
                if(args.Length > 0) {
                    if(args == PreviousArgs) {
                        return;
                    }
                } else {
                    return;
                }
            }

            if(args.Length > 0) {
                PreviousArgs = args;
            }

            PreviousNormal = message;
            CustomNormalLog(message, args);
        }

        public static void Diagnostic(string message, params object[] args) {
            if(message == PreviousDiagnostic) {
                if(args.Length > 0) {
                    if(args == PreviousArgs) {
                        return;
                    }
                } else {
                    return;
                }
            }

            if(args.Length > 0) {
                PreviousArgs = args;
            }

            PreviousDiagnostic = message;
            CustomDiagnosticLog(message, args);
        }

        public static void NormalSpam(string message, params object[] args) {
            CustomNormalLog(message, args);
        }

        public static void DiagnosticSpam(string message, params object[] args) {
            CustomDiagnosticLog(message, args);
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

        private static void CustomNormalLog(string message, params object[] args) {
            if(args.Length <= 0) {
                Logging.Write(Colors.DeepSkyBlue, string.Format("[{0}]: ", Name) + message);
                return;
            }

            Logging.Write(Colors.DeepSkyBlue, string.Format("[{0}]: ", Name) + message, args);
        }

        private static void CustomDiagnosticLog(string message, params object[] args) {
            if(args.Length <= 0) {
                Logging.WriteDiagnostic(Colors.DeepSkyBlue, string.Format("[{0} Diag]: ", Name) + message);
                return;
            }

            Logging.WriteDiagnostic(Colors.DeepSkyBlue, string.Format("[{0} Diag]: ", Name) + message, args);
        }
    }
}
