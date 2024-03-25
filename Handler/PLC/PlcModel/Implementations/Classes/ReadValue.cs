using OpcUaHm.Common;
using System;

namespace OptimaValue
{
    public class ReadValue
    {
        public object Value { get; set; }
        public Type Type => Value?.GetType();

        public ReadValue(IPlc plc, object value)
        {
            this.plc = plc;
            Value = value;
        }

        public ReadValue(ExtendedPlc plc, object value) : this(plc.Plc, value)
        {
        }

        public string Quality => (PlcType, Value) switch
        {
            (plcType.SiemensPlc, null) => "Bad",
            (plcType.OpcPlc, null) => "Bad",
            (plcType.OpcPlc, OpcUaHm.Common.ReadEvent<object> { Quality: var q }) => q,
            _ => "Good"
        };
        public DateTime LogTime => (PlcType, Value) switch
        {
            (plcType.OpcPlc, OpcUaHm.Common.ReadEvent<object> { SourceTimestamp: var ts }) => ts.DateTime,
            _ => DateTime.Now
        };
        public DateTime SourceTimestamp => (PlcType, Value) switch
        {
            (plcType.OpcPlc, OpcUaHm.Common.ReadEvent<object> { SourceTimestamp: var ts }) => ts.DateTime,
            _ => DateTime.Now
        };
        public DateTime ServerTimestamp => (PlcType, Value) switch
        {
            (plcType.OpcPlc, OpcUaHm.Common.ReadEvent<object> { ServerTimestamp: var ts }) => ts.DateTime,
            _ => DateTime.Now
        };
        public float ValueAsFloat
        {
            get
            {
                if (Value is ReadEvent<object> readEventValue)
                {
                    return ConvertToFloat(readEventValue.Value);
                }
                return ConvertToFloat(Value);
            }
        }

        private float ConvertToFloat(object value)
        {
            return value switch
            {
                null => 0,
                float f => f,
                double d => (float)d,
                int i => i,
                uint ui => ui,
                short s => s,
                ushort us => us,
                long l => l,
                ulong ul => ul,
                byte b => b,
                sbyte sb => sb,
                bool boolValue => boolValue ? 1 : 0,
                _ => 0
            };
        }
        public string ValueAsString
        {
            get
            {
                if (Value is ReadEvent<object> readEventValue)
                {
                    return ConvertToString(readEventValue.Value);
                }
                return ConvertToString(Value);
            }
        }

        private string ConvertToString(object value)
        {
            return value switch
            {
                null => "null",
                Array a => $"[{string.Join(", ", a)}]",
                _ => value.ToString()
            };
        }

        private enum plcType
        {
            SiemensPlc,
            OpcPlc
        }

        private plcType PlcType =>
            plc switch
            {
                SiemensPlc => plcType.SiemensPlc,
                OpcPlc => plcType.OpcPlc,
                _ => throw new Exception("Unknown PLC type")
            };

        private readonly IPlc plc;



        public override string ToString() => ValueAsString;
    }
}