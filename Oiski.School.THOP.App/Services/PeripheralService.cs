using Oiski.School.THOP.App.Models;
using Oiski.School.THOP.Services.Models;
using System.Net.Http.Json;

namespace Oiski.School.THOP.App.Services
{
    public class PeripheralService
    {
        private readonly HTTPService _service;

        public PeripheralService(HTTPService service)
        {
            _service = service;
        }

        public async Task<bool> OpenVentsAsync(string locationId, string deviceId, bool open = true)
        {
            HttpResponseMessage response = await _service.Client.PostAsJsonAsync("thop/ventilation", new VentControlDevice
            {
                DeviceId = deviceId,
                LocationId = locationId,
                Open = open
            });

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LightsOnAsync(string locationId, string deviceId, bool on = true)
        {
            HttpResponseMessage response = await _service.Client.PostAsJsonAsync("thop/light", new LightControlDevice
            {
                DeviceId = deviceId,
                LocationId = locationId,
                On = on
            });

            return response.IsSuccessStatusCode;
        }
    }
}
