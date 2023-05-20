using MQTTnet.Client;
using Newtonsoft.Json;
using Oiski.School.THOP.Api.Services.DataContainers;
using Oiski.School.THOP.Api.Services.Influx;
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

            _broker = configuration["HiveMQ:Broker"]!;
            _username = configuration["HiveMQ:Credentials:Username"]!;
            _password = configuration["HiveMQ:Credentials:Password"]!;
            _subs = configuration
                .GetSection("Subs")
                .Get<List<TopicData>>()!;

            _client = client;
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var resultCode = await _client.ConnectWithTLS(_broker, _username, _password);
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

        private async Task PushClimateDataToInflux(string payload)
        {
            HumidexDTO? humidex = null;

            try
            {
                humidex = JsonConvert.DeserializeObject<HumidexDTO>(payload);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Data couldn't be deserialized");
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
