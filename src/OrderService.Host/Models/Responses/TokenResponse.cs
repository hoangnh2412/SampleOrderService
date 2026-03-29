namespace OrderService.Host.Models.Responses;

/// <summary>
/// Kết quả cấp JWT (Bearer).
/// </summary>
public sealed class TokenResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";
    public DateTime ExpiresAtUtc { get; init; }
}
