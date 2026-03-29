using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OrderService.Host.Swagger;

/// <summary>
/// Điền sẵn <c>default</c>/<c>example</c> cho header <c>IdempotentId</c> để Swagger UI không báo required khi Try it out (giá trị lấy khi sinh OpenAPI).
/// </summary>
public sealed class IdempotentIdHeaderParameterFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        if (parameter.In != ParameterLocation.Header)
            return;
        if (!string.Equals(parameter.Name, "IdempotentId", StringComparison.OrdinalIgnoreCase))
            return;

        var id = Guid.NewGuid().ToString("D");
        parameter.Schema ??= new OpenApiSchema { Type = "string", Format = "uuid" };
        parameter.Schema.Default = new OpenApiString(id);
        parameter.Example = new OpenApiString(id);
    }
}
