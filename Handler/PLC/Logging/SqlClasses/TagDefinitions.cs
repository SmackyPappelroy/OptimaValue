using S7.Net;
using System;
using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Configuration;

namespace OptimaValue
{
    public class TagDefinitions : TagStatistics
    {
        [Name("id")]
        public int id { get; set; }
        [Name("active")]
        public bool active { get; set; }
        [Name("name")]
        public string name { get; set; }
        [Name("logType")]
        public LogType logType { get; set; }
        [Name("timeOfDay")]
        public TimeSpan timeOfDay { get; set; }
        [Name("deadband")]
        public float deadband { get; set; }
        [Name("plcName")]
        public string plcName { get; set; }
        [Name("varType")]
        public VarType varType { get; set; }
        [Name("blockNr")]
        public int blockNr { get; set; }
        [Name("dataType")]
        public DataType dataType { get; set; }
        [Name("startByte")]
        public int startByte { get; set; }
        [Name("nrOfElements")]
        public int nrOfElements { get; set; }
        [Name("bitAddress")]
        public byte bitAddress { get; set; } = 0;
        [Name("logFreq")]
        public LogFrequency logFreq { get; set; }
        [Name("LastLogTime")]
        public DateTime LastLogTime { get; set; }
        [Name("tagUnit")]
        public string tagUnit { get; set; } = string.Empty;

        #region Skalering

        [Name("scaleMin")]
        public int scaleMin { get; set; }
        [Name("scaleMax")]
        public int scaleMax { get; set; }
        [Name("scaleOffset")]
        public int scaleOffset { get; set; }

        #endregion

        // Events
        [Name("eventId")]
        public int eventId { get; set; } // The id of the trigger tag
        [Name("IsBooleanTrigger")]
        public bool IsBooleanTrigger { get; set; }
        [Name("boolTrigger")]
        public BooleanTrigger boolTrigger { get; set; }
        [Name("analogTrigger")]
        public AnalogTrigger analogTrigger { get; set; }

        [Name("analogValue")]
        public float analogValue { get; set; } = 0f;

        private List<int> subscribedTags = new List<int>();
        [Name("SubscribedTags")]
        public List<int> SubscribedTags
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

        public override bool Equals(object obj)
        {
            var thisObject = (TagDefinitions)obj;

            if (thisObject.name == this.name)
                return true;

            return false;
        }
    }
}
