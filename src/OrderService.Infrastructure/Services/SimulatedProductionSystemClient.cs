using Microsoft.Extensions.Logging;
using OrderService.Application.Abstractions;

namespace OrderService.Infrastructure.Services;

/// <summary>
/// Giả lập hệ thống Production nội bộ.
/// </summary>
public sealed class SimulatedProductionSystemClient(ILogger<SimulatedProductionSystemClient> logger) : IProductionSystemClient
{
    public Task SendOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var workOrderId = $"WO-SIM-{Guid.NewGuid():N}";
        logger.LogInformation(
            "[ProductionSystem] Enqueue simulated: orderId={OrderId} workOrderId={WorkOrderId} line=default",
            orderId,
            workOrderId);
        return Task.CompletedTask;
    }
}
