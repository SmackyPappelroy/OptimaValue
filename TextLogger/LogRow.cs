using System;

namespace OptimaValue
{
    public class LogRow
    {
        /// <summary>
        /// The Message to be sent
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// The <see cref="Status"/> of the <see cref="Message"/>
        /// </summary>
        public Status status { get; set; } = 0;
        /// <summary>
        /// The composed message to be sent to the text logger
        /// </summary>
        public string LogMessage
        {
            get
            {
                var log = ("-------------------------------\r\n");
                log += ("\nLogg: ");
                log += ($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\r\n");
                log += ("  \r\n");
                if (status == Status.Error)
                    log += "Larm: ";
                else if (status == Status.Warning)
                    log += "Varning: ";
                log += ($"{Message}\r\n\n");
                log += ("-------------------------------");
                return log;
            }
        }
    }
}
