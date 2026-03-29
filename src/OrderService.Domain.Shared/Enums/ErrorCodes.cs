namespace OrderService.Domain.Shared.Enums;

/// <summary>
/// Mã lỗi API (chuỗi 4 chữ số trong JSON, khớp quy ước OpenAPI / docs).
/// Dùng chung giữa Application, Host và các client contract.
/// </summary>
public enum ErrorCodes
{
    /// <summary>4000 — JSON / body không hợp lệ.</summary>
    InvalidRequestBody = 4000,

    /// <summary>4001 — Query hoặc tham số không hợp lệ.</summary>
    InvalidQueryParameters = 4001,

    /// <summary>4010 — Thiếu hoặc sai Bearer token.</summary>
    Unauthorized = 4010,

    /// <summary>4030 — Không đủ quyền thực hiện thao tác.</summary>
    Forbidden = 4030,

    /// <summary>4040 — Không tìm thấy đơn hàng.</summary>
    OrderNotFound = 4040,

    /// <summary>4090 — Đơn đã thanh toán (checkout trùng không idempotent).</summary>
    OrderAlreadyPaid = 4090,

    /// <summary>4220 — Vi phạm nghiệp vụ / tổng tiền không khớp dòng.</summary>
    OrderTotalsMismatch = 4220,

    /// <summary>5000 — Lỗi máy chủ không lường trước.</summary>
    InternalServerError = 5000
}
