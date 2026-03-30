using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.OutboxMessages;
using OrderService.Domain.Events;

namespace OrderService.Application.Messaging;

/// <summary>
/// Broker in-memory: đẩy message vào hàng đợi worker theo <c>eventType</c> (SAD).
/// </summary>
public sealed class RoutedInMemoryMessageBroker(WorkerMessageHub hub, ILogger<RoutedInMemoryMessageBroker> logger)
    : IMessageBroker
{
    public async ValueTask PublishAsync(OutboxMessageDto message, CancellationToken cancellationToken = default)
    {
        var eventType = OutboxPayloadHelper.TryGetEventType(message.PayloadJson);

        switch (eventType)
        {
            case nameof(OrderPaymentProcessing):
                await hub.Payment.Writer.WriteAsync(message, cancellationToken).ConfigureAwait(false);
                return;
            case nameof(OrderPaid):
                await hub.Invoice.Writer.WriteAsync(message, cancellationToken).ConfigureAwait(false);
                await hub.Notification.Writer.WriteAsync(message, cancellationToken).ConfigureAwait(false);
                await hub.Production.Writer.WriteAsync(message, cancellationToken).ConfigureAwait(false);
                return;
            default:
                if (eventType is not null)
                    logger.LogDebug("Outbox message {MessageId} eventType {EventType} has no worker queue; skipped.", message.MessageId, eventType);
                return;
        }
    }
}
