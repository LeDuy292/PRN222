using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Models;

namespace TravelDataAccess.Data
{
    public class TravelDbContext : DbContext
    {
        public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options)
        {
        }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Trip
            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Price).HasPrecision(12, 2);
            });

            // Configure Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Booking
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.ID);
                
                entity.HasOne(b => b.Trip)
                    .WithMany(t => t.Bookings)
                    .HasForeignKey(b => b.TripID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Customer)
                    .WithMany(c => c.Bookings)
                    .HasForeignKey(b => b.CustomerID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
