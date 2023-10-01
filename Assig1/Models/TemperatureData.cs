using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assig1.Models
{
    public partial class TemperatureData
    {
        public int ObjectId { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        public int CountryId { get; set; }

        [Display(Name = "Unit")]
        public string? Unit { get; set; }

        [Display(Name = "Change")]
        public string? Change { get; set; }

        [Display(Name = "Value")]
        public decimal? Value { get; set; }

        public virtual Country Country { get; set; } = null!;
    }
}
