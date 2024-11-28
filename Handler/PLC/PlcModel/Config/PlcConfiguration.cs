using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    public class PlcConfiguration
    {
        public string PlcName { get; set; }
        public CpuType CpuType { get; set; }
        public string Ip { get; set; }
        public short Rack { get; set; }
        public short Slot { get; set; }

        public int ActivePlcId { get; set; }
        public bool Active { get; set; }
        public string BrokerAddress { get; set; }
        public int BrokerPort { get; set; }

        #region Plc time synchronization
        public int SyncTimeDbNr { get; set; }
        public int SyncTimeOffset { get; set; }
        public string SyncBoolAddress { get; set; }
        public bool SyncActive { get; set; }
        public DateTime lastSyncTime { get; set; }

        public PlcConfiguration(string plcName
            , CpuType cpuType
            , string ip, short rack, short slot
            , int activePlcId, bool active
            , int syncTimeDbNr
            , int syncTimeOffset
            , string syncBoolAddress
            , bool syncActive, DateTime lastSyncTime)
        {
            PlcName = plcName;
            CpuType = cpuType;
            Ip = ip;
            Rack = rack;
            Slot = slot;
            ActivePlcId = activePlcId;
            Active = active;
            SyncTimeDbNr = syncTimeDbNr;
            SyncTimeOffset = syncTimeOffset;
            SyncBoolAddress = syncBoolAddress;
            SyncActive = syncActive;
            this.lastSyncTime = lastSyncTime;
        }
        #endregion


    }
}
