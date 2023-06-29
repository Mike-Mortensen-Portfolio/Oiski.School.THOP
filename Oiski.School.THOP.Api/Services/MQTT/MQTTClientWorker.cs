using MQTTnet.Client;
using Newtonsoft.Json;
using Oiski.School.THOP.Api.Services.Influx;
using Oiski.School.THOP.Services.Models;
using System.Globalization;

namespace Oiski.School.THOP.Api.Services.MQTT
{
    public class MQTTClientWorker : BackgroundService
    {
        private readonly ILogger<MQTTClientWorker> _logger;
        private readonly MyMQTTClient _client;
        private readonly InfluxService _service;
        private readonly string _broker;
        private readonly string _username;
        private readonly string _password;
        private readonly List<TopicData> _subs;

        public MQTTClientWorker(ILogger<MQTTClientWorker> logger, IConfiguration configuration, MyMQTTClient client, InfluxService service)
        {
            _logger = logger;

            _broker = configuration["RabbitMQ:Broker"]!;
            _username = configuration["RabbitMQ:Credentials:Username"]!;
            _password = configuration["RabbitMQ:Credentials:Password"]!;
            _subs = configuration
                .GetSection("Subs")
                .Get<List<TopicData>>()!;

            _client = client;
            _service = service;
        }

        /// <summary>
        /// This method is executed by the hosted service handler and will register an event handler for the
        /// <see cref="MyMQTTClient"/> reciever
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var resultCode = await _client.Connect(_broker, _username, _password);
            if (resultCode != MqttClientConnectResultCode.Success)
                _logger.LogInformation("{resultCode}", resultCode.ToString());

            foreach (var sub in _subs)
                await _client.SubAsync(sub.Topic!, sub.QoS);

            _client.HandleMessage(async (payload) =>
            {
                _logger.LogInformation("Payload: {Payload}", payload);


                await PushClimateDataToInflux(payload);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>The <see cref="Task"/> that represents the <see langword="asynchronous"/> operation</returns>
        private async Task PushClimateDataToInflux(string payload)
        {
            HumidexDto? humidex = null;

            try
            {
                humidex = JsonConvert.DeserializeObject<HumidexDto>(payload);
            }
            catch (JsonReaderException)
            {
                _logger.LogWarning("Data couldn't be deserialized");
            }

            if (humidex == null)
                return;

            await _service.WriteAsync("Humidex", new("locationId", humidex.LocationId), new[]
            {
                new KeyValuePair<string, string> (nameof (humidex.Sensor).ToLowerInvariant(), humidex.Sensor),
                new KeyValuePair<string, string> (nameof (humidex.Temperature).ToLowerInvariant(), humidex.Temperature.ToString("F", new CultureInfo("en-Us"))),
                new KeyValuePair<string, string> (nameof (humidex.Humidity).ToLowerInvariant(), humidex.Humidity.ToString("F", new CultureInfo("en-Us")))
            });
        }
    }
}
