using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Text;

namespace Oiski.School.THOP.Api.Services.MQTT
{
    public class MyMQTTClient : IDisposable
    {
        private MqttFactory _factory;
        private IMqttClient? _client;
        private Action<string>? _logger;

        /// <summary>
        /// Instantiates a new instance of type <see cref="MyMQTTClient"/>
        /// </summary>
        public MyMQTTClient()
        {
            _factory = new MqttFactory();
            _client = _factory.CreateMqttClient();
        }

        /// <summary>
        /// Instantiates a new instance of type <see cref="MyMQTTClient"/> with a logger <see cref="Delegate"/> attached
        /// </summary>
        /// <param name="logger"></param>
        public MyMQTTClient(Action<string> logger) : this()
        {
            _logger = logger;
        }

        public void Dispose()
        {
            _factory = null!;
            _client?.DisconnectAsync(new MqttClientDisconnectOptionsBuilder()
                .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
                .Build());
            _client = null;
            _logger = null;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="broker"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<MqttClientConnectResultCode> Connect(string broker, string username = null!, string password = null!)
        {
            if (_client == null)
                throw new NullReferenceException("Client can't be null");

            var mqttClientbuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(broker);
            if (username != null && password != null)
                mqttClientbuilder
                .WithCredentials(username, password);
            var mqttClientOptions = mqttClientbuilder
                .Build();

            _logger?.Invoke($"Connecting to: {broker}...");

            var result = await _client.ConnectAsync(mqttClientOptions, CancellationToken.None);

            _logger?.Invoke($"Connection status: {result.ResultCode}");

            return result.ResultCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="broker"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public async Task<MqttClientConnectResultCode> ConnectWithTLS(string broker, string username, string password)
        {
            if (_client == null)
                throw new NullReferenceException("Client can't be null");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidDataException("Username and password must be specified when using TLS");

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(broker, 8883)
                .WithCredentials(username, password)
                .WithTls()
                .Build();

            _logger?.Invoke($"Connecting to: {broker} with TLS...");

            using var timeout = new CancellationTokenSource(5000);
            var result = await _client.ConnectAsync(mqttClientOptions, timeout.Token);

            _logger?.Invoke($"Connection status: {result.ResultCode}");

            return result.ResultCode;
        }

        /// <summary>
        /// Use this to handle recieved <strong>MQTT</strong> messages
        /// <br/>
        /// <br/>
        /// <strong>Note:</strong> Do not loop this method or excute multiple times, as the <paramref name="handler"/>
        /// will be registered as a listener and scoop every incommming message in its sub range
        /// </summary>
        /// <param name="handler"></param>
        /// <exception cref="NullReferenceException"></exception>
        public void HandleMessage(Action<string> handler)
        {
            if (_client == null)
                throw new NullReferenceException("Client can't be null");

            _client.ApplicationMessageReceivedAsync += (e) =>
            {
                _logger?.Invoke($"Message recieved:");
                handler(e.GetPayload());

                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="qualityOfService"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task SubAsync(string topic, MqttQualityOfServiceLevel qualityOfService = MqttQualityOfServiceLevel.AtLeastOnce)
        {
            if (_factory == null)
                throw new NullReferenceException("Factory can't be null");

            if (_client == null)
                throw new NullReferenceException("Client can't be null");

            var mqttSubscribeOptions = _factory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter((f) =>
                {
                    f.WithTopic(topic);
                    f.WithQualityOfServiceLevel(qualityOfService);
                })
            .Build();

            _logger?.Invoke($"Subscribing to: {topic} with QoS: {qualityOfService} ({(int)qualityOfService})");
            var result = await _client.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
            _logger?.Invoke($"Sub status: {result.Items.First().ResultCode}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="payload"></param>
        /// <param name="qualityOfService"></param>
        /// <param name="retain"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<MqttClientPublishResult> PubAsync(string topic, string payload, MqttQualityOfServiceLevel qualityOfService = MqttQualityOfServiceLevel.AtLeastOnce, bool retain = false)
        {
            if (topic == null)
                throw new NullReferenceException("Topic can't be null");

            if (payload == null)
                throw new NullReferenceException("Payload can't be null");

            var payloadBytes = Encoding.Default.GetBytes(payload);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithQualityOfServiceLevel(qualityOfService)
                .WithRetainFlag(retain)
                .WithPayload(payloadBytes)
                .Build();

            _logger?.Invoke($"Publishing message to: {topic} ({(retain ? "Retained" : "Not Retained")}) with QoS: {qualityOfService}({(int)qualityOfService}){Environment.NewLine}Payload: {payload}");
            var result = await _client!.PublishAsync(message, CancellationToken.None);
            _logger?.Invoke($"Message status: {(result.IsSuccess ? "Success" : "Failure")}");

            return result;
        }
    }
}