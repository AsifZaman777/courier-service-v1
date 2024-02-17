using Courier_Service_V1.Data;
using Courier_Service_V1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Courier_Service_V1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult Rider()
        {
            return View();
        }

        public IActionResult AddRider()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddRider(Rider rider)
        {
            
            if (rider == null)
            {
                   return NotFound();
            }
            if (ModelState.IsValid)
            {
                int riderCount = _context.Riders.Count() +1;
                rider.Id = "RID-00"+riderCount.ToString();


                _context.Riders.Add(rider);
                return RedirectToAction("Index");
            }
            else
            {
                return View(rider);
            }
            
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}
