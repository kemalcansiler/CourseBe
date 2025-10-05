using Course.Application.DTOs;

namespace Course.Application.Services;

public interface ICourseService
{
    Task<PagedResponse<CourseDto>> GetCoursesAsync(CourseListRequest request, CancellationToken cancellationToken = default);
    Task<CourseDetailDto?> GetCourseByIdAsync(int courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseDto>> GetFeaturedCoursesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FilterOptionDto>> GetFiltersAsync(CancellationToken cancellationToken = default);
}
