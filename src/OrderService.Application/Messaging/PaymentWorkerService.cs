using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs.OutboxMessages;
using OrderService.Domain.Events;

namespace OrderService.Application.Messaging;

/// <summary>
/// Xử lý <see cref="OrderPaymentProcessing"/> — gọi Payment Gateway (logic nghiệp vụ tách khỏi Host).
/// </summary>
public sealed class PaymentWorkerService(
    WorkerMessageHub hub,
    IPaymentGatewayClient paymentGateway,
    ILogger<PaymentWorkerService> logger)
{
    public Task ExecuteAsync(CancellationToken stoppingToken) => ConsumeAsync(hub.Payment.Reader, stoppingToken);

    private async Task ConsumeAsync(ChannelReader<OutboxMessageDto> reader, CancellationToken stoppingToken)
    {
        await foreach (var message in reader.ReadAllAsync(stoppingToken).ConfigureAwait(false))
        {
            var orderId = OutboxPayloadHelper.TryGetEntityIdFromInnerPayload(message.PayloadJson) ?? message.EntityId;
            try
            {
                await paymentGateway.PostPaymentAsync(orderId, stoppingToken).ConfigureAwait(false);
                logger.LogInformation("Payment worker completed for order {OrderId} message {MessageId}.", orderId, message.MessageId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Payment worker failed for order {OrderId} message {MessageId}.", orderId, message.MessageId);
            }
        }
    }
}
