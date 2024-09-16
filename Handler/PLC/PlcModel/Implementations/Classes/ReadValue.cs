using OpcUaHm.Common;
using System;

namespace OptimaValue;

public class ReadValue
{
    public object Value { get; set; }
    public Type Type => Value?.GetType();

    private readonly IPlc plc;

    public ReadValue(IPlc plc, object value)
    {
        this.plc = plc;
        Value = value;
    }

    public ReadValue(ExtendedPlc plc, object value) : this(plc.Plc, value)
    {
    }

    public string Quality => (GetPlcType(), Value) switch
    {
        (PlcTypeEnum.SiemensPlc, null) => "Bad",
        (PlcTypeEnum.OpcPlc, null) => "Bad",
        (PlcTypeEnum.OpcPlc, OpcUaHm.Common.ReadEvent<object> { Quality: var q }) => q,
        _ => "Good"
    };

    public DateTime LogTime => (GetPlcType(), Value) switch
    {
        (PlcTypeEnum.OpcPlc, OpcUaHm.Common.ReadEvent<object> { SourceTimestamp: var ts }) => ts.DateTime,
        _ => DateTime.Now
    };

    public DateTime SourceTimestamp => (GetPlcType(), Value) switch
    {
        (PlcTypeEnum.OpcPlc, OpcUaHm.Common.ReadEvent<object> { SourceTimestamp: var ts }) => ts.DateTime,
        _ => DateTime.Now
    };

    public DateTime ServerTimestamp => (GetPlcType(), Value) switch
    {
        (PlcTypeEnum.OpcPlc, OpcUaHm.Common.ReadEvent<object> { ServerTimestamp: var ts }) => ts.DateTime,
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
            null => string.Empty,
            Array a => $"[{string.Join(", ", a)}]",
            _ => value.ToString()
        };
    }

    private enum PlcTypeEnum
    {
        SiemensPlc,
        OpcPlc
    }

    private PlcTypeEnum GetPlcType() =>
        plc switch
        {
            SiemensPlc => PlcTypeEnum.SiemensPlc,
            OpcPlc => PlcTypeEnum.OpcPlc,
            _ => throw new Exception("Unknown PLC type")
        };

    public override string ToString() => ValueAsString;
}
