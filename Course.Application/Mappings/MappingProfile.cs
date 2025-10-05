using AutoMapper;
using Course.Application.DTOs;
using Course.Core.Entities;

namespace Course.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<User, ProfileDto>();
        CreateMap<UpdateProfileRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // Category mappings
        CreateMap<Category, CategoryDto>();

        // Course mappings
        CreateMap<Course.Core.Entities.Course, CourseDto>();
        CreateMap<Course.Core.Entities.Course, CourseDetailDto>();
    }
}
