using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Assig1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Assig1.ViewModels
{
	public class CitiesViewModel
	{
        [Display(Name = "City Search")]
        [StringLength(100, ErrorMessage = "The {0} must be less than {1} characters")]
        public string? SearchText { get; set; }

        [Display(Name = "Country ID")]
        public int? CountryId { get; set; }

        [Display(Name = "Region ID")]
        public int? RegionId { get; set; }

        [Display(Name = "Country")]
        public Country TheCountry { get; set; }

        [Display(Name = "City ID")]
        public int? CityId { get; set; }

        [Display(Name = "City Detail List")]
        public List<City_CityDetail>? CityDetailList { get; set; }

        [Display(Name = "City Detail")]
        public City_CityDetail? TheCityDetail { get; set; }

        [Display(Name = "Year List")]
        public SelectList? YearList { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Station Type List")]
        public SelectList? StationTypeList { get; set; }

        [Display(Name = "Station Type")]
        public string? StationType { get; set; }

        [Display(Name = "Chart Legend")]
        public string? ChartLegend { get; set; }

        [Display(Name = "Page Source")]
        public string? PageSource { get; set; }
    }
}

