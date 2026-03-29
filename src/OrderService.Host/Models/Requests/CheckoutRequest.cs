using System.ComponentModel.DataAnnotations;

namespace OrderService.Host.Models.Requests;

public sealed class CheckoutRequest
{
    [Required]
    public string PaymentServiceId { get; set; } = string.Empty;

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;
}
