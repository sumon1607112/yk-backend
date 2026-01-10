var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => 
    policy.WithOrigins("http://localhost:50000")
          .AllowAnyHeader()
          .AllowAnyMethod());
});

// 1. Add Services
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Built-in .NET 10 Generator

var app = builder.Build();

app.UseCors();

// 2. Configure HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseAuthorization();
app.MapControllers();

app.Run();