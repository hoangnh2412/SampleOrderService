namespace OrderService.Application.Exceptions;

/// <summary>HTTP 404 — tài nguyên không tồn tại.</summary>
public sealed class ApiNotFoundException : ApiBusinessException
{
    public override int HttpStatus => 404;

    public ApiNotFoundException(ErrorCodes code, string message, IReadOnlyList<FieldError>? errors = null)
        : base(code, message, errors)
    {
    }
}
