using System;
using System.Drawing;

namespace OptimaValue
{
    public class OnlineStatusEventArgs : EventArgs
    {
        public string Message { get; set; }

        public string PlcName { get; set; }

        public Color Color { get; set; } = Color.Red;

        public ConnectionStatus connectionStatus { get; set; }

        public string ElapsedTime { get; set; }

    }
}
