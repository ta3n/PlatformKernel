using MassTransit;
using PlatformKernel.IntegrationEvent;
using PlatformKernel.IntegrationEvent.Events;
using PlatformKernel.MassTransit.Constants;
using PlatformKernel.MassTransit.Options;

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
