namespace OrderService.Application.Commands.Orders;

/// <summary>
/// Callback từ cổng thanh toán: cập nhật trạng thái đơn (thành công / thất bại).
/// </summary>
public sealed record PaymentWebhookCommand(Guid OrderId, string TransactionId, string Status);
