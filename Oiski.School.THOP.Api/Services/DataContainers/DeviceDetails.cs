using System.ComponentModel.DataAnnotations;

namespace Oiski.School.THOP.Api.Services.DataContainers
{
    public class DeviceDetails
    {
        [Required] public string LocationId { get; init; } = null!;
        [Required] public string DeviceId { get; init; } = null!;
    }
}
