using TravelDataAccess.Models;

namespace TravelDataAccess.Services
{
    public interface IAuthService
    {
        Task<Customer?> AuthenticateAsync(string code, string password);
        bool IsAdmin(string code, string password);
    }
}
