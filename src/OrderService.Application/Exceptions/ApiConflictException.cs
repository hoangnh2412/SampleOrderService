namespace OrderService.Application.Exceptions;

/// <summary>HTTP 409 — xung đột trạng thái (ví dụ đã thanh toán).</summary>
public sealed class ApiConflictException : ApiBusinessException
{
    public override int HttpStatus => 409;

    public ApiConflictException(ErrorCodes code, string message, IReadOnlyList<FieldError>? errors = null)
        : base(code, message, errors)
    {
    }
}
