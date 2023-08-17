using System;
using System.Collections.Generic;

namespace Assig1.Models
{
    public partial class Country
    {
        public Country()
        {
            Cities = new HashSet<City>();
            CountryEmissions = new HashSet<CountryEmission>();
            TemperatureData = new HashSet<TemperatureData>();
        }

        public int CountryId { get; set; }
        public string? Iso3 { get; set; }
        public string CountryName { get; set; } = null!;
        public int? RegionId { get; set; }
        public string? ImageUrl { get; set; }

        public virtual Region? Region { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<CountryEmission> CountryEmissions { get; set; }
        public virtual ICollection<TemperatureData> TemperatureData { get; set; }
    }
}
