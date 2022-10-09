using OpcDaNet.Da;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue
{
    public abstract class PlcBase
    {
        public bool IsConnected { get; }
        public CpuType CpuType { get; }
        public ConnectionStatus ConnectionStatus { get; set; }
        public string PlcName { get; set; }

        public event Action<ConnectionStatus> OnConnectionChanged;

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public object Read(ITagDefinition tag)
        {
            throw new NotImplementedException();
        }

        public object Read(string address)
        {
            throw new NotImplementedException();
        }

        public Task ReadAsync(ITagDefinition tag)
        {
            throw new NotImplementedException();
        }

        public Task ReadAsync(string address)
        {
            throw new NotImplementedException();
        }

        public void Write(ITagDefinition tag, object value)
        {
            throw new NotImplementedException();
        }

        public void Write(string address, object value)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(string address, object value)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(ITagDefinition tag, object value)
        {
            throw new NotImplementedException();
        }
    }
}
