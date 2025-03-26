using AIMSR.Models;
using Microsoft.EntityFrameworkCore;

namespace AIMSR.Data // Make sure this namespace is correct
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Profile> Profiles { get; set; } // Ensure this exists
    }
}
