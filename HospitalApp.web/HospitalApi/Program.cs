using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using HospitalApp.Web.HospitalApi.Data;
using HospitalApp.Web.HospitalApi.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add this helper function at the top of the file, after the using statements
static string ConvertPostgresUrlToConnectionString(string? databaseUrl)
{
    if (string.IsNullOrEmpty(databaseUrl))
        throw new ArgumentNullException(nameof(databaseUrl), "DATABASE_URL environment variable is not set");

    // Check if it's already in the correct format
    if (databaseUrl.StartsWith("Server=") || databaseUrl.StartsWith("Host="))
        return databaseUrl;

    try
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var host = uri.Host;
        var port = uri.Port;
        var database = uri.AbsolutePath.TrimStart('/');

        return $"Host={host};Port={port};Database={database};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
    }
    catch (Exception ex)
    {
        throw new ArgumentException($"Invalid database URL format: {ex.Message}");
    }
}

// Add services to the container
builder.Services.AddControllers();

// Configure environment-specific CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins(builder.Configuration["AllowedOrigins"]?.Split(',') ?? 
                             new[] { "https://your-frontend-domain.com" })
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

// Modify the database context configuration
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var connectionString = builder.Environment.IsDevelopment()
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : ConvertPostgresUrlToConnectionString(databaseUrl);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(connectionString);
        options.EnableDetailedErrors();
    }
    else
    {
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(5);
            npgsqlOptions.CommandTimeout(30);
        });
        options.ConfigureWarnings(warnings => warnings.Log(
            CoreEventId.SensitiveDataLoggingEnabled
        ));
    }
});

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("basic", () => HealthCheckResult.Healthy("Application is running"));

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "HospitalApp",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "HospitalAppUser",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ?? 
                    builder.Configuration["Jwt:Key"] ?? 
                    throw new InvalidOperationException("JWT key not configured")))
        };
    });

var app = builder.Build();

// Configure error handling for production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Configure middleware
app.UseHttpsRedirection();
app.UseRouting();

// Health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        };
        
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(result);
    }
});

// Apply CORS
app.UseCors();

// Add Authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure database is ready (with retry for production)
if (!app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var retryCount = 0;
        const int maxRetries = 3;

        while (retryCount < maxRetries)
        {
            try
            {
                logger.LogInformation("Attempting database migration...");
                await db.Database.MigrateAsync();
                logger.LogInformation("Database migration successful");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Migration attempt {retryCount + 1} failed");
                retryCount++;
                if (retryCount == maxRetries) throw;
                await Task.Delay(2000 * retryCount); // Exponential backoff
            }
        }
    }
}

app.Run();