using Oiski.School.THOP.Services.Models;
using Polly;
using System.Diagnostics;
using System.Net.Http.Json;
using Polly;

namespace Oiski.School.THOP.Services
{
    /// <summary>
    /// Represents a service that exposes the <strong>peripheral</strong> API endpoints in the <strong>THOP</strong> Ecosystem
    /// </summary>
    public class PeripheralService
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Instantiates a new instance of type <see cref="PeripheralService"/> with an <see cref="HttpClient"/> wrapper
        /// </summary>
        /// <param name="client"></param>
        public PeripheralService(HttpClient client)
        {
            _client = client;
        }

        public async Task<bool> OpenVentsAsync(string locationId, string deviceId, bool open = true)
        {
            var attemptCounter = 0;
            var response = await Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(retryCount: 5, sleepDurationProvider:
                attempt =>
                {
                    attemptCounter = attempt;
                    return TimeSpan.FromSeconds(Math.Pow(2, attempt));
                },
                onRetry: (ex, time) =>
                {
                    Debug.WriteLine($"An error occured (Attempt: {attemptCounter} - trying again in: {time}...): {ex}");
                })
                .ExecuteAsync(async () =>
                {
                    Debug.WriteLine("Pushing vent control");

                    return await _client.PostAsJsonAsync("thop/ventilation", new VentControlDevice
                    {
                        DeviceId = deviceId,
                        LocationId = locationId,
                        Open = open
                    });
                });

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LightsOnAsync(string locationId, string deviceId, bool on = true)
        {
            var attemptCounter = 0;
            var response = await Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(retryCount: 5, sleepDurationProvider:
                attempt =>
                {
                    attemptCounter = attempt;
                    return TimeSpan.FromSeconds(Math.Pow(2, attempt));
                },
                onRetry: (ex, time) =>
                {
                    Debug.WriteLine($"An error occured (Attempt: {attemptCounter} - trying again in: {time}...): {ex}");
                })
                .ExecuteAsync(async () =>
                {
                    Debug.WriteLine("Pushing light control");

                    return await _client.PostAsJsonAsync("thop/light", new LightControlDevice
                    {
                        DeviceId = deviceId,
                        LocationId = locationId,
                        On = on
                    });
                });

            return response.IsSuccessStatusCode;
        }
    }
}
