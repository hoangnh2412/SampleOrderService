namespace OrderService.Application.Abstractions;

/// <summary>
/// Cổng thanh toán bên thứ ba (SAD — Payment worker).
/// </summary>
public interface IPaymentGatewayClient
{

    Task PrePaymentAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task PostPaymentAsync(Guid orderId, CancellationToken cancellationToken = default);
}
