using System;

namespace OptimaValue.API.Models;

public class DataPoint
{
    public string PlcName { get; set; }
    public string TagName { get; set; }
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
}
