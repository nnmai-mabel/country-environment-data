using System;
using Assig1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Assig1.ViewModels
{
	public class CitiesViewModel
	{
        public string? SearchText { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public Country TheCountry { get; set; }
        public int? CityId { get; set; }
        public List<City_CityDetail>? CityDetailList { get; set; }
        public City_CityDetail? TheCityDetail { get; set; }
        public SelectList? YearList { get; set; }
        public int Year { get; set; }
        public SelectList? StationTypeList { get; set; }
        public string? StationType { get; set; }
        public string? ChartLegend { get; set; }
    }
}

