using EventSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventSystem.Server.Configurations;

public static class DatabaseConfigurations
{
    public static void ConfigureDataBase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");

        builder.Services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(connectionString));
    }
}
