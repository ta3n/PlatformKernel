using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using Rebus.Config;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;
using SharedKernel.Rebus.Constants;
using SharedKernel.Rebus.Options;

namespace SharedKernel.Rebus;

/// <summary>
/// Provides extension methods for configuring Rebus in shared-kernel based services.
/// </summary>
public static class Extensions
{
    private static readonly InMemNetwork SharedInMemoryNetwork = new();

    /// <summary>
    /// Configures and registers Rebus with transport settings resolved from the <c>Rebus</c> configuration section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="configure">Optional callback for additional Rebus configuration such as routing or sagas.</param>
    /// <param name="onCreated">Optional callback invoked after the bus has been created but before it starts consuming messages.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddRebusCustom(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<RebusConfigurer, RebusConfigurer>? configure = null,
        Func<IBus, Task>? onCreated = null
    )
    {
        var section = configuration.GetSection(RebusOptions.SectionName);
        var options = section.Get<RebusOptions>()
            ?? throw new InvalidOperationException(
                $"Configuration section '{RebusOptions.SectionName}' is missing or invalid."
            );

        Validate(options);
        services.Configure<RebusOptions>(section);

        services.AddRebus(
            rebus =>
            {
                var configured = ConfigureTransport(rebus, options).Options(
                    optionsConfigurer =>
                    {
                        optionsConfigurer.SetNumberOfWorkers(options.NumberOfWorkers);
                        optionsConfigurer.SetMaxParallelism(options.MaxParallelism);
                    }
                );

                return configure?.Invoke(configured) ?? configured;
            },
            onCreated: onCreated
        );

        return services;
    }

    private static RebusConfigurer ConfigureTransport(
        RebusConfigurer rebus,
        RebusOptions options
    )
    {
        if (IsTransportType(options.TransportType, RebusTransportType.InMemory))
        {
            return rebus.Transport(
                transport => transport.UseInMemoryTransport(
                    SharedInMemoryNetwork,
                    options.InputQueueName
                )
            );
        }

        if (IsTransportType(options.TransportType, RebusTransportType.RabbitMq))
        {
            return rebus.Transport(
                transport => transport.UseRabbitMq(
                    GetRabbitMqConnectionString(options.RabbitMq),
                    options.InputQueueName
                )
            );
        }

        throw new NotSupportedException(
            $"Rebus transport type '{options.TransportType}' is not supported."
        );
    }

    private static string GetRabbitMqConnectionString(
        RebusRabbitMqOptions options
    )
    {
        return !string.IsNullOrWhiteSpace(options.ConnectionString)
            ? options.ConnectionString
            : throw new InvalidOperationException(
                "Rebus:RabbitMq:ConnectionString must be configured when TransportType is RabbitMq."
            );
    }

    private static void Validate(
        RebusOptions options
    )
    {
        if (
            !IsTransportType(options.TransportType, RebusTransportType.InMemory)
            && !IsTransportType(options.TransportType, RebusTransportType.RabbitMq)
        )
        {
            throw new NotSupportedException(
                $"Rebus transport type '{options.TransportType}' is not supported."
            );
        }

        if (string.IsNullOrWhiteSpace(options.InputQueueName))
        {
            throw new InvalidOperationException("Rebus:InputQueueName must be configured.");
        }

        if (IsTransportType(options.TransportType, RebusTransportType.RabbitMq))
        {
            _ = GetRabbitMqConnectionString(options.RabbitMq);
        }

        if (options.NumberOfWorkers <= 0)
        {
            throw new InvalidOperationException(
                "Rebus:NumberOfWorkers must be greater than zero."
            );
        }

        if (options.MaxParallelism <= 0)
        {
            throw new InvalidOperationException(
                "Rebus:MaxParallelism must be greater than zero."
            );
        }
    }

    private static bool IsTransportType(
        string transportType,
        string expectedTransportType
    )
    {
        return string.Equals(
            transportType,
            expectedTransportType,
            StringComparison.OrdinalIgnoreCase
        );
    }
}
