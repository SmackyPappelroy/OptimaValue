using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    public interface IPlcExtensions
    {
        #region Messages
        string ExternalStatusMessage { get; set; }
        Status ExternalStatus { get; set; }

        string ExternalElapsedTime { get; set; }
        ConnectionStatus ExternalCommunicationStatus { get; set; }
        System.Drawing.Color ExternalOnlineColor { get; set; }
        string ExternalOnlineMessage { get; set; }
        #endregion

        #region Sync Plc
        int SyncTimeDbNr { get; set; }
        int SyncTimeOffset { get; set; }
        string SyncBoolAddress { get; set; }
        bool SyncActive { get; set; }
        DateTime lastSyncTime { get; set; }
        #endregion

        string ConnectionString { get; }
        CpuType CpuType { get; }
        DateTime UpTimeStart { get; }
        System.Timers.Timer timerPing { get; }
        bool isSubscribed { get; }
        public OpcType OpcType { get; }
        bool isOpc { get; }

        public bool LoggerIsStarted { get; set; }
        public event EventHandler<EventArgs> StartedEvent;

        public bool ActiveTagsInPlc { get; }
    }
}
