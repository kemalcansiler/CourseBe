using Course.Application.DTOs;
using Course.Core.Entities;

namespace Course.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<UserDto> GetCurrentUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<string> GenerateJwtTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync();
}
