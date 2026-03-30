using OrderService.Domain.Events;
using OrderService.Domain.Shared.Enums;
using OrderService.Domain.Shared.Extensions;

namespace OrderService.Domain.Entities;

public sealed class Order : BaseAggregateRoot
{
    public Guid Id { get; private set; }

    /// <summary>Giá trị header <c>IdempotentId</c>; chuỗi rỗng nghĩa là không dùng idempotent.</summary>
    public string IdempotentId { get; private set; } = string.Empty;

    /// <summary>Mã đơn hàng (theo ERD sad.md); OpenAPI gọi là <c>name</c> khi hiển thị/tra cứu.</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Ngày tạo đơn hàng (nghiệp vụ).</summary>
    public DateTime OrderDate { get; private set; }

    public decimal TotalAmount { get; private set; }
    public decimal TotalDiscountAmount { get; private set; }
    public decimal TotalPaymentAmount { get; private set; }

    /// <summary>Trạng thái xử lý đơn hàng.</summary>
    public OrderStatus Status { get; private set; }

    /// <summary>Trạng thái thanh toán đơn hàng.</summary>
    public PaymentStatus PaymentStatus { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public string CreatedByName { get; private set; } = string.Empty;

    public DateTime? PaymentAt { get; private set; }
    public Guid? PaymentBy { get; private set; }
    public string? PaymentByName { get; private set; }

    public List<OrderDetail> Details { get; private set; } = [];

    private Order()
    {
    }

    /// <summary>
    /// Factory đơn mới kèm dòng chi tiết. Tổng tiền phải khớp dòng (kiểm tra ở Application trước khi gọi).
    /// </summary>
    public Order(
        DateTime createdAtUtc,
        DateTime orderDate,
        decimal totalAmount,
        decimal totalDiscountAmount,
        decimal totalPaymentAmount,
        IReadOnlyList<OrderLineSpec> lineSpecs,
        Guid createdBy,
        string createdByName,
        string idempotentId)
    {
        if (lineSpecs.Count == 0)
            throw new ArgumentException("At least one line item is required.", nameof(lineSpecs));

        var order = new Order
        {
            Id = Guid.NewGuid(),
            IdempotentId = idempotentId.Trim(),
            Code = StringExtension.NewOrderCode(orderDate),
            OrderDate = orderDate,
            TotalAmount = totalAmount,
            TotalDiscountAmount = totalDiscountAmount,
            TotalPaymentAmount = totalPaymentAmount,
            Status = OrderStatus.Draft,
            PaymentStatus = PaymentStatus.Unpaid,
            CreatedAtUtc = createdAtUtc,
            CreatedBy = createdBy,
            CreatedByName = createdByName.Trim(),
            PaymentAt = null,
            PaymentBy = null,
            PaymentByName = null,
            Details = []
        };

        foreach (var item in lineSpecs)
            order.Details.Add(OrderDetail.Create(order.Id, item));

        var items = order.Details.Select(d => new OrderCreatedLineItem(
            d.Id,
            d.ProductId,
            d.ProductName,
            d.UnitPrice,
            d.Quantity,
            d.Amount,
            d.DiscountAmount,
            d.PaymentAmount)).ToList();

        AddDomainEvent(new OrderCreated(
            order.Id,
            order.Code,
            OrderStatus.Draft,
            PaymentStatus.Unpaid,
            createdAtUtc,
            items));
    }

    public void ProcessPayment()
    {
        if (PaymentStatus == PaymentStatus.Paid)
            throw new InvalidOperationException("Order is already paid.");

        PaymentStatus = PaymentStatus.Pending;

        AddDomainEvent(new OrderPaymentProcessing(Id, CreatedAtUtc));
    }

    /// <summary>
    /// Xác nhận thanh toán đồng bộ (mẫu theo OpenAPI checkout 200).
    /// </summary>
    public void CompletePayment(
        Guid paymentBy,
        string paymentByName,
        DateTime paymentAtUtc)
    {
        if (PaymentStatus == PaymentStatus.Paid)
            throw new InvalidOperationException("Order is already paid.");

        PaymentStatus = PaymentStatus.Paid;
        Status = OrderStatus.Paid;
        PaymentAt = paymentAtUtc;

        if (string.IsNullOrWhiteSpace(paymentByName))
            PaymentByName = paymentByName.Trim();
        else
            PaymentByName = paymentByName.Trim();

        PaymentBy = paymentBy;

        AddDomainEvent(new OrderPaid(Id, paymentAtUtc));
    }
}
