using EventSystem.Application.Settings;

namespace EventSystem.Server.Configurations;

public static class JwtSettingsConfiguration
{
    public static void ConfigureJwtSettings(this WebApplicationBuilder builder)
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");

        var lifetime = jwtSection["Lifetime"];
        var securityKey = jwtSection["SecurityKey"];
        var audience = jwtSection["Audience"];
        var issuer = jwtSection["Issuer"];

        var jwtSettings = new JwtAppSettings(issuer, audience, securityKey, lifetime);

        builder.Services.AddSingleton<JwtAppSettings>(jwtSettings);
    }
}
