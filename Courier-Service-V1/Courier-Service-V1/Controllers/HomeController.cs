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
            Response.Cookies.Delete("RiderId");
            Response.Cookies.Delete("AdminId");
            Response.Cookies.Delete("MerchantId");
           

            return RedirectToAction("Login", "Home");
        }
        public IActionResult Index()
        {
            UpdateLayout();
            return View();
        }

        
        public IActionResult Rider()
        {
            var riders = _context.Riders.ToList();
            if (riders == null)
            {
                return NotFound();
            }
            return View(riders);
            
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

            // Save changes to the database
            _context.SaveChanges();

            // Redirect to the parcel details page or any other desired page
            return RedirectToAction("Index", "Home");
        }

        //handle merchant
        public IActionResult Merchant()
        {
            var merchants = _context.Merchants.ToList();
            if (merchants == null)
            {
                return NotFound();
            }
            return View(merchants);
        }

        public IActionResult AddMerchant()
        {
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
