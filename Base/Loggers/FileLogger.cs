using OptimaValue.Properties;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace OptimaValue
{
    public class FileLogger
    {
        #region Private members
        protected string FilePath;
        /// <summary>
        /// Make logging thread-safe
        /// </summary>
        private ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();


        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public FileLogger(string filePath)
        {
            //Normalize filepath
            FilePath = filePath?.Replace('/', '\\').Trim();

        }

        /// <summary>
        /// An event that fires when a log occurs
        /// </summary>
        public event Action<(string Message, string hmiString, Severity LogSeverity)> NewLog = (details) => { };


        #region Methods
        /// <summary>
        /// Logs a message
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
            // Locks the thread
            readerWriterLockSlim.EnterWriteLock();
            string logString = string.Empty;
            string hmiString = string.Empty;
            string exceptionString = string.Empty;

            logString = $"[{(severity.ToString() + "]").PadRight(10)}";
            logString += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            hmiString = logString;


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
            NewLog.Invoke((logString, hmiString, severity));

            if (Settings.Default.Debug)
                LogToFile("\r\n\r\n" + logString + "\r\n\r\n");

            // Unlocks the thread
            readerWriterLockSlim.ExitWriteLock();
        }

        private void LogToFile(string logString)
        {
            var tempString = "--------------------------------------------------------------";
            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);

            using (StreamWriter stream = new StreamWriter(FilePath + "Log.txt", true))
            {
                stream.Write(logString + tempString);
            }
        }
        #endregion
    }
}
