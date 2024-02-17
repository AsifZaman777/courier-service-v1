using Courier_Service_V1.Data;
using Microsoft.AspNetCore.Mvc;

namespace Courier_Service_V1.Controllers
{
    public class RiderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RiderController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }



        public IActionResult AllParcel()
        {
            var riderId = HttpContext.Request.Cookies["RiderId"];
            var parcel = _context.Parcels.Where(p => p.RiderId == riderId).ToList();
            return View(parcel);
        }

        //status change to Transit
        public IActionResult Transit(string id)
        {
            var parcel = _context.Parcels.Find(id);
            parcel.Status = "Transit";
            _context.Parcels.Update(parcel);
            _context.SaveChanges();
            return RedirectToAction("AllParcel");
        }

        //status change to Delivered
        public IActionResult Delivered(string id)
        {
            var parcel = _context.Parcels.Find(id);
            parcel.Status = "Delivered";
            _context.Parcels.Update(parcel);
            _context.SaveChanges();
            return RedirectToAction("AllParcel");
        }
        //Cancel Parcel
        public IActionResult Cancel(string id)
        {
            var parcel = _context.Parcels.Find(id);
            parcel.Status = "Cancelled";
            _context.Parcels.Update(parcel);
            _context.SaveChanges();
            return RedirectToAction("AllParcel");
        }
        //return parcel
        public IActionResult Return(string id)
        {
            var parcel = _context.Parcels.Find(id);
            parcel.Status = "Returned";
            _context.Parcels.Update(parcel);
            _context.SaveChanges();
            return RedirectToAction("AllParcel");
        }
    }
}
