using Course.Api.Controllers;
using Course.Application.DTOs;
using Course.Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Course.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var loginResponse = new LoginResponse
        {
            Token = "test-token",
            RefreshToken = "refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = new UserDto
            {
                Id = "1",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe"
            }
        };

        _authServiceMock
            .Setup(s => s.LoginAsync(loginRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loginResponse);

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<LoginResponse>().Subject;
        response.Token.Should().Be("test-token");
        response.User.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        _authServiceMock
            .Setup(s => s.LoginAsync(loginRequest, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid email or password"));

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_WithException_ShouldReturnBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        _authServiceMock
            .Setup(s => s.LoginAsync(loginRequest, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnOkWithToken()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        var registerResponse = new RegisterResponse
        {
            Token = "test-token",
            RefreshToken = "refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = new UserDto
            {
                Id = "1",
                Email = "jane@example.com",
                FirstName = "Jane",
                LastName = "Doe"
            }
        };

        _authServiceMock
            .Setup(s => s.RegisterAsync(registerRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(registerResponse);

        // Act
        var result = await _controller.Register(registerRequest);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<RegisterResponse>().Subject;
        response.Token.Should().Be("test-token");
        response.User.Email.Should().Be("jane@example.com");
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "existing@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        _authServiceMock
            .Setup(s => s.RegisterAsync(registerRequest, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("User with this email already exists"));

        // Act
        var result = await _controller.Register(registerRequest);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_WithPasswordMismatch_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Password = "password123",
            ConfirmPassword = "differentpassword"
        };

        _authServiceMock
            .Setup(s => s.RegisterAsync(registerRequest, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Password and confirm password do not match"));

        // Act
        var result = await _controller.Register(registerRequest);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}

