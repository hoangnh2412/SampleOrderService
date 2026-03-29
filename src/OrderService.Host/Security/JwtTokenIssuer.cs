using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OrderService.Host.Security;

/// <summary>
/// Tạo JWT access token dùng cùng <c>Jwt:*</c> với middleware xác thực.
/// </summary>
public class JwtTokenIssuer(IConfiguration configuration)
{
    private static readonly TimeSpan DefaultLifetime = TimeSpan.FromHours(1);

    public (string Token, DateTime ExpiresAtUtc) CreateAccessToken(MockAuthAccount user)
    {
        var jwt = configuration.GetSection("Jwt");
        var signingKey = jwt["SigningKey"]
            ?? throw new InvalidOperationException("Configuration 'Jwt:SigningKey' is required.");
        var issuer = jwt["Issuer"]
            ?? throw new InvalidOperationException("Configuration 'Jwt:Issuer' is required.");
        var audience = jwt["Audience"]
            ?? throw new InvalidOperationException("Configuration 'Jwt:Audience' is required.");

        var expiresAt = DateTime.UtcNow.Add(DefaultLifetime);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString("D")),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")),
            new(ClaimTypes.NameIdentifier, user.Id.ToString("D")),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.PrimarySid, user.Id.ToString("D")),
            new("name", user.DisplayName)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: creds);

        var encoded = new JwtSecurityTokenHandler().WriteToken(token);
        return (Token: encoded, ExpiresAtUtc: expiresAt);
    }
}
