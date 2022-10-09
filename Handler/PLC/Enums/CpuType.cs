using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    public enum CpuType
    {
        //
        // Summary:
        //     S7 200 cpu type
        S7200 = 0,
        //
        // Summary:
        //     Siemens Logo 0BA8
        Logo0BA8 = 1,
        //
        // Summary:
        //     S7 200 Smart
        S7200Smart = 2,
        //
        // Summary:
        //     S7 300 cpu type
        S7300 = 10,
        //
        // Summary:
        //     S7 400 cpu type
        S7400 = 20,
        //
        // Summary:
        //     S7 1200 cpu type
        S71200 = 30,
        //
        // Summary:
        //     S7 1500 cpu type
        S71500 = 40,
        //
        // OPC UA
        //
        OpcUa = 1000,
        //
        // OPC DA
        //
        OpcDa = 1100,
        //
        // Unknown
        //
        Unknown = 9999
    }
}
