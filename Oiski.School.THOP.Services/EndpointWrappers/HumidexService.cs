using Oiski.School.THOP.Services.Models;
using System.Globalization;
using System.Net.Http.Json;
using Polly;
using System.Diagnostics;

namespace Oiski.School.THOP.Services
{
    /// <summary>
    /// Represents a service that exposes the <strong>Humidex</strong> API endpoints in the <strong>THOP</strong> Ecosystem
    /// </summary>
    public class HumidexService
    {
        private readonly HttpClient _client;

        /// <summary>
        /// The details that defines the current device
        /// </summary>
        public DeviceDetails DeviceDetails { get; set; }

        /// <summary>
        /// Instantiates a new instance of type <see cref="HumidexService"/> with an <see cref="HttpClient"/> wrapper
        /// </summary>
        /// <param name="client"></param>
        public HumidexService(HttpClient client)
        {
            _client = client;
            DeviceDetails = new DeviceDetails
            {
                DeviceId = "oiski_1010",
                LocationId = "home"
            };
        }

        public async Task<List<HumidexDto>> GetAllAsync(HumidexOptions options = null!)
        {
            options ??= new HumidexOptions
            {
                Sensor = "",
                EndTime = null,
                StartTime = null,
                LocationId = "",
                MaxCount = null
            };

            var startTime = ((options.StartTime != null) ? ($"&StartTime={options.StartTime.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}") : (null));
            var endTime = ((options.EndTime != null) ? ($"&EndTime={options.EndTime.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}") : (null));
            var maxCount = ((options.MaxCount != null) ? ($"&MaxCount={options.MaxCount.Value}") : (null));
            var query = $"Sensor={options.Sensor}&LocationId={options.LocationId}{startTime ?? string.Empty}{endTime ?? string.Empty}{maxCount ?? string.Empty}";

            var attemptCounter = 0;
            List<HumidexDto> readings = new List<HumidexDto>();
            try
            {
                readings = await Policy
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
                       Debug.WriteLine("Fetching readings");

                       return await _client?.GetFromJsonAsync<List<HumidexDto>>($"thop/humidex?{query}")! ?? new List<HumidexDto>();
                   });
            }
            catch (Exception)
            {
                Debug.WriteLine("Couldn't connect to server while fetching readings");
            }

            return readings;
        }
    }
}
