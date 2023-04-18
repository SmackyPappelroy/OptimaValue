using S7.Net;
using System;
using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Configuration;

namespace OptimaValue
{
    public class TagDefinitions : TagStatistics, ITagDefinition
    {
        [Name("Id")]
        public int Id { get; set; }
        [Name("Active")]
        public bool Active { get; set; }
        [Name("Name")]
        public string Name { get; set; }
        [Name("Description")]
        public string Description { get; set; }
        [Name("LogType")]
        public LogType LogType { get; set; }
        [Name("TimeOfDay")]
        public TimeSpan TimeOfDay { get; set; }
        [Name("Deadband")]
        public float Deadband { get; set; }
        [Name("PlcName")]
        public string PlcName { get; set; }
        [Name("VarType")]
        public VarType VarType { get; set; }
        [Name("BlockNr")]
        public int BlockNr { get; set; }
        [Name("DataType")]
        public DataType DataType { get; set; }
        [Name("StartByte")]
        public int StartByte { get; set; }
        [Name("NrOfElements")]
        public int NrOfElements { get; set; }
        [Name("BitAddress")]
        public byte BitAddress { get; set; } = 0;
        [Name("LogFreq")]
        public LogFrequency LogFreq { get; set; }
        [Name("LastLogTime")]
        public DateTime LastLogTime { get; set; }
        [Name("TagUnit")]
        public string TagUnit { get; set; } = string.Empty;

        [Name("Calculation")]
        public string Calculation { get; set; } = string.Empty;
        #region Skalering
        [Name("rawMin")]
        public float rawMin { get; set; }
        [Name("rawMax")]
        public float rawMax { get; set; }

        [Name("scaleMin")]
        public float scaleMin { get; set; }
        [Name("scaleMax")]
        public float scaleMax { get; set; }
        [Name("scaleOffset")]
        public float scaleOffset { get; set; }

        #endregion

        // Events
        [Name("EventId")]
        public int EventId { get; set; } // The id of the trigger tag
        [Name("IsBooleanTrigger")]
        public bool IsBooleanTrigger { get; set; }
        [Name("BoolTrigger")]
        public BooleanTrigger BoolTrigger { get; set; }
        [Name("AnalogTrigger")]
        public AnalogTrigger AnalogTrigger { get; set; }

        [Name("AnalogValue")]
        public float AnalogValue { get; set; } = 0f;

        private List<int> subscribedTags = new List<int>();
        [Name("SubscribedTags")]

        public string OpcTagName => PlcName + "." + Name;
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
                            if (tag.EventId == Id && tag.EventId != 0)
                            {
                                subscribedTags.Add(tag.Id);
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

            if (thisObject.Name == this.Name)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
