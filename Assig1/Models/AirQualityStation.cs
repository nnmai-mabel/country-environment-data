using System;
using System.Collections.Generic;

namespace Assig1.Models
{
    public partial class AirQualityStation
    {
        public int StationTypeId { get; set; }
        public int AqdId { get; set; }
        public int? Number { get; set; }

        public virtual AirQualityData Aqd { get; set; } = null!;
        public virtual MonitorStationType StationType { get; set; } = null!;
    }
}
