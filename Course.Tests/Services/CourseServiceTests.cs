using AutoMapper;
using Course.Application.DTOs;
using Course.Application.Services.Implementations;
using Course.Core.Entities;
using Course.Core.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Course.Tests.Services;

public class CourseServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IGenericRepository<Core.Entities.Course>> _courseRepositoryMock;
    private readonly Mock<IGenericRepository<Category>> _categoryRepositoryMock;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _courseRepositoryMock = new Mock<IGenericRepository<Core.Entities.Course>>();
        _categoryRepositoryMock = new Mock<IGenericRepository<Category>>();

        _unitOfWorkMock.Setup(u => u.Courses).Returns(_courseRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepositoryMock.Object);

        _courseService = new CourseService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetCoursesAsync_WithNoFilters_ShouldReturnAllCourses()
    {
        // Arrange
        var request = new CourseListRequest
        {
            Page = 0,
            PageSize = 12,
            SortBy = "most-popular"
        };

        var courses = new List<Core.Entities.Course>
        {
            new() {
                Id = 1,
                Title = "Test Course 1",
                Description = "Description 1",
                ShortDescription = "Short 1",
                Price = 99.99m,
                Duration = 120,
                Level = "Beginner",
                Language = "English",
                Rating = 4.5,
                EnrollmentCount = 100,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Programming" },
                CreatedAt = DateTime.UtcNow
            },
            new() {
                Id = 2,
                Title = "Test Course 2",
                Description = "Description 2",
                ShortDescription = "Short 2",
                Price = 149.99m,
                Duration = 180,
                Level = "Intermediate",
                Language = "English",
                Rating = 4.7,
                EnrollmentCount = 150,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Programming" },
                CreatedAt = DateTime.UtcNow
            }
        };

        var courseDtos = new List<CourseDto>
        {
            new() { Id = 1, Title = "Test Course 1" },
            new() { Id = 2, Title = "Test Course 2" }
        };

        _courseRepositoryMock
            .Setup(r => r.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<Core.Entities.Course>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(courses);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyList<CourseDto>>(It.IsAny<List<Core.Entities.Course>>()))
            .Returns(courseDtos);

        // Act
        var result = await _courseService.GetCoursesAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(0);
        result.PageSize.Should().Be(12);
    }

    [Fact]
    public async Task GetCoursesAsync_WithCategoryFilter_ShouldReturnFilteredCourses()
    {
        // Arrange
        var request = new CourseListRequest
        {
            Page = 0,
            PageSize = 12,
            SortBy = "most-popular",
            Category = new List<string> { "1" }
        };

        var courses = new List<Core.Entities.Course>
        {
            new() {
                Id = 1,
                Title = "Programming Course",
                Description = "Description",
                ShortDescription = "Short",
                Price = 99.99m,
                Duration = 120,
                Level = "Beginner",
                Language = "English",
                Rating = 4.5,
                EnrollmentCount = 100,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Programming" },
                CreatedAt = DateTime.UtcNow
            }
        };

        var courseDtos = new List<CourseDto>
        {
            new() { Id = 1, Title = "Programming Course" }
        };

        _courseRepositoryMock
            .Setup(r => r.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<Core.Entities.Course>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(courses);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyList<CourseDto>>(It.IsAny<List<Core.Entities.Course>>()))
            .Returns(courseDtos);

        // Act
        var result = await _courseService.GetCoursesAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetCoursesAsync_WithRatingFilter_ShouldReturnCoursesAboveMinRating()
    {
        // Arrange
        var request = new CourseListRequest
        {
            Page = 0,
            PageSize = 12,
            SortBy = "most-popular",
            Ratings = new List<string> { "4.0" }
        };

        var courses = new List<Core.Entities.Course>
        {
            new() {
                Id = 1,
                Title = "High Rated Course",
                Description = "Description",
                ShortDescription = "Short",
                Price = 99.99m,
                Duration = 120,
                Level = "Beginner",
                Language = "English",
                Rating = 4.5,
                EnrollmentCount = 100,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Programming" },
                CreatedAt = DateTime.UtcNow
            },
            new() {
                Id = 2,
                Title = "Low Rated Course",
                Description = "Description",
                ShortDescription = "Short",
                Price = 99.99m,
                Duration = 120,
                Level = "Beginner",
                Language = "English",
                Rating = 3.5,
                EnrollmentCount = 50,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Programming" },
                CreatedAt = DateTime.UtcNow
            }
        };

        var courseDtos = new List<CourseDto>
        {
            new() { Id = 1, Title = "High Rated Course" }
        };

        _courseRepositoryMock
            .Setup(r => r.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<Core.Entities.Course>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(courses);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyList<CourseDto>>(It.IsAny<List<Core.Entities.Course>>()))
            .Returns(courseDtos);

        // Act
        var result = await _courseService.GetCoursesAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().Title.Should().Be("High Rated Course");
    }

    [Fact]
    public async Task GetCoursesAsync_WithSortByHighestRated_ShouldReturnSortedCourses()
    {
        // Arrange
        var request = new CourseListRequest
        {
            Page = 0,
            PageSize = 12,
            SortBy = "highest-rated"
        };

        var courses = new List<Core.Entities.Course>
        {
            new() {
                Id = 1,
                Title = "Course 1",
                Description = "Description",
                ShortDescription = "Short",
                Price = 99.99m,
                Duration = 120,
                Level = "Beginner",
                Language = "English",
                Rating = 4.0,
                EnrollmentCount = 100,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Programming" },
                CreatedAt = DateTime.UtcNow
            },
            new() {
                Id = 2,
                Title = "Course 2",
                Description = "Description",
                ShortDescription = "Short",
                Price = 99.99m,
                Duration = 120,
                Level = "Beginner",
                Language = "English",
                Rating = 4.8,
                EnrollmentCount = 100,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Programming" },
                CreatedAt = DateTime.UtcNow
            }
        };

        var sortedCourseDtos = new List<CourseDto>
        {
            new() { Id = 2, Title = "Course 2" },
            new() { Id = 1, Title = "Course 1" }
        };

        _courseRepositoryMock
            .Setup(r => r.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<Core.Entities.Course>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(courses);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyList<CourseDto>>(It.IsAny<List<Core.Entities.Course>>()))
            .Returns(sortedCourseDtos);

        // Act
        var result = await _courseService.GetCoursesAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.First().Id.Should().Be(2); // Highest rated should be first
    }

    [Fact]
    public async Task GetCoursesAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var request = new CourseListRequest
        {
            Page = 1,
            PageSize = 2,
            SortBy = "most-popular"
        };

        var courses = new List<Core.Entities.Course>
        {
            new() { Id = 1, Title = "Course 1", Description = "Desc", ShortDescription = "Short", Price = 99, Duration = 120, Level = "Beginner", Language = "English", Rating = 4.5, EnrollmentCount = 100, CategoryId = 1, Category = new Category { Id = 1, Name = "Cat" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Title = "Course 2", Description = "Desc", ShortDescription = "Short", Price = 99, Duration = 120, Level = "Beginner", Language = "English", Rating = 4.5, EnrollmentCount = 100, CategoryId = 1, Category = new Category { Id = 1, Name = "Cat" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, Title = "Course 3", Description = "Desc", ShortDescription = "Short", Price = 99, Duration = 120, Level = "Beginner", Language = "English", Rating = 4.5, EnrollmentCount = 100, CategoryId = 1, Category = new Category { Id = 1, Name = "Cat" }, CreatedAt = DateTime.UtcNow },
            new() { Id = 4, Title = "Course 4", Description = "Desc", ShortDescription = "Short", Price = 99, Duration = 120, Level = "Beginner", Language = "English", Rating = 4.5, EnrollmentCount = 100, CategoryId = 1, Category = new Category { Id = 1, Name = "Cat" }, CreatedAt = DateTime.UtcNow }
        };

        var pagedCourseDtos = new List<CourseDto>
        {
            new() { Id = 3, Title = "Course 3" },
            new() { Id = 4, Title = "Course 4" }
        };

        _courseRepositoryMock
            .Setup(r => r.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<Core.Entities.Course>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(courses);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyList<CourseDto>>(It.IsAny<List<Core.Entities.Course>>()))
            .Returns(pagedCourseDtos);

        // Act
        var result = await _courseService.GetCoursesAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(4);
        result.Page.Should().Be(1);
    }

    [Fact]
    public async Task GetCourseByIdAsync_WithValidId_ShouldReturnCourse()
    {
        // Arrange
        var courseId = 1;
        var course = new Core.Entities.Course
        {
            Id = 1,
            Title = "Test Course",
            Description = "Description",
            ShortDescription = "Short Description",
            Price = 99.99m,
            Duration = 120,
            Level = "Beginner",
            Language = "English",
            Rating = 4.5,
            EnrollmentCount = 100,
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Programming" }
        };

        var courseDetailDto = new CourseDetailDto
        {
            Id = 1,
            Title = "Test Course"
        };

        _courseRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Ardalis.Specification.ISpecification<Core.Entities.Course>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        _mapperMock
            .Setup(m => m.Map<CourseDetailDto>(course))
            .Returns(courseDetailDto);

        // Act
        var result = await _courseService.GetCourseByIdAsync(courseId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(courseId);
        result.Title.Should().Be("Test Course");
    }

    [Fact]
    public async Task GetCourseByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var courseId = 999;

        _courseRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Ardalis.Specification.ISpecification<Core.Entities.Course>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Core.Entities.Course?)null);

        // Act
        var result = await _courseService.GetCourseByIdAsync(courseId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetFeaturedCoursesAsync_ShouldReturnFeaturedCourses()
    {
        // Arrange
        var featuredCourses = new List<Core.Entities.Course>
        {
            new() {
                Id = 1,
                Title = "Featured Course",
                Description = "Description",
                ShortDescription = "Short",
                Price = 99.99m,
                Duration = 120,
                Level = "Beginner",
                Language = "English",
                Rating = 4.8,
                EnrollmentCount = 200,
                IsFeatured = true,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Programming" },
                CreatedAt = DateTime.UtcNow
            }
        };

        var courseDtos = new List<CourseDto>
        {
            new() { Id = 1, Title = "Featured Course" }
        };

        _courseRepositoryMock
            .Setup(r => r.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<Core.Entities.Course>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(featuredCourses);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyList<CourseDto>>(featuredCourses))
            .Returns(courseDtos);

        // Act
        var result = await _courseService.GetFeaturedCoursesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Featured Course");
    }

    [Fact]
    public async Task GetCategoriesAsync_ShouldReturnAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Programming" },
            new() { Id = 2, Name = "Design" },
            new() { Id = 3, Name = "Business" }
        };

        var categoryDtos = new List<CategoryDto>
        {
            new() { Id = 1, Name = "Programming" },
            new() { Id = 2, Name = "Design" },
            new() { Id = 3, Name = "Business" }
        };

        _categoryRepositoryMock
            .Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyList<CategoryDto>>(categories))
            .Returns(categoryDtos);

        // Act
        var result = await _courseService.GetCategoriesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.Name == "Programming");
    }

    [Fact]
    public async Task GetFiltersAsync_ShouldReturnAllFilterOptions()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Programming" },
            new() { Id = 2, Name = "Design" }
        };

        var courses = new List<Core.Entities.Course>();

        _categoryRepositoryMock
            .Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        _courseRepositoryMock
            .Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(courses);

        // Act
        var result = await _courseService.GetFiltersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
        result.Should().Contain(f => f.Key == "category");
        result.Should().Contain(f => f.Key == "ratings");
        result.Should().Contain(f => f.Key == "duration");
        result.Should().Contain(f => f.Key == "instructional_level");
        result.Should().Contain(f => f.Key == "price");
    }
}

