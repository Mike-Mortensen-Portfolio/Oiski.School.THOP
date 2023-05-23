namespace Oiski.School.THOP.App.Models
{
    public class HumidexOptions
    {
        public string Sensor { get; set; }
        public string LocationId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? MaxCount { get; set; }
    }
}
