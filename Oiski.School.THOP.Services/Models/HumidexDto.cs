namespace Oiski.School.THOP.Services.Models
{
    public class HumidexDto
    {
        public string LocationId { get; set; } = null!;
        public string Sensor { get; set; } = null!;
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DateTime? Time { get; set; }
    }
}
