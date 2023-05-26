using MQTTnet.Protocol;

namespace Oiski.School.THOP.Api.Services.MQTT
{
    /// <summary>
    /// Defines a set of data used to build the relationship for a given topic between broker and client
    /// </summary>
    public class TopicData
    {
        public string? Topic { get; set; }
        public MqttQualityOfServiceLevel QoS { get; set; }
    }
}
