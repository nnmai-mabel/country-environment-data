using Microsoft.AspNetCore.Mvc;
using Assig1.Data;

namespace Assig1.Controllers
{
    public class RegionsController : Controller
    {
        private readonly EnvDataContext _context;

        public RegionsController(EnvDataContext context)
        {
            _context = context;
        }

        // GET: Regions
        public async Task<IActionResult> Index()
        {
            // This is the only ViewBag you can use to set the active Menu Item.
            ViewBag.Active = "Regions";

            #region RegionsQuery
            var RegionList = _context.Regions
                .Select(r => r)
                .Distinct()
                .OrderBy(r => r.RegionName)
                .ToList();
            #endregion

            return View(RegionList);
        }

        private bool RegionExists(int id)
        {
          return (_context.Regions?.Any(e => e.RegionId == id)).GetValueOrDefault();
        }
    }
}
