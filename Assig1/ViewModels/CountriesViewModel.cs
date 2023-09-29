using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Assig1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Assig1.ViewModels
{
	public class CountriesViewModel
	{
        [Display(Name = "City Search")]
        [StringLength(100, ErrorMessage = "The {0} must be less than {1} characters")]
        public string? SearchText { get; set; }

        [Display(Name = "Region ID")]
        public int? RegionId { get; set; }

        [Display(Name = "Country ID")]
        public int? CountryId { get; set; }

        [Display(Name = "Region Select List")]
        public SelectList? RegionSelectList { get; set; }

        [Display(Name = "Countries")]
        public List<Models.Country>? Countries { get; set; }

        [Display(Name = "Country List")]
        public List<Country_CountryDetail>? CountryList { get; set; }

        [Display(Name = "Country Detail")]
        public Country_CountryDetail? TheCountryDetail { get; set; }

        [Display(Name = "Year List")]
        public SelectList? YearList { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Chart Legend")]
        public string? ChartLegend { get; set; }

        [Display(Name = "Page Source")]
        public string? ChartAggregation { get; set; }
    }
}

