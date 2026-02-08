using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TravelDataAccess.Models;
using TravelDataAccess.Repositories;
using TravelManagementApp.Helpers;
using TravelManagementApp.ViewModels;

namespace TravelManagementApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITripRepository _tripRepository;

        public BookingController(IBookingRepository bookingRepository, ITripRepository tripRepository)
        {
            _bookingRepository = bookingRepository;
            _tripRepository = tripRepository;
        }

        private CustomerSessionViewModel? GetCurrentCustomer()
        {
            return HttpContext.Session.GetObjectFromJson<CustomerSessionViewModel>("Customer");
        }

        // GET: Booking (My Bookings)
        public async Task<IActionResult> Index(bool? pendingOnly)
        {
            var customer = GetCurrentCustomer();
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.CustomerName = customer.FullName;
            ViewBag.PendingOnly = pendingOnly ?? false;

            IEnumerable<Booking> bookings;

            if (customer.Role == "Admin")
            {
                 // Admin sees all bookings
                 bookings = await _bookingRepository.GetAllAsync();
                 if (pendingOnly == true)
                 {
                     bookings = bookings.Where(b => b.Status == "Pending");
                 }
            }
            else
            {
                if (pendingOnly == true)
                {
                    bookings = await _bookingRepository.GetPendingByCustomerIdAsync(customer.ID);
                }
                else
                {
                    bookings = await _bookingRepository.GetByCustomerIdAsync(customer.ID);
                }
            }

            // Also pass user role to view for controlling UI elements
            ViewBag.IsAdmin = (customer.Role == "Admin");

            return View(bookings);
        }

        // GET: Booking/Create
        public async Task<IActionResult> Create()
        {
            var customer = GetCurrentCustomer();
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var trips = await _tripRepository.GetAllAsync();
            ViewBag.Trips = new SelectList(trips.Where(t => t.Status == "Available"), "ID", "Destination");
            return View();
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            var customer = GetCurrentCustomer();
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            booking.CustomerID = customer.ID;
            // Requirement: User selects date. We rely on booking.BookingDate binding from form.
            // Default status pending
            booking.Status = "Pending";

            ModelState.Remove("Customer");
            ModelState.Remove("Trip");

            // Basic validation for BookingDate: must be >= Today
            if (booking.BookingDate < DateTime.Today)
            {
                 ModelState.AddModelError("BookingDate", "Booking date cannot be in the past.");
            }

            // Check for duplicate booking
            var existingBookings = await _bookingRepository.GetByCustomerIdAsync(customer.ID);
            if (existingBookings.Any(b => b.TripID == booking.TripID && 
                                        b.BookingDate.Date == booking.BookingDate.Date && 
                                        b.Status != "Cancelled"))
            {
                ModelState.AddModelError("", "You have already booked this trip for the selected date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _bookingRepository.AddAsync(booking);
                    TempData["SuccessMessage"] = "Booking created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating booking: " + ex.Message);
                }
            }

            var trips = await _tripRepository.GetAllAsync();
            ViewBag.Trips = new SelectList(trips.Where(t => t.Status == "Available"), "ID", "Destination");
            return View(booking);
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var customer = GetCurrentCustomer();
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var booking = await _bookingRepository.GetByIdAsync(id.Value);
            
            // Fix: Admin can edit any booking. Customer can only edit their own.
            if (booking == null)
            {
                return NotFound();
            }
            
            if (customer.Role != "Admin" && booking.CustomerID != customer.ID)
            {
                return Forbid(); // Or NotFound() for security
            }

            ViewBag.StatusList = new SelectList(new[] { "Pending", "Confirmed", "Cancelled" });
            return View(booking);
        }

        // POST: Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string status)
        {
            var customer = GetCurrentCustomer();
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

             // Fix: Admin can edit any booking. Customer can only edit their own.
            bool isAdmin = customer.Role == "Admin";
            if (!isAdmin && booking.CustomerID != customer.ID)
            {
                return Forbid();
            }

            // Requirement: User can only Cancel.
            // If User tries to set anything other than Cancelled, reject it?
            // "User chỉ được huỷ và pending chờ admin duyệt booking"
            if (!isAdmin) 
            {
                if (status != "Cancelled")
                {
                    TempData["ErrorMessage"] = "You can only Cancel bookings. status 'Confirmed' is for Admins only.";
                    return RedirectToAction(nameof(Index));
                }
                 // User can only cancel PENDING bookings? Or any?
                 // Usually only pending bookings can be cancelled by user easily. 
                 // Assuming logic: User can cancel Pending. Can they cancel Confirmed? Policies vary.
                 // Given "pending chờ admin duyệt", implies flow is Pending -> Admin Confirms.
                 // If User cancels, it goes to Cancelled.
            }

            booking.Status = status;

            try
            {
                await _bookingRepository.UpdateAsync(booking);
                TempData["SuccessMessage"] = "Booking status updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating booking: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
