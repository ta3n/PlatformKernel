using Liberty.MassTransit.Constants;
using Liberty.MassTransit.Options;
using Liberty.SysIntegrationEvent;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;

namespace Liberty.Reservation.User.WebAPI.Configurations;

public static class MassTransitStartup
{
    public static IServiceCollection AddTransportModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection("MessageQueueSettings");
        var (queueType, rabbitMqOptions) = section.Get<MessageQueueOptions>()!;

        switch (queueType)
        {
            case MessageQueueType.RabbitMq:
                var rabbitUrl = rabbitMqOptions?.Url!;

                EndpointConvention.Map<BookingAggregateAuditLogEvent>(
                    new Uri($"{rabbitUrl}/{ReservationQueues.BookingAggregationAuditLogQueue}")
                );

                break;
        }

        return services;
    }
}
