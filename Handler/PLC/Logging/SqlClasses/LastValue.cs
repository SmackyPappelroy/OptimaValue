using System;
namespace OptimaValue
{
    public class LastValue
    {
        public int tag_id { get; set; }
        public DateTime last_updated { get; set; }
        public ReadValue ReadValue { get; set; }
        public object value => ReadValue.Value;
    }
}
