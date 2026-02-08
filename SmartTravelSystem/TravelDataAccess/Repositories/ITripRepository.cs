using TravelDataAccess.Models;

namespace TravelDataAccess.Repositories
{
    public interface ITripRepository
    {
        Task<IEnumerable<Trip>> GetAllAsync();
        Task<Trip?> GetByIdAsync(int id);
        Task<Trip?> GetByCodeAsync(string code);
        Task AddAsync(Trip trip);
        Task UpdateAsync(Trip trip);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
