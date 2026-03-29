namespace OrderService.Host.Models.Responses;

public sealed class OrderDataResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateOnly Date { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public Guid CreatedBy { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public decimal TotalDiscountAmount { get; init; }
    public decimal TotalPaymentAmount { get; init; }
    public List<OrderLineItemResponse> Items { get; init; } = [];
}
