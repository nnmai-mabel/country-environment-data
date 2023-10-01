using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assig1.Models
{
    public partial class Element
    {
        public Element()
        {
            ItemElements = new HashSet<ItemElement>();
        }

        public int ElementId { get; set; }

        [Display(Name = "Element Name")]
        public string ElementName { get; set; } = null!;

        [Display(Name = "Unit")]
        public string Unit { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public virtual ICollection<ItemElement> ItemElements { get; set; }
    }
}
