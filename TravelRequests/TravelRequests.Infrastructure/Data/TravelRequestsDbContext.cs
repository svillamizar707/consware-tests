using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TravelRequests.Domain.Entities;

namespace TravelRequests.Infrastructure.Data
{
    public class TravelRequestsDbContext : DbContext
    {
        public TravelRequestsDbContext(DbContextOptions<TravelRequestsDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TravelRequest> TravelRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.TravelRequests)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
        }
    }
}
