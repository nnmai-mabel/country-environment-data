using System;
using System.Collections.Generic;

namespace Assig1.Models
{
    public partial class TemperatureData
    {
        public int ObjectId { get; set; }
        public int Year { get; set; }
        public int CountryId { get; set; }
        public string? Unit { get; set; }
        public string? Change { get; set; }
        public decimal? Value { get; set; }

        public virtual Country Country { get; set; } = null!;
    }
}
