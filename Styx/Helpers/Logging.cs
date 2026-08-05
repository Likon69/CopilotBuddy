using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;

// Alias to resolve ambiguity with System.Drawing.Color
using WpfColor = System.Windows.Media.Color;
using WpfColors = System.Windows.Media.Colors;
using DrawingColor = System.Drawing.Color;

namespace Styx.Helpers
{
    /// <summary>
    /// Log levels matching Honorbuddy WoD.
    /// </summary>
    public enum LogLevel
    {
        None = 0,
        Quiet = 1,
        Normal = 2,
        Verbose = 3,
        Diagnostic = 4
    }

    /// <summary>
    /// Logging system compatible with Honorbuddy WoD style.
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// Represents a single log message with level, color, timestamp and content.
        /// </summary>
        public class LogMessage
        {
            public LogLevel Level { get; set; }
            public WpfColor Color { get; set; }
            public DateTime Timestamp { get; set; }
            public string Message { get; set; }

            public LogMessage(LogLevel level, WpfColor color, DateTime timestamp, string message)
            {
                Level = level;
                Color = color;
                Timestamp = timestamp;
                Message = message;
            }

            public override string ToString()
            {
                return string.Format("[{0:HH:mm:ss.fff}] [{1}] {2}", Timestamp, Level.ToString()[0], Message);
            }
        }

        #region Properties

        /// <summary>
        /// Gets the application startup path.
        /// </summary>
        public static string ApplicationPath => Application.StartupPath;

