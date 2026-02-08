using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Data;
using TravelDataAccess.Models;

namespace TravelDataAccess.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly TravelDbContext _context;

        public CustomerRepository(TravelDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customer?> GetByCodeAsync(string code)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<Customer?> GetByCodeAndPasswordAsync(string code, string password)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Code == code && c.Password == password);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Customers.AnyAsync(c => c.ID == id);
        }
    }
}
