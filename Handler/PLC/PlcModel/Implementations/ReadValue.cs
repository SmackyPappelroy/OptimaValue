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
        public ReadValue(IPlc plc, object value)
        {
            this.plc = plc;
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
    }


}
