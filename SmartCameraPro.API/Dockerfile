# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SmartCameraPro.API/*.csproj SmartCameraPro.API/
RUN dotnet restore SmartCameraPro.API/SmartCameraPro.API.csproj

COPY . .
WORKDIR /src/SmartCameraPro.API
RUN dotnet publish -c Release -o /app

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet","SmartCameraPro.API.dll"]
