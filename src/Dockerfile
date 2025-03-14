# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/OrderManagement.API/OrderManagement.API.csproj", "API/OrderManagement.API/"]
COPY ["BuildingBlocks/Common.Logging/Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]
COPY ["BuildingBlocks/OrderManagement.BuildingBlocks.Domain/OrderManagement.BuildingBlocks.Domain.csproj", "BuildingBlocks/OrderManagement.BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/OrderManagement.BuildingBlocks.Application/OrderManagement.BuildingBlocks.Application.csproj", "BuildingBlocks/OrderManagement.BuildingBlocks.Application/"]
COPY ["BuildingBlocks/OrderManagement.BuildingBlocks.Infrastructure/OrderManagement.BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/OrderManagement.BuildingBlocks.Infrastructure/"]

COPY ["Modules/UserAccess/OrderManagement.Modules.UserAccess.Domain/OrderManagement.Modules.UserAccess.Domain.csproj", "Modules/UserAccess/OrderManagement.Modules.UserAccess.Domain/"]
COPY ["Modules/UserAccess/OrderManagement.Modules.UserAccess.Application/OrderManagement.Modules.UserAccess.Application.csproj", "Modules/UserAccess/OrderManagement.Modules.UserAccess.Application/"]
COPY ["Modules/UserAccess/OrderManagement.Modules.UserAccess.Infrastructure/OrderManagement.Modules.UserAccess.Infrastructure.csproj", "Modules/UserAccess/OrderManagement.Modules.UserAccess.Infrastructure/"]
COPY ["Modules/UserAccess/OrderManagement.Modules.UserAccess.IntegrationEvents/OrderManagement.Modules.UserAccess.IntegrationEvents.csproj", "Modules/UserAccess/OrderManagement.Modules.UserAccess.IntegrationEvents/"]

COPY ["Modules/Registrations/OrderManagement.Modules.Registrations.Domain/OrderManagement.Modules.Registrations.Domain.csproj", "Modules/Registrations/OrderManagement.Modules.Registrations.Domain/"]
COPY ["Modules/Registrations/OrderManagement.Modules.Registrations.Application/OrderManagement.Modules.Registrations.Application.csproj", "Modules/Registrations/OrderManagement.Modules.Registrations.Application/"]
COPY ["Modules/Registrations/OrderManagement.Modules.Registrations.Infrastructure/OrderManagement.Modules.Registrations.Infrastructure.csproj", "Modules/Registrations/OrderManagement.Modules.Registrations.Infrastructure/"]
COPY ["Modules/Registrations/OrderManagement.Modules.Registrations.IntegrationEvents/OrderManagement.Modules.Registrations.IntegrationEvents.csproj", "Modules/Registrations/OrderManagement.Modules.Registrations.IntegrationEvents/"]

COPY ["Modules/Products/OrderManagement.Modules.Products.Domain/OrderManagement.Modules.Products.Domain.csproj", "Modules/Products/OrderManagement.Modules.Products.Domain/"]
COPY ["Modules/Products/OrderManagement.Modules.Products.Application/OrderManagement.Modules.Products.Application.csproj", "Modules/Products/OrderManagement.Modules.Products.Application/"]
COPY ["Modules/Products/OrderManagement.Modules.Products.Infrastructure/OrderManagement.Modules.Products.Infrastructure.csproj", "Modules/Products/OrderManagement.Modules.Products.Infrastructure/"]
COPY ["Modules/Products/OrderManagement.Modules.Products.IntegrationEvents/OrderManagement.Modules.Products.IntegrationEvents.csproj", "Modules/Products/OrderManagement.Modules.Products.IntegrationEvents/"]

COPY ["Modules/Orders/OrderManagement.Modules.Orders.Domain/OrderManagement.Modules.Orders.Domain.csproj", "Modules/Orders/OrderManagement.Modules.Orders.Domain/"]
COPY ["Modules/Orders/OrderManagement.Modules.Orders.Application/OrderManagement.Modules.Orders.Application.csproj", "Modules/Orders/OrderManagement.Modules.Orders.Application/"]
COPY ["Modules/Orders/OrderManagement.Modules.Orders.Infrastructure/OrderManagement.Modules.Orders.Infrastructure.csproj", "Modules/Orders/OrderManagement.Modules.Orders.Infrastructure/"]
COPY ["Modules/Orders/OrderManagement.Modules.Orders.IntegrationEvents/OrderManagement.Modules.Orders.IntegrationEvents.csproj", "Modules/Orders/OrderManagement.Modules.Orders.IntegrationEvents/"]

RUN dotnet restore "API/OrderManagement.API/OrderManagement.API.csproj"
COPY . .
WORKDIR "/src/API/OrderManagement.API"
RUN dotnet build "OrderManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderManagement.API.dll"]