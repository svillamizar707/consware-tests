using Microsoft.EntityFrameworkCore;
using TravelRequests.Application.Interfaces;
using TravelRequests.Domain.Entities;
using TravelRequests.Infrastructure.Data;

namespace TravelRequests.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TravelRequestsDbContext _context;

        public UserRepository(TravelRequestsDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByIdAsync(int id)
            => await _context.Users.FindAsync(id);

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
            => await _context.Users.ToListAsync();

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
