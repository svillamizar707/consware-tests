using Microsoft.EntityFrameworkCore;
using TravelRequests.Application.Interfaces;
using TravelRequests.Domain.Entities;
using TravelRequests.Infrastructure.Data;

namespace TravelRequests.Infrastructure.Repositories
{
    public class TravelRequestRepository : ITravelRequestRepository
    {
        private readonly TravelRequestsDbContext _context;

        public TravelRequestRepository(TravelRequestsDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TravelRequest request)
        {
            await _context.TravelRequests.AddAsync(request);
        }

        public async Task<IEnumerable<TravelRequest>> GetByUserIdAsync(int userId)
            => await _context.TravelRequests.Where(r => r.UserId == userId).ToListAsync();

        public async Task<TravelRequest?> GetByIdAsync(int id)
            => await _context.TravelRequests.FindAsync(id);

        public async Task<IEnumerable<TravelRequest>> GetAllAsync()
            => await _context.TravelRequests.Include(r => r.User).ToListAsync();

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
