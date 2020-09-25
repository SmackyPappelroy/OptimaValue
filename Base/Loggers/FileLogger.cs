using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace OptimaValue
{
    public class FileLogger
    {
        #region Properties
        protected string FilePath;
        /// <summary>
        /// Make logging thread-safe
        /// </summary>
        private ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();

        /// <summary>
        /// Enables logging to file
        /// </summary>
        public bool EnableFileLog;
        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath">Format: C:\Katalog\</param>
        /// <param name="enableFileLog">Log to file?</param>
        public FileLogger(string filePath, bool enableFileLog)
        {
            //Normalize filepath
            FilePath = filePath?.Replace('/', '\\').Trim();

            // Check that the last character is a \
            if (!(FilePath.Last().CompareTo('\\') == 0))
                FilePath += '\\';

            EnableFileLog = enableFileLog;
        }

        /// <summary>
        /// An event that fires when a log occurs
        /// <para></para>
        /// How to access in UI-thread:
        /// <code>
        /// private void MyLogger_NewLog((string Message, string hmiString, Severity LogSeverity,DateTime Tid) obj) <para></para>
        ///   { <para></para>
        ///     if (InvokeRequired)  <para></para>
        ///     { <para></para>
        ///         Invoke((MethodInvoker)delegate { MyLogger_NewLog(obj); }); <para></para>
        ///     return; <para></para>
        ///     var tid = obj.Tid; <para></para>
        ///   }
        /// </code>
        /// </summary>
        public event Action<(string Message, string hmiString, Severity LogSeverity, DateTime Tid, bool LogSuccess, Exception exception)> NewLog = (details) => { };


        #region Methods
        /// <summary>
        /// Logs a message if <see cref="EnableFileLog"/> is True<para></para>
        /// Also fires an event <see cref="NewLog"/> when a new log has been made
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="logLevel">How much to log</param>
        /// <param name="severity">The severity of the log</param>
        /// <param name="origin">What genereated the log</param>
        /// <param name="filePath">The path to the file that generated the log message</param>
        /// <param name="ex">Optional exception</param>
        /// <param name="lineNumber">What line number generated the log</param>
        public void Log(string message, Severity severity = Severity.Normal, Exception ex = null, Level logLevel = Level.Debug, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var tiden = DateTime.Now;

            try
            {
                // Locks the thread
                readerWriterLockSlim.EnterWriteLock();
                string logString = string.Empty;
                string hmiString = string.Empty;
                string exceptionString = string.Empty;

                logString = $"[{(severity.ToString() + "]").PadRight(10)}";
                logString += tiden.ToString("yyyy-MM-dd HH:mm:ss.fff");
                hmiString = tiden.ToString("yyyy-MM-dd HH:mm:ss.fff");

                if (logLevel >= Level.Critical)
                    logString += $"{Environment.NewLine}[{(filePath + "]")}";
                if (logLevel >= Level.Informative)
                    logString += $"{Environment.NewLine}{(origin + "()")}";
                if (logLevel >= Level.Debug)
                    logString += $"{Environment.NewLine}Line number: [{(lineNumber + "]")}";

                if (ex != null)
                {
                    logString += Environment.NewLine;
                    logString += ex.ToString();
                }

                logString += Environment.NewLine + "Meddelande: " + message;
                hmiString += Environment.NewLine + message;

                // Raise an event
                NewLog.Invoke((logString, hmiString, severity, tiden, true, ex));

                if (EnableFileLog)
                    LogToFile("\r\n\r\n" + logString + "\r\n\r\n");

                // Unlocks the thread
                readerWriterLockSlim.ExitWriteLock();
            }
            catch (Exception exc)
            {
                // Raise an event
                NewLog.Invoke((string.Empty, string.Empty, Severity.Error, tiden, true, exc));
            }

        }

        private void LogToFile(string logString)
        {
            var tempString = "--------------------------------------------------------------";
            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);

            using (StreamWriter stream = new StreamWriter(FilePath + "Log.txt", true))
                stream.Write(logString + tempString);
        }
        #endregion
    }
}
