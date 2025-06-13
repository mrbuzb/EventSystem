
using EventSystem.Server.Configurations;
using EventSystem.Server.Endpoints;
using EventSystem.Server.Middlewares;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

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




            builder.ConfigureSerilog();
            builder.ConfigureDataBase();
            builder.ConfigurationJwtAuth();
            builder.ConfigureJwtSettings();
            //builder.ConfigureSerilog();
            builder.Services.ConfigureDependecies();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = "swagger"; // <-- faqat /swagger'da ochiladi
                });
            }


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapAdminEndpoints();
            app.MapAuthEndpoints();
            app.MapEventEndpoints();
            app.MapRoleEndpoints();

            app.MapControllers();

            app.Run();
        }
    }
}
