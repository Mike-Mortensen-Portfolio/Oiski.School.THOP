using Oiski.School.THOP.Services.Models;

namespace Oiski.School.THOP.Web.Services
{
    public class GlobalState
    {
        public DeviceDetails Device { get; set; } = new DeviceDetails()
        {
            DeviceId = "oiski_1010",
            LocationId = "home"
        };
    }
}
