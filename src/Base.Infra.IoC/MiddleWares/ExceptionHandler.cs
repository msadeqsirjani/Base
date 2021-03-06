namespace Base.Infra.IoC.MiddleWares;

public class ExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogService _logService;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandler(RequestDelegate next, ILogService logService, IWebHostEnvironment environment)
    {
        _next = next;
        _logService = logService;
        _environment = environment;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 200;


        switch (exception)
        {
            case MessageException messageException:
                return context.Response.WriteAsync(JsonSerializer.Serialize(Result.WithException(messageException.Message)));
            case UnAuthorizedException unAuthorizedException:
                {
                    context.Response.StatusCode = 401;

                    return context.Response.WriteAsync(
                        JsonSerializer.Serialize(Result.WithException(unAuthorizedException.Message)));
                }
            default:
                _logService.LogError(exception);

                return context.Response.WriteAsync(_environment.IsDevelopment()
                    ? JsonSerializer.Serialize(Result.WithException(exception))
                    : JsonSerializer.Serialize(Result.WithException(Statement.Failure)));
        }
    }
}