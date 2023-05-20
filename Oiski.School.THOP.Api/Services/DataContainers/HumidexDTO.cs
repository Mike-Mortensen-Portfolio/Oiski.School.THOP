namespace Oiski.School.THOP.Api.Services.DataContainers
{
    public class HumidexDTO
    {
        public string LocationId { get; set; } = null!;
        public string Sensor { get; set; } = null!;
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DateTime? Time { get; set; }
    }
}
