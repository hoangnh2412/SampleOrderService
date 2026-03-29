using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Application.Queries.Orders;
using OrderService.Application.ReadModels;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Features.Orders.SearchOrders;

public sealed class SearchOrdersPagedHandler(IOrderRepository orders)
    : IQueryHandler<SearchOrdersPagedQuery, PagedOrdersReadModel>
{
    private const int MaxPageSize = 100;

    public async Task<PagedOrdersReadModel> HandleAsync(SearchOrdersPagedQuery query, CancellationToken cancellationToken = default)
    {
        if (query.Page < 0)
            throw new ApiBadRequestException(
                ErrorCodes.InvalidQueryParameters,
                "Invalid query parameters.",
                [new FieldError("page", "Must be at least 0.")]);

        if (query.Size < 1)
            throw new ApiBadRequestException(
                ErrorCodes.InvalidQueryParameters,
                "Invalid query parameters.",
                [new FieldError("size", "Must be at least 1.")]);

        if (query.Size > MaxPageSize)
            throw new ApiUnprocessableEntityException(
                ErrorCodes.InvalidQueryParameters,
                "Invalid query parameters.",
                [new FieldError("size", "Must not exceed 100.")]);

        var (items, total) = await orders.SearchPagedAsync(query.Name, query.Page, query.Size, cancellationToken);
        // query.Size is validated to be at least 1 above.
        var totalPages = (int)Math.Ceiling(total / (double)query.Size);
        var summaries = items.Select(ToSummary).ToList();
        return new PagedOrdersReadModel(total, totalPages, query.Page, query.Size, summaries);
    }

    private static OrderSummaryReadModel ToSummary(Order o) =>
        new(
            o.Id,
            o.Code,
            o.OrderDate,
            o.Status,
            o.PaymentStatus,
            o.CreatedAtUtc,
            o.CreatedBy,
            o.CreatedByName,
            o.TotalAmount,
            o.TotalDiscountAmount,
            o.TotalPaymentAmount);
}
