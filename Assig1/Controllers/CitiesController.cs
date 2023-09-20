using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assig1.Data;
using Assig1.Models;
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
                //.Select(city => new City_CityDetail
                //{
                //    TheCity = city.theCity,
                //    TheCountry = city.theCountry
                //});
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
                    })
                ;
            //.GroupJoin(_context.AirQualityData, // Join with the air quality data table
            //        cityCountryRegion => cityCountryRegion.TheCity.CityId,
            //        air => air.CityId,
            //        (cityCountryRegion, airGroup) => new
            //        {
            //            TheCity = cityCountryRegion.TheCity,
            //            TheCountry = cityCountryRegion.TheCountry,
            //            TheRegion = cityCountryRegion.TheRegion,
            //            TheAirGroups = airGroup
            //        })
            //.SelectMany(
            //    cityCountryRegionAir => cityCountryRegionAir.TheAirGroups.DefaultIfEmpty(),
            //    (cityCountryRegionAir, air) => new City_CityDetail
            //    {
            //        TheCity = cityCountryRegionAir.TheCity,
            //        TheCountry = cityCountryRegionAir.TheCountry,
            //        TheRegion = cityCountryRegionAir.TheRegion,
            //        TheAirQualityData = air
            //    });
            //.GroupBy(cityCountryRegionAir => cityCountryRegionAir.TheCity)
            //.Select(group => new
            //{
            //    city = group.Key,
            //    airMinYear = group.Min(cityDetail => cityDetail.TheAirQualityData.Year),
            //    airMaxYear = group.Max(cityDetail => cityDetail.TheAirQualityData.Year)
            //});
            #endregion
            #region CitiesListQuery
            //var CitiesList = _context.Cities
            //    .Select(c => c); // Select all cities

            //var envDataContext = CitiesList
            //    .GroupJoin(_context.Countries,
            //    city => city.CountryId,
            //    country => country.CountryId,
            //    (city, countryGroup) => new
            //    {
            //        theCity = city,
            //        theCountries = countryGroup
            //    })
            //    .SelectMany(
            //    city => city.theCountries.DefaultIfEmpty(),
            //    (city, country) => new
            //    {
            //        theCity = city.theCity,
            //        theCountry = country
            //    })
            //    .OrderBy(city => city.theCity.CityName)
            //    .Select(city => new
            //    {
            //        TheCity = city.theCity,
            //        TheCountry = city.theCountry
            //    });

            //if (vm.CountryId != null)
            //{
            //    envDataContext = envDataContext
            //        .Where(c => c.TheCountry.CountryId == vm.CountryId);
            //}
            #endregion

            #region AirQualityData
            var airQualityDataQuery = _context.AirQualityData
                .Select(a => a);
            #endregion

            if (!string.IsNullOrWhiteSpace(vm.SearchText))
            {
                cityCountryQuery = cityCountryQuery
                    .Where(i => i.TheCity.CityName.StartsWith(vm.SearchText));
            }

            var cities = await cityCountryQuery
                .Select(cityAir => new City_CityDetail
                {
                    TheCity = cityAir.TheCity,
                    TheRegion = cityAir.TheRegion,
                    TheCountry = cityAir.TheCountry,
                    //TheAirQualityData = cityAir.TheAirQualityData,
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
            //vm.CityList = await envDataContext
            //    .Select(city => new City_CityDetail
            //    {
            //        TheCity = city.TheCity
            //    })
            //    .ToListAsync();
            return View(vm);
            //var envDataContext = _context.Cities.Include(c => c.Country);
            //return View(await envDataContext.ToListAsync());
        }

        // GET: Cities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cities == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.Country)
                .FirstOrDefaultAsync(m => m.CityId == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // GET: Cities/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryId");
            return View();
        }

        // POST: Cities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CityId,CityName,CountryId")] City city)
        {
            if (ModelState.IsValid)
            {
                _context.Add(city);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryId", city.CountryId);
            return View(city);
        }

        // GET: Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cities == null)
            {
                return NotFound();
            }

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryId", city.CountryId);
            return View(city);
        }

        // POST: Cities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CityId,CityName,CountryId")] City city)
        {
            if (id != city.CityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CityExists(city.CityId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryId", city.CountryId);
            return View(city);
        }

        // GET: Cities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cities == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.Country)
                .FirstOrDefaultAsync(m => m.CityId == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cities == null)
            {
                return Problem("Entity set 'EnvDataContext.Cities'  is null.");
            }
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                _context.Cities.Remove(city);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CityExists(int id)
        {
            return (_context.Cities?.Any(e => e.CityId == id)).GetValueOrDefault();
        }
    }
}
