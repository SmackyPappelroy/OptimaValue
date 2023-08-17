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
        public IPlc Plc { get; set; }
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
            PlcConfiguration = config;
            InitializePlc();
            InitializeProperties();
            SubscribeEvents(true);
            onlineTimer.Tick += OnlineTimer_Tick;
        }

        private void InitializePlc()
        {
            if ((int)PlcConfiguration.CpuType < 1000)
            {
                Plc = new SiemensPlc((S7.Net.CpuType)PlcConfiguration.CpuType,
                    PlcConfiguration.Ip, PlcConfiguration.Rack, PlcConfiguration.Slot);
            }
            else if (PlcConfiguration.CpuType == CpuType.OpcUa)
            {
                Plc = new OpcPlc(PlcConfiguration.Ip, OpcType.OpcUa, Id);
            }
            else if (PlcConfiguration.CpuType == CpuType.OpcDa)
            {
                Plc = new OpcPlc(PlcConfiguration.Ip, OpcType.OpcDa, Id);
            }
        }

        private void InitializeProperties()
        {
            Plc.PlcName = PlcConfiguration.PlcName;
            Plc.ConnectionString = PlcConfiguration.Ip;
            PlcName = PlcConfiguration.PlcName;
            ActivePlcId = PlcConfiguration.ActivePlcId;
            Active = PlcConfiguration.Active;
            SyncTimeDbNr = PlcConfiguration.SyncTimeDbNr;
            SyncBoolAddress = PlcConfiguration.SyncBoolAddress;
            SyncActive = PlcConfiguration.SyncActive;
            lastSyncTime = PlcConfiguration.lastSyncTime;
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
        private bool isSubscribed = false;
        public bool isOpc => Plc.isNotPlc;

        public bool IsConnected => Plc.IsConnected; // isOpc ? OpcClient.Status == OpcStatus.Connected : base.IsConnected;
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

        private string plcName;
        public string PlcName
        {
            get => plcName;
            set
            {
                Plc.PlcName = value;
                plcName = value;
            }
        }
        public Int32 ActivePlcId { get; set; }
        public short WatchDog => Plc.WatchDog;
        public bool Alarm { get; set; } = false;

        public ConnectionStatus ConnectionStatus => Plc.ConnectionStatus;
        public TimeSpan UpTime => Plc.UpTime;
        public string UpTimeString => Plc.UpTimeString;

        #endregion

        #region Events
        private void SubscribeEvents(bool subscribeToEvents, bool isOpc = false)
        {
            if (!isSubscribed && subscribeToEvents)
            {
                PlcStatusEvent.NewMessage += PlcStatusEvent_NewMessage;
                OnlineStatusEvent.NewMessage += OnlineStatusEvent_NewMessage;
                isSubscribed = true;
            }
            else if (isSubscribed && !subscribeToEvents)
            {
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


    }
    #endregion

}
