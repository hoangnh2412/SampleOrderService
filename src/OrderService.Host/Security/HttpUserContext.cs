using System.Security.Claims;
using OrderService.Application.Abstractions;

namespace OrderService.Host.Security;

public sealed class HttpUserContext(IHttpContextAccessor accessor) : IUserContext
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;

    public string UserName =>
        User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User?.FindFirstValue("sub")
        ?? "anonymous";

    public string? FullName =>
        User?.FindFirstValue("name")
        ?? User?.FindFirstValue(ClaimTypes.Name);

    public Guid Id => Guid.Parse(User?.FindFirstValue(ClaimTypes.PrimarySid) ?? "00000000-0000-0000-0000-000000000000");
}
