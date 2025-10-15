using Microsoft.EntityFrameworkCore;
using OrdersService.Infrastructure.Data.Models;

namespace OrdersService.Infrastructure.Data;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options)
    {
    }

    public DbSet<OrderRecord> Orders => Set<OrderRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrderRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PayloadJson)
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();
        });
    }
}
