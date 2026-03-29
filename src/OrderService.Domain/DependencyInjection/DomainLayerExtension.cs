using Microsoft.Extensions.Hosting;

namespace OrderService.Domain.DependencyInjection;

public static class DomainLayerExtension
{
    public static IHostApplicationBuilder AddDomainLayer(this IHostApplicationBuilder builder)
    {
        return builder;
    }
}
