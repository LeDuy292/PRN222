using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Data;
using TravelDataAccess.Models;

namespace TravelDataAccess.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly TravelDbContext _context;

        public TripRepository(TravelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trip>> GetAllAsync()
        {
            return await _context.Trips.ToListAsync();
        }

        public async Task<Trip?> GetByIdAsync(int id)
        {
            return await _context.Trips.FindAsync(id);
        }

        public async Task<Trip?> GetByCodeAsync(string code)
        {
            return await _context.Trips.FirstOrDefaultAsync(t => t.Code == code);
        }

        public async Task AddAsync(Trip trip)
        {
            await _context.Trips.AddAsync(trip);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Trip trip)
        {
            _context.Trips.Update(trip);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var trip = await GetByIdAsync(id);
            if (trip != null)
            {
                _context.Trips.Remove(trip);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Trips.AnyAsync(t => t.ID == id);
        }
    }
}
