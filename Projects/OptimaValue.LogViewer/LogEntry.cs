using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.LogViewer;

public class LogEntry
{
    public string LogLevel { get; set; }
    public string LogDate { get; set; }
    public DateTime LogDateTime => DateTime.Parse(LogDate);
    public string FilePath { get; set; }
    public string Method { get; set; }
    public string LineNumber { get; set; }
    public string Message { get; set; }
    public string Exception { get; set; }

    public override string ToString()
    {
        return $"{LogLevel} {Environment.NewLine} {LogDate} {Environment.NewLine}Filsökväg: {FilePath} {Environment.NewLine}Metod: {Method} {Environment.NewLine}Linje-nummer: {LineNumber} {Environment.NewLine}{Message} {Environment.NewLine}Exception: {Exception}";
    }
}

