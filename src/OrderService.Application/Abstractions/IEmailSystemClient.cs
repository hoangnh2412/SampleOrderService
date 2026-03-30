namespace OrderService.Application.Abstractions;

/// <summary>
/// Email / thông báo bên thứ ba (SAD — Notification worker).
/// </summary>
public interface IEmailSystemClient
{
    Task SendPaymentConfirmationAsync(Guid orderId, CancellationToken cancellationToken = default);
}
