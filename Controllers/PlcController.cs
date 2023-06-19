using Microsoft.AspNetCore.Mvc;
using OptimaValue.API.Models;
using OptimaValue.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlcController : ControllerBase
{

    public PlcController()
    {
    }

    [HttpGet("plc-list")]
    public ActionResult<List<string>> GetPlcList()
    {
        var plcList = PlcConfig.PlcList.Select(plc => plc.PlcName).ToList();
        return Ok(plcList);
    }

    [HttpGet("tag-list")]
    public ActionResult<List<string>> GetTagList([FromQuery] string plcName)
    {
        if (plcName is null)
        {
            return BadRequest("Invalid PLC name");
        }
        string lowerPlcName = plcName.ToLower();
        if (TagsToLog.AllLogValues.Count == 0)
        {
            TagsToLog.GetAllTagsFromSql();
        }

        var tagList = TagsToLog.AllLogValues.Where(tag => tag.PlcName.ToLower() == lowerPlcName).Select(tag => tag.Name).ToList();
        return Ok(tagList);
    }


    [HttpGet("fetch")]
    public ActionResult<List<DataPoint>> FetchCurrentValues([FromQuery] List<string> tagnames, [FromQuery] string plcName, [FromQuery] string clientId)
    {
        if (plcName is null)
        {
            return BadRequest("Invalid PLC name");
        }
        string lowerPlcName = plcName.ToLower();

        // Check if the clientId is valid
        if (string.IsNullOrEmpty(clientId))
        {
            return BadRequest("Invalid client ID");
        }

        // Check if the plc has connectionstatus connected
        if (!PlcDataStore.IsPlcConnected(plcName))
        {
            return BadRequest("PLC is not connected");
        }

        // Get the monitored tags for the specified client
        if (!PlcDataStore.MonitoredTagsPerApplication.TryGetValue(clientId, out var monitoredTags))
        {
            return BadRequest("Invalid client ID");
        }

        List<DataPoint> dataPoints = new List<DataPoint>();

        var commaSplittedList = tagnames.SelectMany(x => x.Split(',')).ToList();

        foreach (var tagName in commaSplittedList)
        {
            if (monitoredTags.Any(tagRequest => string.Equals(tagRequest.TagName, tagName, StringComparison.OrdinalIgnoreCase)))
            {
                string lowerTagName = tagName.ToLower();
                DataPoint dataPoint = PlcDataStore.LatestValues
                    .Where(kvp => kvp.Value.PlcName.ToLower() == $"{lowerPlcName}"
                        && kvp.Value.TagName.ToLower() == $"{lowerTagName}")
                    .Select(kvp => kvp.Value)
                    .FirstOrDefault();

                if (dataPoint != null && dataPoint.PlcName.ToLower() == lowerPlcName)
                {
                    dataPoints.Add(dataPoint);
                }
            }
        }

        return dataPoints;
    }


    [HttpPost("monitor")]
    public IActionResult MonitorTags([FromBody] List<TagRequest> tagRequests, [FromQuery] string clientId)
    {
        // Update the tags to monitor in your Windows Forms application
        // E.g., store the tagRequests in the PlcDataStore and use them in the PlcLoop
        if (TagsToLog.AllLogValues.Count == 0)
        {
            TagsToLog.GetAllTagsFromSql();
        }

        var tagFound = TagsToLog.AllLogValues
              .Where(x => tagRequests.Any(req => string.Equals(req.TagName, x.Name, StringComparison.OrdinalIgnoreCase)
              && string.Equals(req.PlcName, x.PlcName, StringComparison.OrdinalIgnoreCase)) && x.Active)
              .ToList();

        if (tagFound.Count == 0)
        {
            return BadRequest("No tags found");
        }

        if (!PlcDataStore.MonitoredTagsPerApplication.ContainsKey(clientId))
        {
            PlcDataStore.MonitoredTagsPerApplication.Add(clientId, new HashSet<TagRequest>(tagRequests));
        }
        else
        {
            foreach (var tagRequest in tagRequests)
            {
                if (!PlcDataStore.MonitoredTagsPerApplication[clientId].Any(existingTag =>
                    string.Equals(existingTag.TagName, tagRequest.TagName, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(existingTag.PlcName, tagRequest.PlcName, StringComparison.OrdinalIgnoreCase)))
                {
                    PlcDataStore.MonitoredTagsPerApplication[clientId].Add(tagRequest);
                }
            }
        }

        return Ok();
    }

    [HttpDelete("unmonitor")]
    public IActionResult UnmonitorTag([FromBody] TagRequest tagRequest, [FromQuery] string clientId)
    {
        if (string.IsNullOrEmpty(clientId))
        {
            return BadRequest("Invalid client ID");
        }

        if (!PlcDataStore.MonitoredTagsPerApplication.TryGetValue(clientId, out var monitoredTags))
        {
            return BadRequest("Invalid client ID");
        }

        monitoredTags = new HashSet<TagRequest>(
            monitoredTags.Where(tag => !(tag.TagName.Equals(tagRequest.TagName, StringComparison.OrdinalIgnoreCase)
                && tag.PlcName.Equals(tagRequest.PlcName, StringComparison.OrdinalIgnoreCase))));

        // Update the MonitoredTagsPerApplication dictionary with the modified monitoredTags
        PlcDataStore.MonitoredTagsPerApplication[clientId] = monitoredTags;

        return Ok();
    }


    [HttpGet("monitored-tags")]
    public ActionResult<List<TagRequest>> GetMonitoredTags([FromQuery] string clientId)
    {
        // Check if the clientId is valid
        if (string.IsNullOrEmpty(clientId))
        {
            return BadRequest("Invalid client ID");
        }

        // Get the monitored tags for the specified client
        if (!PlcDataStore.MonitoredTagsPerApplication.TryGetValue(clientId, out var monitoredTags))
        {
            return BadRequest("Invalid client ID");
        }

        // Return the monitored tags as a list
        return Ok(monitoredTags.ToList());
    }

}

