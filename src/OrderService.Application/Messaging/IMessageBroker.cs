using OrderService.Application.DTOs.OutboxMessages;

namespace OrderService.Application.Messaging;

/// <summary>
/// Message broker demo (in-memory, định tuyến theo event — <see cref="RoutedInMemoryMessageBroker"/>).
/// </summary>
public interface IMessageBroker
{
    ValueTask PublishAsync(OutboxMessageDto message, CancellationToken cancellationToken = default);
}
