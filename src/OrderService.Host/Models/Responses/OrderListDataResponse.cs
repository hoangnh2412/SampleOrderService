namespace OrderService.Host.Models.Responses;

public sealed class OrderListDataResponse
{
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public int Page { get; init; }
    public int Size { get; init; }
    public List<OrderSummaryResponse> Items { get; init; } = [];
}
