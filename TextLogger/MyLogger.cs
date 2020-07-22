using System;
using System.IO;
using System.Threading;

namespace OptimaValue
{
    public class MyLogger : ThreadedLogger
    {
        private static readonly object ThreadSafeObject = new object();
        private static readonly string myPath = @"c:\OptimaValueLog\";
        private static readonly string logFile = @"LogFile.txt";

        protected override void AsyncLogMessage(LogRow row)
        {
            lock (ThreadSafeObject)
            {
                var total = Path.Combine(myPath, logFile);
                try
                {
                    if (!Directory.Exists(myPath))
                        Directory.CreateDirectory(myPath);
                    if (!File.Exists(total))
                        File.Create(total);
                    Thread.Sleep(1000);
                    using (StreamWriter w = File.AppendText(total))
                    {
                        w.Write($"{row.LogMessage}");
                    }
                }
                catch (Exception)
                {
                }



            }
        }

    }
}
