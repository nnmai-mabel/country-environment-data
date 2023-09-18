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
                .Select(c => new
                {
                    c.theCountry,
                    c.theRegion
                });

            //var envDataContext = countriesWithRegions

            //    .OrderBy(c => c.theCountry.CountryName)
            //    .SelectMany(
            //    c => c.theRegions.DefaultIfEmpty(),
            //    (c, r) => new
            //    {
            //        theCountry = c.theCountry,
            //        theRegion = r
            //    }
            //    );

            //var envDataContext = CountriesList
            //    .Join(_context.Regions,
            //    c => c.RegionId,
            //    r => r.RegionId,
            //    (c, r) => new { theCountry = c, theRegion = r })
            //    .OrderBy(c => c.theCountry.CountryName)
            //    .Select(c => new 
            //    {
            //        c.theCountry,
            //        c.theRegion
            //    });


            //var envDataContext = CountriesList;
            //.Include(i => i.Region);

            // Implement countries search
            var Regions = _context.Regions
                .OrderBy(r => r.RegionName)
                .Select(r => new
                {
                    RegionId = r.RegionId,
                    RegionName = r.RegionName
                })
                .ToList();

            vm.RegionSelectList = new SelectList(Regions,
                nameof(Region.RegionId),
                nameof(Region.RegionName));

            if (!string.IsNullOrWhiteSpace(vm.SearchText))
            {
                envDataContext = envDataContext
                    .Where(i => i.theCountry.CountryName.Contains(vm.SearchText));
            }

            if (vm.RegionId != null)
            {
                envDataContext = envDataContext
                    .Where(c => c.theRegion.RegionId == vm.RegionId);
            }
            #endregion
            vm.CountryList = await envDataContext
                .Select(c => new Country_CountryDetail
                {
                    TheCountry = c.theCountry
                })
                .ToListAsync();
            return View(vm);
            //var envDataContext = _context.Countries.Include(c => c.Region);
            //return View(await envDataContext.ToListAsync());
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(CountriesViewModel vm)
        {
            #region CountryRegion
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
            var yearList = _context.CountryEmissions
                .Select(ce => ce.Year)
                .Distinct()
                .OrderByDescending(ce => ce)
                .ToList();

            vm.YearList = new SelectList(yearList);
            #endregion

            var country = await countryRegionQuery
                .FirstOrDefaultAsync();

            if (country == null)
            {
                return NotFound();
            }
            vm.TheCountryDetail = country;

            return View(vm);
        }

        // Action for fetching country emissions data
        [Produces("application/json")]
        public IActionResult CountryEmissionsReportData(CountriesViewModel vm)
        {
            if (vm.Year > 0)
            {
                // Calculate data on Items
                if (vm.ChartLegend == "Items")
                {
                    // Calculate average
                    if (vm.ChartAggregation == "Average")
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
                            .Where(ce => ce.theCountryEmission.CountryId == vm.CountryId)
                            .Where(ce => ce.theCountryEmission.Year == vm.Year)
                            .GroupBy(group => new
                            {
                                countryId = group.theCountryEmission.CountryId,
                                year = group.theCountryEmission.Year,
                                itemId = group.theCountryEmission.ItemId,
                                item = group.theItem.ItemName
                            })
                            .Select(group => new
                            {
                                countryId = group.Key.countryId, // use the name in group by
                                year = group.Key.year,
                                itemId = group.Key.itemId,
                                item = group.Key.item,
                                valueItem = group.Average(ce => ce.theCountryEmission.Value)
                            });
                        return Json(itemEmissionsSummary);

                        //var countryEmissionsSummary = _context.CountryEmissions
                        //    .Where(ce => ce.Year == vm.Year)
                        //    .Where(ce => ce.CountryId == vm.CountryId)
                        //    .GroupBy(ce => new { ce.CountryId, ce.ItemId, ce.Year })
                        //    .Select(group => new
                        //    {
                        //        countryId = group.Key.CountryId,
                        //        year = group.Key.Year,
                        //        item = group.Key.ItemId,
                        //        valueItem = group.Average(ce => ce.Value)
                        //    });
                        //return Json(countryEmissionsSummary);
                    }

                    // Calculate sum
                    else
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
                            .Where(ce => ce.theCountryEmission.CountryId == vm.CountryId)
                            .Where(ce => ce.theCountryEmission.Year == vm.Year)
                            .GroupBy(group => new
                            {
                                countryId = group.theCountryEmission.CountryId,
                                year = group.theCountryEmission.Year,
                                itemId = group.theCountryEmission.ItemId,
                                item = group.theItem.ItemName
                            })
                            .Select(group => new
                            {
                                countryId = group.Key.countryId, // use the name in group by
                                year = group.Key.year,
                                itemId = group.Key.itemId,
                                item = group.Key.item,
                                valueItem = group.Average(ce => ce.theCountryEmission.Value)
                            });
                        return Json(itemEmissionsSummary);

                        //var countryEmissionsSummary = _context.CountryEmissions
                        //    .Where(ce => ce.Year == vm.Year)
                        //    .Where(ce => ce.CountryId == vm.CountryId)
                        //    .GroupBy(ce => new { ce.CountryId, ce.ItemId, ce.Year })
                        //    .Select(group => new
                        //    {
                        //        countryId = group.Key.CountryId,
                        //        year = group.Key.Year,
                        //        item = group.Key.ItemId,
                        //        valueItem = group.Sum(ce => ce.Value)
                        //    });
                        //return Json(countryEmissionsSummary);
                    }
                }

                // Calculate based on Elements
                else
                {
                    // Calculate average
                    if (vm.ChartAggregation == "Average")
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
                                valueElement = group.Average(ce => ce.theCountryEmission.Value)
                            });
                        return Json(elementEmissionsSummary);

                        //var countryEmissionsSummary = _context.CountryEmissions
                        //    .Where(ce => ce.Year == vm.Year)
                        //    .Where(ce => ce.CountryId == vm.CountryId)
                        //    .GroupBy(ce => new { ce.CountryId, ce.ElementId, ce.Year })
                        //    .Select(group => new
                        //    {
                        //        countryId = group.Key.CountryId,
                        //        year = group.Key.Year,
                        //        element = group.Key.ElementId,
                        //        valueElement = group.Average(ce => ce.Value)
                        //    });
                        //return Json(countryEmissionsSummary);
                    }

                    // Calculate sum
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
                                countryId = group.Key.countryId,
                                year = group.Key.year,
                                elementId = group.Key.elementId,
                                element = group.Key.element,
                                valueElement = group.Sum(ce => ce.theCountryEmission.Value)
                            });
                        return Json(elementEmissionsSummary);

                        //var countryEmissionsSummary = _context.CountryEmissions
                        //    .Where(ce => ce.Year == vm.Year)
                        //    .Where(ce => ce.CountryId == vm.CountryId)
                        //    .GroupBy(ce => new { ce.CountryId, ce.ElementId, ce.Year })
                        //    .Select(group => new
                        //    {
                        //        countryId = group.Key.CountryId,
                        //        year = group.Key.Year,
                        //        element = group.Key.ElementId,
                        //        valueElement = group.Sum(ce => ce.Value)
                        //    });
                        //return Json(countryEmissionsSummary);
                    }
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

        // GET: Countries/Create
        public IActionResult Create()
        {
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionId");
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CountryId,Iso3,CountryName,RegionId,ImageUrl")] Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionId", country.RegionId);
            return View(country);
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionId", country.RegionId);
            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CountryId,Iso3,CountryName,RegionId,ImageUrl")] Country country)
        {
            if (id != country.CountryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.CountryId))
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
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionId", country.RegionId);
            return View(country);
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .Include(c => c.Region)
                .FirstOrDefaultAsync(m => m.CountryId == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Countries == null)
            {
                return Problem("Entity set 'EnvDataContext.Countries'  is null.");
            }
            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                _context.Countries.Remove(country);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(int id)
        {
            return (_context.Countries?.Any(e => e.CountryId == id)).GetValueOrDefault();
        }
    }
}
