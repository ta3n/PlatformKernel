# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled-extra AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /src

# Copy the rest of the code
COPY src/liberty-asp/Liberty.ApplicationShared/ liberty-asp/Liberty.ApplicationShared/
COPY src/liberty-asp/Liberty.Entity liberty-asp/Liberty.Entity
COPY src/liberty-asp/Liberty.Pagination/ liberty-asp/Liberty.Pagination/
COPY src/liberty-asp/Liberty.SentrySelfHosted/ liberty-asp/Liberty.SentrySelfHosted/
COPY src/liberty-asp/Liberty.Serilog/ liberty-asp/Liberty.Serilog/
COPY src/liberty-asp/Liberty.ServiceDefaults/ liberty-asp/Liberty.ServiceDefaults/
COPY src/liberty-asp/Liberty.Specification/ liberty-asp/Liberty.Specification/
COPY src/liberty-asp/Liberty.SysException/ liberty-asp/Liberty.SysException/
COPY src/liberty-asp/Liberty.UnitOfWork/ liberty-asp/Liberty.UnitOfWork/
COPY src/liberty-asp/Liberty.GmoPaymentGateway/ liberty-asp/Liberty.GmoPaymentGateway/
COPY src/liberty-asp/Liberty.Cache/ liberty-asp/Liberty.Cache/
COPY src/modules/Liberty.Reservation.Application/ modules/Liberty.Reservation.Application/
COPY src/modules/Liberty.Reservation.DbMigration/ modules/Liberty.Reservation.DbMigration/

WORKDIR "/src/modules/Liberty.Reservation.DbMigration"
RUN dotnet restore && \
  dotnet tool install -g dotnet-ef --version 8.0.14
ENV PATH=$PATH:/root/.dotnet/tools
RUN dotnet ef --version

WORKDIR "/src/modules/Liberty.Reservation.DbMigration"
RUN dotnet ef migrations script --idempotent -o "SqlScripts/migrations_$(date +%Y%m%d%H%M%S).sql" && \
  dotnet build --tl -c Release -o /app/build

# Publish stage
FROM build AS publish
WORKDIR "/src/modules/Liberty.Reservation.DbMigration"
RUN dotnet publish -c Release -r linux-x64 -o /app/publish \
  -p:PublishReadyToRun=true \
  -p:PublishReadyToRunComposite=true \
  -p:PublishSingleFile=false \
  -p:SelfContained=false \
  -p:UseAppHost=false

# Final stage/image
FROM base AS final
USER $APP_UID
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Liberty.Reservation.DbMigration.dll"]
