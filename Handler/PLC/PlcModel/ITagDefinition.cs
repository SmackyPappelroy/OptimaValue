using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    public interface ITagDefinition
    {
        string Name { get; set; }
        string PlcName { get; set; }
        int BlockNr { get; set; }
        int StartByte { get; set; }
        S7.Net.DataType DataType { get; set; }
        VarType VarType { get; set; }
        int NrOfElements { get; set; }
        byte BitAddress { get; set; }
        string Calculation { get; set; }
    }
}
