using MQTTnet.Protocol;

namespace Oiski.School.THOP.Api.Services.MQTT
{
    public class TopicData
    {
        public string? Topic { get; set; }
        public MqttQualityOfServiceLevel QoS { get; set; }
    }
}
