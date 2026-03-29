namespace OrderService.Application.Exceptions;

/// <summary>HTTP 401 — thiếu / sai xác thực (thường do filter Host; có thể ném từ Application khi cần).</summary>
public sealed class ApiUnauthorizedException : ApiBusinessException
{
    public override int HttpStatus => 401;

    public ApiUnauthorizedException(ErrorCodes code, string message, IReadOnlyList<FieldError>? errors = null)
        : base(code, message, errors)
    {
    }
}
