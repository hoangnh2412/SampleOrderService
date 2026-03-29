using OrderService.Domain.Shared.Enums;

namespace OrderService.Domain.Shared.Extensions;

/// <summary>
/// Định dạng <see cref="ErrorCodes"/> thành chuỗi gửi trong envelope JSON (<c>code</c>).
/// </summary>
public static class ErrorCodeExtension
{
    /// <summary>Trả về mã dạng <c>"4220"</c> (4 chữ số).</summary>
    public static string ToWireCode(this ErrorCodes code) => ((int)code).ToString("D4");
}
