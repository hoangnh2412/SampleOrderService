using System.ComponentModel.DataAnnotations;

namespace OrderService.Host.Models.Requests;

/// <summary>
/// Payload tạo đơn hàng mới (POST /api/v1/orders).
/// </summary>
public sealed class CreateOrderRequest
{
    /// <summary>
    /// Ngày đơn theo lịch (UTC); bắt buộc trong payload JSON.
    /// </summary>
    [Required(ErrorMessage = "Ngày đơn là bắt buộc.")]
    public required DateTime Date { get; set; }

    /// <summary>
    /// Tổng tiền hàng trước chiết khấu (tổng các dòng).
    /// </summary>
    [Required(ErrorMessage = "Tổng tiền hàng là bắt buộc.")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Tổng số tiền được chiết khấu trên toàn đơn.
    /// </summary>
    [Required(ErrorMessage = "Tổng chiết khấu là bắt buộc.")]
    public decimal TotalDiscountAmount { get; set; }

    /// <summary>
    /// Tổng số tiền khách phải thanh toán sau chiết khấu.
    /// </summary>
    [Required(ErrorMessage = "Tổng thanh toán là bắt buộc.")]
    public decimal TotalPaymentAmount { get; set; }

    /// <summary>
    /// Danh sách dòng hàng trong đơn; phải có ít nhất một dòng.
    /// </summary>
    [Required(ErrorMessage = "Danh sách dòng hàng là bắt buộc.")]
    [MinLength(1, ErrorMessage = "Phải có ít nhất một dòng hàng.")]
    public List<OrderLineItemRequest> Items { get; set; } = [];
}
