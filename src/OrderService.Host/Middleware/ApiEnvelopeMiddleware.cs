using System.Diagnostics;
using System.Text.Json;
using OrderService.Application.Exceptions;
using OrderService.Domain.Shared.Enums;
using OrderService.Domain.Shared.Extensions;
using OrderService.Host.Models;

namespace OrderService.Host.Middleware;

/// <summary>
/// Chuẩn hóa phản hồi API: bắt exception trả <see cref="ResponseEnvelop"/> lỗi;
/// thành công (2xx, JSON, <c>/api</c>) luôn bọc payload thô từ action vào envelope — action không trả envelope.
/// </summary>
public sealed class ApiEnvelopeMiddleware(RequestDelegate next, ILogger<ApiEnvelopeMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;
        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
                throw;

            context.Response.Body = originalBody;
            context.Response.Headers.TryAdd("RequestId", context.Items["RequestId"] as string ?? Guid.NewGuid().ToString());
            context.Response.Headers.TryAdd("TraceId", Activity.Current?.Id ?? context.TraceIdentifier);
            context.Response.ContentType = "application/json";

            if (ex is ApiBusinessException biz)
            {
                context.Response.StatusCode = biz.HttpStatus;
                logger.LogWarning(ex, "Business rule {Code}: {Message}", biz.ErrorCode, biz.Message);
                await context.Response.WriteAsJsonAsync(new ResponseEnvelop
                {
                    Code = biz.ErrorCode,
                    Message = biz.Message,
                    Errors = biz.Errors.Select(e => new FieldErrorDto(e.FieldName, e.Message)).ToList()
                });
                return;
            }

            logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ResponseEnvelop
            {
                Code = ErrorCodes.InternalServerError.ToWireCode(),
                Message = "Internal server error",
                Errors = []
            });
            return;
        }

        try
        {
            if (ShouldWrapSuccess(context, buffer))
            {
                buffer.Position = 0;
                using var doc = await JsonDocument.ParseAsync(buffer, cancellationToken: context.RequestAborted);
                context.Response.Body = originalBody;
                context.Response.Headers.Remove("Content-Length");

                await using var writer = new Utf8JsonWriter(originalBody, new JsonWriterOptions
                {
                    Indented = false
                });
                writer.WriteStartObject();
                writer.WriteString("code", "0000");
                writer.WriteString("message", "Success");
                writer.WriteStartArray("errors");
                writer.WriteEndArray();
                writer.WritePropertyName("data");
                doc.RootElement.WriteTo(writer);
                writer.WriteEndObject();
                await writer.FlushAsync(context.RequestAborted);
            }
            else
            {
                context.Response.Body = originalBody;
                buffer.Position = 0;
                await buffer.CopyToAsync(originalBody, context.RequestAborted);
            }
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static bool ShouldWrapSuccess(HttpContext context, MemoryStream buffer)
    {
        var status = context.Response.StatusCode;
        if (status < StatusCodes.Status200OK || status > 299)
            return false;

        if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            return false;

        var ct = context.Response.ContentType;
        if (ct is null || !ct.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            return false;

        return buffer.Length > 0;
    }
}
