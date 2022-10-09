using System;

namespace OptimaValue
{
    public class ReadErrorEventArgs : EventArgs
    {
        public bool Error { get; set; }
    }
    public class TagStatistics
    {

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

        public void ClearScanTime()
        {
            NrFailedReadAttempts = 0;
            NrSuccededReadAttempts = 0;
        }

        private int nrSuccededReadAttempts = 0;
        public int NrSuccededReadAttempts
        {
            get => nrSuccededReadAttempts;
            set
            {
                nrSuccededReadAttempts = value;
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
