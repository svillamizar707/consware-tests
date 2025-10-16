using TravelRequests.Domain.Entities;

namespace TravelRequests.Application.Interfaces
{
    public interface ITravelRequestRepository
    {
        Task AddAsync(TravelRequest request);
        Task<IEnumerable<TravelRequest>> GetByUserIdAsync(int userId);
        Task<TravelRequest?> GetByIdAsync(int id);
        Task<IEnumerable<TravelRequest>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
