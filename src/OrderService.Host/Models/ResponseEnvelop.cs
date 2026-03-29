namespace OrderService.Host.Models;

/// <summary>
/// Khung phản hồi API chung (<c>code</c>, <c>message</c>, <c>errors</c>) — dùng cho lỗi; không có <c>data</c> (OpenAPI cho phép bỏ qua).
/// </summary>
public class ResponseEnvelop
{
    public required string Code { get; init; }
    public required string Message { get; init; }
    public IReadOnlyList<FieldErrorDto> Errors { get; init; } = [];
}

/// <summary>
/// Phản hồi thành công với payload <c>data</c> kiểu <typeparamref name="T"/>.
/// </summary>
public sealed class ResponseEnvelop<T> : ResponseEnvelop
{
    public required T Data { get; init; }
}

public sealed record FieldErrorDto(string FieldName, string Message);
