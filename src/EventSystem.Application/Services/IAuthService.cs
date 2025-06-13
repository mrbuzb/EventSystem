using EventSystem.Application.Dtos;

namespace EventSystem.Application.Services;

public interface IAuthService
{
    Task<long> SignUpUserAsync(UserCreateDto userCreateDto);
    Task<LoginResponseDto> LoginUserAsync(UserLoginDto userLoginDto);
    Task<LoginResponseDto> RefreshTokenAsync(RefreshRequestDto request);
    Task EailCodeSender(string email);
    Task LogOut(string token);
    Task<bool> ConfirmCode(string userCode, long userId);
}