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



        #region Properties
        private DateTime UpTimeStart = DateTime.MaxValue;
        private DispatcherTimer timerPing = new DispatcherTimer();
        private bool isSubscribed = false;
        private bool subscribe = false;

        public bool Active = false;
        public int Id = 0;

        public DateTime LastReconnect = DateTime.MinValue;
        public int MaxReconnectRetries { get; private set; } = 4;
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
                        UpTimeStart = DateTime.Now;
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
                    return DateTime.Now - UpTimeStart;
            }
        }
        public string UpTimeString
        {
            get
            {
                if (UpTimeStart == DateTime.MaxValue)
                    return "0s";
                var tid = DateTime.Now - UpTimeStart;
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


        #region Logging classes
        public Logger logger;
        #endregion

        #region Constructor
        public ExtendedPlc(CpuType cpu, string ip, short rack, short slot) : base(cpu, ip, rack, slot)
        {
            SubscribeEvents(true);
            timerPing.Interval = new TimeSpan(0, 0, 10);
            logger = new Logger(this);
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
