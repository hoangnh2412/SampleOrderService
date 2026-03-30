using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs.OutboxMessages;

namespace OrderService.Application.Messaging;

/// <summary>
/// Xử lý sự kiện thanh toán xong (OrderPaid) — gửi email xác nhận (logic tách khỏi Host).
/// </summary>
public sealed class NotificationWorkerService(
    WorkerMessageHub hub,
    IEmailSystemClient emailSystem,
    ILogger<NotificationWorkerService> logger)
{
    public Task ExecuteAsync(CancellationToken stoppingToken) => ConsumeAsync(hub.Notification.Reader, stoppingToken);

    private async Task ConsumeAsync(ChannelReader<OutboxMessageDto> reader, CancellationToken stoppingToken)
    {
        await foreach (var message in reader.ReadAllAsync(stoppingToken).ConfigureAwait(false))
        {
            var orderId = OutboxPayloadHelper.TryGetEntityIdFromInnerPayload(message.PayloadJson) ?? message.EntityId;
            try
            {
                await emailSystem.SendPaymentConfirmationAsync(orderId, stoppingToken).ConfigureAwait(false);
                logger.LogInformation("Notification worker sent email for order {OrderId} message {MessageId}.", orderId, message.MessageId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Notification worker failed for order {OrderId} message {MessageId}.", orderId, message.MessageId);
            }
        }
    }
}
