using Course.Api.Controllers;
using Course.Application.DTOs;
using Course.Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Course.Tests.Controllers;

public class CoursesControllerTests
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly CoursesController _controller;

    public CoursesControllerTests()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _controller = new CoursesController(_courseServiceMock.Object);
    }

    [Fact]
    public async Task GetCourses_ReturnsOkWithCourses()
    {
        // Arrange
        var request = new CourseListRequest { Page = 1, PageSize = 10 };
        var expected = new PagedResponse<CourseDto>
        {
            Data = new List<CourseDto>
            {
                new CourseDto { Id = 1, Title = "Test Course 1" },
                new CourseDto { Id = 2, Title = "Test Course 2" }
            },
            Page = 1,
            PageSize = 10,
            TotalCount = 2
        };

        _courseServiceMock.Setup(x => x.GetCoursesAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetCourses(request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedValue = okResult.Value as PagedResponse<CourseDto>;
        returnedValue.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetCourses_ReturnsBadRequestOnException()
    {
        // Arrange
        var request = new CourseListRequest();
        _courseServiceMock.Setup(x => x.GetCoursesAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test error"));

        // Act
        var result = await _controller.GetCourses(request);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetCourseById_ReturnsOkWithCourse()
    {
        // Arrange
        var courseId = 1;
        var expected = new CourseDetailDto
        {
            Id = courseId,
            Title = "Test Course",
            Description = "Test Description"
        };

        _courseServiceMock.Setup(x => x.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetCourseById(courseId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedValue = okResult.Value as CourseDetailDto;
        returnedValue.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetCourseById_ReturnsNotFoundWhenCourseDoesNotExist()
    {
        // Arrange
        var courseId = 999;
        _courseServiceMock.Setup(x => x.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseDetailDto?)null);

        // Act
        var result = await _controller.GetCourseById(courseId);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetCourseById_ReturnsBadRequestOnException()
    {
        // Arrange
        var courseId = 1;
        _courseServiceMock.Setup(x => x.GetCourseByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test error"));

        // Act
        var result = await _controller.GetCourseById(courseId);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetFeaturedCourses_ReturnsOkWithCourses()
    {
        // Arrange
        var expected = new List<CourseDto>
        {
            new CourseDto { Id = 1, Title = "Featured Course 1", IsFeatured = true },
            new CourseDto { Id = 2, Title = "Featured Course 2", IsFeatured = true }
        };

        _courseServiceMock.Setup(x => x.GetFeaturedCoursesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetFeaturedCourses();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedValue = okResult.Value as IReadOnlyList<CourseDto>;
        returnedValue.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetFeaturedCourses_ReturnsBadRequestOnException()
    {
        // Arrange
        _courseServiceMock.Setup(x => x.GetFeaturedCoursesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test error"));

        // Act
        var result = await _controller.GetFeaturedCourses();

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetCategories_ReturnsOkWithCategories()
    {
        // Arrange
        var expected = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Name = "Programming" },
            new CategoryDto { Id = 2, Name = "Design" }
        };

        _courseServiceMock.Setup(x => x.GetCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetCategories();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedValue = okResult.Value as IReadOnlyList<CategoryDto>;
        returnedValue.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetCategories_ReturnsBadRequestOnException()
    {
        // Arrange
        _courseServiceMock.Setup(x => x.GetCategoriesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test error"));

        // Act
        var result = await _controller.GetCategories();

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetFilters_ReturnsOkWithFilters()
    {
        // Arrange
        var expected = new List<FilterOptionDto>
        {
            new FilterOptionDto
            {
                Label = "Category",
                Key = "category",
                Options = new List<FilterBucketDto>
                {
                    new FilterBucketDto { Label = "Programming", Value = "1" }
                }
            }
        };

        _courseServiceMock.Setup(x => x.GetFiltersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetFilters();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedValue = okResult.Value as IReadOnlyList<FilterOptionDto>;
        returnedValue.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetFilters_ReturnsBadRequestOnException()
    {
        // Arrange
        _courseServiceMock.Setup(x => x.GetFiltersAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test error"));

        // Act
        var result = await _controller.GetFilters();

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }
}

