using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    public class IntouchReal
    {
        [Name(":IOReal")]
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

        [Name("RetentiveAlarmParameters")]
        public string RetentiveAlarmParameters { get; set; }

        [Name("AlarmValueDeadband")]
        public int AlarmValueDeadband { get; set; }

        [Name("AlarmDevDeadband")]
        public int AlarmDevDeadband { get; set; }

        [Name("EngUnits")]
        public string EngUnits { get; set; }

        [Name("InitialValue")]
        public float InitialValue { get; set; }

        [Name("MinEU")]
        public float MinEu { get; set; }

        [Name("MaxEU")]
        public float MaxEu { get; set; }

        [Name("Deadband")]
        public float Deadband { get; set; }

        [Name("LogDeadband")]
        public float LogDeadband { get; set; }

        [Name("LoLoAlarmState")]
        public string LoLoAlarmState { get; set; }

        [Name("LoLoAlarmValue")]
        public int LoLoAlarmValue { get; set; }

        [Name("LoLoAlarmPri")]
        public int LoLoAlarmPri { get; set; }

        [Name("LoAlarmState")]
        public string LoAlarmState { get; set; }

        [Name("LoAlarmValue")]
        public int LoAlarmValue { get; set; }

        [Name("LoAlarmPri")]
        public int LoAlarmPri { get; set; }

        [Name("HiAlarmState")]
        public string HiAlarmState { get; set; }

        [Name("HiAlarmValue")]
        public float HiAlarmValue { get; set; }

        [Name("HiAlarmPri")]
        public float HiAlarmPri { get; set; }

        [Name("HiHiAlarmState")]
        public string HiHiAlarmState { get; set; }

        [Name("HiHiAlarmValue")]
        public float HiHiAlarmValue { get; set; }

        [Name("HiHiAlarmPri")]
        public float HiHiAlarmPri { get; set; }

        [Name("MinorDevAlarmState")]
        public string MinorDevAlarmState { get; set; }

        [Name("MinorDevAlarmValue")]
        public float MinorDevAlarmValue { get; set; }

        [Name("MinorDevAlarmPri")]
        public float MinorDevAlarmPri { get; set; }

        [Name("MajorDevAlarmState")]
        public string MajorDevAlarmState { get; set; }

        [Name("MajorDevAlarmValue")]
        public float MajorDevAlarmValue { get; set; }

        [Name("MajorDevAlarmPri")]
        public float MajorDevAlarmPri { get; set; }

        [Name("DevTarget")]
        public float DevTarget { get; set; }

        [Name("ROCAlarmState")]
        public string ROCDevAlarmState { get; set; }

        [Name("ROCAlarmValue")]
        public float ROCDevAlarmValue { get; set; }

        [Name("ROCAlarmPri")]
        public float ROCDevAlarmPri { get; set; }

        [Name("ROCTimeBase")]
        public string ROCTimeBase { get; set; }

        [Name("MinRaw")]
        public float MinRaw { get; set; }

        [Name("MaxRaw")]
        public float MaxRaw { get; set; }

        [Name("Conversion")]
        public string Conversion { get; set; }

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
        public float AlarmAckModel { get; set; }

        [Name("LoLoAlarmDisable")]
        public float LoLoAlarmDisable { get; set; }

        [Name("LoAlarmDisable")]
        public float LoAlarmDisable { get; set; }

        [Name("HiAlarmDisable")]
        public float HiAlarmDisable { get; set; }

        [Name("HiHiAlarmDisable")]
        public float HiHiAlarmDisable { get; set; }

        [Name("MinDevAlarmDisable")]
        public float MinDevAlarmDisable { get; set; }

        [Name("MajDevAlarmDisable")]
        public float MajDevAlarmDisable { get; set; }

        [Name("RocAlarmDisable")]
        public float RocDevAlarmDisable { get; set; }

        [Name("LoLoAlarmInhibitor")]
        public string LoLoAlarmInhibitor { get; set; }

        [Name("LoAlarmInhibitor")]
        public string LoAlarmInhibitor { get; set; }

        [Name("HiAlarmInhibitor")]
        public string HiAlarmInhibitor { get; set; }

        [Name("HiHiAlarmInhibitor")]
        public string HiHiAlarmInhibitor { get; set; }

        [Name("MinDevAlarmInhibitor")]
        public string MinDevAlarmInhibitor { get; set; }

        [Name("MajDevAlarmInhibitor")]
        public string MajDevAlarmInhibitor { get; set; }

        [Name("RocDevAlarmInhibitor")]
        public string RocDevAlarmInhibitor { get; set; }

        [Name("SymbolicName")]
        public string SymbolicName { get; set; }
    }
}
