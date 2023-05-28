using Oiski.School.THOP.Services.Models;
using System.Net.Http.Json;
using Oiski.School.THOP.Services;

namespace Oiski.School.THOP.App.Services
{
    /// <summary>
    /// Represents a service that exposes the <strong>Humidex</strong> API endpoints in the <strong>THOP</strong> Ecosystem
    /// </summary>
    public class HumidexMauiService : HumidexService
    {
        /// <summary>
        /// Instantiates a new instance of type <see cref="HumidexMauiService"/> with an <see cref="HttpClient"/> wrapper
        /// </summary>
        /// <param name="service"></param>
        public HumidexMauiService(HTTPService service) : base(service.Client)
        {
            DeviceDetails = new DeviceDetails
            {
                DeviceId = "oiski_1010",
                LocationId = "home"
            };
        }
    }
}