namespace OrderService.Domain.Entities;

/// <summary>
/// Thông tin một dòng chi tiết khi tạo đơn (khớp OpenAPI OrderLineItemRequest).
/// </summary>
public readonly record struct OrderLineSpec(
    string ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Amount,
    decimal DiscountAmount,
    decimal PaymentAmount);
