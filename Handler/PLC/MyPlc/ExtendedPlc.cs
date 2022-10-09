using DocumentFormat.OpenXml.Drawing.Charts;
using OpcUa.DA;
using OpcUaHm.Common;
using System;
using System.Drawing;
using System.Linq;

namespace OptimaValue
{
    public enum OpcType
    {
        None,
        OpcUa,
        OpcDa,
    }

    public class ExtendedPlc
    {
        public IPlc Plc;
        public PlcConfiguration PlcConfiguration { get; }


        #region Constructor
        /// <summary>
        /// Create new controller
        /// </summary>
        /// <param name="cpu"></param>
        /// <param name="ip"></param>
        /// <param name="rack"></param>
        /// <param name="slot"></param>
        public ExtendedPlc(PlcConfiguration config)
        {
            this.PlcConfiguration = config;
            if ((int)config.CpuType < 1000)
            {
                Plc = new SiemensPlc((S7.Net.CpuType)config.CpuType,
                    config.Ip, config.Rack, config.Slot);
            }
            else if (config.CpuType == CpuType.OpcUa)
            {
                Plc = new OpcPlc(config.Ip, OpcType.OpcUa, Id);
            }
            else if (config.CpuType == CpuType.OpcDa)
            {
                Plc = new OpcPlc(config.Ip, OpcType.OpcDa, Id);
            }


            Plc.PlcName = config.PlcName;
            Plc.ConnectionString = config.Ip;
            PlcName = config.PlcName;
            ActivePlcId = config.ActivePlcId;
            Active = config.Active;
            SyncTimeDbNr = config.SyncTimeDbNr;
            SyncBoolAddress = config.SyncBoolAddress;
            SyncActive = config.SyncActive;
            lastSyncTime = config.lastSyncTime;

            if (!isOpc)
                SubscribeEvents(true);
            else
                SubscribeEvents(true, true);
            timerPing.Interval = 30 * 1000;
            onlineTimer.Tick += OnlineTimer_Tick;
        }

        #endregion

        private void OnlineTimer_Tick(object sender, EventArgs e)
        {
            if (this != null)
                this.SendPlcOnlineMessage(this.ConnectionStatus, this.UpTimeString);
            else
                this.SendPlcOnlineMessage(ConnectionStatus.Disconnected, string.Empty);
        }

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
        public CpuType CpuType => Plc.CpuType;
        private DateTime UpTimeStart = DateTime.MaxValue;
        private readonly System.Timers.Timer timerPing = new System.Timers.Timer();
        private bool isSubscribed = false;
        public bool isOpc => Plc.isNotPlc;

        public new bool IsConnected => Plc.IsConnected; // isOpc ? OpcClient.Status == OpcStatus.Connected : base.IsConnected;
        public string OpcBaseFolder => IsConnected && isOpc ? ((OpcPlc)Plc).RootNodeName : "";


        #endregion

        #region Methods


        #endregion

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


        public bool ActiveTagsInPlc
        {
            get
            {
                if (TagsToLog.AllLogValues.Count == 0)
                    return false;
                else
                {
                    if (TagsToLog.AllLogValues.Any(t => t.PlcName.Equals(PlcName) && t.Active))
                        return true;
                    else
                        return false;
                }
            }
        }

        public bool Active = false;
        public int Id = 0;

        public DateTime LastReconnect = DateTime.MinValue;
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

        #region Events
        private void SubscribeEvents(bool subscribeToEvents, bool isOpc = false)
        {
            if (!isSubscribed && subscribeToEvents)
            {
                if (!isOpc)
                    timerPing.Elapsed += TimerPing_Tick;
                else if (((OpcPlc)Plc).Client is UaClient uaClient)
                {
                    uaClient.ServerConnectionLost += OpcClient_ServerConnectionLost;
                    uaClient.ServerConnectionRestored += OpcClient_ServerConnectionRestored;
                }
                else if (((OpcPlc)Plc).Client is DaClient daClient)
                {
                    ConnectionStatus = daClient.Status == OpcStatus.NotConnected ? ConnectionStatus.Disconnected : ConnectionStatus.Connected;
                }

                PlcStatusEvent.NewMessage += PlcStatusEvent_NewMessage;
                OnlineStatusEvent.NewMessage += OnlineStatusEvent_NewMessage;
                isSubscribed = true;
            }
            else if (isSubscribed && !subscribeToEvents)
            {
                if (isOpc)
                    timerPing.Elapsed -= TimerPing_Tick;
                else if (((OpcPlc)Plc).Client is UaClient uaClient)
                {
                    uaClient.ServerConnectionLost -= OpcClient_ServerConnectionLost;
                    uaClient.ServerConnectionRestored -= OpcClient_ServerConnectionRestored;
                }
                else if (((OpcPlc)Plc).Client is DaClient daClient)
                {
                    ConnectionStatus = daClient.Status == OpcStatus.NotConnected ? ConnectionStatus.Disconnected : ConnectionStatus.Connected;
                }
                PlcStatusEvent.NewMessage -= PlcStatusEvent_NewMessage;
                OnlineStatusEvent.NewMessage -= OnlineStatusEvent_NewMessage;
                isSubscribed = false;
            }
        }

        private void OpcClient_ServerConnectionRestored(object sender, EventArgs e)
        {
            ConnectionStatus = ConnectionStatus.Connected;
        }

        private void OpcClient_ServerConnectionLost(object sender, EventArgs e)
        {
            ConnectionStatus = ConnectionStatus.Disconnected;
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
            var plc = (SiemensPlc)Plc;
            if (!plc.Ping())
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                UnableToPing = true;

            }
            else
            {
                UnableToPing = false;
            }
        }
    }



    #endregion

}
