using Microsoft.Extensions.Logging;
using OrderService.Application.Abstractions;

namespace OrderService.Infrastructure.Services;

/// <summary>
/// Giả lập hệ thống hóa đơn bên thứ ba.
/// </summary>
public sealed class SimulatedInvoiceSystemClient(ILogger<SimulatedInvoiceSystemClient> logger) : IInvoiceSystemClient
{
    public Task<string> IssueInvoiceAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var invoiceNo = $"INV-SIM-{DateTime.UtcNow:yyyyMMdd}-{orderId.ToString("N")[..8].ToUpperInvariant()}";
        logger.LogInformation(
            "[InvoiceSystem] Issue simulated: orderId={OrderId} invoiceNo={InvoiceNo} taxCode=DEMO-TAX",
            orderId,
            invoiceNo);
        return Task.FromResult(invoiceNo);
    }
}
