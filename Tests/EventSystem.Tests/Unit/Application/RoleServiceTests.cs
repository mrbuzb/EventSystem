using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventSystem.Application.Dtos;
using EventSystem.Application.Interfaces;
using EventSystem.Application.Services;
using EventSystem.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

public class RoleServiceTests
{
    private readonly Mock<IRoleRepository> _roleRepoMock = new();

    private RoleService CreateService() => new(_roleRepoMock.Object);

    [Fact]
    public async Task GetAllRolesAsync_ShouldReturnListOfRoleGetDto()
    {
        // Arrange
        var roles = new List<UserRole>
        {
            new UserRole { Id = 1, Name = "Admin", Description = "Administrator" },
            new UserRole { Id = 2, Name = "User", Description = "Regular user" }
        };

        _roleRepoMock.Setup(r => r.GetAllRolesAsync()).ReturnsAsync(roles);

        var service = CreateService();

        // Act
        var result = await service.GetAllRolesAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Admin");
        result[1].Name.Should().Be("User");
    }

    [Fact]
    public async Task GetRoleIdAsync_ShouldReturnCorrectId()
    {
        // Arrange
        string roleName = "User";
        _roleRepoMock.Setup(r => r.GetRoleIdAsync(roleName)).ReturnsAsync(2);

        var service = CreateService();

        // Act
        var result = await service.GetRoleIdAsync(roleName);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task GetAllUsersByRoleAsync_ShouldReturnUsersWithRole()
    {
        // Arrange
        string roleName = "User";

        var users = new List<User>
        {
            new User
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                //Email = "john@example.com",
                PhoneNumber = "123456789",
                UserName = "johndoe",
                Role = new UserRole { Name = "User" }
            },
            new User
            {
                UserId = 2,
                FirstName = "Jane",
                LastName = "Smith",
                //Email = "jane@example.com",
                PhoneNumber = "987654321",
                UserName = "janesmith",
                Role = new UserRole { Name = "User" }
            }
        };

        _roleRepoMock.Setup(r => r.GetAllUsersByRoleAsync(roleName)).ReturnsAsync(users);

        var service = CreateService();

        // Act
        var result = await service.GetAllUsersByRoleAsync(roleName);

        // Assert
        result.Should().HaveCount(2);
        result.All(u => u.Role == "User").Should().BeTrue();
        result.Select(u => u.UserName).Should().Contain(new[] { "johndoe", "janesmith" });
    }
}
