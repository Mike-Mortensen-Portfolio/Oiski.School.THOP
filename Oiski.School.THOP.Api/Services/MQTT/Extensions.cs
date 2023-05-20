using MQTTnet.Client;
using System.Text;
using System.Text.Json;

namespace Oiski.School.THOP.Api.Services.MQTT
{
    public static class Extensions
    {
        /// <summary>
        /// Dump <paramref name="obj"/> to the <see cref="Console"/> as a <strong>JSON</strong> <see langword="object"/>
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TObject DumpToConsole<TObject>(this TObject obj)
        {
            var output = "NULL";
            if (obj != null)
                output = JsonSerializer.Serialize(obj, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            Console.WriteLine($"[{obj?.GetType().Name}]:\r\n{output}");

            return obj;
        }

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
        /// Add the <see cref="MyMQTTClient"/> as a hosted service
        /// </summary>
        /// <param name="builder"></param>
        public static void HookMQTTWorker(this WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<MQTTClientWorker>();
        }
    }
}
