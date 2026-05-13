using System.Net;
using System.Text.Json;
using Tawasol.Application.Common.Models;

namespace Tawasol.API.Middleware;

public class MaintenanceModeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public MaintenanceModeMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var isMaintenance = _configuration.GetValue<bool>("MaintenanceMode:Enabled");
        
        if (isMaintenance && !context.User.IsInRole("Hakim"))
        {
            context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            context.Response.ContentType = "application/json";

            var response = Result.Failure("The system is currently undergoing maintenance. Please try again later.", "Maintenance Mode");
            
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(json);
            return;
        }

        await _next(context);
    }
}
