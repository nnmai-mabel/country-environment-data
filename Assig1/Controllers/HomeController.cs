using Assig1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Assig1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // This is the only ViewBag you can use to set the active Menu Item.
            ViewBag.Active = "Home"; 
            return View();
        }

        public IActionResult About()
        {
            // This is the only ViewBag you can use to set the active Menu Item.
            ViewBag.Active = nameof(About); 
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}