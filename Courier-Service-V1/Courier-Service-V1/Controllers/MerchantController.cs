using Microsoft.AspNetCore.Mvc;

namespace Courier_Service_V1.Controllers
{
    public class MerchantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
