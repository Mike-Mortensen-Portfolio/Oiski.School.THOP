using CsvHelper.Configuration.Attributes;
using Oiski.School.THOP.Services.Models;
using System.ComponentModel.DataAnnotations;

namespace Oiski.School.THOP.Api.Services.DataContainers
{
    /// <summary>
    /// Defines a filter for querying <see cref="HumidexDto"/> data through the API
    /// </summary>
    public class HumidexFilter
    {
        public string? Sensor { get; set; }
        public string? LocationId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? StartTime { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? EndTime { get; set; }
        public int? MaxCount { get; set; }
    }
}
