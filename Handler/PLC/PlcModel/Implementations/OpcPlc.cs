using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
using OpcUa.DA;
using OpcUaHm.Common;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Windows.Media.Core;
using Windows.Media.Protection.PlayReady;

namespace OptimaValue
{
    internal class OpcPlc : IPlc
    {
        public IClient<Node> Client;
        private OpcType opcType;

        public Image Image { get; set; }
        public event Action<ConnectionStatus> OnConnectionChanged;
        public string ConnectionString { get; set; }
        public string PlcName { get; set; }
        public bool IsConnected => Client.Status == OpcStatus.Connected;
        public string RootNodeName => Client.RootNode.Name;
        public OpcType OpcType
        {
            get
            {
                if (Client is UaClient)
                {
                    return OpcType.OpcUa;
                }
                return OpcType.OpcDa;
            }
        }

        public ConnectionStatus ConnectionStatus
        {
            get;
            set;
        }
        public int Id { get; set; } = 0;
        public CpuType CpuType
        {
            get
            {
                switch (OpcType)
                {
                    case OpcType.OpcUa:
                        return CpuType.OpcUa;
                    case OpcType.OpcDa:
                        return CpuType.OpcDa;
                    default:
                        return CpuType.Unknown;
                }
            }
        }

        public bool isNotPlc { get; } = true;
        public DateTime UpTimeStart { get; set; } = DateTime.MaxValue;
        public TimeSpan UpTime { get; }
        public string UpTimeString { get; }

        public bool Active { get; set; } = false;
        public DateTime LastReconnect { get; set; } = DateTime.MinValue;
        public bool UnableToPing { get; private set; }
        public int ActivePlcId { get; set; }

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
        public string ExternalStatusMessage { get; set; } = string.Empty;
        public Status ExternalStatus { get; set; } = Status.Ok;
        public string ExternalElapsedTime { get; set; } = string.Empty;
        public ConnectionStatus ExternalCommunicationStatus { get; set; } = ConnectionStatus.Disconnected;
        public Color ExternalOnlineColor { get; set; } = Color.Gray;
        public string ExternalOnlineMessage { get; set; } = "Ej ansluten";


        // Constructor
        public OpcPlc(string connectionString, OpcType opcType, int id)
        {
            this.Id = id;
            ConnectionString = connectionString;
            this.opcType = opcType;
            Initialize();
        }

        private void Initialize()
        {
            if (opcType == OpcType.OpcUa)
                Client = new UaClient(new Uri(ConnectionString));
            else
                Client = new DaClient(ConnectionString);
        }

        public void Connect()
        {
            if (Client == null)
                Initialize();
            Task.Run(async () => await Client.Connect());
        }

        public async Task ConnectAsync()
        {
            if (Client == null)
                Initialize();
            await Client.Connect();
        }

        public void Disconnect()
        {
            Client.Dispose();
        }

        public object Read(PlcTag tag)
        {
            return Client.Read<object>(tag.PlcName + "." + tag.Name);

        }

        public object Read(string address)
        {
            return Client.Read<object>(address);
        }

        public async Task<object> ReadAsync(PlcTag tag)
        {
            return await Client.ReadAsync<object>(tag.PlcName + "." + tag.Name);
        }

        public async Task<object> ReadAsync(string address)
        {
            return await Client.ReadAsync<object>(address);
        }

        public void Write(PlcTag tag, object value)
        {
            Client.Write(tag.Address, value);
        }

        public void Write(string address, object value)
        {
            Client.Write(address, value);
        }

        public async Task WriteAsync(string address, object value)
        {
            await Client.WriteAsync(address, value);
        }

        public async Task WriteAsync(PlcTag tag, object value)
        {
            await Client.WriteAsync(tag.PlcName + "." + tag.Name, value);
        }

        public void Dispose()
        {
            Client.Dispose();
            GC.SuppressFinalize(this);
        }



        private void OpcClient_ServerConnectionRestored(object sender, EventArgs e)
        {
            ConnectionStatus = ConnectionStatus.Connected;
        }

        private void OpcClient_ServerConnectionLost(object sender, EventArgs e)
        {
            ConnectionStatus = ConnectionStatus.Disconnected;
        }

        public void WriteBytes(PlcTag tag, byte[] value)
        {
            throw new NotImplementedException();
        }

        public Task WriteBytesAsync(PlcTag tag, byte[] value)
        {
            throw new NotImplementedException();
        }

        public bool Ping()
        {
            throw new NotImplementedException();
        }

        public byte[] ReadBytes(PlcTag tag)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadBytesAsync(PlcTag tag)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadBytes(PlcTag tag, int nrOfElements)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadBytesAsync(PlcTag tag, int nrOfElements)
        {
            throw new NotImplementedException();
        }
    }
}
