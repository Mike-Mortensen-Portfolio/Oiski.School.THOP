using Oiski.School.THOP.Services;
using Oiski.School.THOP.Services.Models;
using System.Net.Http.Json;

namespace Oiski.School.THOP.App.Services
{
    /// <summary>
    /// Represents a service that exposes the <strong>Humidex</strong> API endpoints in the <strong>THOP</strong> Ecosystem
    /// </summary>
    public class HumidexService
    {
        private readonly HTTPService _service;

        /// <summary>
        /// The details that defines 
        /// </summary>
        public DeviceDetails DeviceDetails { get; set; }

        /// <summary>
        /// Instantiates a new instance of type <see cref="HumidexService"/> with an <see cref="HttpClient"/> wrapper
        /// </summary>
        /// <param name="service"></param>
        public HumidexService(HTTPService service)
        {
            _service = service;
            DeviceDetails = new DeviceDetails
            {
                DeviceId = "oiski_1010",
                LocationId = "home"
            };
        }

        public async Task<List<HumidexDto>> GetAllAsync(HumidexOptions options = null)
        {
            if (options == null)
                options = new HumidexOptions
                {
                    Sensor = "",
                    EndTime = null,
                    StartTime = null,
                    LocationId = "",
                    MaxCount = null
                };

            var startTime = ((options.StartTime != null) ? ($"&StartTime={options.StartTime.Value.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fffZ}") : (null));
            var endTime = ((options.EndTime != null) ? ($"&EndTime={options.EndTime.Value.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fffZ}") : (null));
            var maxCount = ((options.MaxCount != null) ? ($"&MaxCount={options.MaxCount.Value}") : (null));
            var query = $"Sensor={options.Sensor}&LocationId={options.LocationId}{startTime ?? string.Empty}{endTime ?? string.Empty}{maxCount ?? string.Empty}";

            List<HumidexDto> readings = await _service.Client.GetFromJsonAsync<List<HumidexDto>>($"thop/humidex?{query}");

            return readings;
        }
    }
}
