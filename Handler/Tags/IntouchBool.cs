using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    public class IntouchBool
    {
        [Name(":IODisc")]
        public string Name { get; set; }

        [Name("Group")]
        public string Group { get; set; }

        [Name("Comment")]
        public string Comment { get; set; }

        [Name("Logged")]
        public string Logged { get; set; }

        [Name("EventLogged")]
        public string EventLogged { get; set; }

        [Name("EventLoggingPriority")]
        public int EventLoggingPriority { get; set; }

        [Name("RetentiveValue")]
        public string RetentiveValue { get; set; }

        [Name("InitialDisc")]
        public string InitialDisc { get; set; }

        [Name("OffMsg")]
        public string OffMsg { get; set; }

        [Name("OnMsg")]
        public string OnMsg { get; set; }

        [Name("AlarmState")]
        public string AlarmState { get; set; }

        [Name("AlarmPri")]
        public int AlarmPri { get; set; }

        [Name("DConversion")]
        public string DConversion { get; set; }

        [Name("AccessName")]
        public string AccessName { get; set; }

        [Name("ItemUseTagname")]
        public string ItemUseTagname { get; set; }

        [Name("ItemName")]
        public string ItemName { get; set; }

        [Name("ReadOnly")]
        public string ReadOnly { get; set; }

        [Name("AlarmComment")]
        public string AlarmComment { get; set; }

        [Name("AlarmAckModel")]
        public int AlarmAckModel { get; set; }

        [Name("DSCAlarmDisable")]
        public int DSCAlarmDisable { get; set; }

        [Name("DSCAlarmInhibitor")]
        public string DSCAlarmInhibitor { get; set; }

        [Name("SymbolicName")]
        public string SymbolicName { get; set; }
    }
}
