using Courier_Service_V1.Data;
using Courier_Service_V1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Courier_Service_V1.Controllers
{
    public class MerchantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MerchantController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }

        private void UpdateLayout()
        {

            var merchantId = HttpContext.Request.Cookies["MerchantId"];
            if (string.IsNullOrEmpty(merchantId))
            {
                return;
            }
            var merchant = _context.Merchants.Find(merchantId);
            if (merchant == null)
            {
                return;
            }
            ViewBag.TotalPickupRequest = _context.Parcels.Count(x => x.MerchantId == merchantId && x.Status == "Pickup Request");
            ViewBag.TotalDispatched = _context.Parcels.Count(x => x.MerchantId == merchantId && x.Status == "Dispatched");
            ViewBag.TotalTransit = _context.Parcels.Count(x => x.MerchantId == merchantId && x.Status == "In Transit");
            ViewBag.TotalDelivered = _context.Parcels.Count(x => x.MerchantId == merchantId && x.Status == "Delivered");
            ViewBag.TotalCancelled = _context.Parcels.Count(x => x.MerchantId == merchantId && x.Status == "Cancelled");
            ViewBag.TotalReturned = _context.Parcels.Count(x => x.MerchantId == merchantId && x.Status == "Returned");
            ViewBag.TotalParcel = _context.Parcels.Count(x => x.MerchantId == merchantId);

            //parcel list for the merchant
            ViewBag.ParcelList = _context.Parcels.Where(x => x.MerchantId == merchantId).ToList();

            //today pickup request
            ViewBag.TodayPickupRequest = _context.Parcels.Count(x => x.MerchantId == merchantId && x.PickupRequestDate == DateTime.Today.Date);
            //today dispatched parcel
            ViewBag.TodayDispatched = _context.Parcels.Count(x => x.MerchantId == merchantId && x.DispatchDate == DateTime.Today.Date);
            //today delivered parcel
            ViewBag.TodayDelivered = _context.Parcels.Count(x => x.MerchantId == merchantId && x.DeliveryDate == DateTime.Today.Date);
            //today cancelled parcel
            ViewBag.TodayCancelled = _context.Parcels.Count(x => x.MerchantId == merchantId && x.Status == "Cancelled" && x.DeliveryDate == DateTime.Today.Date);
            //today returned parcel
            ViewBag.TodayReturned = _context.Parcels.Count(x => x.MerchantId == merchantId && x.Status == "Returned" && x.DeliveryDate == DateTime.Today.Date);
            //today on transit parcel
            ViewBag.TodayTransit = _context.Parcels.Count(x => x.MerchantId == merchantId && x.Status == "Transit");


          



        }
        private bool IsMerchantLoggedIn()
        {
            var merchantId = HttpContext.Request.Cookies["MerchantId"];
            if (string.IsNullOrEmpty(merchantId))
            {
                return false;
            }
            var merchant = _context.Merchants.Find(merchantId);
            if (merchant == null)
            {
                return false;
            }
            return true;
        }
        public IActionResult Index()
        {
            if (!IsMerchantLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            UpdateLayout();
            return View();
        }

        public IActionResult Profile()
        {
            if (!IsMerchantLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            var merchantId = HttpContext.Request.Cookies["MerchantId"];
            var merchant = _context.Merchants.Find(merchantId);
            return View(merchant);
            
        }

        //update profile
        [HttpPost]
        public IActionResult UpdateProfile(Merchant merchant, IFormFile? file)
        {
            if (!IsMerchantLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            var merchantId = HttpContext.Request.Cookies["MerchantId"];
            if (string.IsNullOrEmpty(merchantId))
            {
                
                return RedirectToAction("Login", "Home");
            }

            var merchantToUpdate = _context.Merchants.Find(merchantId);
            if (merchantToUpdate == null)
            {
                
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string riderPath = Path.Combine(wwwRootPath, "Images", "Merchant");

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(merchantToUpdate.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, merchantToUpdate.ImageUrl.TrimStart('~', '/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save new image
                    using (var fileStream = new FileStream(Path.Combine(riderPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    merchant.ImageUrl = @"/Images/Merchant/" + fileName;
                    merchantToUpdate.ImageUrl = merchant.ImageUrl;
                }

                // Update other rider information
                merchantToUpdate.Name = merchant.Name;
                merchantToUpdate.CompanyName = merchant.CompanyName;

               
                merchantToUpdate.Email = merchant.Email;
                merchantToUpdate.ContactNumber = merchant.ContactNumber;
                merchantToUpdate.FullAddress = merchant.FullAddress;

                _context.Merchants.Update(merchantToUpdate);
                _context.SaveChanges();
                return RedirectToAction("Profile");
            }

            // If ModelState is not valid, return to the profile page with validation errors
            return View("Profile", merchant);
        }

        

        public IActionResult AddNewParcel()
        {
            if (!IsMerchantLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public IActionResult AddParcel(Parcel parcel)
        {
            if (!IsMerchantLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(parcel);
            }
            parcel.PickupRequestDate = DateTime.Now.Date;
            _context.Parcels.Add(parcel);
            _context.SaveChanges();
            TempData["success"] = "Parcel Added Successfully";
            return RedirectToAction("Index");
        }

        public IActionResult ChangePassword()
        {
            if (!IsMerchantLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(ResetPassword resetPassword)
        {
            if (resetPassword == null)
            {
                return NotFound();
            }
            //adminid from cookie

            if (ModelState.IsValid)
            {
                var merchant = _context.Merchants.Find(resetPassword.Id);
                if (merchant == null)
                {
                    return NotFound();
                }
                if (merchant.Password == resetPassword.OldPassword && resetPassword.NewPassword == resetPassword.ConfirmPassword)
                {
                    merchant.Password = resetPassword.NewPassword;
                    _context.SaveChanges();
                    TempData["success"] = "Password Changed Successfully";
                    return RedirectToAction("Login","Home");
                }

                else if (merchant.Password != resetPassword.OldPassword)
                {
                    TempData["error"] = "Old Password is Incorrect";
                    return View(resetPassword);
                }
                else if (resetPassword.NewPassword != resetPassword.ConfirmPassword)
                {
                    TempData["error"] = "New Password and Confirm Password does not match";
                    return View(resetPassword);
                }
                else
                {
                    return View(resetPassword);
                }
            }
            else
            {
                return View(resetPassword);
            }

        }


    }
}
