using OrderService.Domain.Entities;

namespace OrderService.Domain.Repositories;

/// <summary>
/// Port đọc/ghi outbox cho <see cref="BrokerPublisher"/> (Infrastructure triển khai bằng EF).
/// </summary>
public interface IOutboxMessageRepository
{
    Task<IReadOnlyList<OutboxMessage>> FetchMessagesAsync(int batchSize, CancellationToken cancellationToken = default);

    Task MarkProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);

    Task MarkFailedAsync(Guid messageId, CancellationToken cancellationToken = default);
}
