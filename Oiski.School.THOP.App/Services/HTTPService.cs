namespace Oiski.School.THOP.App.Services
{
    public class HTTPService
    {
        public HTTPService()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(AppConstants.TUNNEL_URL);
        }

        public HttpClient Client { get; }
    }
}
