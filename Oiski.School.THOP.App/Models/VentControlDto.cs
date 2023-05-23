using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oiski.School.THOP.App.Models
{
    public class VentControlDto
    {
        public string LocationId { get; init; }
        public string DeviceId { get; init; }
        public bool Open { get; set; }
    }
}
