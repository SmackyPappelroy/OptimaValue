using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    public class ReadValue
    {
        private DateTime createdDate;
        public TimeSpan ReadTime { get; set; }

        public ReadValue(IPlc plc, object value)
        {
            this.plc = plc;
            internalValue = value;
            createdDate = DateTime.Now;
        }

        public ReadValue(ExtendedPlc plc, object value)
        {
            this.plc = plc.Plc;
            internalValue = value;
            createdDate = DateTime.Now;
        }

        private enum plcType
        {
            SiemensPlc,
            OpcPlc
        }

        private plcType PlcType
        {
            get
            {
                if (plc is SiemensPlc)
                    return plcType.SiemensPlc;
                else if (plc is OpcPlc)
                    return plcType.OpcPlc;
                else
                    throw new Exception("Unknown PLC type");
            }
        }

        private IPlc plc { get; }

        private object internalValue;
        public object Value
        {
            get
            {
                switch (PlcType)
                {
                    case plcType.SiemensPlc:
                        return internalValue;
                    case plcType.OpcPlc:
                        var val = internalValue as OpcUaHm.Common.ReadEvent<object>;
                        return val.Value;
                    default:
                        throw new Exception("Unknown PLC type");
                }
            }
            set => this.internalValue = value;
        }

        public Type Type => Value == null ? null : Value.GetType();

        public string Quality
        {
            get
            {
                if (PlcType == plcType.SiemensPlc)
                {
                    if (Value == null)
                    {
                        return "Bad";
                    }
                    else
                    {
                        return "Good";
                    }
                }
                else if (PlcType == plcType.OpcPlc)
                {
                    if (Value == null)
                    {
                        return "Bad";
                    }
                    else
                    {
                        var value = internalValue as OpcUaHm.Common.ReadEvent<object>;
                        return value.Quality;
                    }
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public DateTime LogTime
        {
            get
            {
                if (PlcType == plcType.SiemensPlc)
                {
                    return createdDate;
                }
                else if (PlcType == plcType.OpcPlc)
                {
                    var value = internalValue as OpcUaHm.Common.ReadEvent<object>;
                    return value.SourceTimestamp.DateTime;
                }
                else
                {
                    return createdDate;
                }
            }
        }

        public DateTime SourceTimestamp
        {
            get
            {
                if (PlcType == plcType.SiemensPlc)
                {
                    return createdDate;
                }
                else if (PlcType == plcType.OpcPlc)
                {
                    var value = internalValue as OpcUaHm.Common.ReadEvent<object>;
                    return value.SourceTimestamp.DateTime;
                }
                else
                {
                    return createdDate;
                }
            }
        }

        public DateTime ServerTimestamp
        {
            get
            {
                if (PlcType == plcType.SiemensPlc)
                {
                    return createdDate;
                }
                else if (PlcType == plcType.OpcPlc)
                {
                    var value = internalValue as OpcUaHm.Common.ReadEvent<object>;
                    return value.ServerTimestamp.DateTime;
                }
                else
                {
                    return createdDate;
                }
            }
        }

        public float ValueAsFloat
        {
            get
            {
                if (Value == null)
                {
                    return 0;
                }
                else
                {
                    if (Type.IsArray)
                    {
                        return 0;
                    }
                    else if (Value is float)
                    {
                        return (float)Value;
                    }
                    else if (Value is double)
                    {
                        return (float)Convert.ToDouble(Value);
                    }
                    else if (Value is int)
                    {
                        return (float)(int)Value;
                    }
                    else if (Value is uint)
                    {
                        return (float)(uint)Value;
                    }
                    else if (Value is short)
                    {
                        return (float)(short)Value;
                    }
                    else if (Value is ushort)
                    {
                        return (float)(ushort)Value;
                    }
                    else if (Value is long)
                    {
                        return (float)(long)Value;
                    }
                    else if (Value is ulong)
                    {
                        return (float)(ulong)Value;
                    }
                    else if (Value is byte)
                    {
                        return (float)(byte)Value;
                    }
                    else if (Value is sbyte)
                    {
                        return (float)(sbyte)Value;
                    }
                    else if (Value is bool)
                    {
                        return (bool)Value ? 1 : 0;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public string ValueAsString
        {
            get
            {
                if (Value == null)
                {
                    return "null";
                }
                else
                {
                    if (Type.IsArray)
                    {
                        var array = Value as Array;
                        var sb = new StringBuilder();
                        sb.Append("[");
                        for (int i = 0; i < array.Length; i++)
                        {
                            sb.Append(array.GetValue(i).ToString());
                            if (i < array.Length - 1)
                            {
                                sb.Append(", ");
                            }
                        }
                        sb.Append("]");
                        return sb.ToString();
                    }
                    else
                    {
                        return Value.ToString();
                    }
                }
            }
        }

        public override string ToString() => Value.ToString();
    }


}
