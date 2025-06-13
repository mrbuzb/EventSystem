using System.Net;
using System.Text.Json;
using EventSystem.Core.Errors;

namespace EventSystem.Server.Middlewares;
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = 500;
        context.Response.ContentType = "application/json";

        if(exception is EntityNotFoundException)
        {
            code = 404;
        }
        else if (exception is AuthException || exception is UnauthorizedException)
        {
            code = 401;
        }
        else if (exception is ForbiddenException || exception is NotAllowedException)
        {
            code = 403;
        }

        context.Response.StatusCode = code;

        var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Serverda xatolik yuz berdi. Iltimos, keyinroq urinib ko‘ring.",
                Detail = exception.Message
            };

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}
