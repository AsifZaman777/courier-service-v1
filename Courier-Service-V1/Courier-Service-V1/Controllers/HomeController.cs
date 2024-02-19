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

            //today pickuprequest
            var todayPickupRequest = _context.Parcels.Where(p => p.PickupRequestDate == DateTime.Now.Date).Count();
            ViewBag.TodayPickupRequest = todayPickupRequest;

            //Today Dispatched
            var todayDispatched = _context.Parcels.Where(p => p.DispatchDate == DateTime.Now.Date).Count();
            ViewBag.TodayDispatched = todayDispatched;

            //Today Delivered
            var todayDelivered = _context.Parcels.Where(p => p.DeliveryDate == DateTime.Now.Date).Count();
            ViewBag.TodayDelivered = todayDelivered;

            //Today Cancelled
            var todayCancelled = _context.Parcels.Where(p => p.CancelDate == DateTime.Now.Date).Count();
            ViewBag.TodayCancelled = todayCancelled;
            //Today Returned
            var todayReturned = _context.Parcels.Where(p => p.ReturnDate == DateTime.Now.Date).Count();
            ViewBag.TodayReturned = todayReturned;

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string IsRememberME)
        {
            //check admin, merchant, rider
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



                //Add id to cookie
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

           

            return RedirectToAction("Login", "Home");
        }

        //forget password
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgetPassword(string email,string password,string cpassword)
        {
            //apply for admin,Merchant and rider
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
              if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"Images\Rider");

                    if (!string.IsNullOrEmpty(rider.ImageUrl))
                    {
                        //delete old Image
                        var oldImagePath = Path.Combine(wwwRootPath, rider.ImageUrl.Trim('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                    }


                    using (var FileSteam = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(FileSteam);
                    }

                    rider.ImageUrl = @"\Images\Rider\" + fileName;
                }

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

            var parcels = _context.Parcels.ToList();
            if (parcels == null)
            {
                return NotFound();
            }
            return View(parcels);
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

            // Redirect to the parcel details page or any other desired page
            return RedirectToAction("Index", "Home");
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
            var merchant = _context.Merchants.Find(id);
            if (merchant == null)
            {
                return NotFound();
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
                    }
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
