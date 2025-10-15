using Microsoft.EntityFrameworkCore;
using ReportsService.Infrastructure.Data.Models;

namespace ReportsService.Infrastructure.Data;

public class ReportsDbContext : DbContext
{
    public ReportsDbContext(DbContextOptions<ReportsDbContext> options)
        : base(options)
    {
    }

    public DbSet<OrderCompletedEventRecord> OrderCompletedEvents => Set<OrderCompletedEventRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrderCompletedEventRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderId)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.RiderId)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.CompletedAt)
                .IsRequired();
            entity.Property(e => e.OrderTotal)
                .IsRequired();
            entity.Property(e => e.PlatformFee)
                .IsRequired();
        });
    }
}
