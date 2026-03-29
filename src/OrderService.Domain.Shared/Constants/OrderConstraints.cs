namespace OrderService.Domain.Shared.Constants;

public static class OrderConstraints
{
    public const int CodeMaxLength = 64;
    public const int PersonNameMaxLength = 256;
    public const int ProductNameMaxLength = 512;
    public const int TransactionIdMaxLength = 128;
    public const int OrderMessagePayloadMaxLength = 8000;
    public const int ProductIdMaxLength = 128;
    public const int IdempotentIdMaxLength = 256;
}
