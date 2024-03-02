using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Courier_Service_V1.Data;
using Courier_Service_V1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Courier_Service_V1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;

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

            var merchantCount = _context.Merchants.Count();
            ViewBag.MerchantCount = merchantCount;

            //pickup count
            var pickupCount = _context.Parcels.Where(p => p.Status == "Pickup Request").Count();
            ViewBag.PickupCount = pickupCount;

            //dispatch count
            var dispatchCount = _context.Parcels.Where(p => p.Status == "Dispatched").Count();
            ViewBag.DispatchCount = dispatchCount;

            //Transit Count
            var transitCount = _context.Parcels.Where(p => p.Status == "Transit").Count();
            ViewBag.TransitCount = transitCount;

            //delivered count
            var deliveredCount = _context.Parcels.Where(p => p.Status == "Delivered").Count();
            ViewBag.DeliveredCount = deliveredCount;

            //cancelled count
            var cancelledCount = _context.Parcels.Where(p => p.Status == "Cancelled").Count();
            ViewBag.CancelledCount = cancelledCount;

            //return count
            var returnCount = _context.Parcels.Where(p => p.Status == "Returned").Count();
            ViewBag.ReturnCount = returnCount;

            //total parcel
            var totalParcel = _context.Parcels.Count();
            ViewBag.TotalParcel = totalParcel;

            DateTime todayStart = DateTime.Today;
            DateTime tomorrowStart = todayStart.AddDays(1);

            var todayPickupRequest = _context.Parcels
                .Where(p => p.PickupRequestDate >= todayStart && p.PickupRequestDate < tomorrowStart)
                .Count();

            ViewBag.TodayPickupRequest = todayPickupRequest;

            //Today Dispatched
            var todayDispatched = _context.Parcels
            .Where(p => p.DispatchDate >= todayStart && p.DispatchDate < tomorrowStart)
            .Count();
            ViewBag.TodayDispatched = todayDispatched;

            // Today Delivered
            var todayDelivered = _context.Parcels
                .Where(p => p.DeliveryDate >= todayStart && p.DeliveryDate < tomorrowStart)
                .Count();
            ViewBag.TodayDelivered = todayDelivered;

            // Today Cancelled
            var todayCancelled = _context.Parcels
                .Where(p => p.CancelDate >= todayStart && p.CancelDate < tomorrowStart)
                .Count();
            ViewBag.TodayCancelled = todayCancelled;

            // Today Returned
            var todayReturned = _context.Parcels
                .Where(p => p.ReturnDate >= todayStart && p.ReturnDate < tomorrowStart)
                .Count();
            ViewBag.TodayReturned = todayReturned;

            //all parcel list for today
            var todayParcelList = _context.Parcels
                .Where(p => p.PickupRequestDate >= todayStart && p.PickupRequestDate < tomorrowStart).Include(u=>u.Merchant)
                .ToList();
            ViewBag.TodayParcelList = todayParcelList;

        }

        public IActionResult Home()
        {
            UpdateLayout();
            return View();
        }

        [HttpPost]
        public IActionResult Home(Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Contacts.Add(contact);
                _context.SaveChanges();
                TempData["success"] = "Message Sent Successfully";
                return RedirectToAction("Home");
            }
            return View(contact);
        }

        public IActionResult Queries()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            var contacts = _context.Contacts.ToList();
            if (contacts == null)
            {
                return NotFound();
            }
            return View(contacts);
        }

        //Register Merchant
        public IActionResult RegisterMerchant()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterMerchant(Merchant merchant, IFormFile? file)
        {
            if (merchant == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //check if merchant already exists
                var merchantExists = _context.Merchants.Where(u => u.Email == merchant.Email).FirstOrDefault();
                if (merchantExists != null)
                {
                    TempData["error"] = "Merchant Email Already Exists";
                    return View(merchant);
                }
                //handle image
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string MerchantPath = Path.Combine(wwwRootPath, @"Images\Merchant");
                    using (var FileSteam = new FileStream(Path.Combine(MerchantPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(FileSteam);
                    }
                    merchant.ImageUrl = @"\Images\Merchant\" + fileName;
                }
                else
                {
                    merchant.ImageUrl = "";
                }

                _context.Merchants.Add(merchant);
                _context.SaveChanges();
                TempData["success"] = "Merchant Registered Successfully";
                return RedirectToAction("Home");
            }
            else
            {
                return View(merchant);
            }
        }

        //delete query
        public IActionResult DeleteQuery(string? id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }
            var contact = _context.Contacts.Find(id);
            if (contact == null)
            {
                return NotFound();
            }
            _context.Contacts.Remove(contact);
            _context.SaveChanges();
            TempData["error"] = "Query Deleted Successfully";
            return RedirectToAction("Queries");
        }

        //delete admin
        public IActionResult DeleteAdmin(string? id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }
            var admin = _context.Admins.Find(id);
            if (admin == null)
            {
                return NotFound();
            }
            _context.Admins.Remove(admin);
            _context.SaveChanges();
            TempData["error"] = "Admin Deleted Successfully";
            return RedirectToAction("ApplicationUser");
        }



        public IActionResult Login()
        {
            
            if (Request.Cookies["AdminId"] != null)
            {
                TempData["success"] = "You are logged in as Admin";
                return RedirectToAction("Index", "Home");
            }
            else if (Request.Cookies["RiderId"] != null)
            {
                TempData["success"] = "You are  logged in as Rider";
                return RedirectToAction("Index", "Rider");
            }
            else if (Request.Cookies["MerchantId"] != null)
            {
                TempData["success"] = "You are logged in as Merchant";
                return RedirectToAction("Index", "Merchant");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string IsRememberME)
        {
           
            var admin = _context.Admins.FirstOrDefault(a => a.Email == email && a.Password == password);
            var rider =_context.Riders.FirstOrDefault(a => a.Email == email && a.Password == password);
            var merchant =_context.Merchants.FirstOrDefault(a => a.Email == email && a.Password == password);
            CookieOptions options = new CookieOptions();

            if(admin != null)
            {
                TempData["success"] = "Login Successful";
                if (IsRememberME == "on")
                {
                    options.Expires = DateTime.Now.AddDays(7);
                }
                else
                {
                    options.Expires = DateTime.Now.AddDays(1);
                }
                Response.Cookies.Append("AdminId", admin.Id, options);
                Response.Cookies.Append("AdminEmail", admin.Email, options);
                return RedirectToAction("Index","Home");
            }
           



            if (rider !=null)
            {
                TempData["success"] = "Login Successful";
               

               

                if (IsRememberME == "on")
                {
                    options.Expires = DateTime.Now.AddDays(7);
                }
                else
                {
                    options.Expires = DateTime.Now.AddDays(1);
                }



                //store rider id and email in cookie
                Response.Cookies.Append("RiderId", rider.Id, options);
                Response.Cookies.Append("RiderEmail", rider.Email, options);


               
                return RedirectToAction("Index","Rider");
            }
            else if (merchant != null)
            {
                TempData["success"] = "Login Successful";
               
               
                if (IsRememberME == "on")
                {
                    options.Expires = DateTime.Now.AddDays(7);
                }
                else
                {
                    options.Expires = DateTime.Now.AddDays(1);
                }
               
                Response.Cookies.Append("MerchantId", merchant.Id, options);
                Response.Cookies.Append("MerchantEmail", merchant.Email, options);
                return RedirectToAction("Index","Merchant");
            }
            else
            {
                TempData["error"] = "Invalid Email or Password";
                return View();
            }

        }

        public IActionResult Logout()
        {
            
            Response.Cookies.Delete("AdminId");
            Response.Cookies.Delete("AdminEmail");
            Response.Cookies.Delete("RiderId");
            Response.Cookies.Delete("RiderEmail");
            Response.Cookies.Delete("MerchantId");
            Response.Cookies.Delete("MerchantEmail");
            TempData["success"] = "Logout Successfully";
            return RedirectToAction("Home", "Home");
            
        }

        //forget password
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgetPassword(string email,string password,string cpassword)
        {
            
            var admin = _context.Admins.FirstOrDefault(a => a.Email == email);
            var rider = _context.Riders.FirstOrDefault(a => a.Email == email);
            var merchant = _context.Merchants.FirstOrDefault(a => a.Email == email);
            if(admin !=null)
            {
                //check password and confirm password
                if (password != cpassword)
                {
                    TempData["error"] = "Password and Confirm Password does not match";
                    return RedirectToAction("ForgetPassword", "Home");
                }
                admin.Password = password;
                _context.SaveChanges();
                TempData["success"] = "Password Updated Successfully";
                return RedirectToAction("Login", "Home");
            }
            else if (rider != null)
            {
                //check password and confirm password
                if (password != cpassword)
                {
                    TempData["error"] = "Password and Confirm Password does not match";
                    return RedirectToAction("ForgetPassword", "Home");
                }
                rider.Password = password;
                _context.SaveChanges();
                TempData["success"] = "Password Updated Successfully";
                return RedirectToAction("Login", "Home");
            }
            else if (merchant != null)
            {
                //check password and confirm password
                if (password != cpassword)
                {
                    TempData["error"] = "Password and Confirm Password does not match";
                    return RedirectToAction("ForgetPassword", "Home");
                }
                merchant.Password = password;
                _context.SaveChanges();
                TempData["success"] = "Password Updated Successfully";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                TempData["error"] = "Invalid Email";
                return RedirectToAction("ForgetPassword", "Home");
            }

        }

        //isAdminLogged in or not
        public bool IsAdminLoggedIn()
        {
            if (Request.Cookies["AdminId"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public IActionResult Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            UpdateLayout();
            return View();
        }

        
        public IActionResult Rider()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            var riders = _context.Riders.ToList();
            if (riders == null)
            {
                return NotFound();
            }
            return View(riders);
            
        }

        public IActionResult AddRider()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
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
                //check email exists or not
                var email = _context.Riders.FirstOrDefault(a => a.Email == rider.Email);
                if (email != null)
                {
                    TempData["error"] = "Email Already Exists";
                    return View(rider);
                }
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
                    //upload to aws s3
                    var s3Client = new AmazonS3Client("AKIAU6GDYMTHTIZML6UG", "9Mjr5N26gAtUX6aOyGBNy688zMgP9Dt46ndJOIh/", RegionEndpoint.USEast1);
                    var fileTransferUtility = new TransferUtility(s3Client);
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        FilePath = RiderPath + "\\" + fileName,
                        BucketName = "courierbuckets3",
                        Key = "Rider/" + fileName,
                        CannedACL = S3CannedACL.PublicRead
                    };
                    fileTransferUtility.Upload(uploadRequest);
                    //after upload delete from local storage
                    System.IO.File.Delete(RiderPath + "\\" + fileName);
                    rider.ImageUrl = "https://courierbuckets3.s3.amazonaws.com/Rider/" + fileName;

                    //rider.ImageUrl = @"\Images\Rider\" + fileName;
                }
                else
                {
                    rider.ImageUrl = "";
                }

                _context.Riders.Add(rider);
                _context.SaveChanges();
                TempData["success"] = "Rider Added Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(rider);
            }
            
        }




        public IActionResult DeleteRider(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rider = _context.Riders.Find(id);
            if (rider == null)
            {
                return NotFound();
            }

            // Find all parcels associated with the rider
            var parcels = _context.Parcels.Where(p => p.RiderId == id).ToList();

            // Set RiderId to null for all associated parcels
            foreach (var parcel in parcels)
            {
                parcel.RiderId = null;
            }

            // Save changes to update the parcels
            _context.SaveChanges();

            //delete from aws s3
            if (rider.ImageUrl != null)
            {
                string[] split = rider.ImageUrl.Split("/");
                string key = "Rider/" + split[split.Length - 1];
                var s3Client = new AmazonS3Client("AKIAU6GDYMTHTIZML6UG", "9Mjr5N26gAtUX6aOyGBNy688zMgP9Dt46ndJOIh/", RegionEndpoint.USEast1);
                var fileTransferUtility = new TransferUtility(s3Client);
                fileTransferUtility.S3Client.DeleteObjectAsync("courierbuckets3", key);
            }

            // Remove the rider
            _context.Riders.Remove(rider);
            _context.SaveChanges();

            TempData["error"] = "Rider Deleted Successfully";
            return RedirectToAction("Rider");
        }


        //edit rider
        public IActionResult EditRider(string? id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }
            var rider = _context.Riders.Find(id);
            if (rider == null)
            {
                return NotFound();
            }
            return View(rider);
        }

        [HttpPost]
        public IActionResult EditRider(Rider rider, IFormFile? file)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            if (rider == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //handle image
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string riderPath = Path.Combine(wwwRootPath, "Images", "Rider");

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(rider.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, rider.ImageUrl.TrimStart('~', '/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                        //delete from aws s3
                        string[] split = rider.ImageUrl.Split("/");
                        string key = "Rider/" + split[split.Length - 1];
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
                    var uploadfileTransferUtility = new TransferUtility(news3Client);
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        FilePath = riderPath + "\\" + fileName,
                        BucketName = "courierbuckets3",
                        Key = "Rider/" + fileName,
                        CannedACL = S3CannedACL.PublicRead
                    };
                    uploadfileTransferUtility.Upload(uploadRequest);
                    //after upload delete from local storage
                    System.IO.File.Delete(riderPath + "\\" + fileName);
                    rider.ImageUrl = "https://courierbuckets3.s3.amazonaws.com/Rider/" + fileName;


                   
                }

                // Update other properties
                _context.Riders.Update(rider);
                _context.SaveChanges();
                TempData["success"] = "Rider Updated Successfully";
                return RedirectToAction("Rider");
            }
            else
            {
                return View(rider);
            }
        }


        public IActionResult Parcel()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            var parcels = _context.Parcels.Include(u=>u.Merchant).ToList();
            if (parcels == null)
            {
                return NotFound();
            }
            return View(parcels);
        }


        public IActionResult PaymentInHand(string? id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var parcel = _context.Parcels.Find(id);
            if (parcel == null)
            {
                return NotFound();
            }

            parcel.PaymentInHand = "PaymentInHand";
            _context.Parcels.Update(parcel);
            _context.SaveChanges();

            TempData["success"] = "Payment In Hand";
            return RedirectToAction("Parcel");
        }

        public IActionResult PaymentNotInHand(string? id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var parcel = _context.Parcels.Find(id);
            if (parcel == null)
            {
                return NotFound();
            }

            parcel.PaymentInHand = "PaymentNotInHand";
            TempData["success"] = "Payment Not In Hand";
            _context.Parcels.Update(parcel);
            _context.SaveChanges();

            TempData["success"] = "Payment Payment In Hand";
            return RedirectToAction("Parcel");
        }

        //assign a parcel
        public IActionResult AssignParcel(string id)
        {

            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            // Find the parcel by ID
            var parcel = _context.Parcels.Find(id);
            if (parcel == null)
            {
                return NotFound();
            }

            // Get a list of available riders
            var riders = _context.Riders.Where(u=>u.State == "Available");

            // Pass the list of riders to the view
            ViewBag.Riders = riders;
           

            return View(parcel);
        }
        [HttpPost]
        public IActionResult AssignParcel(string id, string riderId)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }


            // Find the parcel by ID
            var parcel = _context.Parcels.Find(id);
            if (parcel == null)
            {
                return NotFound();
            }

            // Find the rider by ID
            var rider = _context.Riders.Find(riderId);
            if (rider == null)
            {
                return NotFound();
            }

            // Assign the rider to the parcel
            parcel.Rider = rider;
            parcel.Status = "Dispatched";
            parcel.DispatchDate = DateTime.Now.Date;
            

            // Save changes to the database
            _context.SaveChanges();
            TempData["success"] = "Parcel Assigned Successfully";
            // Redirect to the parcel details page or any other desired page
            return RedirectToAction("Parcel", "Home");
        }

        //handle merchant
        public IActionResult Merchant()
        {

            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            var merchants = _context.Merchants.ToList();
            if (merchants == null)
            {
                return NotFound();
            }
            return View(merchants);
        }

        public IActionResult AddMerchant()
        {

            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public IActionResult AddMerchant(Merchant merchant, IFormFile? file)
        {



            if (merchant == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //check if merchant already exists
                var merchantExists = _context.Merchants.Where(u => u.Email == merchant.Email).FirstOrDefault();
                if (merchantExists != null)
                {
                    TempData["error"] = "Merchant Email Already Exists";
                    return View(merchant);
                }
               
                //handle image
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string MerchantPath = Path.Combine(wwwRootPath, @"Images\Merchant");
                    using (var FileSteam = new FileStream(Path.Combine(MerchantPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(FileSteam);
                    }

                    //upload to aws s3
                    var s3Client = new AmazonS3Client("AKIAU6GDYMTHTIZML6UG", "9Mjr5N26gAtUX6aOyGBNy688zMgP9Dt46ndJOIh/", RegionEndpoint.USEast1);
                    var fileTransferUtility = new TransferUtility(s3Client);
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        FilePath = MerchantPath + "\\" + fileName,
                        BucketName = "courierbuckets3",
                        Key = "Merchant/" + fileName,
                        CannedACL = S3CannedACL.PublicRead
                    };
                    fileTransferUtility.Upload(uploadRequest);
                    //after upload delete from local storage
                    System.IO.File.Delete(MerchantPath + "\\" + fileName);
                    merchant.ImageUrl = "https://courierbuckets3.s3.amazonaws.com/Merchant/" + fileName;


                    //merchant.ImageUrl = @"\Images\Merchant\" + fileName;
                }
                else
                {
                    merchant.ImageUrl = "";
                }

                _context.Merchants.Add(merchant);
                _context.SaveChanges();
                TempData["success"] = "Merchant Added Successfully";
                return RedirectToAction("Merchant");
            }
            else
            {
                return View(merchant);
            }
        }

        
        public IActionResult DeleteMerchant(string? id)
        {

            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            // Find all parcels associated with the rider
            var parcels = _context.Parcels.Where(p => p.MerchantId == id).ToList();

            // Set RiderId to null for all associated parcels
            foreach (var parcel in parcels)
            {
                parcel.MerchantId = null;
            }

            // Save changes to update the parcels
            _context.SaveChanges();

            var merchant = _context.Merchants.Find(id);
            if (merchant == null)
            {
                return NotFound();
            }

            //delete from aws s3
            if (merchant.ImageUrl != null)
            {
                string[] split = merchant.ImageUrl.Split("/");
                string key = "Merchant/" + split[split.Length - 1];
                var s3Client = new AmazonS3Client("AKIAU6GDYMTHTIZML6UG", "9Mjr5N26gAtUX6aOyGBNy688zMgP9Dt46ndJOIh/", RegionEndpoint.USEast1);
                var fileTransferUtility = new TransferUtility(s3Client);
                fileTransferUtility.S3Client.DeleteObjectAsync("courierbuckets3", key);
            }


            _context.Merchants.Remove(merchant);
            _context.SaveChanges();
            TempData["error"] = "Merchant Deleted Successfully";
            return RedirectToAction("Merchant");
        }

        //edit merchant
        public IActionResult EditMerchant(string? id)
        {

            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }
            var merchant = _context.Merchants.Find(id);
            if (merchant == null)
            {
                return NotFound();
            }
            return View(merchant);
        }

        [HttpPost]
        public IActionResult EditMerchant(Merchant merchant, IFormFile? file)
        {

            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            if (merchant == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //handle image
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    //handle if prevoius image exist
                    if (merchant.ImageUrl != null)
                    {
                        
                        string imagePath = Path.Combine(wwwRootPath, merchant.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                        //delete from aws s3
                        string[] split = merchant.ImageUrl.Split("/");
                        string key = "Merchant/" + split[split.Length - 1];
                        var s3Client = new AmazonS3Client("AKIAU6GDYMTHTIZML6UG", "9Mjr5N26gAtUX6aOyGBNy688zMgP9Dt46ndJOIh/", RegionEndpoint.USEast1);
                        var fileTransferUtility = new TransferUtility(s3Client);
                        fileTransferUtility.S3Client.DeleteObjectAsync("courierbuckets3", key);


                    }
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string MerchantPath = Path.Combine(wwwRootPath, @"Images\Merchant");
                    using (var FileSteam = new FileStream(Path.Combine(MerchantPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(FileSteam);
                    }

                    //upload to aws s3
                    var news3Client = new AmazonS3Client("AKIAU6GDYMTHTIZML6UG", "9Mjr5N26gAtUX6aOyGBNy688zMgP9Dt46ndJOIh/", RegionEndpoint.USEast1);
                    var updatedfileTransferUtility = new TransferUtility(news3Client);
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        FilePath = MerchantPath + "\\" + fileName,
                        BucketName = "courierbuckets3",
                        Key = "Merchant/" + fileName,
                        CannedACL = S3CannedACL.PublicRead
                    };
                    updatedfileTransferUtility.Upload(uploadRequest);
                    //after upload delete from local storage
                    System.IO.File.Delete(MerchantPath + "\\" + fileName);
                    merchant.ImageUrl = "https://courierbuckets3.s3.amazonaws.com/Merchant/" + fileName;
                }
                

                _context.Merchants.Update(merchant);
                _context.SaveChanges();
                TempData["success"] = "Merchant Updated Successfully";
                return RedirectToAction("Merchant");
            }
            else
            {
                return View(merchant);
            }
        }

        public IActionResult ChangePassword()
        {
            if (!IsAdminLoggedIn())
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
            
           
            if (ModelState.IsValid)
            {
                var admin = _context.Admins.Find(resetPassword.Id);
                if (admin == null)
                {
                    return NotFound();
                }
                if (admin.Password == resetPassword.OldPassword)
                {
                    admin.Password = resetPassword.NewPassword;
                    _context.SaveChanges();
                    TempData["success"] = "Password Changed Successfully";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["error"] = "Old Password is Incorrect";
                    return View(resetPassword);
                }
            }
            else
            {
                return View(resetPassword);
            }

        }

        public IActionResult DeleteParcel(string? Id)
        {
            var parcel = _context.Parcels.Find(Id);
            if (parcel == null)
            {
                return NotFound();
            }
            _context.Parcels.Remove(parcel);
            _context.SaveChanges();
            TempData["error"] = "Parcel Deleted Successfully";
            return RedirectToAction("Parcel");
        }

        public IActionResult ApplicationUser()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            var users = _context.Admins.ToList();
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
            
        }
        #region API

        [HttpGet]
        public JsonResult GetParcel()
        {
            var parcels = _context.Parcels.ToList();
            return Json(new { data = parcels });
        }

        #endregion
        public IActionResult AddAdmin()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public IActionResult AddAdmin(Admin admin)
        {
            if (admin == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _context.Admins.Add(admin);
                _context.SaveChanges();
                TempData["success"] = "Admin Added Successfully";
                return RedirectToAction("ApplicationUser");
            }
            else
            {
                return View(admin);
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
