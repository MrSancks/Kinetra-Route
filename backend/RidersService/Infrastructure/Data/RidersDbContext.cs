using Microsoft.EntityFrameworkCore;
using RidersService.Domain.Entities;

namespace RidersService.Infrastructure.Data;

public class RidersDbContext : DbContext
{
    public RidersDbContext(DbContextOptions<RidersDbContext> options) : base(options)
    {
    }

    public DbSet<Rider> Riders => Set<Rider>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<RiderDocument> Documents => Set<RiderDocument>();
    public DbSet<DeliveryHistory> Deliveries => Set<DeliveryHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Rider>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.Email).IsUnique();
            entity.HasIndex(r => r.PhoneNumber).IsUnique();

            entity.Property(r => r.FirstName).HasMaxLength(100);
            entity.Property(r => r.LastName).HasMaxLength(100);
            entity.Property(r => r.Email).HasMaxLength(150);
            entity.Property(r => r.PhoneNumber).HasMaxLength(30);

            entity.HasOne(r => r.Vehicle)
                .WithOne(v => v.Rider)
                .HasForeignKey<Vehicle>(v => v.RiderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.Documents)
                .WithOne(d => d.Rider)
                .HasForeignKey(d => d.RiderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.Deliveries)
                .WithOne(d => d.Rider)
                .HasForeignKey(d => d.RiderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Type).HasMaxLength(50);
            entity.Property(v => v.Brand).HasMaxLength(100);
            entity.Property(v => v.Model).HasMaxLength(100);
            entity.Property(v => v.PlateNumber).HasMaxLength(20);
            entity.Property(v => v.Color).HasMaxLength(50);
        });

        modelBuilder.Entity<RiderDocument>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Number).HasMaxLength(50);
            entity.HasIndex(d => new { d.RiderId, d.Type }).IsUnique();
        });

        modelBuilder.Entity<DeliveryHistory>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.OrderId).HasMaxLength(100);
        });
    }
}
