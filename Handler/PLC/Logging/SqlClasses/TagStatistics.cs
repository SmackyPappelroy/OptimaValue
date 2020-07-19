using System;

namespace OptimaValue
{
    public class ReadErrorEventArgs : EventArgs
    {
        public bool Error { get; set; }
    }
    public class TagStatistics
    {

        private DateTime lastTimeLogged = DateTime.MinValue;
        private TimeSpan totalScanTime = TimeSpan.FromSeconds(0);
        private TimeSpan lastScanTime = TimeSpan.MinValue;

        #region Events
        public event EventHandler<ReadErrorEventArgs> ReadErrorEvent;

        protected virtual void OnReadErrorEvent(ReadErrorEventArgs e)
        {
            ReadErrorEvent?.Invoke(this, e);
        }

        private void RaiseReadError(bool error)
        {
            var errorArgs = new ReadErrorEventArgs()
            {
                Error = error
            };
            OnReadErrorEvent(errorArgs);
        }

        #endregion

        private void CalculateScanTime()
        {
            if (lastTimeLogged == DateTime.MinValue)
            {
                ScanTime = "-";
                lastTimeLogged = DateTime.Now;
            }
            else
            {
                lastScanTime = DateTime.Now - lastTimeLogged;
                totalScanTime = totalScanTime + lastScanTime;
                lastTimeLogged = DateTime.Now;
                ScanTime = lastScanTime.ToString(@"mm\:ss\.ffff");
            }
        }
        private void CalculateAverageScanTime()
        {
            if (totalScanTime == TimeSpan.FromSeconds(0))
                AverageScanTime = "-";
            else
            {
                totalScanTime = lastScanTime + totalScanTime;
                var ticks = totalScanTime.Ticks / (NrFailedReadAttempts + nrSuccededReadAttempts);
                var newTime = new TimeSpan(ticks / 2);
                AverageScanTime = newTime.ToString(@"mm\:ss\.ffff");
            }
        }


        private int nrSuccededReadAttempts = 0;
        public int NrSuccededReadAttempts
        {
            get => nrSuccededReadAttempts;
            set
            {
                nrSuccededReadAttempts = value;
                CalculateScanTime();
                CalculateAverageScanTime();
            }
        }
        private int nrFailedReadAttempts = 0;
        public int NrFailedReadAttempts
        {
            get => nrFailedReadAttempts;
            set
            {
                if (nrFailedReadAttempts == 0 && value > 0)
                    RaiseReadError(true);
                else if (value == 0 && nrFailedReadAttempts > 0)
                    RaiseReadError(false);

                nrFailedReadAttempts = value;
                CalculateScanTime();
                CalculateAverageScanTime();
            }
        }
        public string PercentOk
        {
            get
            {
                if (NrSuccededReadAttempts > 0)
                    return (NrSuccededReadAttempts / (NrSuccededReadAttempts + NrFailedReadAttempts)).ToString("P");
                else
                    return 1.ToString("P");
            }
        }
        public string LastErrorMessage { get; set; }
        public string ScanTime { get; set; }
        public string AverageScanTime { get; set; }
        public int TimesLogged { get; set; } = 0;

    }
}
