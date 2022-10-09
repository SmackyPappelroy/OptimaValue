using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    public interface IPlc : IDisposable
    {
        System.Drawing.Image Image { get; set; }
        string PlcName { get; set; }
        event Action<ConnectionStatus> OnConnectionChanged;
        ConnectionStatus ConnectionStatus { get; set; }
        bool IsConnected { get; }
        CpuType CpuType { get; }
        bool isNotPlc { get; }
        string ConnectionString { get; set; }

        DateTime UpTimeStart { get; set; }
        public TimeSpan UpTime { get; }
        public string UpTimeString { get; }
        DateTime LastReconnect { get; set; }

        public short WatchDog { get; }
        bool Alarm { get; set; }

        bool Ping();
        void Connect();
        Task ConnectAsync();
        void Disconnect();
        object Read(PlcTag tag);
        object Read(string address);
        byte[] ReadBytes(PlcTag tag);
        byte[] ReadBytes(PlcTag tag, int nrOfElements);
        Task<byte[]> ReadBytesAsync(PlcTag tag);
        Task<byte[]> ReadBytesAsync(PlcTag tag, int nrOfElements);
        Task<object> ReadAsync(PlcTag tag);
        Task<object> ReadAsync(string address);
        void Write(PlcTag tag, object value);
        void Write(string address, object value);
        void WriteBytes(PlcTag tag, byte[] value);
        Task WriteAsync(string address, object value);
        Task WriteBytesAsync(PlcTag tag, byte[] value);
        Task WriteAsync(PlcTag tag, object value);
    }
}
