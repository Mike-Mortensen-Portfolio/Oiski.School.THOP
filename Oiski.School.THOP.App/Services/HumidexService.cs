using Oiski.School.THOP.App.Models;
using System.Net.Http.Json;

namespace Oiski.School.THOP.App.Services
{
    public class HumidexService
    {
        private readonly HTTPService _service;

        public HumidexService(HTTPService service)
        {
            _service = service;
        }

        public async Task<List<HumidexDto>> GetAllAsync(HumidexOptions options = null)
        {
            if (options == null)
                options = new HumidexOptions
                {
                    Sensor = "",
                    EndTime = null,
                    StartTime = null,
                    LocationId = ""
                };

            var startTime = ((options.StartTime != null) ? ($"&StartTime={options.StartTime.Value.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fffZ}") : (null));
            var endTime = ((options.EndTime != null) ? ($"&EndTime={options.EndTime.Value.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fffZ}") : (null));
            var query = $"Sensor={options.Sensor}&LocationId={options.LocationId}{startTime ?? string.Empty}{endTime ?? string.Empty}";

            List<HumidexDto> readings = new List<HumidexDto>();

            try
            {
                readings = await _service.Client.GetFromJsonAsync<List<HumidexDto>>($"thop/humidex?{query}");
            }
            catch (Exception)
            {
                //  TODO: Handle Error
            }

            return readings;
        }
    }
}
