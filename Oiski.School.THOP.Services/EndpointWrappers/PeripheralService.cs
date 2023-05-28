using Oiski.School.THOP.Services.Models;
using System.Net.Http.Json;

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
            HttpResponseMessage response = await _client.PostAsJsonAsync("thop/ventilation", new VentControlDevice
            {
                DeviceId = deviceId,
                LocationId = locationId,
                Open = open
            });

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LightsOnAsync(string locationId, string deviceId, bool on = true)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("thop/light", new LightControlDevice
            {
                DeviceId = deviceId,
                LocationId = locationId,
                On = on
            });

            return response.IsSuccessStatusCode;
        }
    }
}
