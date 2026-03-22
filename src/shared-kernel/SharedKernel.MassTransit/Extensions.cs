using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.MassTransit.Constants;
using SharedKernel.MassTransit.Options;

namespace SharedKernel.MassTransit;

/// <summary>
/// Provides extension methods for configuring MassTransit within an application.
/// </summary>
public static class Extensions
{
    private const string MessageQueueSettingsSectionName = "MessageQueueSettings";

    /// <summary>
    /// Configures and adds MassTransit services using either RabbitMQ or Kafka based on the application configuration.
    /// Kafka is configured through a rider and uses the in-memory transport as the underlying MassTransit bus.
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
    /// <param name="rabbitMqFactoryConfigurator">
    /// An optional callback to configure the RabbitMQ bus factory settings with the provided <see cref="IBusRegistrationContext"/> and <see cref="IRabbitMqBusFactoryConfigurator"/>. Defaults to null if not provided.
    /// </param>
    /// <param name="riderConfigure">
    /// An optional callback to register Kafka consumers and producers on the rider when Kafka is selected.
    /// </param>
    /// <param name="kafkaFactoryConfigurator">
    /// An optional callback to configure Kafka topic endpoints and producers when Kafka is selected.
    /// </param>
    /// <param name="inMemoryFactoryConfigurator">
    /// An optional callback to customize the in-memory transport used as the base bus when Kafka is selected.
    /// </param>
    /// <param name="configureEndpoints">
    /// When set to <see langword="true"/>, invokes <see cref="IBusFactoryConfigurator{T}.ConfigureEndpoints(IBusRegistrationContext)"/> after the transport-specific configuration.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with the configured MassTransit settings.
    /// </returns>
    public static IServiceCollection AddMassTransitCustom(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configure = null,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? rabbitMqFactoryConfigurator = null,
        Action<IRiderRegistrationConfigurator>? riderConfigure = null,
        Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>? kafkaFactoryConfigurator = null,
        Action<IBusRegistrationContext, IInMemoryBusFactoryConfigurator>? inMemoryFactoryConfigurator = null,
        bool configureEndpoints = false
    )
    {
        var options = configuration
            .GetSection(MessageQueueSettingsSectionName)
            .Get<MessageQueueOptions>()
            ?? throw new InvalidOperationException(
                $"Configuration section '{MessageQueueSettingsSectionName}' is missing or invalid."
            );

        if (string.Equals(options.QueueType, MessageQueueType.RabbitMq, StringComparison.OrdinalIgnoreCase))
        {
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
                            ConfigureRabbitMq(cfg, options.RabbitMqOptions);

                            rabbitMqFactoryConfigurator?.Invoke(context, cfg);

                            if (configureEndpoints)
                            {
                                cfg.ConfigureEndpoints(context);
                            }
                        }
                    );
                }
            );

            return services;
        }

        if (string.Equals(options.QueueType, MessageQueueType.Kafka, StringComparison.OrdinalIgnoreCase))
        {
            services.AddMassTransit(
                busConfigurator =>
                {
                    configure?.Invoke(busConfigurator);

                    busConfigurator.UsingInMemory(
                        (
                            context,
                            cfg
                        ) =>
                        {
                            inMemoryFactoryConfigurator?.Invoke(context, cfg);

                            if (configureEndpoints)
                            {
                                cfg.ConfigureEndpoints(context);
                            }
                        }
                    );

                    busConfigurator.AddRider(
                        riderConfigurator =>
                        {
                            riderConfigure?.Invoke(riderConfigurator);

                            riderConfigurator.UsingKafka(
                                (
                                    context,
                                    cfg
                                ) =>
                                {
                                    var kafkaHost = options.KafkaOptions?.Host;
                                    if (string.IsNullOrWhiteSpace(kafkaHost))
                                    {
                                        throw new InvalidOperationException(
                                            "MessageQueueSettings:KafkaOptions:Host must be configured when QueueType is Kafka."
                                        );
                                    }

                                    cfg.Host(kafkaHost);
                                    kafkaFactoryConfigurator?.Invoke(context, cfg);
                                }
                            );
                        }
                    );
                }
            );

            return services;
        }

        throw new NotSupportedException(
            $"Message queue type '{options.QueueType}' is not supported."
        );
    }

    private static void ConfigureRabbitMq(
        IRabbitMqBusFactoryConfigurator cfg,
        RabbitMqOptions? rabbitMqOptions
    )
    {
        var rabbitUrl = rabbitMqOptions?.Url;
        if (string.IsNullOrWhiteSpace(rabbitUrl))
        {
            throw new InvalidOperationException(
                "MessageQueueSettings:RabbitMqOptions:Url must be configured when QueueType is RabbitMq."
            );
        }

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
    }
}
