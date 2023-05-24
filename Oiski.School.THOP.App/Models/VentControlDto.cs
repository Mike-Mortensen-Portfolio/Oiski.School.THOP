using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Oiski.School.THOP.App.Models
{
    public class VentControlDto : DeviceDetails
    {
        [JsonPropertyName("On")]
        public bool Open { get; set; }
    }
}
