using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Data;
using TravelDataAccess.Models;

namespace TravelDataAccess.Controllers
{
    public class TripController : Controller
    {
        private readonly TravelDbContext _context;

        public TripController(TravelDbContext context)
        {
            _context = context;
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

        // GET: Trip
        public async Task<IActionResult> Index()
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            var trips = await _context.Trips.ToListAsync();
            return View(trips);
        }

        // GET: Trip/Create (Admin only)
        public IActionResult Create()
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Index", "Login");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admin only.";
                return RedirectToAction("Index");
            }

            return View();
        }

        // POST: Trip/Create (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trip trip)
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Index", "Login");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admin only.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                // Check if Code already exists
                if (await _context.Trips.AnyAsync(t => t.Code == trip.Code))
                {
                    ModelState.AddModelError("Code", "Trip code already exists");
                    return View(trip);
                }

                _context.Add(trip);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Trip created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(trip);
        }

        // GET: Trip/Edit/5 (Admin only)
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Index", "Login");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admin only.";
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }
            return View(trip);
        }

        // POST: Trip/Edit/5 (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trip trip)
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Index", "Login");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admin only.";
                return RedirectToAction("Index");
            }

            if (id != trip.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if Code already exists (excluding current trip)
                    if (await _context.Trips.AnyAsync(t => t.Code == trip.Code && t.ID != id))
                    {
                        ModelState.AddModelError("Code", "Trip code already exists");
                        return View(trip);
                    }

                    _context.Update(trip);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Trip updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TripExists(trip.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trip);
        }

        // GET: Trip/Delete/5 (Admin only)
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Index", "Login");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admin only.";
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trip/Delete/5 (Admin only)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Index", "Login");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admin only.";
                return RedirectToAction("Index");
            }

            var trip = await _context.Trips.FindAsync(id);
            if (trip != null)
            {
                // Check if trip has bookings
                var hasBookings = await _context.Bookings.AnyAsync(b => b.TripID == id);
                if (hasBookings)
                {
                    TempData["ErrorMessage"] = "Cannot delete trip with existing bookings.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Trips.Remove(trip);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Trip deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TripExists(int id)
        {
            return await _context.Trips.AnyAsync(e => e.ID == id);
        }
    }
}
