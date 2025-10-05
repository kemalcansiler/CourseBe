using AutoMapper;
using Course.Application.DTOs;
using Course.Application.Services.Implementations;
using Course.Core.Entities;
using Course.Core.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Course.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _configurationMock = new Mock<IConfiguration>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);

        // Setup JWT configuration
        var jwtSectionMock = new Mock<IConfigurationSection>();
        jwtSectionMock.Setup(x => x["Key"]).Returns("ThisIsAVerySecretKeyForJWTTokenGeneration12345");
        jwtSectionMock.Setup(x => x["Issuer"]).Returns("CourseApi");
        jwtSectionMock.Setup(x => x["Audience"]).Returns("CourseClient");
        
        _configurationMock.Setup(c => c.GetSection("Jwt")).Returns(jwtSectionMock.Object);

        _authService = new AuthService(_unitOfWorkMock.Object, _mapperMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnLoginResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };

        var userDto = new UserDto
        {
            Id = "1",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(loginRequest.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Should().BeEquivalentTo(userDto);
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "invalid@example.com",
            Password = "password123"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(loginRequest.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(loginRequest));
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(loginRequest.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(loginRequest));
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "John",
            LastName = "Doe",
            IsActive = false
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(loginRequest.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(loginRequest));
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnRegisterResponse()
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

        var userDto = new UserDto
        {
            Id = "1",
            Email = "jane@example.com",
            FirstName = "Jane",
            LastName = "Doe"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(registerRequest.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(userDto);

        // Act
        var result = await _authService.RegisterAsync(registerRequest);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Should().BeEquivalentTo(userDto);
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithPasswordMismatch_ShouldThrowArgumentException()
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

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.RegisterAsync(registerRequest));

        exception.Message.Should().Contain("Password and confirm password do not match");
    }

    [Fact]
    public async Task RegisterAsync_WithShortPassword_ShouldThrowArgumentException()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Password = "short",
            ConfirmPassword = "short"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.RegisterAsync(registerRequest));

        exception.Message.Should().Contain("Password must be at least 8 characters long");
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldThrowArgumentException()
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

        var existingUser = new User
        {
            Id = 1,
            Email = "existing@example.com",
            FirstName = "Existing",
            LastName = "User"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(registerRequest.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.RegisterAsync(registerRequest));

        exception.Message.Should().Contain("User with this email already exists");
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithValidUserId_ShouldReturnUserDto()
    {
        // Arrange
        var userId = "1";
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };

        var userDto = new UserDto
        {
            Id = "1",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _authService.GetCurrentUserAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(userDto);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithInvalidUserId_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var userId = "invalid";

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.GetCurrentUserAsync(userId));
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithNonExistentUser_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var userId = "999";

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.GetCurrentUserAsync(userId));
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldReturnValidToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var token = await _authService.GenerateJwtTokenAsync(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts separated by dots
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_ShouldReturnValidGuid()
    {
        // Act
        var refreshToken = await _authService.GenerateRefreshTokenAsync();

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        Guid.TryParse(refreshToken, out _).Should().BeTrue();
    }
}

