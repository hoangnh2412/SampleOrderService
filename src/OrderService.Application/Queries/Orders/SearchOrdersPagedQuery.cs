namespace OrderService.Application.Queries.Orders;

public sealed record SearchOrdersPagedQuery(string? Name, int Page, int Size);
