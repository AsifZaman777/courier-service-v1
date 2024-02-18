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


        private bool IsRiderLoggedIn()
        {
           
            var riderId = HttpContext.Request.Cookies["RiderId"];
            if (string.IsNullOrEmpty(riderId))
            {
                return false;
            }

            
            var rider = _context.Riders.Find(riderId);
            if (rider == null)
            {
                return false;
            }

            return true;
        }




        public IActionResult Index()
        {
           
            if (!IsRiderLoggedIn())
            {
                
                return RedirectToAction("Login", "Home");
            }
            return View();
        }



        public IActionResult AllParcel()
        {
            if (!IsRiderLoggedIn())
            {

                return RedirectToAction("Login", "Home");
            }

            var riderId = HttpContext.Request.Cookies["RiderId"];
            var parcel = _context.Parcels.Where(p => p.RiderId == riderId).ToList();
            return View(parcel);
        }

        //status change to Transit
        public IActionResult Transit(string id)
        {
            if (!IsRiderLoggedIn())
            {

                return RedirectToAction("Login", "Home");
            }

            var riderId = HttpContext.Request.Cookies["RiderId"];
            var parcel = _context.Parcels.Find(id);
            parcel.Status = "Transit";
            //find rider by riderId
            var rider = _context.Riders.Find(riderId);
            rider.State = "Busy";
            _context.Parcels.Update(parcel);
            _context.Riders.Update(rider);
            _context.SaveChanges();
            return RedirectToAction("AllParcel");
        }

        //status change to Delivered
        public IActionResult Delivered(string id)
        {
            if (!IsRiderLoggedIn())
            {

                return RedirectToAction("Login", "Home");
            }

            var riderId = HttpContext.Request.Cookies["RiderId"];
            //find rider by riderId
            var rider = _context.Riders.Find(riderId);
            var parcel = _context.Parcels.Find(id);
            parcel.Status = "Delivered";
            rider.State = "Available";
            _context.Parcels.Update(parcel);
            _context.Riders.Update(rider);
            _context.SaveChanges();
            return RedirectToAction("AllParcel");
        }
        //Cancel Parcel
        public IActionResult Cancel(string id)
        {
            if (!IsRiderLoggedIn())
            {

                return RedirectToAction("Login", "Home");
            }

            var riderId = HttpContext.Request.Cookies["RiderId"];
            //find rider by riderId
            var rider = _context.Riders.Find(riderId);
            var parcel = _context.Parcels.Find(id);
            parcel.Status = "Cancelled";
            rider.State = "Available";
            _context.Parcels.Update(parcel);
            _context.Riders.Update(rider);
            _context.SaveChanges();
            return RedirectToAction("AllParcel");
        }
        //return parcel
        public IActionResult Return(string id)
        {
            if (!IsRiderLoggedIn())
            {

                return RedirectToAction("Login", "Home");
            }

            var riderId = HttpContext.Request.Cookies["RiderId"];
            //find rider by riderId
            var rider = _context.Riders.Find(riderId);
            var parcel = _context.Parcels.Find(id);
            parcel.Status = "Returned";
            rider.State = "Available";
            _context.Parcels.Update(parcel);
            _context.Riders.Update(rider);
            _context.SaveChanges();
            return RedirectToAction("AllParcel");
        }
    }
}
