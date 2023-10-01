using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assig1.Data;
using Assig1.ViewModels;

namespace Assig1.Controllers
{
    public class CitiesController : Controller
    {
        private readonly EnvDataContext _context;

        public CitiesController(EnvDataContext context)
        {
            _context = context;
        }

        // GET: Cities
        public async Task<IActionResult> Index(CitiesViewModel vm)
        {
            #region CityCountryRegionQuery
            // Join Cities table and Countries table
            var cityCountryQuery = _context.Cities
                .GroupJoin(_context.Countries,
                city => city.CountryId,
                country => country.CountryId,
                (city, countryGroup) => new
                {
                    theCity = city,
                    theCountries = countryGroup
                })
                .SelectMany(
                city => city.theCountries.DefaultIfEmpty(),
                (city, country) => new
                {
                    theCity = city.theCity,
                    theCountry = country
                })
                .Where(m => m.theCity.CountryId == vm.CountryId)
                .OrderBy(city => city.theCity.CityName)
                .GroupJoin(_context.Regions, // Join with the Regions table
                    cityCountry => cityCountry.theCountry.RegionId,
                    region => region.RegionId,
                    (cityCountry, regionGroup) => new
                    {
                        TheCity = cityCountry.theCity,
                        TheCountry = cityCountry.theCountry,
                        TheRegions = regionGroup
                    })
                .SelectMany(
                    cityCountryRegion => cityCountryRegion.TheRegions.DefaultIfEmpty(),
                    (cityCountryRegion, region) => new City_CityDetail
                    {
                        TheCity = cityCountryRegion.TheCity,
                        TheCountry = cityCountryRegion.TheCountry,
                        TheRegion = region,
                    });
            #endregion

            #region AirQualityData
            var airQualityDataQuery = _context.AirQualityData
                .Select(a => a);
            #endregion

            // Show city suitable with search text
            if (!string.IsNullOrWhiteSpace(vm.SearchText))
            {
                cityCountryQuery = cityCountryQuery
                    .Where(i => i.TheCity.CityName.StartsWith(vm.SearchText));
            }

            // Get the aggregations
            var cities = await cityCountryQuery
                .Select(cityAir => new City_CityDetail
                {
                    TheCity = cityAir.TheCity,
                    TheRegion = cityAir.TheRegion,
                    TheCountry = cityAir.TheCountry,
                    AirMinYear = cityAir.TheCity.AirQualityData.Select(a => a.Year).Min(),
                    AirMaxYear = cityAir.TheCity.AirQualityData.Select(a => a.Year).Max(),
                    AirRecordCount = (cityAir.TheCity.AirQualityData != null ? cityAir.TheCity.AirQualityData.Select(a => a.Year).Count() : 0)
                })
                .ToListAsync();

            // Get the first city in order to show the country name
            var cityDetail = await cityCountryQuery
                .FirstOrDefaultAsync();

            if (cities == null)
            {
                return NotFound();
            }

            vm.CityDetailList = cities;
            vm.TheCityDetail = cityDetail;

            var country = _context.Countries
                .Where(c => c.CountryId == vm.CountryId)
                .FirstOrDefault();
            vm.TheCountry = country;

            return View(vm);
        }

