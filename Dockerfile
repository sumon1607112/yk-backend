FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution file
COPY *.slnx ./

# Copy all project files
COPY YK.APIGateway/*.csproj ./YK.APIGateway/
COPY YK.Application/*.csproj ./YK.Application/
COPY YK.Domain/*.csproj ./YK.Domain/
COPY YK.Infrastructure/*.csproj ./YK.Infrastructure/
COPY YK.WebAPI/*.csproj ./YK.WebAPI/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Publish the API Gateway (startup project)
RUN dotnet publish YK.APIGateway/YK.APIGateway.csproj -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "YK.APIGateway.dll"]