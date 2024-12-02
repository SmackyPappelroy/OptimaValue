using FileLogger;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace OptimaValue
{
    public class LoggingStats
    {
        private long _totalCycleTimeTicks;
        private readonly TimeSpan _minCycleTimeFilter;
        private readonly TimeSpan _maxCycleTimeAlarm;

        public TimeSpan LastCycleTime { get; private set; }
        public TimeSpan AverageCycleTime => TimeSpan.FromTicks(_totalCycleTimeTicks / (TotalCycles > 0 ? TotalCycles : 1));
        public TimeSpan MaxCycleTime { get; private set; } = TimeSpan.Zero;
        public TimeSpan MinCycleTime { get; private set; } = TimeSpan.MaxValue;
        public int TotalCycles { get; private set; }

        public LoggingStats(int minimumMillisecondFilter, int maximumMillisecondAlarm)
        {
            _minCycleTimeFilter = TimeSpan.FromMilliseconds(minimumMillisecondFilter);
            _maxCycleTimeAlarm = TimeSpan.FromMilliseconds(maximumMillisecondAlarm);
        }

        public void Update(TimeSpan cycleTime)
        {
            if (cycleTime < _minCycleTimeFilter)
                return;

            LastCycleTime = cycleTime;
            TotalCycles++;

            // Uppdatera total cykeltid i ticks
            _totalCycleTimeTicks += cycleTime.Ticks;

            // Uppdatera max cykeltid och logga om den överskrider gränsen
            if (cycleTime > MaxCycleTime)
            {
                MaxCycleTime = cycleTime;
                if (MaxCycleTime > _maxCycleTimeAlarm)
                {
                    Task.Run(() => Logger.LogError($"Max cykeltid överskriden: {MaxCycleTime.FormatTime()}"));
                }
            }

            // Uppdatera min cykeltid
            if (cycleTime < MinCycleTime)
            {
                MinCycleTime = cycleTime;
            }
        }

        public void Reset()
        {
            LastCycleTime = TimeSpan.Zero;
            MaxCycleTime = TimeSpan.Zero;
            MinCycleTime = TimeSpan.MaxValue;
            TotalCycles = 0;
            _totalCycleTimeTicks = 0;
        }

        public override string ToString()
        {
            string minCycleTimeString = MinCycleTime == TimeSpan.MaxValue ? "N/A" : MinCycleTime.FormatTime();
            return $"Last: {LastCycleTime.FormatTime()}, Avg: {AverageCycleTime.FormatTime()}, Max: {MaxCycleTime.FormatTime()}, Min: {minCycleTimeString}, Total Cycles: {TotalCycles}";
        }
    }

    public static class TimeSpanExtensions
    {
        public static string FormatTime(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalMilliseconds < 1000)
                return $"{timeSpan.TotalMilliseconds:F2} ms";
            if (timeSpan.TotalSeconds < 60)
                return $"{timeSpan.TotalSeconds:F2} sek";
            if (timeSpan.TotalMinutes < 60)
                return $"{timeSpan.TotalMinutes:F2} min";
            if (timeSpan.TotalHours < 24)
                return $"{timeSpan.TotalHours:F2} timmar";

            return $"{timeSpan.TotalDays:F2} dagar";
        }
    }
}
