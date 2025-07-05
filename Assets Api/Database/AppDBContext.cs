using Microsoft.EntityFrameworkCore;

namespace Assets_Api.Database
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        // Define DbSets for your entities here, e.g.:
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
}

