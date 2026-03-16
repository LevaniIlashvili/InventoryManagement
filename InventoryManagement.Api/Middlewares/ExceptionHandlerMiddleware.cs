using InventoryManagement.Application.Exceptions;
using InventoryManagement.Application.Exceptionsl;
using InventoryManagement.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace InventoryManagement.Api.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
            await ConvertException(context, ex);
        }
    }

    private Task ConvertException(HttpContext context, Exception ex)
    {
        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

        context.Response.ContentType = "application/json";

        var result = string.Empty;

        switch (ex)
        {
            //case ValidationException validationException:
            //    httpStatusCode = HttpStatusCode.BadRequest;

            //    var errorMessage = string.Join("; ", validationException.Errors
            //        .Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            //    result = JsonSerializer.Serialize(new
            //    {
            //        error = errorMessage
            //    });
            //    break;

            case DomainException or BadRequestException:
                httpStatusCode = HttpStatusCode.BadRequest;
                break;
            case NotFoundException:
                httpStatusCode = HttpStatusCode.NotFound;
                break;
            default:
                _logger.LogError(ex, "An unexpected error occurred.");
                result = JsonSerializer.Serialize(new { error = "An unexpected error occurred. Please try again later." });
                break;
        }

        context.Response.StatusCode = (int)httpStatusCode;

        if (result == string.Empty)
        {
            result = JsonSerializer.Serialize(new { error = ex.Message });
        }

        return context.Response.WriteAsync(result);
    }
}