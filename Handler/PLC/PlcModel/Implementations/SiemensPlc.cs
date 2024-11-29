using FileLogger;
using S7.Net;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OptimaValue
{
    public class SiemensPlc : IPlc, IDisposable
    {
        private S7.Net.Plc myPlc;
        private CpuType cpuType;
        private bool disposed = false;

        public Image Image { get; set; }
        public event Action<ConnectionStatus> OnConnectionChanged;
        public event EventHandler<EventArgs> StartedEvent;

        private ConnectionStatus status;
        public ConnectionStatus ConnectionStatus
        {
            get => status;
            set
            {
                if ((status == ConnectionStatus.Disconnected || status == ConnectionStatus.Connecting) && value == ConnectionStatus.Connected)
                {
                    UpTimeStart = DateTime.UtcNow;
                }

                status = value;
                OnConnectionChanged?.Invoke(value);
            }
        }

        public SiemensPlc() { }

        public SiemensPlc(S7.Net.CpuType cpuType, string ip, short rack, short slot)
        {
            this.cpuType = (CpuType)cpuType;
            myPlc = new Plc(cpuType, ip, rack, slot);
        }

        public string PlcName { get; set; }
        public bool IsConnected
        {
            get
            {
                if (!myPlc.IsConnected)
                    ConnectionStatus = ConnectionStatus.Disconnected;
                return ConnectionStatus == ConnectionStatus.Connected;
            }
        }

        public bool IsStreamConnected => myPlc.IsConnected;
        public CpuType CpuType => cpuType;
        public int Id { get; set; } = 0;
        public DateTime UpTimeStart { get; set; } = DateTime.MaxValue;

        public TimeSpan UpTime => UpTimeStart == DateTime.MaxValue ? TimeSpan.Zero : DateTime.UtcNow - UpTimeStart;
        public string UpTimeString => GetUptimeString();

        private string GetUptimeString()
        {
            if (UpTimeStart == DateTime.MaxValue)
                return "0s";

            var tid = UpTime;
            var sb = new StringBuilder();

            if (tid.Days > 0) sb.Append($"{tid.Days}d ");
            if (tid.Hours > 0 || sb.Length > 0) sb.Append($"{tid.Hours}h ");
            if (tid.Minutes > 0 || sb.Length > 0) sb.Append($"{tid.Minutes}m ");
            sb.Append($"{tid.Seconds}s");

            return sb.ToString();
        }

        public bool Active { get; set; } = false;
        public DateTime LastReconnect { get; set; } = DateTime.MinValue;

        private short watchDog = 0;
        public short WatchDog
        {
            get
            {
                watchDog = (short)((watchDog + 1) % 5001);
                return watchDog;
            }
        }

        public bool Alarm { get; set; } = false;
        public bool isNotPlc { get; } = false;

        #region Messages
        public string ExternalStatusMessage { get; set; } = string.Empty;
        public Status ExternalStatus { get; set; } = Status.Ok;
        public string ExternalElapsedTime { get; set; } = string.Empty;
        public ConnectionStatus ExternalCommunicationStatus { get; set; } = ConnectionStatus.Disconnected;
        public Color ExternalOnlineColor { get; set; } = Color.Gray;
        public string ExternalOnlineMessage { get; set; } = "Ej ansluten";
        public string ConnectionString { get; set; }
        public DateTime LastPlcStatusCheck { get; set; }
        public int TotalConnectionAttempts { get; set; }
        public int FailedConnectionAttempts { get; set; }
        public TimeSpan TotalReconnectTime { get; set; }
        #endregion

        #region Sync Plc
        public int SyncTimeDbNr;
        public int SyncTimeOffset;
        public string SyncBoolAddress = string.Empty;
        public bool SyncActive;
        public DateTime lastSyncTime;
        #endregion

        public bool Connect()
        {
            var stopwatch = Stopwatch.StartNew();
            TotalConnectionAttempts++;
            myPlc.Open();
            SetConnectionStatus();
            TotalReconnectTime += stopwatch.Elapsed;
            return myPlc.IsConnected;
        }

        public async Task ConnectAsync(int timeOut = 1000)
        {
            var stopwatch = Stopwatch.StartNew();
            TotalConnectionAttempts++;

            try
            {
                using CancellationTokenSource cts = new CancellationTokenSource(timeOut);
                await myPlc.OpenAsync(cts.Token).ConfigureAwait(false);
                SetConnectionStatus();
            }
            catch (OperationCanceledException ex)
            {
                Logger.LogError("Timeout when connecting to PLC", ex);
                throw new TimeoutException($"Timeout when connecting to PLC {PlcName}", ex);
            }
            finally
            {
                stopwatch.Stop();
                TotalReconnectTime += stopwatch.Elapsed;
            }
        }

        public void Disconnect()
        {
            myPlc?.Close();
            SetConnectionStatus();
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await myPlc.OpenAsync().ConfigureAwait(false);
                return IsConnected;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Misslyckades att ansluta till {PlcName}", ex);
                return false;
            }
            finally
            {
                myPlc.Close();
            }
        }

        public async Task<bool> IsCpuInRunAsync()
        {
            try
            {
                byte result = await myPlc.ReadStatusAsync().ConfigureAwait(false);
                return result == 0x08;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ReadValue Read(PlcTag tag)
        {
            try
            {
                var temp = myPlc.Read(tag.DataType, tag.BlockNr, tag.StartByte, (S7.Net.VarType)tag.VarType, tag.NrOfElements, tag.BitAddress);
                return new ReadValue(this, temp);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading tag {tag.Name} from PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public ReadValue Read(string address)
        {
            try
            {
                var temp = myPlc.Read(address);
                return new ReadValue(this, temp);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading address {address} from PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public async Task<ReadValue> ReadAsync(PlcTag tag, CancellationToken cancellationToken = default)
        {
            try
            {
                var temp = await myPlc.ReadAsync(tag.DataType, tag.BlockNr, tag.StartByte, (S7.Net.VarType)tag.VarType, tag.NrOfElements, tag.BitAddress, cancellationToken).ConfigureAwait(false);
                return new ReadValue(this, temp);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading tag {tag.Name} from PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public async Task<ReadValue> ReadAsync(string address, CancellationToken cancellationToken = default)
        {
            try
            {
                var temp = await myPlc.ReadAsync(address, cancellationToken).ConfigureAwait(false);
                return new ReadValue(this, temp);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading address {address} from PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public byte[] ReadBytes(PlcTag tag)
        {
            try
            {
                return myPlc.ReadBytes(tag.DataType, tag.BlockNr, tag.StartByte, tag.NrOfElements);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading bytes for tag {tag.Name} from PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public async Task<byte[]> ReadBytesAsync(PlcTag tag, CancellationToken cancellationToken = default)
        {
            try
            {
                return await myPlc.ReadBytesAsync(tag.DataType, tag.BlockNr, tag.StartByte, tag.NrOfElements, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading bytes for tag {tag.Name} from PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public byte[] ReadBytes(PlcTag tag, int nrOfElements)
        {
            try
            {
                return myPlc.ReadBytes(tag.DataType, tag.BlockNr, tag.StartByte, nrOfElements);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading bytes for tag {tag.Name} from PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public async Task<byte[]> ReadBytesAsync(PlcTag tag, int nrOfElements, CancellationToken cancellationToken = default)
        {
            try
            {
                return await myPlc.ReadBytesAsync(tag.DataType, tag.BlockNr, tag.StartByte, nrOfElements, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading bytes for tag {tag.Name} from PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public void Write(PlcTag tag, object value)
        {
            try
            {
                myPlc.Write(tag.DataType, tag.BlockNr, tag.StartByte, value, tag.BitAddress);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error writing value to tag {tag.Name} in PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public void Write(string address, object value)
        {
            try
            {
                myPlc.Write(address, value);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error writing value to address {address} in PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public async Task WriteAsync(string address, object value, CancellationToken cancellationToken = default)
        {
            try
            {
                await myPlc.WriteAsync(address, value, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error writing value to address {address} in PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public async Task WriteAsync(PlcTag tag, object value, CancellationToken cancellationToken = default)
        {
            try
            {
                await myPlc.WriteAsync(tag.DataType, tag.BlockNr, tag.StartByte, value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error writing value to tag {tag.Name} in PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public void WriteBytes(PlcTag tag, byte[] value)
        {
            try
            {
                myPlc.WriteBytes(tag.DataType, tag.BlockNr, tag.StartByte, value);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error writing bytes to tag {tag.Name} in PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public async Task WriteBytesAsync(PlcTag tag, byte[] value, CancellationToken cancellationToken = default)
        {
            try
            {
                await myPlc.WriteBytesAsync(tag.DataType, tag.BlockNr, tag.StartByte, value).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error writing bytes to tag {tag.Name} in PLC {PlcName}", ex);
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;

            myPlc.Close();
            myPlc = null;

            disposed = true;
            GC.SuppressFinalize(this);
        }

        private void SetConnectionStatus()
        {
            if (myPlc is null)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                return;
            }
            ConnectionStatus = myPlc.IsConnected ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
        }
    }
}
