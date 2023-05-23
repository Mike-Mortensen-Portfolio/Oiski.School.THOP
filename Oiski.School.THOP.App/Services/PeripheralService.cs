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
            HttpResponseMessage response = await _service.Client.PostAsJsonAsync<VentControlDto>("thop/ventilation", new VentControlDto
            {
                DeviceId = deviceId,
                LocationId = locationId,
                Open = open
            });

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LightsOnAsync(string locationId, string deviceId, bool on = true)
        {
            //  TODO: Add Light Switch
            throw new NotImplementedException();

            //HttpResponseMessage response = new HttpResponseMessage();

            //try
            //{
            //    response = await _service.Client.PostAsJsonAsync<VentControlDto>("thop/ventilation", new VentControlDto
            //    {
            //        DeviceId = deviceId,
            //        LocationId = locationId,
            //        Open = open
            //    });
            //}
            //catch (Exception)
            //{
            //    //  TODO: Handle Exception
            //}


            //return response.IsSuccessStatusCode;
        }
    }
}
