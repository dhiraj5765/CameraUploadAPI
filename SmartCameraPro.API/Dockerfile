# Use .NET 8 SDK to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution & project files
COPY SmartCameraPro.API/SmartCameraPro.API.csproj SmartCameraPro.API/
RUN dotnet restore SmartCameraPro.API/SmartCameraPro.API.csproj

# Copy everything
COPY . .

# Build API
RUN dotnet publish SmartCameraPro.API/SmartCameraPro.API.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SmartCameraPro.API.dll"]
