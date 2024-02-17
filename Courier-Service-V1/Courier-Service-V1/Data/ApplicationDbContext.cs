using Courier_Service_V1.Models;
using Microsoft.EntityFrameworkCore;

namespace Courier_Service_V1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Rider> Riders { get; set; }
        public DbSet<Merchant> Merchants { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }
    }
    
}
