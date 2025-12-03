using Microsoft.EntityFrameworkCore;
using SuppliersService.Models;

namespace SuppliersService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Supplier> Suppliers { get; set; }
    }
}
