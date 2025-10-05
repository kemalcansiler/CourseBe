namespace Course.Application.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int Duration { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public int EnrollmentCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public CategoryDto Category { get; set; } = null!;
    public UserDto Instructor { get; set; } = null!;
}

public class CourseDetailDto : CourseDto
{
    public ICollection<CourseSectionDto> Sections { get; set; } = new List<CourseSectionDto>();
    public ICollection<CourseReviewDto> Reviews { get; set; } = new List<CourseReviewDto>();
}

public class CourseSectionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public ICollection<CourseLessonDto> Lessons { get; set; } = new List<CourseLessonDto>();
}

public class CourseLessonDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public string? Content { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
    public bool IsFree { get; set; }
    public ICollection<CourseResourceDto> Resources { get; set; } = new List<CourseResourceDto>();
}

public class CourseResourceDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public bool IsFree { get; set; }
}

public class CourseReviewDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserDto User { get; set; } = null!;
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

public class CourseListRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public string? Level { get; set; }
    public string? Language { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
    
    // Frontend filter parameters (arrays)
    public List<string>? Category { get; set; } // Category IDs
    public List<string>? Ratings { get; set; } // Minimum ratings
    public List<string>? Duration { get; set; } // Duration ranges
    public List<string>? LevelFilter { get; set; } // Levels
}

public class PagedResponse<T>
{
    public IReadOnlyList<T> Data { get; set; } = new List<T>();
    public int Page { get; set; } // 0-indexed
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages - 1; // 0-indexed: page 0 to totalPages-1
    public bool HasPreviousPage => Page > 0; // 0-indexed: has previous if page > 0
}
