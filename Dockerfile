FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FixHub.sln .
COPY FixHub.API/FixHub.API.csproj FixHub.API/
COPY FixHub.Application/FixHub.Application.csproj FixHub.Application/
COPY FixHub.Domain/FixHub.Domain.csproj FixHub.Domain/
COPY FixHub.Infrastructure/FixHub.Infrastructure.csproj FixHub.Infrastructure/

RUN dotnet restore FixHub.API/FixHub.API.csproj

COPY . .
RUN dotnet publish FixHub.API/FixHub.API.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FixHub.API.dll"]
