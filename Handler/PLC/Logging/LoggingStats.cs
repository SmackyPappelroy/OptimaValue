using FileLogger;
using System;

namespace OptimaValue
{
    public class LoggingStats
    {
        private TimeSpan _totalCycleTime;

        private TimeSpan minCycleTimeFilter;
        private TimeSpan maxCycleTimeAlarm;

        public TimeSpan LastCycleTime { get; private set; }
        public TimeSpan AverageCycleTime { get; private set; }
        public TimeSpan MaxCycleTime { get; private set; }
        public TimeSpan MinCycleTime { get; private set; }
        public int TotalCycles { get; private set; }

        public LoggingStats(int minimumMillisecondFilter, int maximumMillisecondAlarm)
        {
            minCycleTimeFilter = TimeSpan.FromMilliseconds(minimumMillisecondFilter);
            maxCycleTimeAlarm = TimeSpan.FromMilliseconds(maximumMillisecondAlarm);
            Reset();
        }

        public void Update(TimeSpan cycleTime)
        {
            if (cycleTime < minCycleTimeFilter)
            {
                return;
            }
            LastCycleTime = cycleTime;
            TotalCycles++;
            _totalCycleTime += cycleTime;

            AverageCycleTime = TimeSpan.FromTicks(_totalCycleTime.Ticks / TotalCycles);

            if (cycleTime > MaxCycleTime)
            {
                MaxCycleTime = cycleTime;
                if (MaxCycleTime > maxCycleTimeAlarm)
                {
                    Logger.LogError($"Max cykeltid överskriden: {MaxCycleTime.FormatTime()}");
                }
            }

            if (cycleTime < MinCycleTime || MinCycleTime == TimeSpan.Zero)
            {
                MinCycleTime = cycleTime;
            }
        }

        public void Reset()
        {
            LastCycleTime = TimeSpan.Zero;
            AverageCycleTime = TimeSpan.Zero;
            MaxCycleTime = TimeSpan.Zero;
            MinCycleTime = TimeSpan.Zero;
            TotalCycles = 0;
            _totalCycleTime = TimeSpan.Zero;
        }



        public override string ToString()
        {
            return $"Last: {LastCycleTime.FormatTime()}, Avg: {AverageCycleTime.FormatTime()}, Max: {MaxCycleTime.FormatTime()}, Min: {MinCycleTime.FormatTime()}, Total: {TotalCycles}";
        }
    }

    public static class TimeSpanExtensions
    {
        public static string FormatTime(this TimeSpan timeSpan)
        {
            string format;
            double value;

            if (timeSpan.TotalMilliseconds < 1)
            {
                format = "{0:F2} ms";
                value = timeSpan.TotalMilliseconds;
            }
            else if (timeSpan.TotalSeconds < 1)
            {
                format = "{0:F2} ms";
                value = timeSpan.TotalMilliseconds;
            }
            else if (timeSpan.TotalMinutes < 1)
            {
                format = "{0:F2} sek";
                value = timeSpan.TotalSeconds;
            }
            else if (timeSpan.TotalHours < 1)
            {
                format = "{0:F2} min";
                value = timeSpan.TotalMinutes;
            }
            else if (timeSpan.TotalDays < 1)
            {
                format = "{0:F2} timmar";
                value = timeSpan.TotalHours;
            }
            else
            {
                format = "{0:F2} dagar";
                value = timeSpan.TotalDays;
            }

            return string.Format(format, value);
        }
    }
}
