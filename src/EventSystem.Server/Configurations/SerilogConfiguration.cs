using Serilog;

namespace EventSystem.Server.Configurations;

public static class SelilogConfiguration
{
    public static void ConfigureSerilog(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        builder.Logging.AddSerilog();
    }
}
