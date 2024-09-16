using System;

namespace OptimaValue
{
    public class PlcStatusEventArgs : EventArgs
    {
        public string Message { get; }
        public Status Status { get; }
        public string PlcName { get; }

        public PlcStatusEventArgs(string message, string plcName, Status status)
        {
            Message = message;
            PlcName = plcName;
            Status = status;
        }
    }

}
