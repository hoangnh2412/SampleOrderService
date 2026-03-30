using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OrderService.Application.Messaging;

/// <summary>
/// Vòng lặp quét outbox và publish (logic service); host/presentation chỉ gọi <see cref="HandleAsync"/>.
/// </summary>
public sealed class OutboxMessageService(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxMessageService> logger)
{
    public async Task HandleAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var publisher = scope.ServiceProvider.GetRequiredService<BrokerPublisher>();
                var count = await publisher.PublishAsync(50, stoppingToken);
                var delay = count > 0 ? TimeSpan.FromMilliseconds(200) : TimeSpan.FromSeconds(2);
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox message service iteration failed.");
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}
