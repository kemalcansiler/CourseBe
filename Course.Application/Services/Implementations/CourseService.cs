using AutoMapper;
using Ardalis.Specification;
using Course.Application.DTOs;
using Course.Application.Services;
using Course.Core.Interfaces;
using Course.Core.Specifications;

namespace Course.Application.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CourseService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<CourseDto>> GetCoursesAsync(CourseListRequest request, CancellationToken cancellationToken = default)
    {
        ISpecification<Course.Core.Entities.Course> spec = new CourseWithDetailsSpec();
        
        // Apply filters
        if (!string.IsNullOrEmpty(request.Search))
        {
            spec = new CourseBySearchSpec(request.Search);
        }
        else if (request.CategoryId.HasValue)
        {
            spec = new CourseByCategorySpec(request.CategoryId.Value);
        }
        else if (!string.IsNullOrEmpty(request.Level))
        {
            spec = new CourseByLevelSpec(request.Level);
        }

        // Get all courses first
        var courses = await _unitOfWork.Courses.ListAsync(spec, cancellationToken);
        
        // Apply frontend category filter (multiple categories)
        if (request.Category != null && request.Category.Any())
        {
            var categoryIds = request.Category.Select(c => int.TryParse(c, out var id) ? id : 0).Where(id => id > 0).ToList();
            if (categoryIds.Any())
            {
                courses = courses.Where(c => categoryIds.Contains(c.CategoryId)).ToList();
            }
        }
        
        // Apply frontend ratings filter (minimum rating)
        if (request.Ratings != null && request.Ratings.Any())
        {
            var minRating = request.Ratings
                .Select(r => double.TryParse(r, out var rating) ? rating : 0)
                .Max();
            if (minRating > 0)
            {
                courses = courses.Where(c => c.Rating >= minRating).ToList();
            }
        }
        
        // Apply frontend duration filter
        if (request.Duration != null && request.Duration.Any())
        {
            var filteredByDuration = new List<Core.Entities.Course>();
            foreach (var durationRange in request.Duration)
            {
                switch (durationRange.ToLower())
                {
                    case "short": // 0-1 hour (0-60 minutes)
                        filteredByDuration.AddRange(courses.Where(c => c.Duration <= 60));
                        break;
                    case "medium": // 1-3 hours (60-180 minutes)
                        filteredByDuration.AddRange(courses.Where(c => c.Duration > 60 && c.Duration <= 180));
                        break;
                    case "long": // 3-6 hours (180-360 minutes)
                        filteredByDuration.AddRange(courses.Where(c => c.Duration > 180 && c.Duration <= 360));
                        break;
                    case "extra-long": // 6+ hours (360+ minutes)
                        filteredByDuration.AddRange(courses.Where(c => c.Duration > 360));
                        break;
                }
            }
            if (filteredByDuration.Any())
            {
                courses = filteredByDuration.Distinct().ToList();
            }
        }
        
        // Apply frontend level filter (multiple levels)
        if (request.LevelFilter != null && request.LevelFilter.Any())
        {
            courses = courses.Where(c => request.LevelFilter.Contains(c.Level, StringComparer.OrdinalIgnoreCase)).ToList();
        }
        
        // Apply legacy filters
        if (request.Language != null)
        {
            courses = courses.Where(c => c.Language == request.Language).ToList();
        }
        
        if (request.MinPrice.HasValue)
        {
            courses = courses.Where(c => c.Price >= request.MinPrice.Value).ToList();
        }
        
        if (request.MaxPrice.HasValue)
        {
            courses = courses.Where(c => c.Price <= request.MaxPrice.Value).ToList();
        }

        // Apply sorting
        courses = request.SortBy?.ToLower() switch
        {
            // Frontend sort options
            "most-popular" => courses.OrderByDescending(c => c.EnrollmentCount).ToList(),
            "highest-rated" => courses.OrderByDescending(c => c.Rating).ToList(),
            "newest" => courses.OrderByDescending(c => c.CreatedAt).ToList(),
            "price-low-to-high" => courses.OrderBy(c => c.Price).ToList(),
            "price-high-to-low" => courses.OrderByDescending(c => c.Price).ToList(),
            // Legacy backend sort options
            "title" => request.SortDirection == "desc" 
                ? courses.OrderByDescending(c => c.Title).ToList()
                : courses.OrderBy(c => c.Title).ToList(),
            "price" => request.SortDirection == "desc"
                ? courses.OrderByDescending(c => c.Price).ToList()
                : courses.OrderBy(c => c.Price).ToList(),
            "rating" => request.SortDirection == "desc"
                ? courses.OrderByDescending(c => c.Rating).ToList()
                : courses.OrderBy(c => c.Rating).ToList(),
            "enrollmentcount" => request.SortDirection == "desc"
                ? courses.OrderByDescending(c => c.EnrollmentCount).ToList()
                : courses.OrderBy(c => c.EnrollmentCount).ToList(),
            _ => courses.OrderByDescending(c => c.EnrollmentCount).ToList() // Default to most popular
        };

        var totalCount = courses.Count;
        var page = Math.Max(0, request.Page);
        var pagedCourses = courses
            .Skip(page * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var courseDtos = _mapper.Map<IReadOnlyList<CourseDto>>(pagedCourses);

        return new PagedResponse<CourseDto>
        {
            Data = courseDtos,
            Page = page, // Return 0-indexed page
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<CourseDetailDto?> GetCourseByIdAsync(int courseId, CancellationToken cancellationToken = default)
    {
        var spec = new CourseByIdWithDetailsSpec(courseId);
        var course = await _unitOfWork.Courses.FirstOrDefaultAsync(spec, cancellationToken);
        
        return course != null ? _mapper.Map<CourseDetailDto>(course) : null;
    }

    public async Task<IReadOnlyList<CourseDto>> GetFeaturedCoursesAsync(CancellationToken cancellationToken = default)
    {
        var spec = new FeaturedCoursesSpec();
        var courses = await _unitOfWork.Courses.ListAsync(spec, cancellationToken);
        
        return _mapper.Map<IReadOnlyList<CourseDto>>(courses);
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Categories.ListAllAsync(cancellationToken);
        
        return _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
    }

    public async Task<IReadOnlyList<FilterOptionDto>> GetFiltersAsync(CancellationToken cancellationToken = default)
    {
        var allCourses = await _unitOfWork.Courses.ListAllAsync(cancellationToken);
        var allCategories = await _unitOfWork.Categories.ListAllAsync(cancellationToken);

        var filters = new List<FilterOptionDto>();

        // Category filters
        var categoryOptions = allCategories.Select(c => new FilterBucketDto
        {
            Label = c.Name,
            Value = c.Id.ToString(),
            IsSelected = false
        }).ToList();

        var categoryFilters = new FilterOptionDto
        {
            Label = "Category",
            Key = "category",
            Options = categoryOptions
        };
        filters.Add(categoryFilters);

        // Rating filters
        var ratingFilters = new FilterOptionDto
        {
            Label = "Ratings",
            Key = "ratings",
            Options = new List<FilterBucketDto>
            {
                new() { Label = "4.5 & up", Value = "4.5", IsSelected = false },
                new() { Label = "4.0 & up", Value = "4.0", IsSelected = false },
                new() { Label = "3.5 & up", Value = "3.5", IsSelected = false },
                new() { Label = "3.0 & up", Value = "3.0", IsSelected = false }
            }
        };
        filters.Add(ratingFilters);

        // Duration filters
        var durationFilters = new FilterOptionDto
        {
            Label = "Video Duration",
            Key = "duration",
            Options = new List<FilterBucketDto>
            {
                new() { Label = "0-1 Hour", Value = "extraShort", IsSelected = false },
                new() { Label = "1-3 Hours", Value = "short", IsSelected = false },
                new() { Label = "3-6 Hours", Value = "medium", IsSelected = false },
                new() { Label = "6-17 Hours", Value = "long", IsSelected = false },
                new() { Label = "17+ Hours", Value = "extraLong", IsSelected = false }
            }
        };
        filters.Add(durationFilters);

        // Level filters
        var levelFilters = new FilterOptionDto
        {
            Label = "Level",
            Key = "instructional_level",
            Options = new List<FilterBucketDto>
            {
                new() { Label = "All Levels", Value = "all", IsSelected = false },
                new() { Label = "Beginner", Value = "beginner", IsSelected = false },
                new() { Label = "Intermediate", Value = "intermediate", IsSelected = false },
                new() { Label = "Expert", Value = "expert", IsSelected = false }
            }
        };
        filters.Add(levelFilters);

        // Price filters
        var priceFilters = new FilterOptionDto
        {
            Label = "Price",
            Key = "price",
            Options = new List<FilterBucketDto>
            {
                new() { Label = "Paid", Value = "price-paid", IsSelected = false },
                new() { Label = "Free", Value = "price-free", IsSelected = false }
            }
        };
        filters.Add(priceFilters);

        return filters;
    }
}
