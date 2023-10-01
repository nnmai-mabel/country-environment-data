﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/*
 * added comment
 */
namespace Assig1.Models
{
    public partial class Item
    {
        public Item()
        {
            InverseParent = new HashSet<Item>();
            ItemElements = new HashSet<ItemElement>();
        }

        public int ItemId { get; set; }

        [Display(Name = "Item Name")]
        public string ItemName { get; set; } = null!;

        public int? ParentId { get; set; }
        public string? ImageUrl { get; set; }

        public virtual Item? Parent { get; set; }
        public virtual ICollection<Item> InverseParent { get; set; }
        public virtual ICollection<ItemElement> ItemElements { get; set; }
    }
}
