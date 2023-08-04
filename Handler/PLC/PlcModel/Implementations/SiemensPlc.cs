using OpcUa.DA;
using OpcUaHm.Common;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace OptimaValue
{
    public class SiemensPlc : IPlc
    {
        private S7.Net.Plc myPlc;
        private CpuType cpuType;
        private readonly System.Timers.Timer timerPing = new System.Timers.Timer()
        {
            Interval = 30 * 1000
        };
        private System.Windows.Forms.Timer onlineTimer = new System.Windows.Forms.Timer()
        {
            Interval = 500,
        };

        public Image Image { get; set; }
        public event Action<ConnectionStatus> OnConnectionChanged;
        public event EventHandler<EventArgs> StartedEvent;

        private ConnectionStatus status;
        public ConnectionStatus ConnectionStatus
        {
            get => status;
            set
            {
                if (status == ConnectionStatus.Disconnected || status == ConnectionStatus.Connecting)
                {
                    if (value == ConnectionStatus.Connected)
                    {
                        UpTimeStart = DateTime.UtcNow;
                        timerPing.Start();
                    }
                }
                if (value != ConnectionStatus.Connected)
                    timerPing.Stop();
                status = value;
                OnConnectionChanged?.Invoke(value);
            }
        }

        private void TimerPing_Tick(object sender, EventArgs e)
        {
            if (!Ping())
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                UnableToPing = true;

            }
            else
            {
                UnableToPing = false;
            }
        }

        public SiemensPlc()
        {
        }

        public SiemensPlc(S7.Net.CpuType cpuType, string ip, short rack, short slot)
        {
            this.cpuType = (CpuType)cpuType;
            myPlc = new Plc(cpuType, ip, rack, slot);
            timerPing.Elapsed += TimerPing_Tick;
        }

        public string PlcName { get; set; }

        public bool IsConnected => myPlc.IsConnected && ConnectionStatus == ConnectionStatus.Connected;

        public CpuType CpuType => cpuType;

        public int Id { get; set; } = 0;
        public DateTime UpTimeStart { get; set; } = DateTime.MaxValue;
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

        public bool Active { get; set; } = false;
        public DateTime LastReconnect { get; set; } = DateTime.MinValue;
        public bool UnableToPing { get; private set; }

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
        #endregion

        #region Sync Plc
        public int SyncTimeDbNr;
        public int SyncTimeOffset;
        public string SyncBoolAddress = string.Empty;
        public bool SyncActive;
        public DateTime lastSyncTime;
        #endregion

        public void Connect()
        {
            myPlc.Open();
            ConnectionStatus = myPlc.IsConnected ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
        }

        public async Task ConnectAsync(int timeOut = 1000)
        {
            try
            {
                using CancellationTokenSource cts = new CancellationTokenSource(timeOut);

                await myPlc.OpenAsync(cts.Token);
                ConnectionStatus = myPlc.IsConnected ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
            }
            catch (OperationCanceledException ex)
            {
                throw new TimeoutException("Timeout when connecting to PLC", ex);
            }
        }

        public void Disconnect()
        {
            myPlc.Close();
            ConnectionStatus = myPlc.IsConnected ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await myPlc.OpenAsync();
                return IsConnected;
            }
            catch (Exception) { }
            finally
            {
                myPlc.Close();
            }
            return false;
        }

        public async Task<bool> IsCpuInRunAsync()
        {
            byte result = 0;
            try
            {
                result = await myPlc.ReadStatusAsync();
                return result == 0x08;
            }
            catch (Exception) { return false; }
        }

        public ReadValue Read(PlcTag tag)
        {
            try
            {
                var temp = myPlc.Read(tag.DataType, tag.BlockNr, tag.StartByte, (S7.Net.VarType)tag.VarType, tag.NrOfElements, tag.BitAddress);
                return new ReadValue(this, temp);
            }
            catch (PlcException)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
            catch (IOException ex)
            {
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
            catch (PlcException)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
            catch (IOException ex)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public async Task<ReadValue> ReadAsync(PlcTag tag, CancellationToken cancellationToken = default)
        {
            try
            {
                var temp = await myPlc.ReadAsync(tag.DataType, tag.BlockNr, tag.StartByte, (S7.Net.VarType)tag.VarType, tag.NrOfElements, tag.BitAddress, cancellationToken);
                return new ReadValue(this, temp);

            }
            catch (PlcException)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
            catch (IOException ex)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public async Task<ReadValue> ReadAsync(string address, CancellationToken cancellationToken = default)
        {
            try
            {
                var temp = await myPlc.ReadAsync(address, cancellationToken);
                return new ReadValue(this, temp);
            }
            catch (PlcException)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
            catch (IOException ex)
            {
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
            catch (PlcException)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
            catch (IOException ex)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
        }

        public async Task<byte[]> ReadBytesAsync(PlcTag tag, CancellationToken cancellationToken = default)
        {
            try
            {
                return await myPlc.ReadBytesAsync(tag.DataType, tag.BlockNr, tag.StartByte, tag.NrOfElements, cancellationToken);
            }
            catch (PlcException)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
            catch (IOException ex) { ConnectionStatus = ConnectionStatus.Disconnected; throw; }
        }

        public byte[] ReadBytes(PlcTag tag, int nrOfElements)
        {
            try
            {
                return myPlc.ReadBytes(tag.DataType, tag.BlockNr, tag.StartByte, nrOfElements);
            }
            catch (PlcException)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
            catch (IOException ex) { ConnectionStatus = ConnectionStatus.Disconnected; throw; }
        }

        public async Task<byte[]> ReadBytesAsync(PlcTag tag, int nrOfElements, CancellationToken cancellationToken = default)
        {
            try
            {
                return await myPlc.ReadBytesAsync(tag.DataType, tag.BlockNr, tag.StartByte, nrOfElements, cancellationToken);
            }
            catch (PlcException)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                throw;
            }
            catch (IOException ex) { ConnectionStatus = ConnectionStatus.Disconnected; throw; }
        }
        public void Write(PlcTag tag, object value)
        {
            myPlc.Write(tag.DataType, tag.BlockNr, tag.StartByte, value, tag.BitAddress);
        }

        public void Write(string address, object value)
        {
            myPlc.Write(address, value);
        }

        public async Task WriteAsync(string address, object value, CancellationToken cancellationToken = default)
        {
            await myPlc.WriteAsync(address, value, cancellationToken);
        }

        public async Task WriteAsync(PlcTag tag, object value, CancellationToken cancellationToken = default)
        {
            await myPlc.WriteAsync(tag.DataType, tag.BlockNr, tag.StartByte, value);
        }

        public void WriteBytes(PlcTag tag, byte[] value)
        {
            myPlc.WriteBytes(tag.DataType, tag.BlockNr, tag.StartByte, value);
        }

        public async Task WriteBytesAsync(PlcTag tag, byte[] value, CancellationToken cancellationToken = default)
        {
            await myPlc.WriteBytesAsync(tag.DataType, tag.BlockNr, tag.StartByte, value);
        }

        public void Dispose()
        {
            timerPing.Elapsed += TimerPing_Tick;
            myPlc.Close();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns True if pingable<para></para>Timeout of 2 seconds <para></para>
        /// Throws a <see cref="NullReferenceException"/> if <paramref name="plc"/> is null<para></para>
        /// Throws a <see cref="PingException"/> if not valid IP address format
        /// </summary>
        /// <returns></returns>
        public bool Ping()
        {
            if (myPlc == null)
                throw new NullReferenceException("Plc kan ej vara null");
            if (!myPlc.IP.CheckValidIpAddress())
                throw new PingException("Inte giltig IP-adress");
            Ping pinger = null;
            var Pingable = false;
            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(myPlc.IP, 2000); // Timeout-tid 2 sekunder
                Pingable = reply.Status == IPStatus.Success;

                if (!Pingable)
                {
                    return false;
                }
            }
            catch (PingException)
            {
                if (!Pingable)
                    return false;
            }
            finally
            {
                if (pinger != null)
                    pinger.Dispose();
            }
            return Pingable;
        }
    }
}
