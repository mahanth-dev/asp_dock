using Microsoft.EntityFrameworkCore;
using wepapp.Models;

namespace wepapp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {}

        public DbSet<Product> Products { get; set; }
    }
}
