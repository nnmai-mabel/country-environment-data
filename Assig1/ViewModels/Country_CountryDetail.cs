using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Assig1.Models;

namespace Assig1.ViewModels
{
	public class Country_CountryDetail
	{
        [Display(Name = "Country")]
        public Country? TheCountry { get; set; }

        [Display(Name = "Region")]
        public Region? TheRegion { get; set; }

        public int CityCount { get; set; }
    }
}

