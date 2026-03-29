using System.ComponentModel;

namespace OrderService.Domain.Shared.Enums;

public enum OrderStatus
{
    [Description("Đơn mới tạo, chưa thanh toán.")]
    Draft = 0,

    [Description("Chờ khách thanh toán.")]
    PendingPayment = 1,

    [Description("Đã thanh toán.")]
    Paid = 2,

    [Description("Đã gửi sang sản xuất / xử lý tiếp.")]
    SentToProduction = 3,

    [Description("Đơn đã hủy.")]
    Cancelled = 4
}
