using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using YK.Auth.Application;
using YK.Auth.Infrastructure;
using YK.Auth.Infrastructure.Abstractions.Persistence.Contexts;
using YK.Auth.WebAPI.Common.Extensions;
using YK.Auth.WebAPI.Common.Middlewares;
using YK.Auth.WebAPI.Common.OpenApi;

var builder = WebApplication.CreateBuilder(args);

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

// Auto-apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();