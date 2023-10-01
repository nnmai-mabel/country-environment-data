using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Assig1.Models;
namespace Assig1.ViewModels
{
	public class City_CityDetail
	{
        [Display(Name = "City")]
        public City? TheCity { get; set; }

        [Display(Name = "Country")]
        public Country? TheCountry { get; set; }

        [Display(Name = "Region")]
        public Region? TheRegion { get; set; }

        [Display(Name = "Earliest Year of Record")]
        public int? AirMinYear { get; set; }

        [Display(Name = "Latest Year of Record")]
        public int? AirMaxYear { get; set; }

        [Display(Name = "Number of Records")]
        public int? AirRecordCount { get; set; }
    }
}

