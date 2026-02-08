using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Data;
using TravelDataAccess.Models;

namespace TravelDataAccess.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly TravelDbContext _context;

        public BookingRepository(TravelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Trip)
                .Include(b => b.Customer)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Trip)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.ID == id);
        }

        public async Task<IEnumerable<Booking>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Bookings
                .Include(b => b.Trip)
                .Where(b => b.CustomerID == customerId)
                .OrderBy(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetPendingByCustomerIdAsync(int customerId)
        {
            return await _context.Bookings
                .Include(b => b.Trip)
                .Where(b => b.CustomerID == customerId && b.Status == "Pending")
                .OrderBy(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Bookings.AnyAsync(b => b.ID == id);
        }
    }
}
