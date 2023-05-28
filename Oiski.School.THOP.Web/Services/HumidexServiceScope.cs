using Oiski.School.THOP.Services;

namespace Oiski.School.THOP.Web.Services
{
    public class HumidexServiceScope : HumidexService
    {
        public HumidexServiceScope(IHttpClientFactory factory) : base(factory.CreateClient("THOP_Api")) { /*Empty*/ }
    }
}
