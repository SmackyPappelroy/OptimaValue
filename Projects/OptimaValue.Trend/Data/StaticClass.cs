using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace OptimaValue.Trend;

public static class StaticClass
{
    private static readonly string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\OptimaValue\\Trend";
    private static readonly string fileName = "LastSaveFile.json";
    public static string LastSaveFile
    {
        get
        {
            if (!System.IO.File.Exists(filePath + "\\" + fileName))
            {
                return "";
            }
            else
            {
                var jsonText = System.IO.File.ReadAllText(filePath + "\\" + fileName);
                return JsonSerializer.Deserialize<string>(jsonText);
            }
        }
        set
        {
            var jsonText = JsonSerializer.Serialize(value);
            Directory.CreateDirectory(filePath);
            File.WriteAllText(filePath + "\\" + fileName, jsonText);
        }
    }
    public static DateTime MinDateSeries;
    public static DateTime MaxDateSeries;
    public static BindingList<Line> Lines { get; internal set; }

    public static void AddLine(Line line)
    {
        if (Lines == null)
        {
            Lines = new();
        }

        if (!Lines.Contains(line))
        {
            Lines.Add(line);
        }
        else
        {
            Lines.Remove(line);
            Lines.Add(line);
        }
    }

    public static void RemoveLine(Line line)
    {
        if (Lines == null)
            return;
        if (Lines.Contains(line))
            Lines.Remove(line);
    }
}


