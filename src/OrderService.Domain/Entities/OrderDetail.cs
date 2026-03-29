namespace OrderService.Domain.Entities;

/// <summary>
/// Chi tiết dòng trên đơn hàng (ERD OrderDetail).
/// </summary>
public sealed class OrderDetail
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string ProductId { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal Amount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal PaymentAmount { get; private set; }

    private OrderDetail()
    {
    }

    internal static OrderDetail Create(Guid orderId, OrderLineSpec item)
    {
        return new OrderDetail
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = item.ProductId.Trim(),
            ProductName = item.ProductName.Trim(),
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            Amount = item.Amount,
            DiscountAmount = item.DiscountAmount,
            PaymentAmount = item.PaymentAmount
        };
    }
}
