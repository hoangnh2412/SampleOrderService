using System.Text.Json;
using System.Text.Json.Serialization;
using OrderService.Domain.Entities;
using OrderService.Domain.Events;

namespace OrderService.Infrastructure.Outbox;

/// <summary>
/// Chuẩn hoá domain event thành JSON lưu <see cref="OutboxMessage.Payload"/> cho Relay (SAD).
/// Mọi implementation <see cref="IDomainEvent"/> được serialize theo kiểu runtime (không cần đăng ký từng loại).
/// </summary>
internal static class DomainEventOutboxSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string Serialize(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var runtimeType = domainEvent.GetType();
        var eventTypeName = runtimeType.Name;
        var payload = JsonSerializer.SerializeToElement(domainEvent, runtimeType, JsonOptions);

        return JsonSerializer.Serialize(new { eventType = eventTypeName, payload }, JsonOptions);
    }
}
