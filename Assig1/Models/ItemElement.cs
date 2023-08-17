using System;
using System.Collections.Generic;

namespace Assig1.Models
{
    public partial class ItemElement
    {
        public ItemElement()
        {
            CountryEmissions = new HashSet<CountryEmission>();
        }

        public int ItemId { get; set; }
        public int ElementId { get; set; }

        public virtual Element Element { get; set; } = null!;
        public virtual Item Item { get; set; } = null!;
        public virtual ICollection<CountryEmission> CountryEmissions { get; set; }
    }
}
