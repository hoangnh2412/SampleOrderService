namespace OrderService.Host.Security;

/// <summary>
/// Tài khoản mock nội bộ (chỉ Host); không serialize ra API.
/// </summary>
public sealed record MockAuthAccount(Guid Id, string Username, string Password, string DisplayName);
