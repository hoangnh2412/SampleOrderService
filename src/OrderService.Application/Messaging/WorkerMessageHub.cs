using System.Threading.Channels;
using OrderService.Application.DTOs.OutboxMessages;

namespace OrderService.Application.Messaging;

/// <summary>
/// Hàng đợi in-memory theo loại worker (SAD — định tuyến từ outbox publish).
/// </summary>
public sealed class WorkerMessageHub
{
    public Channel<OutboxMessageDto> Payment { get; } = Channel.CreateUnbounded<OutboxMessageDto>();
    public Channel<OutboxMessageDto> Invoice { get; } = Channel.CreateUnbounded<OutboxMessageDto>();
    public Channel<OutboxMessageDto> Notification { get; } = Channel.CreateUnbounded<OutboxMessageDto>();
    public Channel<OutboxMessageDto> Production { get; } = Channel.CreateUnbounded<OutboxMessageDto>();
}
