using System;
using Assig1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Assig1.ViewModels
{
	public class CountriesViewModel
	{
        public string SearchText { get; set; }
        public int? RegionId { get; set; }
        public int? CountryId { get; set; }
        public SelectList RegionSelectList { get; set; }
        public List<Models.Country> Countries { get; set; }
		public List<Country_CountryDetail> CountryList { get; set; }
        public Country_CountryDetail TheCountryDetail { get; set; }
        //public CountriesViewModel()
        //{
        //}
    }
}

