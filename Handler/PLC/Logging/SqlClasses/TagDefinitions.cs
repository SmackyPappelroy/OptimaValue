using S7.Net;
using System;
using System.Collections.Generic;

namespace OptimaValue
{
    public class TagDefinitions : TagStatistics
    {
        public decimal id { get; set; }
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
        public string tagUnit { get; set; } = string.Empty;

        #region Skalering
        public int scaleMin { get; set; }
        public int scaleMax { get; set; }
        public int scaleOffset { get; set; }

        #endregion

        // Events
        public decimal eventId { get; set; } // The id of the trigger tag
        public bool IsBooleanTrigger { get; set; }
        public BooleanTrigger boolTrigger { get; set; }
        public AnalogTrigger analogTrigger { get; set; }

        public float analogValue { get; set; } = 0f;

        private List<decimal> subscribedTags = new List<decimal>();
        public List<decimal> SubscribedTags
        {
            get
            {
                subscribedTags.Clear();
                if (TagsToLog.AllLogValues != null)
                {
                    if (TagsToLog.AllLogValues.Count > 1)
                    {
                        foreach (var tag in TagsToLog.AllLogValues)
                        {
                            if (tag.eventId == id && tag.eventId != 0)
                            {
                                subscribedTags.Add(tag.id);
                            }
                        }
                    }
                }
                return subscribedTags;
            }
        }
    }
}
