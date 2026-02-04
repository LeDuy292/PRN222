using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Data;
using TravelDataAccess.Models;
using TravelDataAccess.Services;

namespace TravelDataAccess.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Check if user is logged in
        private bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserCode"));
        }

        // Check if user is Admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        private int? GetCurrentCustomerId()
        {
            return HttpContext.Session.GetInt32("CustomerID");
        }

        // GET: Booking (My Bookings)
        public async Task<IActionResult> Index(string filter = "all", string sort = "date")
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Login");

            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            ViewBag.CurrentFilter = filter;
            ViewBag.CurrentSort = sort;

            var customerId = IsAdmin() ? null : GetCurrentCustomerId();
            if (!IsAdmin() && customerId == null) return RedirectToAction("Index", "Login");

            var bookings = await _bookingService.GetBookingsAsync(customerId, filter, sort);
            return View(bookings);
        }

        // GET: Booking/Create
        public async Task<IActionResult> Create()
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Login");

            var trips = await _bookingService.GetAvailableTripsAsync();
            ViewBag.Trips = new SelectList(trips, "ID", "Destination");
            
            return View();
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int tripId, DateTime bookingDate)
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Login");

            if (IsAdmin())
            {
                TempData["ErrorMessage"] = "Admin cannot create bookings. Please use customer account.";
                return RedirectToAction("Index");
            }

            var customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                TempData["ErrorMessage"] = "Session invalid. Please login again.";
                return RedirectToAction("Index", "Login");
            }

            var error = await _bookingService.CreateBookingAsync(customerId.Value, tripId, bookingDate);
            
            if (error != null)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Login");
            if (id == null) return NotFound();

            var booking = await _bookingService.GetBookingByIdAsync(id.Value);
            if (booking == null) return NotFound();

            // Authorization check
            if (!IsAdmin() && booking.CustomerID != GetCurrentCustomerId())
            {
                TempData["ErrorMessage"] = "Access denied.";
                return RedirectToAction("Index");
            }

            return View(booking);
        }

        // POST: Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string status)
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Login");

            var error = await _bookingService.UpdateBookingStatusAsync(id, status, IsAdmin(), GetCurrentCustomerId());
            
            if (error != null)
            {
                TempData["ErrorMessage"] = error;
            }
            else
            {
                TempData["SuccessMessage"] = "Booking status updated successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Booking/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Login");

            var error = await _bookingService.DeleteBookingAsync(id, IsAdmin(), GetCurrentCustomerId());

            if (error != null)
            {
                TempData["ErrorMessage"] = error;
            }
            else
            {
                TempData["SuccessMessage"] = "Booking deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
