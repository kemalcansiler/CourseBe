using Course.Application.DTOs;

namespace Course.Application.Services;

public interface IProfileService
{
    Task<ProfileDto?> GetProfileAsync(string userId, CancellationToken cancellationToken = default);
    Task<ProfileDto> UpdateProfileAsync(string userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
}
