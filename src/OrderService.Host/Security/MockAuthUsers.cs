namespace OrderService.Host.Security;

/// <summary>
/// Tài khoản giả lập cố định (không dùng DB). Chỉ phục vụ dev/demo.
/// </summary>
internal static class MockAuthUsers
{
    private static readonly MockAuthAccount[] Accounts =
    [
        new MockAuthAccount(Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), "admin", "admin", "Administrator"),
        new MockAuthAccount(Guid.Parse("11111111-2222-3333-4444-555555555555"), "demo", "demo", "Demo User"),
    ];

    /// <summary>Lấy user theo username (không kiểm tra mật khẩu).</summary>
    public static MockAuthAccount? GetByUsername(string username)
    {
        foreach (var user in Accounts)
        {
            if (string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase))
                return user;
        }

        return null;
    }

    /// <summary>Kiểm tra mật khẩu (quyền đăng nhập) sau khi đã có user.</summary>
    public static bool IsPasswordValid(MockAuthAccount user, string password) => user.Password == password;
}
