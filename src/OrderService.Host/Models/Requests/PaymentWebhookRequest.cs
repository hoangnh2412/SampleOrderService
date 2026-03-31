using System.ComponentModel.DataAnnotations;

namespace OrderService.Host.Models.Requests;

public sealed class PaymentWebhookRequest
{
    [Required(ErrorMessage = "orderId is required.")]
    public Guid OrderId { get; set; }

    [Required(ErrorMessage = "transactionId is required.")]
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>Ví dụ: <c>succeeded</c>, <c>failed</c>, <c>canceled</c>.</summary>
    [Required(ErrorMessage = "status is required.")]
    public string Status { get; set; } = string.Empty;
}
