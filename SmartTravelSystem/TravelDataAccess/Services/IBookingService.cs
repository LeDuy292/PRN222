using TravelDataAccess.Models;

namespace TravelDataAccess.Services
{
    public interface IBookingService
    {
        Task<List<Booking>> GetBookingsAsync(int? customerId, string filter, string sort);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<string?> CreateBookingAsync(int customerId, int tripId, DateTime date);
        Task<string?> UpdateBookingStatusAsync(int id, string status, bool isAdmin, int? customerId);
        Task<string?> DeleteBookingAsync(int id, bool isAdmin, int? customerId);
        Task<List<Trip>> GetAvailableTripsAsync();
    }
}
