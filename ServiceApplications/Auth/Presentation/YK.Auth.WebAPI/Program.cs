using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using YK.Auth.Application;
using YK.Auth.Infrastructure;
using YK.Auth.Infrastructure.Persistence;
using YK.Auth.Infrastructure.Persistence.Seeds;
using YK.Auth.WebAPI.Common.Extensions;
using YK.Auth.WebAPI.Common.Middlewares;
using YK.Auth.WebAPI.Common.OpenApi;
using YK.Auth.WebAPI.Common.Options;

var builder = WebApplication.CreateBuilder(args);


builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApiProblemDetails();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOptions<GatewayOptions>()
    .Bind(builder.Configuration.GetSection(GatewayOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.SharedSecret) && o.SharedSecret.Length >= 32, "Gateway:SharedSecret must be configured and at least 32 characters long.")
    .ValidateOnStart();

var app = builder.Build();

// Fail fast on bad config before touching the database
_ = app.Services.GetRequiredService<IOptions<GatewayOptions>>().Value;

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.MapDefaultEndpoints();
app.UseMiddleware<ExceptionHandlingMiddleware>();
// Swagger: always on in Development; in other environments only when Swagger:Enabled = true
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        await db.Database.MigrateAsync();
        await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
        logger.LogInformation("Database migrated and seeded.");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Failed to migrate or seed the database.");
        throw;
    }
}

await app.RunAsync();