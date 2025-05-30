using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSystem.Application.Interfaces;
using EventSystem.Core.Errors;
using EventSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventSystem.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext _context) : IUserRepository
{
    public async Task<long> AddUserAync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user.UserId;
    }

    public Task<bool> CheckUserById(long userId) => _context.Users.AnyAsync(x => x.UserId == userId);

    public Task<bool> CheckUsernameExists(string username) => _context.Users.AnyAsync(_ => _.UserName == username);

    public Task<bool> CheckEmailExists(string email) => _context.Users.AnyAsync(_ => _.Email == email);

    public Task<bool> CheckPhoneNumberExists(string phoneNum) => _context.Users.AnyAsync(_ => _.PhoneNumber == phoneNum);

    public async Task DeleteUserByIdAsync(long userId)
    {
        var user = await GetUserByIdAync(userId);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetUserByIdAync(long id)
    {
        var user = await _context.Users.Include(_ => _.Role).FirstOrDefaultAsync(x => x.UserId == id);
        if (user == null)
        {
            throw new EntityNotFoundException($"Entity with {id} not found");
        }
        return user;
    }

    public async Task<User> GetUserByUserNameAync(string userName)
    {
        var user = await _context.Users.Include(_ => _.Role).FirstOrDefaultAsync(x => x.UserName == userName);
        if (user == null)
        {
            throw new EntityNotFoundException($"Entity with {userName} not found");
        }
        return user;
    }

    public async Task UpdateUserRoleAsync(long userId, string userRole)
    {
        var user = await GetUserByIdAync(userId);
        var role = await _context.UserRoles.FirstOrDefaultAsync(x => x.Name == userRole);
        if (role == null)
        {
            throw new EntityNotFoundException($"Role : {userRole} not found");
        }
        user.RoleId = role.Id;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
