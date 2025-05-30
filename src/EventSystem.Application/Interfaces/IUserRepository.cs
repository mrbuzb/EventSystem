using EventSystem.Domain.Entities;

namespace EventSystem.Application.Interfaces;

public interface IUserRepository
{
    Task<long> AddUserAync(User user);
    Task<User> GetUserByIdAync(long id);
    Task<User> GetUserByUserNameAync(string userName);
    Task UpdateUserRoleAsync(long userId, string userRole);
    Task DeleteUserByIdAsync(long userId);
    Task<bool> CheckUserById(long userId);
    Task<bool> CheckUsernameExists(string username);
    Task<bool> CheckEmailExists(string email);
    Task<bool> CheckPhoneNumberExists(string phoneNum);
}