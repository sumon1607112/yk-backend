using Microsoft.AspNetCore.HttpOverrides;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// CORS: only the frontend URLs from config may call the gateway from a browser
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var authKey = builder.Configuration["Services:Auth:GatewayKey"];
// Must match GatewayKeyMiddleware.HeaderName in the Auth service
const string GatewayKeyHeader = "X-Gateway-Key";

if (string.IsNullOrWhiteSpace(authKey) || authKey.Length < 32)
{
    throw new InvalidOperationException("Services:Auth:GatewayKey must be configured and at least 32 characters long.");
}


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(context =>
    {
        if (context.Route.ClusterId == "auth-cluster")
        {
            context.AddRequestHeader(GatewayKeyHeader, authKey, append: false);
        }
    });

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});

app.MapDefaultEndpoints();
app.UseCors("GatewayCorsPolicy");

// Swagger: always on in Development; in other environments only when Swagger:Enabled = true
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/auth-api/openapi/v1.json", "Auth API");
    });
}

app.MapReverseProxy();

app.Run();