using OrderService.Application.Messaging;

namespace OrderService.Host.Workers;

public sealed class ProductionWorker(ProductionWorkerService service) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => service.ExecuteAsync(stoppingToken);
}
