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
       public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Admin> Admins { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //relationship between merchant and parcel
            modelBuilder.Entity<Parcel>()
                .HasOne(p => p.Merchant)
                .WithMany(m => m.Parcels)
                .HasForeignKey(p => p.MerchantId);
            //relationship between rider and parcel
            modelBuilder.Entity<Parcel>()
                .HasOne(p => p.Rider)
                .WithMany(r => r.Parcels)
                .HasForeignKey(p => p.RiderId);
            //seed Admin Data
            modelBuilder.Entity<Admin>().HasData(new Admin
            {
                Id = "AD-1234",
                Name = "Admin",
                Email = "admin@gmail.com",
                Password = "admin"
            });


        }
    }
    
}
