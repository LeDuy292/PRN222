using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Data;
using TravelDataAccess.Models;

namespace TravelDataAccess.Controllers
{
    public class RegisterController : Controller
    {
        private readonly TravelDbContext _context;

        public RegisterController(TravelDbContext context)
        {
            _context = context;
        }

        // GET: Register
        public IActionResult Index()
        {
            // If user is already logged in, redirect to Trip List
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserCode")))
            {
                return RedirectToAction("Index", "Trip");
            }

            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Check if Code already exists
                if (await _context.Customers.AnyAsync(c => c.Code == customer.Code))
                {
                    ViewBag.ErrorMessage = "Customer code already exists. Please choose another code.";
                    return View(customer);
                }

                // Check if Email already exists
                if (!string.IsNullOrEmpty(customer.Email) && 
                    await _context.Customers.AnyAsync(c => c.Email == customer.Email))
                {
                    ViewBag.ErrorMessage = "Email already exists. Please use another email.";
                    return View(customer);
                }

                _context.Add(customer);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Registration successful! Please login with your credentials.";
                return RedirectToAction("Index", "Login");
            }

            return View(customer);
        }
    }
}
