using OrderService.Application.Messaging;

namespace OrderService.Host.Workers;

/// <summary>
/// Hosted service (Host = presentation); logic vòng lặp ở <see cref="OutboxMessageService"/> (Infrastructure).
/// </summary>
public sealed class PublisherWorker(OutboxMessageService service) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => service.HandleAsync(stoppingToken);
}
