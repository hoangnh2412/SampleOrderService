namespace OrderService.Application.Dtos.Orders;

/// <summary>
/// DTO kết quả use case checkout đơn.
/// </summary>
public sealed class CheckoutResultDto
{
    public Guid OrderId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? PaymentTransactionId { get; init; }
    public DateTime? PaymentAt { get; init; }
    public string PaymentBy { get; init; } = string.Empty;
    public string PaymentByName { get; init; } = string.Empty;
}
