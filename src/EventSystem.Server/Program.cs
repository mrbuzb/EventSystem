
using EventSystem.Server.Configurations;
using EventSystem.Server.Endpoints;

namespace EventSystem.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.ConfigureDataBase();
            builder.ConfigurationJwtAuth();
            builder.ConfigureJwtSettings();
            builder.ConfigureSerilog();
            builder.Services.ConfigureDependecies();
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapAdminEndpoints();
            app.MapAuthEndpoints();
            app.MapEventEndpoints();
            app.MapRoleEndpoints();

            app.MapControllers();

            app.Run();
        }
    }
}
