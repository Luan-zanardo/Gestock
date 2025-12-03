using Microsoft.EntityFrameworkCore;
using MovementService.Models;

namespace MovementService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movement> Movements { get; set; } = null!;
    }
}
