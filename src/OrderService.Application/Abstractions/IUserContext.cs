namespace OrderService.Application.Abstractions;

/// <summary>
/// Thông tin người gọi từ Bearer token (OpenAPI: subject / display name).
/// </summary>
public interface IUserContext
{
    Guid Id { get; }
    string UserName { get; }
    string? FullName { get; }
}
