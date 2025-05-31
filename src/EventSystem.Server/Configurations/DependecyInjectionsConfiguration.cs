using ContactManagement.Application.Validators;
using EventSystem.Application.Dtos;
using EventSystem.Application.Helpers;
using EventSystem.Application.Interfaces;
using EventSystem.Application.Services;
using EventSystem.Application.Validators;
using EventSystem.Infrastructure.Persistence.Repositories;
using FluentValidation;

namespace EventSystem.Server.Configurations;

public static class DependecyInjectionsConfiguration
{
    public static void ConfigureDependecies(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRoleRepository, UserRoleRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
        services.AddScoped<IValidator<UserLoginDto>, UserLoginDtoValidator>();
        services.AddScoped<IValidator<EventUpdateDto>, EventUpdateDtoValidator>();
        services.AddScoped<IValidator<EventCreateDto>, EventCreateDtoValidator>();
    }
}
