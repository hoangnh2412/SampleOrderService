namespace OrderService.Application.Abstractions;

/// <summary>
/// Hệ thống hóa đơn bên thứ ba (SAD — Invoice worker).
/// </summary>
public interface IInvoiceSystemClient
{
    Task<string> IssueInvoiceAsync(Guid orderId, CancellationToken cancellationToken = default);
}
