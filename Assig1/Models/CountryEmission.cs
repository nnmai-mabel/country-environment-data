using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assig1.Models
{
    public partial class CountryEmission
    {
        public int CeId { get; set; }

        [Display(Name = "Year")]
        public int? Year { get; set; }

        public int? CountryId { get; set; }

        [Display(Name = "Value")]
        public decimal? Value { get; set; }

        public int? ItemId { get; set; }
        public int? ElementId { get; set; }

        public virtual Country? Country { get; set; }
        public virtual ItemElement? ItemElement { get; set; }
    }
}
