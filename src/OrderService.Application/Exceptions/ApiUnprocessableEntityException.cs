namespace OrderService.Application.Exceptions;

/// <summary>HTTP 422 — cú pháp OK nhưng vi phạm rule nghiệp vụ (tổng tiền, giới hạn query, …).</summary>
public sealed class ApiUnprocessableEntityException : ApiBusinessException
{
    public override int HttpStatus => 422;

    public ApiUnprocessableEntityException(ErrorCodes code, string message, IReadOnlyList<FieldError>? errors = null)
        : base(code, message, errors)
    {
    }
}
