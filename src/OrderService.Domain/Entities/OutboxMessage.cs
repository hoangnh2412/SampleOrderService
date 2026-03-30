using OrderService.Domain.Shared.Enums;

namespace OrderService.Domain.Entities;

/// <summary>
/// Message/outbox gắn thực thể (ERD OrderMessage). Relay quét <see cref="OrderMessageStatus.New"/> để đẩy broker (SAD).
/// </summary>
public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public Guid EntityId { get; private set; }
    public string Payload { get; private set; } = string.Empty;
    public OrderMessageStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private OutboxMessage()
    {
    }

    /// <summary>
    /// Bản ghi outbox cùng transaction với aggregate; chưa gửi message broker.
    /// </summary>
    public static OutboxMessage Create(Guid entityId, string payload, DateTime createdAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EntityId = entityId,
            Payload = payload,
            Status = OrderMessageStatus.New,
            CreatedAtUtc = createdAtUtc
        };
    }

    /// <summary>Relay đã gửi thành công lên message broker (SAD).</summary>
    public void MarkAsProcessed() => Status = OrderMessageStatus.Processed;

    /// <summary>Gửi broker thất bại; worker có thể retry sau (SAD).</summary>
    public void MarkAsFailed() => Status = OrderMessageStatus.Failed;
}
