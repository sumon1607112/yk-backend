FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution file
COPY *.sln ./

# Copy all project files
COPY APIGateway/YK.APIGateway/*.csproj ./APIGateway/YK.APIGateway/
COPY ServiceApplication/Auth/Core/YK.Application/*.csproj ./ServiceApplication/Auth/Core/YK.Application/
COPY ServiceApplication/Auth/Core/YK.Domain/*.csproj ./ServiceApplication/Auth/Core/YK.Domain/
COPY ServiceApplication/Auth/Infrastructure/YK.Infrastructure/*.csproj ./ServiceApplication/Auth/Infrastructure/YK.Infrastructure/
COPY ServiceApplication/Auth/Presentation/YK.WebAPI/*.csproj ./ServiceApplication/Auth/Presentation/YK.WebAPI/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Publish the API Gateway (startup project)
RUN dotnet publish APIGateway/YK.APIGateway/YK.APIGateway.csproj -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "YK.APIGateway.dll"]