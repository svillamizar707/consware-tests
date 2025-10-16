using TravelRequests.Domain.Entities;

namespace TravelRequests.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
