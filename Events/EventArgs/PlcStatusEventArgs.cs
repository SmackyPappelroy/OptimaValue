namespace OptimaValue
{
    public class PlcStatusEventArgs
    {
        public string PlcName { get; set; }
        public string Message { get; set; }
        public Status Status { get; set; } = 0;
    }
}
