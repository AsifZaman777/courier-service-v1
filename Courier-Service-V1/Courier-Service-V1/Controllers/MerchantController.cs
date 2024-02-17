using Courier_Service_V1.Data;
using Courier_Service_V1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Courier_Service_V1.Controllers
{
    public class MerchantController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MerchantController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddNewParcel()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddParcel(Parcel parcel)
        {
            if (!ModelState.IsValid)
            {
                return View(parcel);
            }
            _context.Parcels.Add(parcel);
            _context.SaveChanges();
            TempData["success"] = "Parcel Added Successfully";
            return RedirectToAction("Index");
        }
    }
}
