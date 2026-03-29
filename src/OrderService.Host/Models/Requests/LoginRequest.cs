using System.ComponentModel.DataAnnotations;

namespace OrderService.Host.Models.Requests;

/// <summary>
/// Thông tin đăng nhập để lấy access token (môi trường dev — user/password giả lập).
/// </summary>
public sealed class LoginRequest
{
    /// <summary>Tên đăng nhập.</summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>Mật khẩu.</summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
