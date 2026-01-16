using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlatformKernel.MassTransit.Constants;
using PlatformKernel.MassTransit.Options;

namespace PlatformKernel.MassTransit;

/// <summary>
/// Provides extension methods for configuring MassTransit within an application.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Configures and adds the MassTransit services with RabbitMQ as the message broker to the service collection.
    /// Enables customization of the bus registration and RabbitMQ bus factory configurators as needed.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the MassTransit services will be added.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> containing the "MessageQueueSettings" section for RabbitMQ configuration options.
    /// </param>
    /// <param name="configure">
    /// An optional callback to customize the <see cref="IBusRegistrationConfigurator"/> during the MassTransit setup. Defaults to null if not provided.
    /// </param>
    /// <param name="factoryConfigurator">
    /// An optional callback to configure the RabbitMQ bus factory settings with the provided <see cref="IBusRegistrationContext"/> and <see cref="IRabbitMqBusFactoryConfigurator"/>. Defaults to null if not provided.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with the configured MassTransit and RabbitMQ settings.
    /// </returns>
    public static IServiceCollection AddMassTransitCustom(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configure = null,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? factoryConfigurator = null
    )
    {
        var section = configuration.GetSection("MessageQueueSettings");
        var (queueType, rabbitMqOptions) = section.Get<MessageQueueOptions>()!;

        switch (queueType)
        {
            case MessageQueueType.RabbitMq:

                services.AddMassTransit(
                    busConfigurator =>
                    {
                        configure?.Invoke(busConfigurator);

                        busConfigurator.UsingRabbitMq(
                            (
                                context,
                                cfg
                            ) =>
                            {
                                var rabbitUrl = rabbitMqOptions?.Url!;
                                cfg.Host(
                                    new Uri(rabbitUrl),
                                    "/",
                                    hostConfigurator =>
                                    {
                                        hostConfigurator.Username(rabbitMqOptions?.Username ?? "guest");
                                        hostConfigurator.Password(rabbitMqOptions?.Password ?? "guest");
                                        hostConfigurator.PublisherConfirmation = true;
                                        hostConfigurator.ConfigureBatchPublish(
                                            batch =>
                                            {
                                                batch.Enabled = true;
                                            }
                                        );
                                    }
                                );

                                cfg.SetQueueArgument("durable", true);
                                cfg.SetQueueArgument("prefetch-count", 16);
                                cfg.SetQueueArgument("x-queue-mode", "lazy");

                                factoryConfigurator?.Invoke(context, cfg);
                            }
                        );
                    }
                );

                break;
        }

        return services;
    }
}
