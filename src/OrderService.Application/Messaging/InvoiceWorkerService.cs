using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs.OutboxMessages;

namespace OrderService.Application.Messaging;

/// <summary>
/// Xử lý sự kiện thanh toán xong (OrderPaid) — xuất hóa đơn (logic tách khỏi Host).
/// </summary>
public sealed class InvoiceWorkerService(
    WorkerMessageHub hub,
    IInvoiceSystemClient invoiceSystem,
    ILogger<InvoiceWorkerService> logger)
{
    public Task ExecuteAsync(CancellationToken stoppingToken) => ConsumeAsync(hub.Invoice.Reader, stoppingToken);

    private async Task ConsumeAsync(ChannelReader<OutboxMessageDto> reader, CancellationToken stoppingToken)
    {
        await foreach (var message in reader.ReadAllAsync(stoppingToken))
        {
            var orderId = OutboxPayloadHelper.TryGetEntityIdFromInnerPayload(message.PayloadJson) ?? message.EntityId;
            try
            {
                var invoiceNo = await invoiceSystem.IssueInvoiceAsync(orderId, stoppingToken);
                logger.LogInformation(
                    "Invoice worker issued {InvoiceNo} for order {OrderId} message {MessageId}.",
                    invoiceNo,
                    orderId,
                    message.MessageId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Invoice worker failed for order {OrderId} message {MessageId}.", orderId, message.MessageId);
            }
        }
    }
}
