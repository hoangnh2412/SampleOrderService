using System.ComponentModel;

namespace OrderService.Domain.Shared.Enums;

public enum PaymentStatus
{
    [Description("Chưa thanh toán.")]
    Unpaid = 0,

    [Description("Đang chờ thanh toán.")]
    Pending = 1,

    [Description("Đã thanh toán một phần.")]
    Partial = 2,

    [Description("Đã thanh toán đủ.")]
    Paid = 3,

    [Description("Thanh toán thất bại.")]
    Failed = 4,

    [Description("Đã hoàn tiền.")]
    Refunded = 5
}
