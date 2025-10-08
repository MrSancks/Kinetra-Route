using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationsService.Domain.Enums;
using NotificationsService.Infrastructure.Data;

namespace NotificationsService.BackgroundTasks;

public class OutboxProcessor : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando procesador de outbox de notificaciones");

        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxAsync(stoppingToken);
            await Task.Delay(PollingInterval, stoppingToken);
        }
    }

    private async Task ProcessOutboxAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var messages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(20)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
        {
            return;
        }

        foreach (var message in messages)
        {
            try
            {
                using var payload = JsonDocument.Parse(message.Payload);
                var notificationId = payload.RootElement.TryGetProperty("NotificationId", out var notificationIdElement)
                    && Guid.TryParse(notificationIdElement.GetString(), out var parsedId)
                    ? parsedId
                    : Guid.Empty;

                if (notificationId != Guid.Empty)
                {
                    var notification = await dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId, cancellationToken);
                    if (notification is not null)
                    {
                        notification.Status = NotificationStatus.Sent;
                        notification.SentAt = DateTimeOffset.UtcNow;
                        notification.DeliveryAttempts += 1;
                    }
                }

                _logger.LogInformation("Publicando evento {EventType} para notificación {NotificationId}", message.EventType, notificationId);
                message.Error = null;
                message.ProcessedOn = DateTimeOffset.UtcNow;
            }
            catch (Exception ex)
            {
                message.Error = ex.Message;
                _logger.LogError(ex, "Error al procesar mensaje de outbox {OutboxId}", message.Id);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
