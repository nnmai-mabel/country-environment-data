﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assig1.Models
{
    public partial class MonitorStationType
    {
        public MonitorStationType()
        {
            AirQualityStations = new HashSet<AirQualityStation>();
        }

        public int StationTypeId { get; set; }

        [Display(Name = "Station Type")]
        public string StationType { get; set; } = null!;
        public string? ImageUrl { get; set; }

        public virtual ICollection<AirQualityStation> AirQualityStations { get; set; }
    }
}
