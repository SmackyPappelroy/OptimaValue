using FileLogger;
using System;

namespace OptimaValue
{
    public class LoggingStats
    {
        private TimeSpan _totalCycleTime;
        private readonly TimeSpan _minCycleTimeFilter;
        private readonly TimeSpan _maxCycleTimeAlarm;

        public TimeSpan LastCycleTime { get; private set; }
        public TimeSpan AverageCycleTime => TotalCycles > 0 ? TimeSpan.FromTicks(_totalCycleTime.Ticks / TotalCycles) : TimeSpan.Zero;
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
            {
                return;
            }

            LastCycleTime = cycleTime;
            TotalCycles++;
            _totalCycleTime += cycleTime;

            if (cycleTime > MaxCycleTime)
            {
                MaxCycleTime = cycleTime;
                if (MaxCycleTime > _maxCycleTimeAlarm)
                {
                    Logger.LogError($"Max cykeltid överskriden: {MaxCycleTime.FormatTime()}");
                }
            }

            MinCycleTime = (cycleTime < MinCycleTime || MinCycleTime == TimeSpan.MaxValue) ? cycleTime : MinCycleTime;
        }

        public void Reset()
        {
            LastCycleTime = TimeSpan.Zero;
            MaxCycleTime = TimeSpan.Zero;
            MinCycleTime = TimeSpan.MaxValue;
            TotalCycles = 0;
            _totalCycleTime = TimeSpan.Zero;
        }



        public override string ToString()
        {
            string minCycleTimeString = MinCycleTime == TimeSpan.MaxValue ? "N/A" : MinCycleTime.FormatTime();
            return $"Last: {LastCycleTime.FormatTime()}, Avg: {AverageCycleTime.FormatTime()}, Max: {MaxCycleTime.FormatTime()}, Min: {minCycleTimeString}, Total: {TotalCycles}";
        }

    }

    public static class TimeSpanExtensions
    {
        public static string FormatTime(this TimeSpan timeSpan)
        {
            // Improved format readability and maintainability
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
