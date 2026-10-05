using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using YK.Auth.Application;
using YK.Auth.Infrastructure;
using YK.Auth.Infrastructure.Abstractions.Persistence.Contexts;
using YK.Auth.Infrastructure.Abstractions.Persistence.Seeds;
using YK.Auth.WebAPI.Common.Extensions;
using YK.Auth.WebAPI.Common.Middlewares;
using YK.Auth.WebAPI.Common.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApiProblemDetails();
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.MapDefaultEndpoints();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "v1");
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// NEW: apply migrations and seed roles/admin on startup (retry if the DB is still waking up)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    const int maxAttempts = 6;
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            await db.Database.MigrateAsync();
            await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
            logger.LogInformation("Database migrated and seeded.");
            break;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            logger.LogWarning(ex, "Database not ready (attempt {Attempt}/{Max}). Retrying in 10s...", attempt, maxAttempts);
            await Task.Delay(TimeSpan.FromSeconds(10));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration/seed failed after {Max} attempts.", maxAttempts);
        }
    }
}

await app.RunAsync();