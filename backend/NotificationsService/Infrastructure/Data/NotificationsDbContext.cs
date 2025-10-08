using Microsoft.EntityFrameworkCore;
using NotificationsService.Domain.Entities;

namespace NotificationsService.Infrastructure.Data;

public class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RecipientId).IsRequired();
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Message).IsRequired();
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventType).IsRequired();
            entity.Property(e => e.Payload).IsRequired();
        });
    }
}
