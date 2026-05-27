
namespace UsersApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    public RequestLoggingMiddleware(RequestDelegate next,ILogger<RequestLoggingMiddleware>logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task Invoke(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;
        var startTime = DateTime.UtcNow;

        _logger.LogInformation("Request : {Method} {Path} started at {startTie}",method,path,startTime);

        await _next(context);

        var statusCode = context.Response.StatusCode;
        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

        if (duration > 1000)
        {
            _logger.LogWarning(
                "SLOW REQUEST: {Method} {Path} took {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                duration);
        }

        _logger.LogInformation(
            "Response : {Method} {Path} responded {StatusCode} in {Duration} ms",
            context.Request.Method,
            context.Request.Path,
            statusCode,
            duration);

    }
}