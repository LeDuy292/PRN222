using Microsoft.AspNetCore.Mvc;
using TravelDataAccess.Models;
using TravelDataAccess.Repositories;
using TravelManagementApp.Helpers;
using TravelManagementApp.ViewModels;

namespace TravelManagementApp.Controllers
{
    public class TripController : Controller
    {
        private readonly ITripRepository _tripRepository;

        public TripController(ITripRepository tripRepository)
        {
            _tripRepository = tripRepository;
        }

        private CustomerSessionViewModel? GetCurrentCustomer()
        {
            return HttpContext.Session.GetObjectFromJson<CustomerSessionViewModel>("Customer");
        }

        private bool IsAdmin()
        {
            var customer = GetCurrentCustomer();
            return customer?.Role == "Admin";
        }

        // GET: Trip
        public async Task<IActionResult> Index()
        {
            var customer = GetCurrentCustomer();
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.CustomerName = customer.FullName;
            ViewBag.IsAdmin = IsAdmin();

            var trips = await _tripRepository.GetAllAsync();
            return View(trips);
        }

        // GET: Trip/Create
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Trip/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trip trip)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _tripRepository.AddAsync(trip);
                    TempData["SuccessMessage"] = "Trip created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += " | Inner: " + ex.InnerException.Message;
                    }
                    ModelState.AddModelError("", "Error creating trip: " + message);
                }
            }
            return View(trip);
        }

        // GET: Trip/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return NotFound();
            }

            var trip = await _tripRepository.GetByIdAsync(id.Value);
            if (trip == null)
            {
                return NotFound();
            }
            return View(trip);
        }

        // POST: Trip/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trip trip)
        {
            if (!IsAdmin())
            {
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
                    await _tripRepository.UpdateAsync(trip);
                    TempData["SuccessMessage"] = "Trip updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating trip: " + ex.Message);
                }
            }
            return View(trip);
        }

        // GET: Trip/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return NotFound();
            }

            var trip = await _tripRepository.GetByIdAsync(id.Value);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trip/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index");
            }

            try
            {
                await _tripRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Trip deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting trip: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
