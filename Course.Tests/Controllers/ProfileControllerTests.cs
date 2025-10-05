using System.Security.Claims;
using Course.Api.Controllers;
using Course.Application.DTOs;
using Course.Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Course.Tests.Controllers;

public class ProfileControllerTests
{
    private readonly Mock<IProfileService> _profileServiceMock;
    private readonly ProfileController _controller;
    private readonly string _testUserId = "test-user-123";

    public ProfileControllerTests()
    {
        _profileServiceMock = new Mock<IProfileService>();
        _controller = new ProfileController(_profileServiceMock.Object);

        // Setup user context
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, _testUserId),
            new Claim(ClaimTypes.Email, "test@example.com")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetProfile_ReturnsOkWithProfile()
    {
        // Arrange
        var expected = new ProfileDto
        {
            Id = _testUserId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            ProfileImageUrl = "avatar.jpg",
            DateOfBirth = DateTime.Parse("1990-01-01"),
            Bio = "Test bio",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _profileServiceMock.Setup(x => x.GetProfileAsync(_testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedValue = okResult.Value as ProfileDto;
        returnedValue.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetProfile_ReturnsUnauthorizedWhenUserIdMissing()
    {
        // Arrange
        var controller = new ProfileController(_profileServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var result = await controller.GetProfile();

        // Assert
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task GetProfile_ReturnsNotFoundWhenProfileDoesNotExist()
    {
        // Arrange
        _profileServiceMock.Setup(x => x.GetProfileAsync(_testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProfileDto?)null);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetProfile_ReturnsUnauthorizedOnUnauthorizedAccessException()
    {
        // Arrange
        _profileServiceMock.Setup(x => x.GetProfileAsync(_testUserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task GetProfile_ReturnsBadRequestOnException()
    {
        // Arrange
        _profileServiceMock.Setup(x => x.GetProfileAsync(_testUserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test error"));

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task UpdateProfile_ReturnsOkWithUpdatedProfile()
    {
        // Arrange
        var request = new UpdateProfileRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Bio = "Updated bio"
        };

        var expected = new ProfileDto
        {
            Id = _testUserId,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            ProfileImageUrl = null,
            DateOfBirth = null,
            Bio = "Updated bio",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _profileServiceMock.Setup(x => x.UpdateProfileAsync(_testUserId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.UpdateProfile(request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedValue = okResult.Value as ProfileDto;
        returnedValue.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateProfile_ReturnsUnauthorizedWhenUserIdMissing()
    {
        // Arrange
        var controller = new ProfileController(_profileServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var request = new UpdateProfileRequest { FirstName = "Test", LastName = "User" };

        // Act
        var result = await controller.UpdateProfile(request);

        // Assert
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task UpdateProfile_ReturnsUnauthorizedOnUnauthorizedAccessException()
    {
        // Arrange
        var request = new UpdateProfileRequest { FirstName = "Test", LastName = "User" };
        _profileServiceMock.Setup(x => x.UpdateProfileAsync(_testUserId, request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

        // Act
        var result = await _controller.UpdateProfile(request);

        // Assert
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task UpdateProfile_ReturnsBadRequestOnException()
    {
        // Arrange
        var request = new UpdateProfileRequest { FirstName = "Test", LastName = "User" };
        _profileServiceMock.Setup(x => x.UpdateProfileAsync(_testUserId, request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test error"));

        // Act
        var result = await _controller.UpdateProfile(request);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }
}

