var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("GatewayCorsPolicy", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("GatewayCorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        // This goes through the regular auth-route
        options.SwaggerEndpoint("/auth-api/openapi/v1.json", "Auth API");
    });
}

app.UseHttpsRedirection();
app.MapReverseProxy();
app.Run();