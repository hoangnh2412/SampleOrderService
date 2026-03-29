using System.ComponentModel;

namespace OrderService.Domain.Shared.Enums;

/// <summary>
/// Trạng thái xử lý message/outbox (ERD OrderMessage.Status).
/// </summary>
public enum OrderMessageStatus
{
    [Description("Mới tạo.")]
    New = 0,

    [Description("Đang chờ xử lý.")]
    Pending = 1,

    [Description("Đã xử lý.")]
    Processed = 2,

    [Description("Xử lý thất bại.")]
    Failed = 3
}
