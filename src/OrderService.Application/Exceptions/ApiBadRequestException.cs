namespace OrderService.Application.Exceptions;

/// <summary>HTTP 400 — body/query không hợp lệ (trước khi vào rule nghiệp vụ sâu).</summary>
public sealed class ApiBadRequestException : ApiBusinessException
{
    public override int HttpStatus => 400;

    public ApiBadRequestException(ErrorCodes code, string message, IReadOnlyList<FieldError>? errors = null)
        : base(code, message, errors)
    {
    }
}
