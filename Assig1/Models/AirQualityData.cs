using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/**
 * Comment
 */
namespace Assig1.Models
{
    public partial class AirQualityData
    {
        public AirQualityData()
        {
            AirQualityStations = new HashSet<AirQualityStation>();
        }

        [Key]
        public int AqdId { get; set; }
        public int CityId { get; set; }
        public int Year { get; set; }
        public int? RowId { get; set; }
        public int? AnnualMean { get; set; }
        public string? TemporalCoverage1 { get; set; }
        public string? AnnualMeanPm10 { get; set; }
        public int? AnnualMeanUgm3 { get; set; }
        public string? TemporalCoverage2 { get; set; }
        public string? AnnualMeanPm25 { get; set; }
        public string? Reference { get; set; }
        public int? DbYear { get; set; }
        public string? Status { get; set; }

        public virtual City City { get; set; } = null!;
        public virtual ICollection<AirQualityStation> AirQualityStations { get; set; }
    }
}
