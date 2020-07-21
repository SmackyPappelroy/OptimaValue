using S7.Net;
using System;

namespace OptimaValue
{
    public class TagDefinitions : TagStatistics
    {
        public int id { get; set; }
        public bool active { get; set; }
        public string name { get; set; }
        public LogType logType { get; set; }
        public TimeSpan timeOfDay { get; set; }
        public float deadband { get; set; }
        public string plcName { get; set; }
        public VarType varType { get; set; }
        public int blockNr { get; set; }
        public DataType dataType { get; set; }
        public int startByte { get; set; }
        public int nrOfElements { get; set; }
        public byte bitAddress { get; set; } = 0;
        public LogFrequency logFreq { get; set; }
        public DateTime LastLogTime { get; set; }

        // New
        public string tagUnit { get; set; } = string.Empty;
    }
}
