using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Application.Commands.Orders;
using OrderService.Application.Dtos.Orders;
using OrderService.Application.Mapping;
using OrderService.Application.Features.Orders.Checkout;
using OrderService.Application.Features.Orders.CreateOrder;
using OrderService.Application.Features.Orders.SearchOrders;
using OrderService.Application.Interfaces;
using OrderService.Application.Queries.Orders;
using OrderService.Application.ReadModels;

namespace OrderService.Application.DependencyInjection;

public static class ApplicationLayerExtension
{
    public static IHostApplicationBuilder AddApplicationLayer(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddAutoMapper(typeof(OrderDomainMappingProfile).Assembly);
        builder.Services.AddScoped<ICommandHandler<CreateOrderCommand, CreateOrderResultDto>, CreateOrderHandler>();
        builder.Services.AddScoped<ICommandHandler<CheckoutOrderCommand, CheckoutResultDto>, CheckoutOrderHandler>();
        builder.Services.AddScoped<IQueryHandler<SearchOrdersPagedQuery, PagedOrdersReadModel>, SearchOrdersPagedHandler>();
        return builder;
    }
}
