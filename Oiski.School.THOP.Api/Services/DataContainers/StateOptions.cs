using System.ComponentModel.DataAnnotations;

namespace Oiski.School.THOP.Api.Services.DataContainers
{
    public class StateOptions : DeviceDetails
    {
        [Required] public bool On { get; set; } = true;
    }
}
