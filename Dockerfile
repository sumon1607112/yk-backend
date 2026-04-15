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

RUN dotnet publish YK.APIGateway/YK.APIGateway.csproj -c Release -o /app/gateway
RUN dotnet publish YK.WebAPI/YK.WebAPI.csproj -c Release -o /app/webapi

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/gateway ./gateway
COPY --from=build /app/webapi ./webapi

EXPOSE 8080

CMD sh -c "dotnet /app/webapi/YK.WebAPI.dll --urls http://+:5001 & dotnet /app/gateway/YK.APIGateway.dll --urls http://+:8080"