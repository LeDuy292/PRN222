using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace TravelDataAccess.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // If user is already logged in, redirect to Trip List
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserCode")))
            {
                return RedirectToAction("Index", "Trip");
            }

            return View();
        }
    }
}
