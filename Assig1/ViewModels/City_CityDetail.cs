using System;
using Assig1.Models;

namespace Assig1.ViewModels
{
	public class City_CityDetail
	{
        public City? TheCity { get; set; }
        public Country? TheCountry { get; set; }
        public Region? TheRegion { get; set; }
        //public AirQualityData TheAirQualityData { get; set; }
        public int? AirMinYear { get; set; }
        public int? AirMaxYear { get; set; }
        public int? AirRecordCount { get; set; }
        //public City_CityDetail()
        //{
        //}
    }
}

