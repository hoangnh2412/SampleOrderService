namespace OrderService.Host.Models.Responses;

public sealed class CheckoutDataResponse
{
    public Guid OrderId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? PaymentTransactionId { get; init; }
    public DateTime? PaymentAt { get; init; }
    public string PaymentBy { get; init; } = string.Empty;
    public string PaymentByName { get; init; } = string.Empty;
}
