using System.Net;
using System.Text.Json;
using Tawasol.Domain.Exceptions;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Common.Exceptions;

namespace Tawasol.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        Result? result = null;

        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                // Format errors as a list of "Property: Message" for simplicity or return the dictionary directly.
                // For Flutter UI, we'll flatten the dictionary to a list of error strings for the Errors property.
                var flattenedErrors = validationException.Errors
                    .SelectMany(kvp => kvp.Value.Select(msg => $"{kvp.Key}: {msg}"))
                    .ToList();
                result = Result.Failure(flattenedErrors, "فشل التحقق من البيانات");
                break;
            case NotFoundException notFoundException:
                code = HttpStatusCode.NotFound;
                result = Result.Failure(notFoundException.Message, "العنصر غير موجود");
                break;
            case DomainException domainException:
                code = HttpStatusCode.BadRequest;
                result = Result.Failure(domainException.Message, "خطأ في منطق الأعمال");
                break;
            default:
                result = Result.Failure("حدث خطأ داخلي في الخادم", "فشلت العملية");
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(result, options));
    }
}
