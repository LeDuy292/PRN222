using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Data;
using TravelDataAccess.Models;

namespace TravelDataAccess.Services
{
    public class AuthService : IAuthService
    {
        private readonly TravelDbContext _context;

        public AuthService(TravelDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> AuthenticateAsync(string code, string password)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Code == code && c.Password == password);
        }

        public bool IsAdmin(string code, string password)
        {
            return code == "ADMIN" && password == "admin123";
        }
    }
}