        /// <summary>
        /// Gets the startup time.
        /// </summary>
        public static DateTime StartTime { get; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the current logging level for UI display.
        /// </summary>
        public static LogLevel LoggingLevel { get; set; } = LogLevel.Normal;

        /// <summary>
        /// Gets or sets the logging level for file output.
        /// </summary>
        public static LogLevel LogFileLevel { get; set; } = LogLevel.Diagnostic;

        /// <summary>
        /// Gets or sets whether file logging is enabled.
        /// </summary>
        public static bool FileLogging { get; set; } = true;

        /// <summary>
        /// Gets or sets the log file path.
        /// </summary>
        public static string LogFilePath { get; set; }

        #endregion

        #region Default Colors

        public static readonly WpfColor ColorQuiet = WpfColors.Gray;
        public static readonly WpfColor ColorNormal = WpfColors.WhiteSmoke;
        public static readonly WpfColor ColorVerbose = WpfColors.DarkOrange;
        public static readonly WpfColor ColorDiagnostic = WpfColors.Orange;
        public static readonly WpfColor ColorError = WpfColors.Red;

        #endregion

        #region Events

        /// <summary>
        /// Delegate for log message events.
        /// </summary>
        public delegate void LogMessageDelegate(ReadOnlyCollection<LogMessage> messages);

        /// <summary>
        /// Event fired when log messages are written.
        /// </summary>
        public static event LogMessageDelegate OnLogMessage;

        // Legacy event for backward compatibility
        public static event Action<LogLevel, string>? OnMessageLogged;

        #endregion

        #region Write Methods (with Color)

        public static void Write(LogLevel level, WpfColor color, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            var logMessage = new LogMessage(level, color, DateTime.Now, message);

            // Fire the new event
            OnLogMessage?.Invoke(new ReadOnlyCollection<LogMessage>(new[] { logMessage }));

            // Fire legacy event
            OnMessageLogged?.Invoke(level, logMessage.ToString());

            // Write to debug output
            Debug.WriteLine(logMessage.ToString());

            // Write to file if enabled
            if (FileLogging && level <= LogFileLevel)
            {
                WriteToFile(logMessage.ToString());
            }
        }

        public static void Write(LogLevel level, WpfColor color, string format, params object[] args)
        {
            Write(level, color, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        #endregion

        #region Write Methods (default colors)

        public static void Write(LogLevel level, string message)
        {
            Write(level, GetColorForLevel(level), message);
        }

        public static void Write(LogLevel level, string format, params object[] args)
        {
            Write(level, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public static void Write(WpfColor color, string message)
        {
            Write(LogLevel.Normal, color, message);
        }

        public static void Write(WpfColor color, string format, params object[] args)
        {
            Write(color, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public static void Write(string message)
        {
            Write(ColorNormal, message);
        }

        public static void Write(string format, params object[] args)
        {
            Write(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        #endregion

        #region Write Methods (System.Drawing.Color compatibility)

        /// <summary>
        /// Converts System.Drawing.Color to System.Windows.Media.Color
        /// </summary>
        private static WpfColor ToWpfColor(DrawingColor color)
        {
            return WpfColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static void Write(DrawingColor color, string message)
        {
            Write(ToWpfColor(color), message);
        }

        public static void Write(DrawingColor color, string format, params object[] args)
        {
            Write(ToWpfColor(color), string.Format(CultureInfo.InvariantCulture, format, args));
        }

        #endregion

        #region WriteQuiet

        public static void WriteQuiet(WpfColor color, string message)
        {
            Write(LogLevel.Quiet, color, message);
        }

        public static void WriteQuiet(WpfColor color, string format, params object[] args)
        {
            Write(LogLevel.Quiet, color, format, args);
        }

        public static void WriteQuiet(string message)
        {
            WriteQuiet(ColorQuiet, message);
        }

        public static void WriteQuiet(string format, params object[] args)
        {
            WriteQuiet(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        #endregion

        #region WriteVerbose

        public static void WriteVerbose(WpfColor color, string message)
        {
            Write(LogLevel.Verbose, color, message);
        }

        public static void WriteVerbose(WpfColor color, string format, params object[] args)
        {
            WriteVerbose(color, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public static void WriteVerbose(string message)
        {
            WriteVerbose(ColorVerbose, message);
        }

        public static void WriteVerbose(string format, params object[] args)
        {
            WriteVerbose(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        #endregion

        #region WriteDiagnostic

        public static void WriteDiagnostic(WpfColor color, string message)
        {
            Write(LogLevel.Diagnostic, color, message);
        }

        public static void WriteDiagnostic(WpfColor color, string format, params object[] args)
        {
            WriteDiagnostic(color, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public static void WriteDiagnostic(string message)
        {
            WriteDiagnostic(ColorDiagnostic, message);
        }

        public static void WriteDiagnostic(string format, params object[] args)
        {
            WriteDiagnostic(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        #endregion

        #region WriteException

        public static void WriteException(LogLevel logLevel, WpfColor color, Exception ex)
        {
            Write(logLevel, color, ex.ToString());
        }

        public static void WriteException(LogLevel logLevel, Exception ex)
        {
            WriteException(logLevel, ColorError, ex);
        }

        public static void WriteException(WpfColor color, Exception ex)
        {
            WriteException(LogLevel.Diagnostic, color, ex);
        }

        public static void WriteException(Exception ex)
        {
            WriteException(LogLevel.Diagnostic, ColorError, ex);
        }

        #endregion

        #region Specialized Methods

        public static void WriteDebug(string message)
        {
            WriteDiagnostic(message);
        }

        public static void WriteDebug(string format, params object[] args)
        {
            WriteDiagnostic(format, args);
        }

        public static void WriteDebug(System.Drawing.Color color, string message)
        {
            Write(LogLevel.Diagnostic, ToWpfColor(color), message);
        }

        public static void WriteDebug(System.Drawing.Color color, string format, params object[] args)
        {
            Write(LogLevel.Diagnostic, ToWpfColor(color), string.Format(format, args));
        }

        public static void WriteError(string message)
        {
            Write(LogLevel.Quiet, ColorError, "[ERROR] " + message);
        }

        public static void WriteWarning(string message)
        {
            Write(LogLevel.Normal, WpfColors.Yellow, "[WARNING] " + message);
        }

        public static void WriteNavigator(string message)
        {
            WriteVerbose("[Navigator] " + message);
        }

        public static void WriteNavigator(string format, params object[] args)
        {
            WriteNavigator(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public static void WriteNavigator(System.Drawing.Color color, string message)
        {
            Write(LogLevel.Verbose, ToWpfColor(color), "[Navigator] " + message);
        }

        public static void WriteNavigator(System.Drawing.Color color, string format, params object[] args)
        {
            WriteNavigator(color, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public static void WriteCombat(string message)
        {
            Write(LogLevel.Normal, WpfColors.LightGreen, "[Combat] " + message);
        }

        /// <summary>
        /// Writes a message to the log file only, without displaying it in the UI.
        /// Ported from HB 4.3.4 Logging.FileOnly.
        /// </summary>
        public static void FileOnly(string message)
        {
            WriteToFile($"[{DateTime.Now:HH:mm:ss.fff}] [F] {message}");
        }

        /// <summary>
        /// Writes a formatted message to the log file only, without displaying it in the UI.
        /// Ported from HB 4.3.4 Logging.FileOnly.
        /// </summary>
        public static void FileOnly(string format, params object[] args)
        {
            FileOnly(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        #endregion

        #region File Logging

        private static readonly object _queueLock = new object();
        private static readonly object _writeLock = new object();
        private static readonly Queue<string> _pendingLines = new Queue<string>();
        private static Thread _writerThread;

        public static void WriteToFileSync(LogLevel level, string message)
        {
            if (level <= LogFileLevel)
            {
                WriteToFile($"[{DateTime.Now:HH:mm:ss.fff}] [{level.ToString()[0]}] {message}");
                FlushPending();
            }
        }

        public static void WriteToFileSync(LogLevel level, string format, params object[] args)
        {
            WriteToFileSync(level, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        private static void WriteToFile(string message)
        {
            if (string.IsNullOrEmpty(LogFilePath))
                return;

            lock (_queueLock)
            {
                _pendingLines.Enqueue(message);

                if (_writerThread == null)
                {
                    _writerThread = new Thread(FileWriterLoop)
                    {
                        IsBackground = true,
                        Name = "CopilotBuddy Log Writer"
                    };
                    _writerThread.Start();
                    AppDomain.CurrentDomain.ProcessExit += (s, e) => FlushPending();
                    AppDomain.CurrentDomain.UnhandledException += (s, e) => FlushPending();
                }
            }
        }

        public static void FlushPending()
        {
            try
            {
                List<string> batch;
                lock (_queueLock)
                {
                    if (_pendingLines.Count == 0)
                        return;

                    batch = new List<string>(_pendingLines);
                    _pendingLines.Clear();
                }

                if (string.IsNullOrEmpty(LogFilePath))
                    return;

                lock (_writeLock)
                {
                    var directory = Path.GetDirectoryName(LogFilePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    using (var writer = new StreamWriter(
                        new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read)))
                    {
                        foreach (var line in batch)
                            writer.WriteLine(line);
                    }
                }
            }
            catch
            {
                // Ignore file write errors
            }
        }

        private static void FileWriterLoop()
        {
            while (true)
            {
                FlushPending();
                Thread.Sleep(75);
            }
        }

        #endregion

        #region Helper Methods

        private static WpfColor GetColorForLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.None => ColorNormal,
                LogLevel.Quiet => ColorQuiet,
                LogLevel.Normal => ColorNormal,
                LogLevel.Verbose => ColorVerbose,
                LogLevel.Diagnostic => ColorDiagnostic,
                _ => ColorNormal
            };
        }

        #endregion
    }
}
