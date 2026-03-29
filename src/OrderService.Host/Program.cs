using OrderService.Host.DependencyInjection;
using OrderService.Host.Middleware;
using OrderService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddHostLayer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ApiEnvelopeMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseHostSwaggerInDevelopment();

app.MapControllers();
app.Run();
