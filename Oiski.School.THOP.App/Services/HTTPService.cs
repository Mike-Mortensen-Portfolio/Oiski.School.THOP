using Oiski.School.THOP.Services;

namespace Oiski.School.THOP.App.Services
{
    /// <summary>
    /// A wrapper that exposes an <see cref="HttpClient"/>. Most likely only useful in the context of a <strong>.NET MAUI</strong> application
    /// <br/>
    /// <br/>
    /// <strong>Note:</strong> This should be registered as a singleton to ensure no more than on instance at any given time
    /// </summary>
    public class HTTPService
    {
        /// <summary>
        /// Instantiates a new instance of type <see cref="HTTPService"/>
        /// </summary>
        public HTTPService()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(AppConstants.TUNNEL_URL);
        }

        public HttpClient Client { get; }
    }
}
