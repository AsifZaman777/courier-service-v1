using Courier_Service_V1.Data;
using Courier_Service_V1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Courier_Service_V1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        private void UpdateLayout()
        {
           
           var riderCount = _context.Riders.Count();
            ViewBag.RiderCount = riderCount;
           

        }
        public IActionResult Index()
        {
            UpdateLayout();
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
        public IActionResult AddRider(Rider rider,IFormFile? file)
        {
            
            if (rider == null)
            {
                   return NotFound();
            }
           
           
            if (ModelState.IsValid)
            {
                //handle image
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string RiderPath = Path.Combine(wwwRootPath, @"Images\Rider");
                    using (var FileSteam = new FileStream(Path.Combine(RiderPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(FileSteam);
                    }

                    rider.ImageUrl = @"\Images\Rider\" + fileName;
                }
                else
                {
                    rider.ImageUrl = "";
                }

                _context.Riders.Add(rider);
                _context.SaveChanges();
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
