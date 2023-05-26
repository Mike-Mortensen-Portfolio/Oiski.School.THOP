using MQTTnet.Client;
using System.Text;
using System.Text.Json;

namespace Oiski.School.THOP.Api.Services.MQTT
{
    public static class Extensions
    {
        /// <summary>
        /// Dumps the payload of an <see cref="MqttApplicationMessageReceivedEventArgs"/> to the <see cref="Console"/>
        /// </summary>
        /// <param name="args"></param>
        public static void DumpToConsole(this MqttApplicationMessageReceivedEventArgs args)
        {
            Console.WriteLine(args.GetPayload());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static string GetPayload(this MqttApplicationMessageReceivedEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args), "Args can't be null");
            var payloadBytes = args.ApplicationMessage.PayloadSegment.Array;

            if (payloadBytes == null)
                throw new NullReferenceException("PayloadBytes can't be null");

            var payload = Encoding.Default.GetString(payloadBytes!);

            return payload;
        }

        /// <summary>
        /// Add the <see cref="MQTTClientWorker"/> as a hosted service
        /// </summary>
        /// <param name="builder"></param>
        public static void HookMQTTWorker(this WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<MQTTClientWorker>();
        }
    }
}
