using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assig1.Models
{
    public partial class City
    {
        public City()
        {
            AirQualityData = new HashSet<AirQualityData>();
        }

        public int CityId { get; set; }

        [Display(Name = "City Name")]
        public string CityName { get; set; } = null!;

        public int CountryId { get; set; }

        public virtual Country Country { get; set; } = null!;
        public virtual ICollection<AirQualityData> AirQualityData { get; set; }
    }
}
