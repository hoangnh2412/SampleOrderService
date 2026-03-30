using Microsoft.Extensions.Logging;
using OrderService.Application.Abstractions;

namespace OrderService.Infrastructure.Services;

/// <summary>
/// Giả lập Payment Gateway — dữ liệu và giao dịch demo.
/// </summary>
public sealed class SimulatedPaymentGatewayClient(ILogger<SimulatedPaymentGatewayClient> logger) : IPaymentGatewayClient
{
    public Task PrePaymentAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var gatewayTxnId = $"GW-SIM-{Guid.NewGuid():N}";
        logger.LogInformation(
            "[PaymentSystem] Pre-payment simulated: orderId={OrderId} gatewayTxnId={Txn}",
            orderId,
            gatewayTxnId);
        return Task.FromResult(gatewayTxnId);
    }

    public Task PostPaymentAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var gatewayTxnId = $"GW-SIM-{Guid.NewGuid():N}";
        logger.LogInformation(
            "[PaymentSystem] Capture simulated: orderId={OrderId} amountRef=from_order total gatewayTxnId={Txn}",
            orderId,
            gatewayTxnId);
        return Task.CompletedTask;
    }
}
