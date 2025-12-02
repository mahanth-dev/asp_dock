using Microsoft.EntityFrameworkCore; // <--- این خط باید حتما باشه
using wepapp.Models; // اسم پروژت اگه wepapp هست

namespace wepapp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // اسم مدل‌هات رو اینجا بذار. مثلا اگه مدل Product داری:
        public DbSet<Product> Products { get; set; }
    }
}
