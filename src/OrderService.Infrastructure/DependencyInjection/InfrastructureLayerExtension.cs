using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Domain.DependencyInjection;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Outbox;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Persistence.Repositories;

namespace OrderService.Infrastructure.DependencyInjection;

public static class InfrastructureLayerExtension
{
    public static IHostApplicationBuilder AddInfrastructureLayer(this IHostApplicationBuilder builder)
    {
        builder.AddDomainLayer();

        var connectionString = builder.Configuration["ConnectionStrings:OrderDatabase"]
            ?? throw new InvalidOperationException("Connection string 'OrderDatabase' not found.");
        builder.Services.AddDbContext<OrderDbContext>(options => options.UseSqlite(connectionString));
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        return builder;
    }
}
