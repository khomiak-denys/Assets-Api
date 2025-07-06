using Microsoft.EntityFrameworkCore;
using Assets_Api.Models;

namespace Assets_Api.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}
        public DbSet<Assets> assets_data { get; set; }
        public DbSet<PriceInfo> priceInfo_data { get; set; }
    }
}

