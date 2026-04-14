FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.slnx ./
COPY YK.APIGateway/*.csproj ./YK.APIGateway/
COPY YK.Application/*.csproj ./YK.Application/
COPY YK.Domain/*.csproj ./YK.Domain/
COPY YK.Infrastructure/*.csproj ./YK.Infrastructure/
COPY YK.WebAPI/*.csproj ./YK.WebAPI/

RUN dotnet restore

COPY . .

RUN dotnet publish YK.WebAPI/YK.WebAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "YK.WebAPI.dll"]