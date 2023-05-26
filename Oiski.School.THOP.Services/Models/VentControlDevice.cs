using System.Text.Json.Serialization;

namespace Oiski.School.THOP.Services.Models
{
    /// <summary>
    /// Defines a set of controls for <strong>THOP</strong> vents on a physical sensor board
    /// </summary>
    public class VentControlDevice : DeviceDetails
    {
        [JsonPropertyName("On")]
        public bool Open { get; set; }
    }
}
