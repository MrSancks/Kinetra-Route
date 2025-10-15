using Microsoft.EntityFrameworkCore;
using RoutesService.Infrastructure.Data.Models;

namespace RoutesService.Infrastructure.Data;

public class RoutesDbContext : DbContext
{
    public RoutesDbContext(DbContextOptions<RoutesDbContext> options)
        : base(options)
    {
    }

    public DbSet<RoutePlanRecord> RoutePlans => Set<RoutePlanRecord>();
    public DbSet<RiderLocationRecord> RiderLocations => Set<RiderLocationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RoutePlanRecord>(entity =>
        {
            entity.HasKey(e => e.DeliveryId);
            entity.Property(e => e.DeliveryId)
                .HasMaxLength(128);
            entity.Property(e => e.PayloadJson)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<RiderLocationRecord>(entity =>
        {
            entity.HasKey(e => e.RiderId);
            entity.Property(e => e.RiderId)
                .HasMaxLength(128);
            entity.Property(e => e.Latitude)
                .IsRequired();
            entity.Property(e => e.Longitude)
                .IsRequired();
            entity.Property(e => e.RecordedAt)
                .IsRequired();
        });
    }
}
