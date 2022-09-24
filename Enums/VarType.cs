using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{

    public enum VarType
    {
        //
        // Summary:
        //     S7 Bit variable type (bool)
        Bit,
        //
        // Summary:
        //     S7 Byte variable type (8 bits)
        Byte,
        //
        // Summary:
        //     S7 Word variable type (16 bits, 2 bytes)
        Word,
        //
        // Summary:
        //     S7 DWord variable type (32 bits, 4 bytes)
        DWord,
        //
        // Summary:
        //     S7 Int variable type (16 bits, 2 bytes)
        Int,
        //
        // Summary:
        //     DInt variable type (32 bits, 4 bytes)
        DInt,
        //
        // Summary:
        //     Real variable type (32 bits, 4 bytes)
        Real,
        //
        // Summary:
        //     LReal variable type (64 bits, 8 bytes)
        LReal,
        //
        // Summary:
        //     Char Array / C-String variable type (variable)
        String,
        //
        // Summary:
        //     S7 String variable type (variable)
        S7String,
        //
        // Summary:
        //     S7 WString variable type (variable)
        S7WString,
        //
        // Summary:
        //     Timer variable type
        Timer,
        //
        // Summary:
        //     Counter variable type
        Counter,
        //
        // Summary:
        //     DateTIme variable type
        DateTime,
        //
        // Summary:
        //     DateTimeLong variable type
        DateTimeLong,
        //
        // Opc tag
        //
        Opc,
    }
}
