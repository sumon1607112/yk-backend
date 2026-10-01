var builder = DistributedApplication.CreateBuilder(args);

var auth = builder.AddProject<Projects.YK_Auth_WebAPI>("auth")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.YK_APIGateway>("gateway")
    .WithReference(auth)
    .WaitFor(auth);

builder.Build().Run();
