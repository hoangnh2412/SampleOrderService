using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OrderService.Application.Commands.Orders;
using OrderService.Application.Configuration;
using OrderService.Application.Dtos.Orders;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Domain.Shared.Enums;
using OrderService.Domain.Shared.Extensions;
using OrderService.Host.Models;
using OrderService.Host.Models.Requests;
using OrderService.Host.Models.Responses;

namespace OrderService.Host.Controllers;

/// <summary>
/// Callback từ cổng thanh toán (không JWT). Xác thực bằng header <c>X-Webhook-Secret</c>.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/v1/webhooks")]
public sealed class WebhookController(
    IOptions<PaymentWebhookOptions> webhookOptions,
    ICommandHandler<PaymentWebhookCommand, CheckoutResultDto> paymentWebhook,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Nhận kết quả thanh toán và cập nhật trạng thái đơn (Paid / Failed).
    /// </summary>
    [HttpPost("payment")]
    [ProducesResponseType(typeof(ResponseEnvelop<CheckoutDataResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseEnvelop), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PaymentCallback(
        [FromBody] PaymentWebhookRequest body,
        [FromHeader(Name = "X-Webhook-Secret")] string? webhookSecret,
        CancellationToken cancellationToken)
    {
        var expected = webhookOptions.Value.SharedSecret;
        if (string.IsNullOrWhiteSpace(expected))
        {
            throw new ApiInternalServerErrorException(
                ErrorCodes.InternalServerError,
                "Payment webhook is not configured.");
        }

        if (!FixedTimeSecretEquals(expected, webhookSecret))
        {
            throw new ApiUnauthorizedException(
                ErrorCodes.Unauthorized,
                "Invalid webhook secret.");
        }

        var command = new PaymentWebhookCommand(body.OrderId, body.TransactionId, body.Status);
        var result = await paymentWebhook.HandleAsync(command, cancellationToken);
        return Ok(mapper.Map<CheckoutDataResponse>(result));
    }

    private static bool FixedTimeSecretEquals(string expected, string? actual)
    {
        if (actual is null)
            return false;

        var e = Encoding.UTF8.GetBytes(expected);
        var a = Encoding.UTF8.GetBytes(actual);
        if (e.Length != a.Length)
            return false;

        return CryptographicOperations.FixedTimeEquals(e, a);
    }
}
