using OrderService.Application.Messaging;

namespace OrderService.Host.Workers;

public sealed class NotificationWorker(NotificationWorkerService service) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => service.ExecuteAsync(stoppingToken);
}
