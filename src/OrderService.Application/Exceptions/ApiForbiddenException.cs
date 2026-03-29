namespace OrderService.Application.Exceptions;

/// <summary>HTTP 403 — đã xác thực nhưng không đủ quyền.</summary>
public sealed class ApiForbiddenException : ApiBusinessException
{
    public override int HttpStatus => 403;

    public ApiForbiddenException(ErrorCodes code, string message, IReadOnlyList<FieldError>? errors = null)
        : base(code, message, errors)
    {
    }
}
