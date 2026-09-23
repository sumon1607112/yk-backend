FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.slnx ./
COPY APIGateway/YK.APIGateway/*.csproj ./APIGateway/YK.APIGateway/
COPY ServiceApplication/Auth/Core/YK.Auth.Application/*.csproj ./ServiceApplication/Auth/Core/YK.Auth.Application/
COPY ServiceApplication/Auth/Core/YK.Auth.Domain/*.csproj ./ServiceApplication/Auth/Core/YK.Auth.Domain/
COPY ServiceApplication/Auth/Infrastructure/YK.Auth.Infrastructure/*.csproj ./ServiceApplication/Auth/Infrastructure/YK.Auth.Infrastructure/
COPY ServiceApplication/Auth/Presentation/YK.Auth.WebAPI/*.csproj ./ServiceApplication/Auth/Presentation/YK.Auth.WebAPI/

RUN dotnet restore

COPY . .

RUN dotnet publish APIGateway/YK.APIGateway/YK.APIGateway.csproj -c Release -o /app/gateway
RUN dotnet publish ServiceApplication/Auth/Presentation/YK.Auth.WebAPI/YK.Auth.WebAPI.csproj -c Release -o /app/webapi

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/gateway ./gateway
COPY --from=build /app/webapi ./webapi

EXPOSE 8080

CMD sh -c "cd /app/webapi && dotnet YK.Auth.WebAPI.dll --urls http://+:5001 & sleep 5 && cd /app/gateway && dotnet YK.APIGateway.dll --urls http://+:8080"