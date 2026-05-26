using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using Coravel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Tawasol.API.Endpoints;
using Tawasol.API.Middleware;
using Tawasol.Application;
using Tawasol.Application.BackgroundJobs;
using Tawasol.Infrastructure;
using Tawasol.Infrastructure.Hubs;
using Tawasol.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. Logging (Serilog)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/tawasol_log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// 2. Service Registration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tawasol API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Layer-specific registrations
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Auth Registration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        RoleClaimType = ClaimTypes.Role,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromSeconds(60);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // allow any origin
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // SignalR requires credentials
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; 
        options.JsonSerializerOptions.WriteIndented = true; 
    });

// Coravel Scheduler
builder.Services.AddScheduler();
builder.Services.AddTransient<PledgeExpirationJob>();

var app = builder.Build();

// 3. Middleware Configuration
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<MaintenanceModeMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });

    // Temporarily disable automatic migration to break the cycle
    await DatabaseInitializer.InitializeDatabaseAsync(app.Services);
    await IdentityDataSeeder.SeedDataAsync(app.Services);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Setup Coravel Job
app.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<PledgeExpirationJob>()
             .Hourly(); // Runs every hour to check for 24h old pledges
});

// 4. Endpoint Mapping
app.MapHealthChecks("/health");
app.MapAuthEndpoints();
app.MapCaseEndpoints();
app.MapDonationEndpoints();
app.MapUserEndpoints();
app.MapAdminEndpoints();
app.MapHub<CaseHub>("/hubs/cases");
app.MapGet("/ping", () => "pong");

try
{
    Log.Information(">>> Tawasol API Starting...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, ">>> Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
