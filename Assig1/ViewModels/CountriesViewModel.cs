using System;
namespace Assig1.ViewModels
{
	public class CountriesViewModel
	{
		public int? RegionId { get; set; }
		public List<Models.Country> Countries { get; set; }
		public Country_CountryDetail CountryList { get; set; }
		//public CountriesViewModel()
		//{
		//}
	}
}

