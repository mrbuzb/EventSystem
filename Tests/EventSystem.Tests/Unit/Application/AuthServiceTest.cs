using System.Data;
using System.Security.Claims;
using EventSystem.Application.Dtos;
using EventSystem.Application.Helpers;
using EventSystem.Application.Helpers.Security;
using EventSystem.Application.Interfaces;
using EventSystem.Application.Services;
using EventSystem.Application.Settings;
using EventSystem.Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

public class AuthServiceTests
{
    private readonly Mock<IRoleRepository> _roleRepoMock = new();
    private readonly Mock<IValidator<UserCreateDto>> _createValidatorMock = new();
    private readonly Mock<IValidator<UserLoginDto>> _loginValidatorMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IRefreshTokenRepository> _refreshRepoMock = new();
    private readonly JwtAppSettings _jwtSettings = new JwtAppSettings(
        issuer: "TestIssuer",
        audience: "TestAudience",
        securityKey: "SuperSecretTestKey1234567890",
        lifetime: "60");

    private AuthService CreateService() => new(
        _roleRepoMock.Object,
        _createValidatorMock.Object,
        _userRepoMock.Object,
        _tokenServiceMock.Object,
        _jwtSettings,
        _loginValidatorMock.Object,
        _refreshRepoMock.Object
    );

    [Fact]
    public async Task SignUpUserAsync_ShouldReturnUserId_WhenInputIsValid()
    {
        // Arrange
        var userCreateDto = new UserCreateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "123456789",
            Password = "Password123!",
            UserName = "johnny"
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(userCreateDto, default))
            .ReturnsAsync(new ValidationResult());

        _roleRepoMock.Setup(r => r.GetRoleIdAsync("User")).ReturnsAsync(2);
        _userRepoMock.Setup(r => r.AddUserAync(It.IsAny<User>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.SignUpUserAsync(userCreateDto);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task LoginUserAsync_ShouldReturnToken_WhenCredentialsAreCorrect()
    {
        // Arrange
        var password = "Password123!";
        var hashed = PasswordHasher.Hasher(password);

        var userLoginDto = new UserLoginDto
        {
            UserName = "johnny",
            Password = password
        };

        _loginValidatorMock.Setup(v => v.Validate(userLoginDto)).Returns(new ValidationResult());

        var user = new User
        {
            UserId = 1,
            UserName = "johnny",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "123456789",
            Password = hashed.Hash, 
            Salt = hashed.Salt,    
            Role = new UserRole { Name = "User" }
        };

        _userRepoMock.Setup(r => r.GetUserByUserNameAync("johnny")).ReturnsAsync(user);

        _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<UserGetDto>())).Returns("access-token");
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("refresh-token");

        var service = CreateService();

        // Act
        var result = await service.LoginUserAsync(userLoginDto);

        // Assert
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
    }


    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenValid()
    {
        // Arrange
        var request = new RefreshRequestDto
        {
            AccessToken = "expired-access-token",
            RefreshToken = "valid-refresh-token"
        };

        var claims = new ClaimsIdentity(new List<Claim> { new("UserId", "1") });
        var principal = new ClaimsPrincipal(claims);

        _tokenServiceMock.Setup(t => t.GetPrincipalFromExpiredToken("expired-access-token")).Returns(principal);

        var refreshToken = new RefreshToken
        {
            Expires = DateTime.UtcNow.AddMinutes(10),
            IsRevoked = false,
            UserId = 1
        };

        _refreshRepoMock.Setup(r => r.SelectRefreshToken("valid-refresh-token", 1)).ReturnsAsync(refreshToken);

        var user = new User
        {
            UserId = 1,
            UserName = "johnny",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "123456789",
            Role = new UserRole { Name = "User" }
        };

        _userRepoMock.Setup(r => r.GetUserByIdAync(1)).ReturnsAsync(user);
        _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<UserGetDto>())).Returns("new-access-token");
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("new-refresh-token");

        var service = CreateService();

        // Act
        var result = await service.RefreshTokenAsync(request);

        // Assert
        result.AccessToken.Should().Be("new-access-token");
        result.RefreshToken.Should().Be("new-refresh-token");
    }
}
