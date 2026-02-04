using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Data;
using TravelDataAccess.Models;

namespace TravelDataAccess.Services
{
    public class BookingService : IBookingService
    {
        private readonly TravelDbContext _context;

        public BookingService(TravelDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetBookingsAsync(int? customerId, string filter, string sort)
        {
            IQueryable<Booking> query = _context.Bookings
                .Include(b => b.Trip)
                .Include(b => b.Customer);

            if (customerId.HasValue)
            {
                query = query.Where(b => b.CustomerID == customerId.Value);
            }

            // Filter
            if (filter == "pending") query = query.Where(b => b.Status == "Pending");
            else if (filter == "confirmed") query = query.Where(b => b.Status == "Confirmed");
            else if (filter == "cancelled") query = query.Where(b => b.Status == "Cancelled");

            // Sort
            if (sort == "date") query = query.OrderBy(b => b.BookingDate);
            else if (sort == "date_desc") query = query.OrderByDescending(b => b.BookingDate);

            return await query.ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Trip)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.ID == id);
        }

        public async Task<List<Trip>> GetAvailableTripsAsync()
        {
            return await _context.Trips
                .Where(t => t.Status == "Available")
                .ToListAsync();
        }

        public async Task<string?> CreateBookingAsync(int customerId, int tripId, DateTime date)
        {
            if (date < DateTime.Today) return "Booking date cannot be in the past.";

            var trip = await _context.Trips.FindAsync(tripId);
            if (trip == null) return "Trip not found.";
            if (trip.Status != "Available") return "This trip is not available for booking.";

            var existingBooking = await _context.Bookings
                .AnyAsync(b => b.TripID == tripId && b.CustomerID == customerId && b.BookingDate.Date == date.Date);

            if (existingBooking) return "You already have a booking for this trip on this date.";

            var booking = new Booking
            {
                TripID = tripId,
                CustomerID = customerId,
                BookingDate = date,
                Status = "Pending"
            };

            _context.Add(booking);
            await _context.SaveChangesAsync();
            return null; // Success
        }

        public async Task<string?> UpdateBookingStatusAsync(int id, string status, bool isAdmin, int? customerId)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return "Booking not found.";

            // Authorization check
            if (!isAdmin && booking.CustomerID != customerId) return "Access denied.";

            // Business rule: Customers cannot confirm
            if (!isAdmin && status == "Confirmed") return "Only administrators can confirm bookings.";

            if (status == "Pending" || status == "Confirmed" || status == "Cancelled")
            {
                booking.Status = status;
                _context.Update(booking);
                await _context.SaveChangesAsync();
                return null; // Success
            }
            
            return "Invalid status.";
        }

        public async Task<string?> DeleteBookingAsync(int id, bool isAdmin, int? customerId)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return "Booking not found.";

            // Authorization check
            if (!isAdmin && booking.CustomerID != customerId) return "Access denied.";

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return null; // Success
        }
    }
}