        // GET: /Cities?{regionId}?&countryId=xx&pageSource=yy
        public async Task<IActionResult> Details(CitiesViewModel vm)
        {
            #region CityCountryRegionQuery
            // Join Cities, Countries, and Regions table
            var cityCountryQuery = _context.Cities
                .GroupJoin(_context.Countries,
                city => city.CountryId,
                country => country.CountryId,
                (city, countryGroup) => new
                {
                    theCity = city,
                    theCountries = countryGroup
                })
                .SelectMany(
                city => city.theCountries.DefaultIfEmpty(),
                (city, country) => new
                {
                    theCity = city.theCity,
                    theCountry = country
                })
                .Where(m => m.theCity.CityId == vm.CityId) // find the cityId
                .OrderBy(city => city.theCity.CityName)
                .GroupJoin(_context.Regions, // Join with the Regions table
                    cityCountry => cityCountry.theCountry.RegionId,
                    region => region.RegionId,
                    (cityCountry, regionGroup) => new
                    {
                        TheCity = cityCountry.theCity,
                        TheCountry = cityCountry.theCountry,
                        TheRegions = regionGroup
                    })
                .SelectMany(
                    cityCountryRegion => cityCountryRegion.TheRegions.DefaultIfEmpty(),
                    (cityCountryRegion, region) => new City_CityDetail
                    {
                        TheCity = cityCountryRegion.TheCity,
                        TheCountry = cityCountryRegion.TheCountry,
                        TheRegion = region
                    })
                ;
            #endregion

            #region StationSelectList
            // Join Air Quality Data table with Air Quality Stations and Monitor Station Types
            var AirQualitySummary = _context.AirQualityData
                .GroupJoin(_context.AirQualityStations,
                aqd => aqd.AqdId,
                aqs => aqs.AqdId,
                (aqd, aqsGroup) => new
                {
                    theAirQualityData = aqd,
                    theAirQualityStationsGroup = aqsGroup
                })
                .SelectMany(
                aqd => aqd.theAirQualityStationsGroup.DefaultIfEmpty(),
                (aqd, aqs) => new
                {
                    theAirQualityData = aqd.theAirQualityData,
                    theAirQualityStations = aqs
                })
                .GroupJoin(_context.MonitorStationTypes,
                aqds => aqds.theAirQualityStations.StationTypeId,
                mst => mst.StationTypeId,
                (aqds, mstGroup) => new
                {
                    theAirQualityDataStations = aqds,
                    theMonitorStationTypesGroup = mstGroup
                })
                .SelectMany(
                aqds => aqds.theMonitorStationTypesGroup.DefaultIfEmpty(),
                (aqds, mst) => new
                {
                    theAirQualityDataStations = aqds.theAirQualityDataStations,
                    theMonitorStationTypes = mst
                })
                .Where(aqds => aqds.theAirQualityDataStations.theAirQualityData.CityId == vm.CityId)
                .GroupBy(group => new
                {
                    year = group.theAirQualityDataStations.theAirQualityData.Year,
                    annualMean = group.theAirQualityDataStations.theAirQualityData.AnnualMean,
                    stationType = group.theMonitorStationTypes.StationType

                })

                .Select(group => new
                {
                    year = group.Key.year,
                    annualMean = group.Key.annualMean,
                    stationType = group.Key.stationType
                });

            vm.YearList = new SelectList(AirQualitySummary
                .Select(item => item.year)
                .Distinct()
                .OrderByDescending(item => item));

            // Only keep the value of station type, not the display text
            vm.StationTypeList = new SelectList(AirQualitySummary
                .Select(item => item.stationType)
                .Distinct()
                .OrderBy(item => item));
            #endregion

            var cities = await cityCountryQuery
                .Select(cityAir => new City_CityDetail
                {
                    TheCity = cityAir.TheCity,
                    TheRegion = cityAir.TheRegion,
                    TheCountry = cityAir.TheCountry,
                    AirMinYear = cityAir.TheCity.AirQualityData.Select(a => a.Year).Min(),
                    AirMaxYear = cityAir.TheCity.AirQualityData.Select(a => a.Year).Max(),
                    AirRecordCount = (cityAir.TheCity.AirQualityData != null ? cityAir.TheCity.AirQualityData.Select(a => a.Year).Count() : 0)
                })
                .ToListAsync();

            // Get the first city in order to show the country name
            var cityDetail = await cityCountryQuery
                .FirstOrDefaultAsync();

            if (cities == null)
            {
                return NotFound();
            }
            vm.CityDetailList = cities;
            vm.TheCityDetail = cityDetail;

            return View(vm);
        }

