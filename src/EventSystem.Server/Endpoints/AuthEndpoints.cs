using System.Security.Claims;
using EventSystem.Application.Dtos;
using EventSystem.Application.Services;
using EventSystem.Core.Errors;
using Microsoft.AspNetCore.Authorization;

namespace EventSystem.Server.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        userGroup.MapPost("/send-code",
            async (string email,IAuthService _service) =>
            {
                await _service.EailCodeSender(email);
            })
            .WithName("SendCode");

        userGroup.MapPost("/confirm-code",
            async (string code,string email,IAuthService _service) =>
            {
                var res = await _service.ConfirmCode(code,email);
                return Results.Ok(res);
            })
            .WithName("ConfirmCode");

        userGroup.MapPost("/sign-up",
        async (UserCreateDto user, IAuthService _service) =>
        {
            return Results.Ok(await _service.SignUpUserAsync(user));
        })
        .WithName("SignUp");

        userGroup.MapPost("/login",
        async (UserLoginDto user, IAuthService _service) =>
        {
            return Results.Ok(await _service.LoginUserAsync(user));
        })
        .WithName("Login");

        userGroup.MapPut("/refresh-token",
        async (RefreshRequestDto refresh, IAuthService _service) =>
        {
            return Results.Ok(await _service.RefreshTokenAsync(refresh));
        })
        .WithName("RefreshToken");

        userGroup.MapDelete("/log-out",
        async (string refreshToken, IAuthService _service) =>
        {
            await _service.LogOut(refreshToken);
            return Results.Ok();
        })
        .WithName("LogOut");
    }
}
