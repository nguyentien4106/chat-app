# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 6060
EXPOSE 6061


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChatApp.Api/ChatApp.Api.csproj", "ChatApp.Api/"]
COPY ["ChatApp.Application/ChatApp.Application.csproj", "ChatApp.Application/"]
COPY ["ChatApp.Domain/ChatApp.Domain.csproj", "ChatApp.Domain/"]
COPY ["ChatApp.Infrastructure/ChatApp.Infrastructure.csproj", "ChatApp.Infrastructure/"]
RUN dotnet restore "./ChatApp.Api/ChatApp.Api.csproj"
COPY . .
WORKDIR "/src/ChatApp.Api"
RUN dotnet build "./ChatApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChatApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatApp.Api.dll"]