namespace OrderService.Host.Middleware;

public sealed class CorrelationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("RequestId", out var rid) && Guid.TryParse(rid, out var g))
            context.Items["RequestId"] = g.ToString();
        else
            context.Items["RequestId"] = Guid.NewGuid().ToString();

        await next(context);
    }
}
