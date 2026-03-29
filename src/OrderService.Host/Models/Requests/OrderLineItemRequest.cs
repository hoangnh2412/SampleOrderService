using System.ComponentModel.DataAnnotations;

namespace OrderService.Host.Models.Requests;

/// <summary>
/// Một dòng hàng trong yêu cầu tạo đơn.
/// </summary>
public sealed class OrderLineItemRequest
{
    /// <summary>
    /// Định danh sản phẩm (SKU hoặc mã nội bộ).
    /// </summary>
    [Required]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Tên sản phẩm hiển thị trên đơn.
    /// </summary>
    [Required]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Đơn giá một đơn vị; không âm.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Số lượng mua; tối thiểu 1.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    /// <summary>
    /// Thành tiền của dòng (thường là đơn giá × số lượng, trước chiết khấu dòng).
    /// </summary>
    [Required]
    public decimal Amount { get; set; }

    /// <summary>
    /// Số tiền chiết khấu áp dụng cho dòng này; không âm.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Số tiền thanh toán cho dòng sau chiết khấu.
    /// </summary>
    [Required]
    public decimal PaymentAmount { get; set; }
}
