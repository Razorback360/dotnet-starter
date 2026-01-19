using System.Net;
using System.Text.Json;
using FluentValidation;

namespace CarDealer.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = "An error occurred",
            Details = null
        };

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Validation failed";
                response.Details = validationException.Errors.Select(e => new
                {
                    property = e.PropertyName,
                    error = e.ErrorMessage
                }).ToList();
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "Unauthorized access";
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = "Resource not found";
                break;

            case InvalidOperationException invalidOpException:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Message = invalidOpException.Message;
                break;

            case ArgumentException argumentException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = argumentException.Message;
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An internal server error occurred";
                _logger.LogError(exception, "Internal server error: {Message}", exception.Message);
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public object? Details { get; set; }
    }
}
