using Oiski.School.THOP.Services;

namespace Oiski.School.THOP.Web.Services
{
    public class PeripheralServiceScope : PeripheralService
    {
        public PeripheralServiceScope(IHttpClientFactory factory) : base(factory.CreateClient("THOP_Api")) { /*Empty*/ }
    }
}
