using Microsoft.Extensions.Logging;
using OrderService.Application.Abstractions;

namespace OrderService.Infrastructure.Services;

/// <summary>
/// Giả lập máy chủ email / thông báo.
/// </summary>
public sealed class SimulatedEmailSystemClient(ILogger<SimulatedEmailSystemClient> logger) : IEmailSystemClient
{
    public Task SendPaymentConfirmationAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var to = $"customer+{orderId:N}@demo.local";
        var messageId = $"MSG-SIM-{Guid.NewGuid():N}";
        logger.LogInformation(
            "[EmailSystem] Send simulated: to={To} subject=Payment confirmed orderId={OrderId} messageId={MessageId}",
            to,
            orderId,
            messageId);
        return Task.CompletedTask;
    }
}
