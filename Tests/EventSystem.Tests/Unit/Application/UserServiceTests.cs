using System.Threading.Tasks;
using EventSystem.Application.Interfaces;
using EventSystem.Application.Services;
using EventSystem.Core.Errors;
using EventSystem.Domain.Entities;
using Moq;
using Xunit;
using FluentAssertions;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();

    private UserService CreateService() => new(_userRepoMock.Object);

    [Fact]
    public async Task DeleteUserByIdAsync_ShouldDelete_WhenRoleIsSuperAdmin()
    {
        // Arrange
        long userId = 1;
        string role = "SuperAdmin";

        var service = CreateService();

        // Act
        await service.DeleteUserByIdAsync(userId, role);

        // Assert
        _userRepoMock.Verify(r => r.DeleteUserByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUserByIdAsync_ShouldDelete_WhenRoleIsAdmin_AndTargetIsUser()
    {
        // Arrange
        long userId = 2;
        string role = "Admin";

        var targetUser = new User
        {
            UserId = userId,
            Role = new UserRole { Name = "User" }
        };

        _userRepoMock.Setup(r => r.GetUserByIdAync(userId)).ReturnsAsync(targetUser);

        var service = CreateService();

        // Act
        await service.DeleteUserByIdAsync(userId, role);

        // Assert
        _userRepoMock.Verify(r => r.DeleteUserByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUserByIdAsync_ShouldThrow_WhenRoleIsAdmin_AndTargetIsAdmin()
    {
        // Arrange
        long userId = 3;
        string role = "Admin";

        var targetUser = new User
        {
            UserId = userId,
            Role = new UserRole { Name = "Admin" }
        };

        _userRepoMock.Setup(r => r.GetUserByIdAync(userId)).ReturnsAsync(targetUser);

        var service = CreateService();

        // Act
        var act = async () => await service.DeleteUserByIdAsync(userId, role);

        // Assert
        await act.Should().ThrowAsync<NotAllowedException>()
            .WithMessage("Admin can not delete Admin or SuperAdmin");

        _userRepoMock.Verify(r => r.DeleteUserByIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ShouldCallRepoMethod()
    {
        // Arrange
        long userId = 4;
        string newRole = "Admin";

        var service = CreateService();

        // Act
        await service.UpdateUserRoleAsync(userId, newRole);

        // Assert
        _userRepoMock.Verify(r => r.UpdateUserRoleAsync(userId, newRole), Times.Once);
    }
}
