using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedKernel.MassTransit;
using SharedKernel.MassTransit.Test.Consumers;
using SharedKernel.MassTransit.Test.Data;
using SharedKernel.MassTransit.Test.Sagas;

namespace SharedKernel.MassTransit.Test.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMassTransitTestApplication(
        this IServiceCollection services,
        IConfiguration configuration,
        AppOptions appOptions
    )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

        services.AddDbContext<MassTransitTestDbContext>(
            options =>
            {
                options.UseNpgsql(connectionString);
            }
        );

        services.AddMassTransitCustom(
            configuration,
            configure: configurator =>
            {
                configurator.AddEntityFrameworkOutboxCustom<MassTransitTestDbContext>();
                configurator.UseEntityFrameworkOutboxForConfiguredEndpoints<MassTransitTestDbContext>();

                RegisterRoleServices(configurator, appOptions.Role);
            },
            configureEndpoints: true
        );

        return services;
    }

    private static void RegisterRoleServices(
        IBusRegistrationConfigurator configurator,
        string role
    )
    {
        if (AppRoles.IsApi(role))
        {
            configurator.AddConsumer<OrderCompletedConsumer, OrderCompletedConsumerDefinition>();
            return;
        }

        if (string.Equals(role, AppRoles.Processor, StringComparison.OrdinalIgnoreCase))
        {
            configurator.AddConsumer<ReserveInventoryConsumer, ReserveInventoryConsumerDefinition>();
            configurator.AddConsumer<ProcessPaymentConsumer, ProcessPaymentConsumerDefinition>();
            return;
        }

        configurator.AddSagaStateMachineWithEntityFrameworkRepository<
            OrderStateMachine,
            OrderState,
            OrderStateDefinition,
            MassTransitTestDbContext
        >(concurrencyMode: ConcurrencyMode.Pessimistic);
    }
}
