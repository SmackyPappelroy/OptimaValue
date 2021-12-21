using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    public class IntouchInt
    {
        [Name("IOInt")]
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
        public int InitialValue { get; set; }

        [Name("MinEu")]
        public int MinEu { get; set; }

        [Name("MaxEu")]
        public int MaxEu { get; set; }

        [Name("Deadband")]
        public int Deadband { get; set; }

        [Name("LogDeadband")]
        public int LogDeadband { get; set; }

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
        public int HiAlarmValue { get; set; }

        [Name("HiAlarmPri")]
        public int HiAlarmPri { get; set; }

        [Name("HiHiAlarmState")]
        public string HiHiAlarmState { get; set; }

        [Name("HiHiAlarmValue")]
        public int HiHiAlarmValue { get; set; }

        [Name("HiHiAlarmPri")]
        public int HiHiAlarmPri { get; set; }

        [Name("MinorDevAlarmState")]
        public string MinorDevAlarmState { get; set; }

        [Name("MinorDevAlarmValue")]
        public int MinorDevAlarmValue { get; set; }

        [Name("MinorDevAlarmPri")]
        public int MinorDevAlarmPri { get; set; }

        [Name("MajorDevAlarmState")]
        public string MajorDevAlarmState { get; set; }

        [Name("MajorDevAlarmValue")]
        public int MajorDevAlarmValue { get; set; }

        [Name("MajorDevAlarmPri")]
        public int MajorDevAlarmPri { get; set; }

        [Name("DevTarget")]
        public int DevTarget { get; set; }

        [Name("ROCDevAlarmState")]
        public string ROCDevAlarmState { get; set; }

        [Name("ROCDevAlarmValue")]
        public int ROCDevAlarmValue { get; set; }

        [Name("ROCDevAlarmPri")]
        public int ROCDevAlarmPri { get; set; }

        [Name("ROCTimeBase")]
        public string ROCTimeBase { get; set; }

        [Name("MinRaw")]
        public int MinRaw { get; set; }

        [Name("MaxRaw")]
        public int MaxRaw { get; set; }

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
        public int AlarmAckModel { get; set; }

        [Name("LoLoAlarmDisable")]
        public int LoLoAlarmDisable { get; set; }

        [Name("LoAlarmDisable")]
        public int LoAlarmDisable { get; set; }

        [Name("HiAlarmDisable")]
        public int HiAlarmDisable { get; set; }

        [Name("HiHiAlarmDisable")]
        public int HiHiAlarmDisable { get; set; }

        [Name("MinDevAlarmDisable")]
        public int MinDevAlarmDisable { get; set; }

        [Name("MajDevAlarmDisable")]
        public int MajDevAlarmDisable { get; set; }

        [Name("RocDevAlarmDisable")]
        public int RocDevAlarmDisable { get; set; }


    }
}
