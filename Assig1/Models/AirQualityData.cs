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

        [Display(Name = "Year")]
        public int Year { get; set; }

        public int? RowId { get; set; }

        [Display(Name = "Annual Mean")]
        public int? AnnualMean { get; set; }

        [Display(Name = "Temporal Coverage 1")]
        public string? TemporalCoverage1 { get; set; }

        [Display(Name = "Annual Mean PM10")]
        public string? AnnualMeanPm10 { get; set; }

        [Display(Name = "Annual Mean Ugm3")]
        public int? AnnualMeanUgm3 { get; set; }

        [Display(Name = "Temporal Coverage 2")]
        public string? TemporalCoverage2 { get; set; }

        [Display(Name = "Annual Mean PM2.5")]
        public string? AnnualMeanPm25 { get; set; }

        [Display(Name = "Reference")]
        public string? Reference { get; set; }

        [Display(Name = "Database Year")]
        public int? DbYear { get; set; }

        [Display(Name = "Status")]
        public string? Status { get; set; }

        public virtual City City { get; set; } = null!;
        public virtual ICollection<AirQualityStation> AirQualityStations { get; set; }
    }
}
