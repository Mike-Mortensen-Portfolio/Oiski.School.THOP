using Oiski.School.THOP.Services;
using Oiski.School.THOP.Services.Models;
using System.Net.Http.Json;

namespace Oiski.School.THOP.App.Services
{
    /// <summary>
    /// Represents a service that exposes the <strong>peripheral</strong> API endpoints in the <strong>THOP</strong> Ecosystem
    /// </summary>
    public class PeripheralMauiService : PeripheralService
    {
        /// <summary>
        /// Instantiates a new instance of type <see cref="PeripheralMauiService"/> with an <see cref="HttpClient"/> wrapper
        /// </summary>
        /// <param name="service"></param>
        public PeripheralMauiService(HTTPService service) : base(service.Client) { /*Empty*/ }
    }
}