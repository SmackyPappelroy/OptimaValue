using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    public class TagInfo
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
        public LogType LogType { get; set; }
        public LogFrequency LogFrequency { get; set; }
    }
}
