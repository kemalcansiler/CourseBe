# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

# Use the .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["Course.Api/Course.Api.csproj", "Course.Api/"]
COPY ["Course.Core/Course.Core.csproj", "Course.Core/"]
COPY ["Course.Application/Course.Application.csproj", "Course.Application/"]
COPY ["Course.Infrastructure/Course.Infrastructure.csproj", "Course.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Course.Api/Course.Api.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/Course.Api"
RUN dotnet build "Course.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Course.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

ENTRYPOINT ["dotnet", "Course.Api.dll"]
