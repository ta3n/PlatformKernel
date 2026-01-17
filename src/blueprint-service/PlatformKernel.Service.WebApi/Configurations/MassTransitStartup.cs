using MassTransit;
using SharedKernel.IntegrationEvent;
using SharedKernel.IntegrationEvent.Events;
using SharedKernel.MassTransit.Constants;
using SharedKernel.MassTransit.Options;

namespace PlatformKernel.Service.WebApi.Configurations;

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
                    new Uri($"{rabbitUrl}/{AppQueues.BookingAggregationAuditLogQueue}")
                );

                break;
        }

        return services;
    }
}
