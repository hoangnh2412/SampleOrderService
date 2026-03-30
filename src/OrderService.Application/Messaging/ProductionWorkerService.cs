using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs.OutboxMessages;

namespace OrderService.Application.Messaging;

/// <summary>
/// Xử lý sự kiện thanh toán xong (OrderPaid) — đẩy đơn sang Production (logic tách khỏi Host).
/// </summary>
public sealed class ProductionWorkerService(
    WorkerMessageHub hub,
    IProductionSystemClient productionSystem,
    ILogger<ProductionWorkerService> logger)
{
    public Task ExecuteAsync(CancellationToken stoppingToken) => ConsumeAsync(hub.Production.Reader, stoppingToken);

    private async Task ConsumeAsync(ChannelReader<OutboxMessageDto> reader, CancellationToken stoppingToken)
    {
        await foreach (var message in reader.ReadAllAsync(stoppingToken).ConfigureAwait(false))
        {
            var orderId = OutboxPayloadHelper.TryGetEntityIdFromInnerPayload(message.PayloadJson) ?? message.EntityId;
            try
            {
                await productionSystem.SendOrderAsync(orderId, stoppingToken).ConfigureAwait(false);
                logger.LogInformation("Production worker enqueued order {OrderId} message {MessageId}.", orderId, message.MessageId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Production worker failed for order {OrderId} message {MessageId}.", orderId, message.MessageId);
            }
        }
    }
}
