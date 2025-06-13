using EventSystem.Domain.Entities;

namespace EventSystem.Application.Interfaces;

public interface IUserRepository
{
    Task<long> AddUserAync(User user);
    Task AddConfirmer(UserConfirme confirmer);
    Task<User> GetUserByIdAync(long id);
    Task UpdateUser(User user);
    Task<User> GetUserByEmail(string email);
    Task<User> GetUserByUserNameAync(string userName);
    Task UpdateUserRoleAsync(long userId, string userRole);
    Task DeleteUserByIdAsync(long userId);
    Task<bool> CheckUserById(long userId);
    Task<bool> CheckUsernameExists(string username);
    Task<long?> CheckEmailExistsAsync(string email);
    Task<bool> CheckPhoneNumberExists(string phoneNum);
}