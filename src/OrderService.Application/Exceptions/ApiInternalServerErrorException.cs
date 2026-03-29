namespace OrderService.Application.Exceptions;

/// <summary>HTTP 500 — lỗi máy chủ / tình huống không lường trước (ưu tiên chỉ ném khi đã log).</summary>
public sealed class ApiInternalServerErrorException : ApiBusinessException
{
    public override int HttpStatus => 500;

    public ApiInternalServerErrorException(ErrorCodes code, string message, IReadOnlyList<FieldError>? errors = null)
        : base(code, message, errors)
    {
    }
}
