using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Shared.Enums;
using OrderService.Domain.Shared.Extensions;
using OrderService.Host.Models;
using OrderService.Host.Models.Requests;
using OrderService.Host.Models.Responses;
using OrderService.Host.Security;

namespace OrderService.Host.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/v1/auth")]
public sealed class AuthController(JwtTokenIssuer tokenIssuer) : ControllerBase
{
    /// <summary>
    /// Đăng nhập giả lập và nhận JWT. Tài khoản mẫu: <c>admin</c>/<c>admin</c>, <c>demo</c>/<c>demo</c>.
    /// </summary>
    [HttpPost("token")]
    [ProducesResponseType(typeof(ResponseEnvelop<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseEnvelop), StatusCodes.Status401Unauthorized)]
    public IActionResult IssueToken([FromBody] LoginRequest body)
    {
        var account = MockAuthUsers.GetByUsername(body.Username);
        if (account is null || !MockAuthUsers.IsPasswordValid(account, body.Password))
        {
            return Unauthorized(new ResponseEnvelop
            {
                Code = ErrorCodes.Unauthorized.ToWireCode(),
                Message = "Invalid username or password",
                Errors = []
            });
        }

        var (token, expiresAtUtc) = tokenIssuer.CreateAccessToken(account);
        return Ok(new TokenResponse
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresAtUtc = expiresAtUtc
        });
    }
}
