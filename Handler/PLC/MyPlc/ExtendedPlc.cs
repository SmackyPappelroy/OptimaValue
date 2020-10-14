using S7.Net;
using System;
using System.Drawing;
using System.Windows.Threading;

namespace OptimaValue
{
    public class ExtendedPlc : Plc
    {
        #region Messages
        public string ExternalStatusMessage = string.Empty;
        public Status ExternalStatus = Status.Ok;

        public string ExternalElapsedTime = string.Empty;
        public ConnectionStatus ExternalCommunicationStatus = ConnectionStatus.Disconnected;
        public Color ExternalOnlineColor = Color.Gray;
        public string ExternalOnlineMessage = "Ej ansluten";
        #endregion

        #region Sync Plc
        public int SyncTimeDbNr;
        public int SyncTimeOffset;
        public string SyncBoolAddress = string.Empty;
        public bool SyncActive;
        public DateTime lastSyncTime;
        #endregion

        #region Properties
        private DateTime UpTimeStart = DateTime.MaxValue;
        private readonly DispatcherTimer timerPing = new DispatcherTimer();
        private bool isSubscribed = false;

        #region Logging
        private bool loggerIsStarted = false;
        public bool LoggerIsStarted
        {
            get => loggerIsStarted;
            set
            {
                loggerIsStarted = value;
                if (loggerIsStarted)
                    onlineTimer.Start();
                else
                    onlineTimer.Stop();
                StartedEvent?.Invoke(typeof(ExtendedPlc), EventArgs.Empty);
            }
        }
        public event EventHandler<EventArgs> StartedEvent;

        private System.Windows.Forms.Timer onlineTimer = new System.Windows.Forms.Timer()
        {
            Interval = 500,
        };

        #endregion


        public bool Active = false;
        public int Id = 0;

        public DateTime LastReconnect = DateTime.MinValue;
        public int MaxReconnectRetries { get; private set; } = 5;
        public int ReconnectRetries { get; set; }
        public bool UnableToPing { get; private set; }
        public string PlcName { get; set; }
        public Int32 ActivePlcId { get; set; }
        private short watchDog = 0;
        public short WatchDog
        {
            get
            {
                watchDog++;
                if (watchDog > 5000)
                    watchDog = 0;
                return watchDog;
            }

        }
        public bool Alarm { get; set; } = false;
        private ConnectionStatus connectionStatus = ConnectionStatus.Disconnected;

        public ConnectionStatus ConnectionStatus
        {
            get => connectionStatus;
            set
            {
                if (connectionStatus == ConnectionStatus.Disconnected || connectionStatus == ConnectionStatus.Connecting)
                {
                    if (value == ConnectionStatus.Connected)
                    {
                        UpTimeStart = DateTime.UtcNow;
                        timerPing.Start();
                        ReconnectRetries = 0;
                    }
                }
                if (value != ConnectionStatus.Connected)
                    timerPing.Stop();
                connectionStatus = value;
            }
        }
        public TimeSpan UpTime
        {
            get
            {
                if (UpTimeStart == DateTime.MaxValue)
                    return TimeSpan.FromSeconds(0);
                else
                    return DateTime.UtcNow - UpTimeStart;
            }
        }
        public string UpTimeString
        {
            get
            {
                if (UpTimeStart == DateTime.MaxValue)
                    return "0s";
                var tid = DateTime.UtcNow - UpTimeStart;
                if (tid < TimeSpan.FromMinutes(1))
                    return tid.ToString("s's'");
                else if (tid < TimeSpan.FromHours(1))
                    return tid.ToString("m'm 's's'");
                else if (tid > TimeSpan.FromHours(1) && (tid < TimeSpan.FromDays(1)))
                    return tid.ToString("h'h 'm'm 's's'");
                else
                    return tid.ToString("d'd 'h'h 'm'm 's's'");
            }
        }
        #endregion

        #region Constructor
        public ExtendedPlc(CpuType cpu, string ip, short rack, short slot) : base(cpu, ip, rack, slot)
        {
            SubscribeEvents(true);
            timerPing.Interval = new TimeSpan(0, 0, 10);
            onlineTimer.Tick += OnlineTimer_Tick;
            // TODO: Måste plockas bort
            //logger = new Logger(this);
        }

        private void OnlineTimer_Tick(object sender, EventArgs e)
        {
            if (this != null)
                this.SendPlcOnlineMessage(this.ConnectionStatus, this.UpTimeString);
            else
                this.SendPlcOnlineMessage(ConnectionStatus.Disconnected, string.Empty);
        }
        #endregion

        #region Events
        private void SubscribeEvents(bool subscribeToEvents)
        {
            if (!isSubscribed && subscribeToEvents)
            {
                timerPing.Tick += TimerPing_Tick;
                PlcStatusEvent.NewMessage += PlcStatusEvent_NewMessage;
                OnlineStatusEvent.NewMessage += OnlineStatusEvent_NewMessage;
                isSubscribed = true;
            }
            else if (isSubscribed && !subscribeToEvents)
            {
                timerPing.Tick -= TimerPing_Tick;
                PlcStatusEvent.NewMessage -= PlcStatusEvent_NewMessage;
                OnlineStatusEvent.NewMessage -= OnlineStatusEvent_NewMessage;
                isSubscribed = false;
            }
        }

        private void OnlineStatusEvent_NewMessage(object sender, OnlineStatusEventArgs e)
        {
            if (e.PlcName == PlcName)
            {
                ExternalElapsedTime = e.ElapsedTime;
                ExternalOnlineColor = e.Color;
                ExternalOnlineMessage = e.Message;
                ExternalCommunicationStatus = e.connectionStatus;
            }
        }

        private void PlcStatusEvent_NewMessage(object sender, PlcStatusEventArgs e)
        {
            if (e.PlcName == e.PlcName)
            {
                ExternalStatus = e.Status;
                ExternalStatusMessage = e.Message;
            }
        }

        private void TimerPing_Tick(object sender, EventArgs e)
        {
            if (!IP.Ping())
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                UnableToPing = true;

            }
            else
            {
                UnableToPing = false;
            }

        }

        #endregion

    }
}
