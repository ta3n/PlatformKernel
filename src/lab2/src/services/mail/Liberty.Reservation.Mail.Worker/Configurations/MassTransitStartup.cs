using System.Reflection;
using Liberty.MassTransit;
using Liberty.MassTransit.Constants;
using Liberty.MassTransit.Options;
using Liberty.Reservation.Mail.Worker.Consumers;
using Liberty.SysIntegrationEvent;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;

namespace Liberty.Reservation.Mail.Worker.Configurations;

public static class MassTransitStartup
{
    public static IServiceCollection AddTransportModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection("MessageQueueSettings");
        var (queueType, rabbitMqOptions) = section.Get<MessageQueueOptions>()!;

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

                switch (queueType)
                {
                    case MessageQueueType.RabbitMq:
                        var rabbitUrl = rabbitMqOptions?.Url!;

                        EndpointConvention.Map<EmailSentAuditLogEvent>(
                            new Uri($"{rabbitUrl}/{ReservationQueues.EmailSentAuditLogQueue}")
                        );

                        EndpointConvention.Map<BookingAggregateFaxAuditLogEvent>(
                            new Uri($"{rabbitUrl}/{ReservationQueues.BookingAggregateFaxAuditLogQueue}")
                        );
                        break;
                }
            }
        );

        return services;
    }
}
