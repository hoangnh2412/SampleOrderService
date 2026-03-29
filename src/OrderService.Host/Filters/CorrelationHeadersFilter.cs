using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OrderService.Host.Filters;

public sealed class CorrelationHeadersFilter : IAsyncAlwaysRunResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var http = context.HttpContext;
        var requestId = http.Items["RequestId"] as string ?? Guid.NewGuid().ToString();
        http.Response.Headers.TryAdd("RequestId", requestId);
        http.Response.Headers.TryAdd("TraceId", Activity.Current?.Id ?? http.TraceIdentifier);
        return next();
    }
}
