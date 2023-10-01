using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assig1.Models
{
    public partial class Region
    {
        public Region()
        {
            Countries = new HashSet<Country>();
        }

        public int RegionId { get; set; }

        [Display(Name = "Region Name")]
        public string RegionName { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public virtual ICollection<Country> Countries { get; set; }
    }
}
