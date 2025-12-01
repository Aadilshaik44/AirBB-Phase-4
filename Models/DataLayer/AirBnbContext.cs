using AirBB.Models.DataLayer.Configuration;
using AirBB.Models.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace AirBB.Models.DataLayer
{
    public class AirBnbContext : DbContext
    {
        public AirBnbContext(DbContextOptions<AirBnbContext> options) : base(options) { }

        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<Residence> Residences { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply Configuration Files
            modelBuilder.ApplyConfiguration(new ClientConfig());
            modelBuilder.ApplyConfiguration(new LocationConfig());
            modelBuilder.ApplyConfiguration(new ResidenceConfig());

            base.OnModelCreating(modelBuilder);
        }
    }
}