using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace OptimaValue
{
    public class MqttPlc : IPlc
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttOptions;
        private ConnectionStatus _connectionStatus;

        public int TotalConnectionAttempts { get; set; }
        public int FailedConnectionAttempts { get; set; }
        public TimeSpan TotalReconnectTime { get; set; }
        public System.Drawing.Image Image { get; set; }
        public string PlcName { get; set; }
        public event Action<ConnectionStatus> OnConnectionChanged;
        public ConnectionStatus ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                if (_connectionStatus != value)
                {
                    _connectionStatus = value;
                    OnConnectionChanged?.Invoke(_connectionStatus); // Anropa händelse vid statusändring
                }
            }
        }
        public bool IsConnected => _mqttClient?.IsConnected ?? false;
        public bool IsStreamConnected => false;
        public CpuType CpuType => CpuType.Unknown; // Kan returnera standardvärden eller kastas som ej implementerat
        public bool isNotPlc => true;
        public string ConnectionString { get; set; }
        public DateTime UpTimeStart { get; set; }
        public TimeSpan UpTime => DateTime.Now - UpTimeStart;
        public string UpTimeString => UpTime.ToString();
        public DateTime LastReconnect { get; set; }
        public DateTime LastPlcStatusCheck { get; set; }
        public short WatchDog => 0;
        public bool Alarm { get; set; }

        public MqttPlc(string brokerAddress, int brokerPort)
        {
            _mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId("Client1")
                .WithTcpServer(brokerAddress, brokerPort)
                .Build();

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            // Event för hantering av anslutning och frånkoppling
            _mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("Ansluten till MQTT Broker.");
                await Task.CompletedTask;
            };

            _mqttClient.DisconnectedAsync += async e =>
            {
                Console.WriteLine("Frånkopplad från MQTT Broker.");
                await Task.CompletedTask;
            };
        }
        public bool Connect() => throw new NotImplementedException();

        public async Task ConnectAsync()
        {
            await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
        }

        public async Task ConnectAsync(int timeOut = 1000)
        {
            await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
        }

        public async Task<bool> TestConnectionAsync()
        {
            return await Task.FromResult(_mqttClient.IsConnected);
        }

        public void Disconnect()
        {
            _mqttClient.DisconnectAsync();
        }



        public async Task DisconnectAsync()
        {
            await _mqttClient.DisconnectAsync();
        }

        // Read-metoden för att prenumerera på ett ämne och få ett meddelande
        public async Task<string> ReadAsync(PlcTag tag, CancellationToken cancellationToken = default)
        {
            string receivedMessage = null;

            // Prenumerera på det specifika ämnet baserat på PlcTag
            await _mqttClient.SubscribeAsync(tag.Address);

            // Lyssna på inkommande meddelanden
            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                if (e.ApplicationMessage.Topic == tag.Address)
                {
                    // Använd PayloadSegment istället för Payload
                    receivedMessage = Encoding.UTF8.GetString(
                        e.ApplicationMessage.PayloadSegment.Array,
                        e.ApplicationMessage.PayloadSegment.Offset,
                        e.ApplicationMessage.PayloadSegment.Count
                    );
                    Console.WriteLine($"Mottagit meddelande: {receivedMessage}");
                }
                return Task.CompletedTask;
            };

            // Vänta tills ett meddelande har mottagits eller tills timeout
            await Task.Delay(5000, cancellationToken);  // Justera timeout vid behov
            return receivedMessage;
        }


        // Write-metoden för att publicera ett meddelande till ett specifikt ämne
        public async Task WriteAsync(PlcTag tag, string message)
        {
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(tag.Address)
                .WithPayload(message)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
                .Build();

            await _mqttClient.PublishAsync(mqttMessage, CancellationToken.None);
            Console.WriteLine($"Publicerat meddelande: {message} till ämne: {tag.Address}");
        }


        public ReadValue Read(PlcTag tag) => throw new NotImplementedException();
        public ReadValue Read(string address) => throw new NotImplementedException();
        public byte[] ReadBytes(PlcTag tag) => throw new NotImplementedException();
        public byte[] ReadBytes(PlcTag tag, int nrOfElements) => throw new NotImplementedException();
        public Task<byte[]> ReadBytesAsync(PlcTag tag, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<byte[]> ReadBytesAsync(PlcTag tag, int nrOfElements, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<ReadValue> ReadAsync(string address, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public void Write(PlcTag tag, object value) => throw new NotImplementedException();
        public void Write(string address, object value) => throw new NotImplementedException();
        public void WriteBytes(PlcTag tag, byte[] value) => throw new NotImplementedException();
        public Task WriteAsync(string address, object value, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task WriteBytesAsync(PlcTag tag, byte[] value, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task WriteAsync(PlcTag tag, object value, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<bool> IsCpuInRunAsync() => Task.FromResult(true);

        Task<ReadValue> IPlc.ReadAsync(PlcTag tag, CancellationToken cancellationToken) => throw new NotImplementedException();

        public void Dispose() => throw new NotImplementedException();
    }
}
