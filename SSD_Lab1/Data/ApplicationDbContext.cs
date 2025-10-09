using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SSD_Lab1.Models;

namespace SSD_Lab1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<SSD_Lab1.Models.Car> Car { get; set; }
        public DbSet<SSD_Lab1.Models.Employee> Employee { get; set; }
        public DbSet<SSD_Lab1.Models.Message> Message { get; set; } = default!;
    }
}
