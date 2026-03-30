namespace OrderService.Application.Abstractions;

/// <summary>
/// Hệ thống Production nội bộ (SAD — Production worker).
/// </summary>
public interface IProductionSystemClient
{
    Task SendOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
}
