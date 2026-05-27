using System;
using System.Net;
using System.Text.Json;

namespace UsersApi.Middleware;
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    public ErrorHandlingMiddleware(RequestDelegate next,ILogger<ErrorHandlingMiddleware>logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,"An excepted error occurred");
            await HandleExceptionAsync(context,ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context,Exception exception)
    {
        var response = new
        {
            status = 500,
            error = "Internal Server Error",
            message = "Something went wrong.Please try again later",
        };
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}