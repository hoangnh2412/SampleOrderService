using OrderService.Domain.Shared.Enums;

namespace OrderService.Application.Dtos.Orders;

/// <summary>
/// DTO kết quả use case tạo đơn (tách khỏi read model / persistence).
/// </summary>
public sealed class CreateOrderResultDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public OrderStatus Status { get; init; }
    public PaymentStatus PaymentStatus { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public Guid CreatedBy { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    public DateTime? PaymentAt { get; init; }
    public Guid? PaymentBy { get; init; }
    public string? PaymentByName { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal TotalDiscountAmount { get; init; }
    public decimal TotalPaymentAmount { get; init; }
    public IReadOnlyList<CreateOrderLineDto> Lines { get; init; } = [];
}
