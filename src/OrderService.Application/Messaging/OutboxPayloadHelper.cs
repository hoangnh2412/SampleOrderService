using System.Text.Json;

namespace OrderService.Application.Messaging;

internal static class OutboxPayloadHelper
{
    /// <summary>
    /// Đọc <c>eventType</c> từ JSON outbox (cùng format với DomainEventOutboxSerializer ở Infrastructure).
    /// </summary>
    public static string? TryGetEventType(string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (doc.RootElement.TryGetProperty("eventType", out var et) && et.ValueKind == JsonValueKind.String)
                return et.GetString();
        }
        catch (JsonException)
        {
        }

        return null;
    }

    /// <summary>
    /// Đọc <c>payload.entityId</c> (Guid) nếu có; fallback null.
    /// </summary>
    public static Guid? TryGetEntityIdFromInnerPayload(string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("payload", out var inner))
                return null;
            if (!inner.TryGetProperty("entityId", out var idEl))
                return null;
            if (idEl.ValueKind == JsonValueKind.String && Guid.TryParse(idEl.GetString(), out var g))
                return g;
        }
        catch (JsonException)
        {
        }

        return null;
    }
}
