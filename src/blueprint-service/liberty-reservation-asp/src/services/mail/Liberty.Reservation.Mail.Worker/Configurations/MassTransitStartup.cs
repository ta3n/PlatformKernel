using Liberty.MassTransit;
using Liberty.Reservation.Mail.Worker.Consumers;
using Liberty.SysIntegrationEvent;
using MassTransit;
using System.Reflection;

namespace Liberty.Reservation.Mail.Worker.Configurations;

public static class MassTransitStartup
{
    public static IServiceCollection AddTransportModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMassTransitCustom(
            configuration,
            configure =>
            {
                var entryAssembly = Assembly.GetExecutingAssembly();
                configure.AddConsumers(entryAssembly);
            },
            (
                context,
                busFactoryConfigurator
            ) =>
            {
                busFactoryConfigurator.ReceiveEndpoint(
                    ReservationQueues.BookingSendMailQueue,
                    endpointConfigurator =>
                    {
                        endpointConfigurator.Durable = true;
                        endpointConfigurator.PrefetchCount = 16;
                        endpointConfigurator.UseMessageRetry(
                            retryConfig => retryConfig.Incremental(
                                5,
                                TimeSpan.FromSeconds(30),
                                TimeSpan.FromSeconds(30)
                            )
                        );
                        endpointConfigurator.Consumer<BookingSendMailConsumer>(context);
                    }
                );
            }
        );

        return services;
    }
}