        // /Cities/AirQualityReportData?cityId=xx
        [Produces("application/json")]
        public IActionResult AirQualityReportData(CitiesViewModel vm)
        {
            if (vm.CityId > 0)
            {
                #region AirQualitySummary
                // Join Air Quality Data with Air Quality Stations and Monitor Station Types
                var AirQualitySummary = _context.AirQualityData
                    .GroupJoin(_context.AirQualityStations,
                    aqd => aqd.AqdId,
                    aqs => aqs.AqdId,
                    (aqd, aqsGroup) => new
                    {
                        theAirQualityData = aqd,
                        theAirQualityStationsGroup = aqsGroup
                    })
                    .SelectMany(
                    aqd => aqd.theAirQualityStationsGroup.DefaultIfEmpty(),
                    (aqd, aqs) => new
                    {
                        theAirQualityData = aqd.theAirQualityData,
                        theAirQualityStations = aqs
                    })
                    .GroupJoin(_context.MonitorStationTypes,
                    aqds => aqds.theAirQualityStations.StationTypeId,
                    mst => mst.StationTypeId,
                    (aqds, mstGroup) => new
                    {
                        theAirQualityDataStations = aqds,
                        theMonitorStationTypesGroup = mstGroup
                    })
                    .SelectMany(
                    aqds => aqds.theMonitorStationTypesGroup.DefaultIfEmpty(),
                    (aqds, mst) => new
                    {
                        theAirQualityDataStations = aqds.theAirQualityDataStations,
                        theMonitorStationTypes = mst
                    })
                    .Where(aqds => aqds.theAirQualityDataStations.theAirQualityData.CityId == vm.CityId)
                    .GroupBy(group => new
                    {
                        cityId = group.theAirQualityDataStations.theAirQualityData.CityId,
                        year = group.theAirQualityDataStations.theAirQualityData.Year,
                        annualMean = group.theAirQualityDataStations.theAirQualityData.AnnualMean,
                        annualMeanPm10 = group.theAirQualityDataStations.theAirQualityData.AnnualMeanPm10,
                        annualMeanPm25 = group.theAirQualityDataStations.theAirQualityData.AnnualMeanPm25,
                        annualMeanUgm3 = group.theAirQualityDataStations.theAirQualityData.AnnualMeanUgm3,
                        temporalCoverage1 = group.theAirQualityDataStations.theAirQualityData.TemporalCoverage1,
                        temporalCoverage2 = group.theAirQualityDataStations.theAirQualityData.TemporalCoverage2,
                        reference = group.theAirQualityDataStations.theAirQualityData.Reference,
                        dbYear = group.theAirQualityDataStations.theAirQualityData.Year,
                        status = group.theAirQualityDataStations.theAirQualityData.Status,
                        stationType = group.theMonitorStationTypes.StationType,
                        number = group.theAirQualityDataStations.theAirQualityStations.Number,

                    })

                    .Select(group => new
                    {
                        cityId = group.Key.cityId,
                        year = group.Key.year,
                        annualMean = group.Key.annualMean,
                        annualMeanPm10 = group.Key.annualMeanPm10,
                        annualMeanPm25 = group.Key.annualMeanPm25,
                        annualMeanUgm3 = group.Key.annualMeanUgm3,
                        temporalCoverage1 = group.Key.temporalCoverage1,
                        temporalCoverage2 = group.Key.temporalCoverage2,
                        reference = group.Key.reference,
                        dbYear = group.Key.year,
                        status = group.Key.status,
                        stationType = group.Key.stationType,
                        number = group.Key.number
                    });

                // Get data suitable for year or station type
                if (vm.Year > 0)
                {
                    AirQualitySummary = AirQualitySummary
                        .Where(aqs => aqs.year == vm.Year);
                }

                if (!string.IsNullOrWhiteSpace(vm.StationType))
                {
                    AirQualitySummary = AirQualitySummary
                        .Where(aqs => aqs.stationType == vm.StationType);
                }
                #endregion

                return Json(AirQualitySummary);
            }
            else
            {
                return BadRequest();
            }
        }
        // /Cities/AirSummaryData?countryId=xx&cityId=yy
        //Action for fetching average, min, max value data
        [Produces("application/json")]
        public IActionResult AirSummaryData(CitiesViewModel vm)
        {
            // Get air summary aggregations value
            if (vm.CityId > 0)
            {
                var airSummary = _context.AirQualityData
                    .Where(aqd => aqd.CityId == vm.CityId)
                    //.Where(ce => ce.Year == vm.Year)
                    .GroupBy(aqd => aqd.CityId)
                    .Select(value => new
                    {
                        //totalValue = value.Sum(aqd => aqd.AnnualMean),
                        minValuePm10 = value.Min(aqd => aqd.AnnualMean),
                        maxValuePm10 = value.Max(aqd => aqd.AnnualMean),
                        averageValuePm10 = value.Average(aqd => aqd.AnnualMean),

                        minValuePm25 = value.Min(aqd => aqd.AnnualMeanUgm3),
                        maxValuePm25 = value.Max(aqd => aqd.AnnualMeanUgm3),
                        averageValuePm25 = value.Average(aqd => aqd.AnnualMeanUgm3)
                    });
                return Json(airSummary);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
