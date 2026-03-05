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
COPY src/liberty-asp/Liberty.Entity/ liberty-asp/Liberty.Entity/
COPY src/liberty-asp/Liberty.Pagination/ liberty-asp/Liberty.Pagination/
COPY src/liberty-asp/Liberty.SentrySelfHosted/ liberty-asp/Liberty.SentrySelfHosted/
COPY src/liberty-asp/Liberty.Serilog/ liberty-asp/Liberty.Serilog/
COPY src/liberty-asp/Liberty.ServiceDefaults/ liberty-asp/Liberty.ServiceDefaults/
COPY src/liberty-asp/Liberty.Specification/ liberty-asp/Liberty.Specification/
COPY src/liberty-asp/Liberty.SysException/ liberty-asp/Liberty.SysException/
COPY src/liberty-asp/Liberty.UnitOfWork/ liberty-asp/Liberty.UnitOfWork/
COPY src/liberty-asp/Liberty.Cache/ liberty-asp/Liberty.Cache/
COPY src/liberty-asp/Liberty.GmoPaymentGateway/ liberty-asp/Liberty.GmoPaymentGateway/
COPY src/liberty-asp/Liberty.Hangfire/ liberty-asp/Liberty.Hangfire/
COPY src/liberty-asp/Liberty.SysIntegrationEvent/ liberty-asp/Liberty.SysIntegrationEvent/
COPY src/liberty-asp/Liberty.MassTransit/ liberty-asp/Liberty.MassTransit/
COPY src/modules/Liberty.Reservation.Application/ modules/Liberty.Reservation.Application/
COPY src/services/manager/Liberty.Reservation.Manager.Application/ services/manager/Liberty.Reservation.Manager.Application/
COPY src/services/manager/Liberty.Reservation.Manager.WebAPI.Application/ services/manager/Liberty.Reservation.Manager.WebAPI.Application/
COPY src/services/manager/Liberty.Reservation.Manager.WebAPI/ services/manager/Liberty.Reservation.Manager.WebAPI/

WORKDIR "/src/services/manager/Liberty.Reservation.Manager.WebAPI"
RUN dotnet restore

WORKDIR "/src/services/manager/Liberty.Reservation.Manager.WebAPI"
RUN dotnet build --tl -c Release -o /app/build
# RUN chmod 644 /app/publish/Certs/liberty.pfx
# RUN dotnet dev-certs https
# RUN dotnet dev-certs https --trust

# Publish stage
FROM build AS publish
WORKDIR "/src/services/manager/Liberty.Reservation.Manager.WebAPI"
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
ENTRYPOINT ["dotnet", "Liberty.Reservation.Manager.WebAPI.dll"]
