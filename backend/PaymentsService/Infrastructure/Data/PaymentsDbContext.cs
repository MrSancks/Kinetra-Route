using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure.Data.Models;

namespace PaymentsService.Infrastructure.Data;

public class PaymentsDbContext : DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<PaymentComputationRecord> Computations => Set<PaymentComputationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PaymentComputationRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ComputationType)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.RequestJson)
                .IsRequired();
            entity.Property(e => e.ResponseJson)
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });
    }
}
