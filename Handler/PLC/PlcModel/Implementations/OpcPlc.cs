using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
using OpcUa.DA;
using OpcUa.UI.Controls;
using OpcUaHm.Common;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Navigation;

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
        public bool IsConnected
        {
            get
            {
                return Client.Status == OpcStatus.Connected;
            }
        }
        public string RootNodeName => Client.RootNode.Name;
        public bool UnableToPing { get; private set; }
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

        private bool opcDaDisconnectedFlag = false;
        private ConnectionStatus status;
        public ConnectionStatus ConnectionStatus
        {
            get
            {
                if (Client is UaClient)
                {
                    if (Client == null || !IsConnected)
                    {
                        status = ConnectionStatus.Disconnected;
                        return status;
                    }
                    return status;
                }
                else if (Client is DaClient daClient)
                {
                    var newStatus = daClient.Status == OpcStatus.NotConnected ? ConnectionStatus.Disconnected : ConnectionStatus.Connected;
                    if (opcDaDisconnectedFlag)
                    {
                        opcDaDisconnectedFlag = false;
                        return ConnectionStatus.Disconnected;
                    }
                    if (newStatus != status)
                    {
                        OnConnectionChanged?.Invoke(newStatus);
                    }
                    return newStatus;
                }
                return ConnectionStatus.Disconnected;
            }
            set
            {
                if (Client is UaClient)
                {
                    if (status == ConnectionStatus.Disconnected && value == ConnectionStatus.Connected)
                        UpTimeStart = DateTime.Now;
                    status = value;
                }
                else if (Client is DaClient daClient)
                {
                    if (value == ConnectionStatus.Disconnected)
                    {
                        opcDaDisconnectedFlag = true;
                    }
                }
                OnConnectionChanged?.Invoke(value);
            }
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

        public bool Active { get; set; } = false;
        public DateTime LastReconnect { get; set; } = DateTime.MinValue;
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
        public DateTime LastPlcStatusCheck { get; set; }


        // Constructor
        public OpcPlc(string connectionString, OpcType opcType, int id)
        {
            this.Id = id;
            ConnectionString = connectionString;
            this.opcType = opcType;
            Initialize();
            if (Client is UaClient uaClient)
            {
                uaClient.ServerConnectionLost += OpcClient_ServerConnectionLost;
                uaClient.ServerConnectionRestored += OpcClient_ServerConnectionRestored;
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

        private void Initialize()
        {
            if (opcType == OpcType.OpcUa)
                Client = new UaClient(new Uri(ConnectionString), Application.ProductName);
            else
                Client = new DaClient(ConnectionString);
        }

        public void Connect()
        {
            if (Client == null)
                Initialize();
            Task.Run(async () => await Client.Connect());
        }

        public async Task ConnectAsync(int timeout = 1000)
        {
            if (Client == null)
                Initialize();

            try
            {
                using CancellationTokenSource cts = new CancellationTokenSource(timeout);
                cts.Token.ThrowIfCancellationRequested();
                await Client.Connect(cts);
            }
            catch (OperationCanceledException ex)
            {
                throw new TimeoutException("Timeout when connecting to OPC server", ex);
            }
        }

        public void Disconnect()
        {
            Client.Dispose();
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await ConnectAsync();
                if (IsConnected)
                    return true;
            }
            catch (Exception) { }
            finally
            {
                Disconnect();
            }
            return false;
        }

        public ReadValue Read(PlcTag tag)
        {
            try
            {
                var temp = Client.Read<object>(tag.Name);
                return new ReadValue(this, temp);
            }
            catch (OpcException)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public ReadValue Read(string address)
        {
            try
            {
                var temp = Client.Read<object>(address);
                return new ReadValue(this, temp);
            }
            catch (OpcException)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }


        public async Task<ReadValue> ReadAsync(PlcTag tag, CancellationToken cancellationToken = default)
        {
            try
            {
                var value = await Client.ReadAsync<object>(tag.Name);
                return new ReadValue(this, value);
            }
            catch (OpcException)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }


        public async Task<ReadValue> ReadAsync(string address, CancellationToken cancellationToken = default)
        {
            try
            {
                var readEvent = await Client.ReadAsync<object>(address);
                var readValue = new ReadValue(this, readEvent.Value);
                return readValue;
            }
            catch (OpcException)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public void Write(PlcTag tag, object value)
        {
            Client.Write(tag.Address, value);
        }

        public void Write(string address, object value)
        {
            Client.Write(address, value);
        }

        public async Task WriteAsync(string address, object value, CancellationToken cancellationToken = default)
        {
            await Client.WriteAsync(address, value);
        }

        public async Task WriteAsync(PlcTag tag, object value, CancellationToken cancellationToken = default)
        {
            await Client.WriteAsync(tag.Name, value);
        }

        public IEnumerable<Node> ExploreOpc(string filter = "", bool onlyFolders = false, bool includeSubVariables = false)
        {
            return Client.ExploreOpc(filter, onlyFolders, includeSubVariables);
        }

        public void Dispose()
        {
            if (Client is UaClient uaClient)
            {
                uaClient.ServerConnectionLost -= OpcClient_ServerConnectionLost;
                uaClient.ServerConnectionRestored -= OpcClient_ServerConnectionRestored;
            }
            Client.Dispose();
            GC.SuppressFinalize(this);
        }

        public BrowseOpcTreeControl CreateOpcTree()
        {
            if (Client is UaClient uaClient)
            {
                return new BrowseOpcTreeControl(uaClient.MySession, uaClient);
            }
            return null;
        }

        public void WriteBytes(PlcTag tag, byte[] value)
        {
            throw new NotImplementedException();
        }

        public Task WriteBytesAsync(PlcTag tag, byte[] value, CancellationToken cancellationToken = default)
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

        public Task<byte[]> ReadBytesAsync(PlcTag tag, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadBytes(PlcTag tag, int nrOfElements)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadBytesAsync(PlcTag tag, int nrOfElements, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCpuInRunAsync()
        {
            throw new NotImplementedException();
        }
    }
}
