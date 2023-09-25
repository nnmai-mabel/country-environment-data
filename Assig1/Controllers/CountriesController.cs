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
using System.Diagnostics.Metrics;
using Humanizer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Cryptography;

namespace Assig1.Controllers
{
    public class CountriesController : Controller
    {
        private readonly EnvDataContext _context;

        public CountriesController(EnvDataContext context)
        {
            _context = context;
        }

        // GET: Countries
        public async Task<IActionResult> Index(CountriesViewModel vm)
        {

            #region CountriesListQuery
            var CountriesList = _context.Countries
                .Select(c => c); // Select all countries

            // Join Regions and Countries table using group join
            var envDataContext = CountriesList
                .GroupJoin(_context.Regions,
                c => c.RegionId,
                r => r.RegionId,
                (c, regionGroup) => new
                {
                    theCountry = c,
                    theRegions = regionGroup
                })
                .SelectMany(
                c => c.theRegions.DefaultIfEmpty(),
                (c, r) => new
                {
                    theCountry = c.theCountry,
                    theRegion = r
                })
                .OrderBy(c => c.theCountry.CountryName)
                .Select(c => new Country_CountryDetail
                {
                    TheCountry = c.theCountry,
                    TheRegion = c.theRegion
                });

            // Implement countries search
            var Regions = _context.Regions
                .OrderBy(r => r.RegionName)
                .Select(r => new
                {
                    RegionId = r.RegionId,
                    RegionName = r.RegionName
                })
                .ToList();

            // Pass result back to view model using select list
            vm.RegionSelectList = new SelectList(Regions,
                nameof(Region.RegionId),
                nameof(Region.RegionName));

            // Return results when user searches country name
            if (!string.IsNullOrWhiteSpace(vm.SearchText))
            {
                envDataContext = envDataContext
                    .Where(i => i.TheCountry.CountryName.Contains(vm.SearchText));
            }

            // Return results when user search regions
            if (vm.RegionId != null)
            {
                envDataContext = envDataContext
                    .Where(c => c.TheRegion.RegionId == vm.RegionId);
            }
            #endregion

            // Pass result back to view model
            vm.CountryList = await envDataContext
                .Select(c => new Country_CountryDetail
                {
                    TheCountry = c.TheCountry,
                    TheRegion = c.TheRegion
                })
                .ToListAsync();

            return View(vm);
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(CountriesViewModel vm)
        {
            #region CountryRegion
            // Join Regions and Countries table using group join
            var countryRegionQuery = _context.Countries
               .GroupJoin(_context.Regions,
               c => c.RegionId,
               r => r.RegionId,
               (c, regionGroup) => new
               {
                   theCountry = c,
                   theRegions = regionGroup
               })
               .SelectMany(
               c => c.theRegions.DefaultIfEmpty(),
               (c, r) => new
               {
                   theCountry = c.theCountry,
                   theRegion = r
               })
               .Where(m => m.theCountry.CountryId == vm.CountryId)
               .Select(c => new Country_CountryDetail
               {
                   TheCountry = c.theCountry,
                   TheRegion = c.theRegion
               });
            #endregion

            #region YearSelectList
            // Return list of years
            var yearList = _context.CountryEmissions
                .Select(ce => ce.Year)
                .Distinct()
                .OrderByDescending(ce => ce)
                .ToList();

            // Pass result back to view model
            vm.YearList = new SelectList(yearList);
            #endregion

            var country = await countryRegionQuery
                .FirstOrDefaultAsync();

            if (country == null)
            {
                return NotFound();
            }

            // Pass result back to view model
            vm.TheCountryDetail = country;

            return View(vm);
        }

        // Action for fetching country emissions data
        [Produces("application/json")]
        public IActionResult CountryEmissionsReportData(CountriesViewModel vm)
        {
            if (vm.Year > 0)
            {
                // Calculate data based on Items
                // Join Country Emissions table with Items and parent Items to show item with its parent item name
                if (vm.ChartLegend == "Items")
                {
                    var itemEmissionsSummary = _context.CountryEmissions
                        .GroupJoin(_context.Items,
                        ce => ce.ItemId,
                        i => i.ItemId,
                        (ce, itemGroup) => new
                        {
                            theCountryEmission = ce,
                            theItems = itemGroup
                        })
                        .SelectMany(
                        ce => ce.theItems.DefaultIfEmpty(),
                        (ce, i) => new
                        {
                            theCountryEmission = ce.theCountryEmission,
                            theItem = i
                        })

                        .GroupJoin(_context.Items,
                        cei => cei.theItem.ParentId,
                        pi => pi.ItemId,
                        (cei, parentItemGroup) => new
                        {
                            theCountryEmissionItem = cei,
                            theParentItem = parentItemGroup
                        })
                        .SelectMany(
                        cei => cei.theParentItem.DefaultIfEmpty(),
                        (cei, pi) => new
                        {
                            theCountryEmissionItem = cei.theCountryEmissionItem,
                            theParentItem = pi
                        })

                        .Where(cei => cei.theCountryEmissionItem.theCountryEmission.CountryId == vm.CountryId)
                        .Where(cei => cei.theCountryEmissionItem.theCountryEmission.Year == vm.Year)
                        .GroupBy(group => new
                        {
                            countryId = group.theCountryEmissionItem.theCountryEmission.CountryId,
                            year = group.theCountryEmissionItem.theCountryEmission.Year,
                            itemId = group.theCountryEmissionItem.theCountryEmission.ItemId,
                            item = group.theParentItem != null
                                ? group.theCountryEmissionItem.theItem.ItemName + " - " + group.theParentItem.ItemName
                                : group.theCountryEmissionItem.theItem.ItemName
                        })
                        .Select(group => new
                        {
                            countryId = group.Key.countryId, // use the name in group by
                            year = group.Key.year,
                            itemId = group.Key.itemId,
                            item = group.Key.item,
                            valueItemAverage = group.Average(ce => ce.theCountryEmissionItem.theCountryEmission.Value),
                            valueItemTotal = group.Sum(ce => ce.theCountryEmissionItem.theCountryEmission.Value)
                        });

                    return Json(itemEmissionsSummary);

                }

                // Calculate based on Elements
                else
                {
                    var elementEmissionsSummary = _context.CountryEmissions
                        .GroupJoin(_context.Elements,
                        ce => ce.ElementId,
                        e => e.ElementId,
                        (ce, elementGroup) => new
                        {
                            theCountryEmission = ce,
                            theElements = elementGroup
                        })
                        .SelectMany(
                        ce => ce.theElements.DefaultIfEmpty(),
                        (ce, e) => new
                        {
                            theCountryEmission = ce.theCountryEmission,
                            theElement = e
                        })
                        .Where(ce => ce.theCountryEmission.CountryId == vm.CountryId)
                        .Where(ce => ce.theCountryEmission.Year == vm.Year)
                        .GroupBy(group => new
                        {
                            countryId = group.theCountryEmission.CountryId,
                            year = group.theCountryEmission.Year,
                            elementId = group.theCountryEmission.ElementId,
                            element = group.theElement.ElementName
                        })
                        .Select(group => new
                        {
                            countryId = group.Key.countryId, // use the name in group by
                            year = group.Key.year,
                            elementId = group.Key.elementId,
                            element = group.Key.element,
                            valueElementAverage = group.Average(ce => ce.theCountryEmission.Value),
                            valueElementTotal = group.Sum(ce => ce.theCountryEmission.Value)
                        });
                    return Json(elementEmissionsSummary);

                }
            }
            else
            {
                return BadRequest();
            }
        }

        //Action for fetching temperature data
        [Produces("application/json")]
        public IActionResult TemperatureReportData(CountriesViewModel vm)
        {
            if (vm.ChartLegend == "Temperature")
            {
                var temperatureSummary = _context.TemperatureData
                    .Where(td => td.CountryId == vm.CountryId)
                    .GroupBy(td => new { td.CountryId, td.Year, td.Value })
                    .Select(group => new
                    {
                        countryId = group.Key.CountryId,
                        year = group.Key.Year,
                        value = group.Key.Value
                    });
                return Json(temperatureSummary);
            }
            else
            {
                return BadRequest();
            }
        }

        //Action for fetching item element data
        [Produces("application/json")]
        public IActionResult ItemElementData(CountriesViewModel vm)
        {
            if (vm.CountryId > 0)
            {
                // Join Country Emissions with Elements and Items table
                var ItemElementSummary = _context.CountryEmissions
                    .GroupJoin(_context.Items,
                    ce => ce.ItemId,
                    i => i.ItemId,
                    (ce, itemGroup) => new
                    {
                        theCountryEmission = ce,
                        theItems = itemGroup
                    })
                    .SelectMany(
                    ce => ce.theItems.DefaultIfEmpty(),
                    (ce, i) => new
                    {
                        theCountryEmission = ce.theCountryEmission,
                        theItem = i
                    })

                    .GroupJoin(_context.Elements,
                    ice => ice.theCountryEmission.ElementId,
                    e => e.ElementId,
                    (ice, elementGroup) => new
                    {
                        theCountryEmissionItem = ice,
                        theElements = elementGroup
                    })
                    .SelectMany(
                    ice => ice.theElements.DefaultIfEmpty(),
                    (ice, e) => new
                    {
                        theCountryEmissionItem = ice.theCountryEmissionItem,
                        theElement = e
                    })

                    .Where(ce => ce.theCountryEmissionItem.theCountryEmission.CountryId == vm.CountryId)
                    .GroupBy(group => new
                    {
                        countryId = group.theCountryEmissionItem.theCountryEmission.CountryId,
                        year = group.theCountryEmissionItem.theCountryEmission.Year,
                        itemId = group.theCountryEmissionItem.theCountryEmission.ItemId,
                        elementId = group.theCountryEmissionItem.theCountryEmission.ElementId,
                        item = group.theCountryEmissionItem.theItem.ItemName,
                        element = group.theElement.ElementName,
                        value = group.theCountryEmissionItem.theCountryEmission.Value
                    })
                    .OrderBy(group => group.Key.year)
                    .ThenBy(group => group.Key.item)
                    .ThenBy(group => group.Key.element)
                    .Select(group => new
                    {
                        countryId = group.Key.countryId, // use the name in group by
                        year = group.Key.year,
                        item = group.Key.item,
                        element = group.Key.element,
                        value = group.Key.value
                    });

                return Json(ItemElementSummary);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
