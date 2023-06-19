using OptimaValue.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimaValue.API.Models;

public static class PlcDataStore
{
    public static Dictionary<string, DataPoint> LatestValues { get; private set; }
        = new Dictionary<string, DataPoint>();
    public static Dictionary<string, HashSet<TagRequest>> MonitoredTagsPerApplication { get; set; }
        = new Dictionary<string, HashSet<TagRequest>>();


    public static bool IsTagMonitored(string tagName, string plcName)
    {
        return MonitoredTagsPerApplication.Values.Any(tagSet =>
            tagSet.Any(tagRequest =>
                string.Equals(tagRequest.TagName, tagName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(tagRequest.PlcName, plcName, StringComparison.OrdinalIgnoreCase)));
    }

    public static bool IsPlcConnected(string plcName) => PlcConfig.PlcList.Any(plc => plc.PlcName.ToLower() == plcName.ToLower() && plc.IsConnected);



    public static void UpdateValue(DataPoint dataPoint)
    {
        LatestValues[dataPoint.TagName] = dataPoint;
    }

    public static void UpdateValues(List<DataPoint> dataPoints)
    {
        foreach (var dataPoint in dataPoints)
        {
            UpdateValue(dataPoint);
        }
    }

}
