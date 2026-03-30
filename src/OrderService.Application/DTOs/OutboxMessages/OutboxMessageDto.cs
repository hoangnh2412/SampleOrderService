namespace OrderService.Application.DTOs.OutboxMessages;

/// <summary>
/// Một message đưa vào broker nội bộ sau khi đọc từ outbox (SAD — BrokerPublisher → MessageBroker).
/// </summary>
public sealed record OutboxMessageDto(
    Guid MessageId,
    Guid EntityId,
    string PayloadJson,
    DateTime CreatedAtUtc);
