namespace OrderService.Application.Configuration;

/// <summary>
/// Cấu hình xác thực callback thanh toán (header <c>X-Webhook-Secret</c> khớp <see cref="SharedSecret"/>).
/// </summary>
public sealed class PaymentWebhookOptions
{
    public const string SectionName = "PaymentWebhook";

    public string SharedSecret { get; set; } = string.Empty;
}
