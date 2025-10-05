using AutoMapper;
using Course.Application.DTOs;
using Course.Application.Services;
using Course.Core.Entities;
using Course.Core.Interfaces;

namespace Course.Application.Services.Implementations;

public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProfileService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProfileDto?> GetProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out var id))
        {
            return null;
        }

        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return null;
        }

        return _mapper.Map<ProfileDto>(user);
    }

    public async Task<ProfileDto> UpdateProfileAsync(string userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(userId, out var id))
        {
            throw new UnauthorizedAccessException("Invalid user ID");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        _mapper.Map(request, user);
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProfileDto>(user);
    }
}
