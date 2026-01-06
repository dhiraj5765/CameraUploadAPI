# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj file first
COPY SmartCameraPro.API/*.csproj ./SmartCameraPro.API/
WORKDIR /src/SmartCameraPro.API
RUN dotnet restore

# copy everything else
WORKDIR /src
COPY . .

# publish app
WORKDIR /src/SmartCameraPro.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SmartCameraPro.API.dll"]
