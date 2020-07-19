using System;

namespace OptimaValue
{
    public class StatusEventArgs : EventArgs
    {
        public string Message { get; set; }
        public Status Status { get; set; } = 0;
    }
}
