using System;

namespace OptimaValue
{
    public class LogValuesSql
    {
        public string tagName { get; set; }
        public string plcName { get; set; }
        public DateTime logTime { get; set; }
        public string value { get; set; }
        public float numericValue { get; set; }
    }
}
