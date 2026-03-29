using System.Globalization;

namespace OrderService.Domain.Shared.Extensions;

/// <summary>
/// Tiện ích chuỗi dùng chung (mã đơn, định dạng…).
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// Sinh mã đơn: <c>MMdd</c> theo <paramref name="orderDate"/> (lịch UTC) + 6 ký tự ngẫu nhiên (0–9, A–Z).
    /// </summary>
    public static string NewOrderCode(DateTime orderDate)
    {
        var utc = orderDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(orderDate, DateTimeKind.Utc)
            : orderDate.ToUniversalTime();
        var prefix = utc.ToString("MMdd", CultureInfo.InvariantCulture);
        Span<char> suffix = stackalloc char[6];
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (var i = 0; i < 6; i++)
            suffix[i] = alphabet[Random.Shared.Next(alphabet.Length)];
        return string.Concat(prefix, new string(suffix));
    }
}
