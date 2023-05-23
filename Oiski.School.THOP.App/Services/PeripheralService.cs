using Oiski.School.THOP.App.Models;
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
            if (Connectivity.NetworkAccess != NetworkAccess.Internet) { return false; }

            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                response = await _service.Client.PostAsJsonAsync<VentControlDto>("thop/ventilation", new VentControlDto
                {
                    DeviceId = deviceId,
                    LocationId = locationId,
                    Open = open
                });
            }
            catch (Exception)
            {
                //  TODO: Handle Exception
            }


            return response.IsSuccessStatusCode;
        }
    }
}
