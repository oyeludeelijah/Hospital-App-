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
using Npgsql;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

static string ConvertPostgresUrlToConnectionString(string? databaseUrl)
{
    if (string.IsNullOrEmpty(databaseUrl))
        throw new ArgumentNullException(nameof(databaseUrl), "DATABASE_URL environment variable is not set");

    if (databaseUrl.StartsWith("Server=") || databaseUrl.StartsWith("Host="))
        return databaseUrl;

    try
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432; // Default to 5432 if port is not specified
        var database = uri.AbsolutePath.TrimStart('/');

        if (string.IsNullOrEmpty(host))
            throw new ArgumentException("Host is missing from the database URL");
        if (userInfo.Length != 2)
            throw new ArgumentException("Username and password are required in the database URL");
        if (string.IsNullOrEmpty(database))
            throw new ArgumentException("Database name is missing from the database URL");

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = port,
            Database = database,
            Username = userInfo[0],
            Password = userInfo[1],
            SslMode = SslMode.Require,
            TrustServerCertificate = true,
            Pooling = true,
            MinPoolSize = 0,
            MaxPoolSize = 100,
            ConnectionIdleLifetime = 300
        };

        return builder.ToString();
    }
    catch (Exception ex)
    {
        throw new ArgumentException($"Invalid database URL format: {ex.Message}", ex);
    }
}

builder.Services.AddControllers();

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
        options.ConfigureWarnings(warnings => 
        {
            warnings.Log(RelationalEventId.ConnectionOpened, RelationalEventId.ConnectionClosed);
            warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning);
        });
        // Disable change tracking for better performance in production
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
});

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHealthChecks()
    .AddCheck("basic", () => HealthCheckResult.Healthy("Application is running"));

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

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // These headers are set by Render
    options.KnownProxies.Clear();
    options.KnownNetworks.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

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

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "Welcome to the Hospital API!");

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
                
                // Ensure database exists - REMOVED FOR PRODUCTION MIGRATION FLOW
                // await db.Database.EnsureCreatedAsync(); 
                // Only rely on MigrateAsync for schema changes in production

                // Apply migrations
                await db.Database.MigrateAsync();
                logger.LogInformation("Database migration successful");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Migration attempt {retryCount + 1} failed");
                retryCount++;
                if (retryCount == maxRetries)
                {
                    logger.LogCritical("All migration attempts failed. Application startup will be terminated.");
                    throw;
                }
                await Task.Delay(2000 * retryCount);
            }
        }
    }
}

app.Run(); 