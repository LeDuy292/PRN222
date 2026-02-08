using Microsoft.AspNetCore.Mvc;
using TravelDataAccess.Repositories;
using TravelManagementApp.Helpers;
using TravelManagementApp.ViewModels;

namespace TravelManagementApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public AccountController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetObjectFromJson<CustomerSessionViewModel>("Customer") != null)
            {
                return RedirectToAction("Index", "Trip");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if code already exists
            var existingCustomer = await _customerRepository.GetByCodeAsync(model.Code);
            if (existingCustomer != null)
            {
                ModelState.AddModelError("Code", "Customer Code already exists.");
                return View(model);
            }

            // Create new customer
            var customer = new TravelDataAccess.Models.Customer
            {
                Code = model.Code,
                FullName = model.FullName,
                Email = model.Email,
                Age = model.Age,
                Password = model.Password, // In a real app, hash this!
                Role = "User"
            };

            await _customerRepository.AddAsync(customer);

            // Auto login or redirect to login? Let's redirect to login with existing flow
            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to trips
            if (HttpContext.Session.GetObjectFromJson<CustomerSessionViewModel>("Customer") != null)
            {
                return RedirectToAction("Index", "Trip");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = await _customerRepository.GetByCodeAndPasswordAsync(model.Code, model.Password);

            if (customer == null)
            {
                ModelState.AddModelError("", "Invalid Code or Password");
                return View(model);
            }

            // Store customer info in session
            var sessionCustomer = new CustomerSessionViewModel
            {
                ID = customer.ID,
                Code = customer.Code,
                FullName = customer.FullName,
                Role = (customer.Code.Equals("ADMIN", StringComparison.OrdinalIgnoreCase)) ? "Admin" : (customer.Role ?? "User")
            };

            HttpContext.Session.SetObjectAsJson("Customer", sessionCustomer);

            return RedirectToAction("Index", "Trip");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
