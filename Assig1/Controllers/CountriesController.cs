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
            //if (vm == null || vm.TheCountry == null)
            //{
            //    return NotFound();
            //}

            //var envDataContext = _context.Countries
            //    .GroupJoin(_context.Regions,
            //    c => c.RegionId,
            //    r => r.RegionId,
            //    (c, regionGroup) => new
            //    {
            //        theCountry = c,
            //        theRegions = regionGroup
            //    })
            //    .SelectMany(
            //    c => c.theRegions.DefaultIfEmpty(),
            //    (c, r) => new
            //    {
            //        theCountry = c.theCountry,
            //        theRegion = r
            //    })
            //    //.OrderBy(c => c.theCountry.CountryName)
            //    .Where(m => m.theCountry.CountryId == vm.CountryId)
            //    .Select(c => new Country_CountryDetail
            //    {
            //       TheCountry = c.theCountry,
            //       //TheRegion = c.theRegion,
            //       CountryId = c.theCountry.CountryId,
            //       //RegionId = c.theRegion.RegionId
            //    });

            var envDataContext = _context.Countries
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
               //.OrderBy(c => c.theCountry.CountryName)
               .Where(m => m.theCountry.CountryId == vm.CountryId)
               .Select(c => new Country_CountryDetail
               {
                   TheCountry = c.theCountry,
                   TheRegion = c.theRegion != null ? c.theRegion : null,
                   CountryId = c.theCountry.CountryId,
                   //RegionId = c.theRegion.RegionId
                   RegionId = c.theRegion != null ? c.theRegion.RegionId : (int?)null
               });
            //.FirstOrDefaultAsync(m => m.theCountry.CountryId == vm.CountryId); ;

            //var country = await envDataContext
            //    .FirstOrDefaultAsync(m => m.theCountry.CountryId == vm.CountryId);
            var country = await envDataContext
                .FirstOrDefaultAsync();
            if (country == null)
            {
                return NotFound();
            }
            vm.TheCountryDetail = country;
                
            return View(vm);
        }
        //public async Task<IActionResult> Details(Country_CountryDetail vm)
        //{
        //    if (vm == null || vm.TheCountry == null)
        //    {
        //        return NotFound();
        //    }

        //    var country = await _context.Countries
        //        .Include(c => c.Region)
        //        .FirstOrDefaultAsync(m => m.CountryId == vm.CountryId);

        //    if (country == null)
        //    {
        //        return NotFound();
        //    }
        //    vm.TheCountry = country;
        //    return View(vm);
        //}


        //if (_context.Countries == null)
        //{
        //    return NotFound();
        //}

        //var country = await _context.Countries
        //    .Include(c => c.Region)
        //    .FirstOrDefaultAsync(m => m.CountryId == vm.TheCountry.CountryId);

        //vm.TheCountry = country;
        //if (vm.TheCountry == null)
        //{
        //    return NotFound();
        //}

        //return View(vm);

        //#region CountriesListQuery
        //var CountriesList = _context.Countries
        //    .Select(c => c); // Select all countries

        //var envDataContext = CountriesList
        //    .GroupJoin(_context.Regions,
        //    c => c.RegionId,
        //    r => r.RegionId,
        //    (c, regionGroup) => new
        //    {
        //        theCountry = c,
        //        theRegions = regionGroup
        //    })
        //    .SelectMany(
        //    c => c.theRegions.DefaultIfEmpty(),
        //    (c, r) => new
        //    {
        //        theCountry = c.theCountry,
        //        theRegion = r
        //    })
        //    //.OrderBy(c => c.theCountry.CountryName)
        //    .Where(c => c.theCountry.CountryId == vm.TheCountryDetail.CountryId)
        //    .Select(c => new
        //    {
        //        c.theCountry,
        //        c.theRegion
        //    });

        //if (vm.RegionId != null)
        //{
        //    envDataContext = envDataContext
        //        .Where(c => c.theRegion.RegionId == vm.RegionId);
        //}
        //#endregion

        //if (vm.CountryId != null)
        //{
        //    envDataContext = envDataContext
        //        .Where(c => c.theCountry.CountryId == vm.TheCountry.CountryId);
        //}

        //vm.TheCountry = await envDataContext
        //     .Select(c => new Country_CountryDetail
        //     {
        //         TheCountry = c.theCountry
        //     })
        //     .ToListAsync();

        //return View(vm);

        //}
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Countries == null)
        //    {
        //        return NotFound();
        //    }

        //    var country = await _context.Countries
        //        .Include(c => c.Region)
        //        .FirstOrDefaultAsync(m => m.CountryId == id);
        //    if (country == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(country);
        //}
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
