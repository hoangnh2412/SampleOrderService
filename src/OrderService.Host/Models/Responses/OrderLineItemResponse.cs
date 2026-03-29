namespace OrderService.Host.Models.Responses;

public sealed class OrderLineItemResponse
{
    public Guid LineId { get; init; }
    public string ProductId { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal Amount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal PaymentAmount { get; init; }
}
