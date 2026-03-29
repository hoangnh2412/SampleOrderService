namespace OrderService.Application.Exceptions;

/// <summary>
/// Lỗi nghiệp vụ / hợp đồng API map sang HTTP response. Mỗi mã HTTP có lớp con riêng (<see cref="ApiBadRequestException"/>, …).
/// </summary>
public abstract class ApiBusinessException : Exception
{
    protected ApiBusinessException(ErrorCodes code, string message, IReadOnlyList<FieldError>? errors = null)
        : base(message)
    {
        Code = code;
        Errors = errors ?? [];
    }

    public ErrorCodes Code { get; }

    /// <summary>Mã 4 chữ số gửi ra client (xem <see cref="ErrorCodeExtension"/>).</summary>
    public string ErrorCode => Code.ToWireCode();

    /// <summary>Status HTTP tương ứng (do lớp con quy định).</summary>
    public abstract int HttpStatus { get; }

    public IReadOnlyList<FieldError> Errors { get; }
}

public sealed record FieldError(string FieldName, string Message);
