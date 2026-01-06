# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY SmartCameraPro.API/*.csproj SmartCameraPro.API/
RUN dotnet restore

COPY . .
WORKDIR /src/SmartCameraPro.API
RUN dotnet publish -c Release -o /app

# Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# create uploads folder inside container
RUN mkdir -p /app/Uploads

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SmartCameraPro.API.dll"]
