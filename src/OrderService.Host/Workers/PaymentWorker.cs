using OrderService.Application.Messaging;

namespace OrderService.Host.Workers;

/// <summary>
/// Hosted service (Host); vòng lặp consumer ở <see cref="PaymentWorkerService"/> (Application).
/// </summary>
public sealed class PaymentWorker(PaymentWorkerService service) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => service.ExecuteAsync(stoppingToken);
}
