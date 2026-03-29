using OrderService.Domain.Shared.Enums;

namespace OrderService.Domain.Entities;

/// <summary>
/// Lịch sử thanh toán theo thực thể (ERD PaymentHistory; EntityId + EntityType).
/// </summary>
public sealed class PaymentHistory
{
    public Guid Id { get; private set; }
    public Guid EntityId { get; private set; }
    public PaymentHistoryEntityType EntityType { get; private set; }
    public decimal Amount { get; private set; }
    public string TransactionId { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public string? CreatedByName { get; private set; }

    private PaymentHistory()
    {
    }

    public static PaymentHistory CreateForOrder(
        Guid orderId,
        decimal amount,
        string transactionId,
        DateTime createdAtUtc,
        Guid? createdBy,
        string? createdByName)
    {
        string? normalizedCreatedByName = null;
        if (!string.IsNullOrWhiteSpace(createdByName))
            normalizedCreatedByName = createdByName.Trim();

        return new PaymentHistory
        {
            Id = Guid.NewGuid(),
            EntityId = orderId,
            EntityType = PaymentHistoryEntityType.Order,
            Amount = amount,
            TransactionId = transactionId,
            CreatedAtUtc = createdAtUtc,
            CreatedBy = createdBy,
            CreatedByName = normalizedCreatedByName
        };
    }
}
