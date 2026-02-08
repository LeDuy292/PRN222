using TravelDataAccess.Models;

namespace TravelDataAccess.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task<IEnumerable<Booking>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Booking>> GetPendingByCustomerIdAsync(int customerId);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
