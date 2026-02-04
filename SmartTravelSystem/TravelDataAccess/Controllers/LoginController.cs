using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Data;
using TravelDataAccess.Models;
using TravelDataAccess.Services;

namespace TravelDataAccess.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;
        private readonly TravelDbContext _context; // Keep context if needed for other methods not yet refactored, otherwise remove

        public LoginController(IAuthService authService, TravelDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        // GET: Login
        public IActionResult Index()
        {
            // Clear session when accessing login page
            HttpContext.Session.Clear();
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Please enter both Username and Password";
                return View();
            }

            // Check for Admin login
            // Map username to code for service calls
            string code = username; 
            if (_authService.IsAdmin(code, password))
            {
                HttpContext.Session.SetString("UserCode", "ADMIN");
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("FullName", "Administrator");
                return RedirectToAction("Index", "Trip");
            }

            // Check for Customer login
            var customer = await _authService.AuthenticateAsync(code, password);

            if (customer != null)
            {
                HttpContext.Session.SetString("UserCode", customer.Code);
                HttpContext.Session.SetString("UserRole", "Customer");
                HttpContext.Session.SetString("FullName", customer.FullName);
                HttpContext.Session.SetInt32("CustomerID", customer.ID);
                return RedirectToAction("Index", "Trip");
            }

            ViewBag.ErrorMessage = "Invalid Username or Password";
            return View();
        }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
