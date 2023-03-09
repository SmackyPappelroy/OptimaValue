using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    public class PlcTag : ITagDefinition
    {

        public PlcTag(ITagDefinition tag)
        {
            Name = tag.Name;
            PlcName = tag.PlcName;
            BlockNr = tag.BlockNr;
            StartByte = tag.StartByte;
            DataType = tag.DataType;
            VarType = tag.VarType;
            NrOfElements = tag.NrOfElements;
            BitAddress = tag.BitAddress;
        }

        public PlcTag(DataType dataType, int blockNr, int startByte)
        {
            DataType = dataType;
            BlockNr = blockNr;
            StartByte = startByte;
        }

        public string Name { get; set; }
        public string PlcName { get; set; }
        public string Address { get; set; }
        public int BlockNr { get; set; }
        public int StartByte { get; set; }
        public S7.Net.DataType DataType { get; set; }
        public VarType VarType { get; set; }
        public int NrOfElements { get; set; }
        public byte BitAddress { get; set; }
    }
}