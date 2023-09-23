﻿using System;
using Assig1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Assig1.ViewModels
{
	public class CitiesViewModel
	{
        public string SearchText { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public int? CityId { get; set; }
        public List<City_CityDetail> CityDetailList { get; set; }
        public City_CityDetail TheCityDetail { get; set; }
        public SelectList StationList { get; set; }
        public string Station { get; set; }
        //public List<City_CityDetail> CountryOfCity { get; set; }
        //public Region? TheRegion { get; set; }
        //public Country TheCountry { get; set; }
        //public CitiesViewModel()
        //{
        //}
    }
}

