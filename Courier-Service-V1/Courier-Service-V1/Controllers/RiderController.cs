using Courier_Service_V1.Data;
using Courier_Service_V1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Courier_Service_V1.Controllers
{
    public class RiderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RiderController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }

        private void UpdateLayout()
        {
            var riderId = HttpContext.Request.Cookies["RiderId"];
            if (string.IsNullOrEmpty(riderId))
            {
                return;
            }
            var rider = _context.Riders.Find(riderId);
            if (rider == null)
            {
                return;
            }
           
            ViewBag.TotalDispatched = _context.Parcels.Count(x => x.RiderId == riderId && x.Status == "Dispatched");
            ViewBag.TotalDelivered = _context.Parcels.Count(x => x.RiderId == riderId && x.Status == "Delivered");
            ViewBag.TotalCancelled = _context.Parcels.Count(x => x.RiderId == riderId && x.Status == "Cancelled");
            ViewBag.TotalReturned = _context.Parcels.Count(x => x.RiderId == riderId && x.Status == "Returned");
            ViewBag.TotalParcel = _context.Parcels.Count(x => x.RiderId == riderId);

            //parcel list for the rider
            ViewBag.ParcelList = _context.Parcels.Where(x => x.RiderId == riderId).ToList();

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
            UpdateLayout();
            return View();
        }

        //profile
        public IActionResult Profile()
        {
            if (!IsRiderLoggedIn())
            {

                return RedirectToAction("Login", "Home");
            }
            var riderId = HttpContext.Request.Cookies["RiderId"];
            var rider = _context.Riders.Find(riderId);
            return View(rider);
        }

        //update profile
        [HttpPost]
        public IActionResult UpdateProfile(Rider rider,IFormFile? file)
        {
            if (!IsRiderLoggedIn())
            {

                return RedirectToAction("Login", "Home");
            }
           //here i want to update specific information not all infomation
           var riderId = HttpContext.Request.Cookies["RiderId"];
            var riderToUpdate = _context.Riders.Find(riderId);


            if(ModelState.IsValid)
            {
                //handle image
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    //handle if prevoius image exist
                    if (rider.ImageUrl != null)
                    {
                        string imagePath = Path.Combine(wwwRootPath, rider.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
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
                riderToUpdate.Name = rider.Name;
                riderToUpdate.Email = rider.Email;
                riderToUpdate.ContactNumber = rider.ContactNumber;
                riderToUpdate.FullAddress = rider.FullAddress;
                riderToUpdate.ImageUrl = rider.ImageUrl;

                _context.Riders.Update(riderToUpdate);
                _context.SaveChanges();
                return RedirectToAction("Profile");
            }
            else
            {
                return RedirectToAction("Profile");
            }

            
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
