using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.OutboxMessages;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Messaging;

/// <summary>
/// Đọc bản ghi outbox trạng thái New và publish lên <see cref="IMessageBroker"/> (SAD — OrderAPI / BrokerPublisher).
/// </summary>
public sealed class BrokerPublisher(
    IOutboxMessageRepository outbox,
    IMessageBroker broker,
    ILogger<BrokerPublisher> logger)
{
    /// <summary>
    /// Xử lý tối đa <paramref name="batchSize"/> message; trả về số message đã publish thành công.
    /// </summary>
    public async Task<int> PublishAsync(int batchSize = 50, CancellationToken cancellationToken = default)
    {
        var batch = await outbox.FetchMessagesAsync(batchSize, cancellationToken);
        var published = 0;

        foreach (var row in batch)
        {
            try
            {
                var envelope = new OutboxMessageDto(row.Id, row.EntityId, row.Payload, row.CreatedAtUtc);
                await broker.PublishAsync(envelope, cancellationToken);
                await outbox.MarkProcessedAsync(row.Id, cancellationToken);
                published++;
                logger.LogInformation("Outbox message {MessageId} published successfully.", row.Id);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Outbox message {MessageId} publish failed; marking Failed.", row.Id);
                await outbox.MarkFailedAsync(row.Id, cancellationToken);
            }
        }

        return published;
    }
}
