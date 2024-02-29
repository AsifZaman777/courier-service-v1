using Amazon.S3.Transfer;
using Amazon.S3;
using Courier_Service_V1.Data;
using Courier_Service_V1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Amazon;

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

            // Today Pickup Request
            DateTime todayStart = DateTime.Today;
            DateTime tomorrowStart = todayStart.AddDays(1);
            ViewBag.TodayPickupRequest = _context.Parcels
                .Count(x => x.MerchantId == merchantId &&
                            x.PickupRequestDate >= todayStart &&
                            x.PickupRequestDate < tomorrowStart);

            // Today Dispatched Parcel
            ViewBag.TodayDispatched = _context.Parcels
                .Count(x => x.MerchantId == merchantId &&
                            x.DispatchDate >= todayStart &&
                            x.DispatchDate < tomorrowStart);

            // Today Delivered Parcel
            ViewBag.TodayDelivered = _context.Parcels
                .Count(x => x.MerchantId == merchantId &&
                            x.DeliveryDate >= todayStart &&
                            x.DeliveryDate < tomorrowStart);

            // Today Cancelled Parcel
            ViewBag.TodayCancelled = _context.Parcels
                .Count(x => x.MerchantId == merchantId &&
                            x.Status == "Cancelled" &&
                            x.DeliveryDate >= todayStart &&
                            x.DeliveryDate < tomorrowStart);

            // Today Returned Parcel
            ViewBag.TodayReturned = _context.Parcels
                .Count(x => x.MerchantId == merchantId &&
                            x.Status == "Returned" &&
                            x.DeliveryDate >= todayStart &&
                            x.DeliveryDate < tomorrowStart);

            //today on transit parcel
            ViewBag.TodayTransit = _context.Parcels.Count(x => x.MerchantId == merchantId && x.Status == "Transit");

            //all parcel list for today
            ViewBag.TodayParcelList = _context.Parcels
                .Where(x => x.MerchantId == merchantId && x.PickupRequestDate >= todayStart && x.PickupRequestDate < tomorrowStart).ToList();

            //count of all parcel for today
            ViewBag.TodayTotalParcel = _context.Parcels
                .Count(x => x.MerchantId == merchantId && x.PickupRequestDate >= todayStart && x.PickupRequestDate < tomorrowStart);
          



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

                        //delete from aws s3
                        string[] split = merchantToUpdate.ImageUrl.Split("/");
                        string key = "Merchant/" + split[split.Length - 1];
                        var s3Client = new AmazonS3Client("AKIAU6GDYMTHTIZML6UG", "9Mjr5N26gAtUX6aOyGBNy688zMgP9Dt46ndJOIh/", RegionEndpoint.USEast1);
                        var fileTransferUtility = new TransferUtility(s3Client);
                        fileTransferUtility.S3Client.DeleteObjectAsync("courierbuckets3", key);
                    }

                    // Save new image
                    using (var fileStream = new FileStream(Path.Combine(riderPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    //upload to aws s3
                    var news3Client = new AmazonS3Client("AKIAU6GDYMTHTIZML6UG", "9Mjr5N26gAtUX6aOyGBNy688zMgP9Dt46ndJOIh/", RegionEndpoint.USEast1);
                    var updatedfileTransferUtility = new TransferUtility(news3Client);
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        FilePath = riderPath + "\\" + fileName,
                        BucketName = "courierbuckets3",
                        Key = "Merchant/" + fileName,
                        CannedACL = S3CannedACL.PublicRead
                    };
                    updatedfileTransferUtility.Upload(uploadRequest);
                    //after upload delete from local storage
                    System.IO.File.Delete(riderPath + "\\" + fileName);
                    merchantToUpdate.ImageUrl = "https://courierbuckets3.s3.amazonaws.com/Merchant/" + fileName;

                   
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
           
            _context.Parcels.Add(parcel);
            _context.SaveChanges();
            TempData["success"] = "Parcel Added Successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Parcels()
        {
            if (!IsMerchantLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            var merchantId = HttpContext.Request.Cookies["MerchantId"];
            var parcels = _context.Parcels.Where(x => x.MerchantId == merchantId).ToList();
            return View(parcels);
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
