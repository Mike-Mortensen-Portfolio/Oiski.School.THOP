namespace Oiski.School.THOP.Services.Models
{
    /// <summary>
    /// Defines the query parameters for a humidex lookup against the <strong>THOP</strong> Api
    /// </summary>
    public class HumidexOptions
    {
        public string? Sensor { get; set; }
        public string? LocationId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? MaxCount { get; set; }
    }
}
