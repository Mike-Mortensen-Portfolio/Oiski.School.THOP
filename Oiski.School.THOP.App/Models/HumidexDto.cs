namespace Oiski.School.THOP.App.Models
{
    public class HumidexDto
    {
        public string LocationId { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DateTime? Time { get; set; }
    }
}
