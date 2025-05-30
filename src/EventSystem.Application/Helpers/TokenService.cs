using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EventSystem.Application.Dtos;
using EventSystem.Application.Settings;
using Microsoft.IdentityModel.Tokens;

namespace EventSystem.Application.Helpers;

public class TokenService : ITokenService
{
    private readonly string _lifetime;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _securityKey;

    public TokenService(JwtAppSettings jwtSetting)
    {
        _lifetime = jwtSetting.Lifetime;
        _issuer = jwtSetting.Issuer;
        _audience = jwtSetting.Audience;
        _securityKey = jwtSetting.SecurityKey;
    }

    public string GenerateToken(UserGetDto user)
    {
        var IdentityClaims = new Claim[]
        {
            new Claim("UserId",user.UserId.ToString()),
            new Claim("FirstName",user.FirstName.ToString()),
            new Claim("LastName",user.LastName.ToString()),
            new Claim("PhoneNumber",user.PhoneNumber.ToString()),
            new Claim("UserName",user.UserName.ToString()),
            new Claim(ClaimTypes.Role,user.Role.ToString()),
            new Claim(ClaimTypes.Email,user.Email.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey!));
        var keyCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresHours = int.Parse(_lifetime);
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: IdentityClaims,
            expires: TimeHelper.GetDateTime().AddHours(expiresHours),
            signingCredentials: keyCredentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey!))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
    }


}
