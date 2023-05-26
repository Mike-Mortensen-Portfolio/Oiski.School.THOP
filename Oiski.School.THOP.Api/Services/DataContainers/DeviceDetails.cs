using System.ComponentModel.DataAnnotations;

namespace Oiski.School.THOP.Api.Services.DataContainers
{
    /// <summary>
    /// Represents a query <see langword="object"/> that defines a set of identifiers for an <strong>MQTT</strong> device
    /// </summary>
    public class DeviceDetails
    {
        [Required] public string LocationId { get; init; } = null!;
        [Required] public string DeviceId { get; init; } = null!;
    }
}
