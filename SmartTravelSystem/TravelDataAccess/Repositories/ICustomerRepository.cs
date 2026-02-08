using TravelDataAccess.Models;

namespace TravelDataAccess.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByCodeAsync(string code);
        Task<Customer?> GetByCodeAndPasswordAsync(string code, string password);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task<bool> ExistsAsync(int id);
    }
}
