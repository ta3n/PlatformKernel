using Aspire.Hosting.Lifecycle;

namespace Liberty.Reservation.AppHost;

internal static class Extensions
{
    /// <summary>
    /// Adds a hook to set the ASPNETCORE_FORWARDEDHEADERS_ENABLED environment variable to true for all projects in the application.
    /// </summary>
    public static IDistributedApplicationBuilder AddForwardedHeaders(
        this IDistributedApplicationBuilder builder
    )
    {
        builder.Services.TryAddLifecycleHook<AddForwardHeadersHook>();
        return builder;
    }

    private sealed class AddForwardHeadersHook : IDistributedApplicationLifecycleHook
    {
        public Task BeforeStartAsync(
            DistributedApplicationModel appModel,
            CancellationToken cancellationToken = default
        )
        {
            foreach (var p in appModel.GetProjectResources())
            {
                p.Annotations.Add(
                    new EnvironmentCallbackAnnotation(
                        context =>
                        {
                            context.EnvironmentVariables["ASPNETCORE_FORWARDEDHEADERS_ENABLED"] = "true";
                        }
                    )
                );
            }

            return Task.CompletedTask;
        }
    }

    public static IResourceBuilder<T> AddWriteToSeqEnv<T>(
        this IResourceBuilder<T> builder,
        IResourceBuilder<SeqResource> seq,
        IResourceBuilder<ParameterResource> seqApiKey
    ) where T : IResourceWithEnvironment
    {
        builder
            .WithEnvironment("SerilogLogging__WriteToSeq__Enabled", "true")
            .WithEnvironment("SerilogLogging__WriteToSeq__Url", seq)
            .WithEnvironment("SerilogLogging__WriteToSeq__ApiKey", seqApiKey);

        return builder;
    }

    public static IResourceBuilder<T> AddRabbitMqEnv<T>(
        this IResourceBuilder<T> builder,
        IResourceBuilder<RabbitMQServerResource> rabbitMq,
        IResourceBuilder<ParameterResource> rabbitMqUser,
        IResourceBuilder<ParameterResource> rabbitMqPassword
    ) where T : IResourceWithEnvironment
    {
        builder
            .WithEnvironment("MessageQueueSettings__QueueType", "RabbitMq")
            .WithEnvironment("MessageQueueSettings__RabbitMqOptions__Url", rabbitMq)
            .WithEnvironment("MessageQueueSettings__RabbitMqOptions__Username", rabbitMqUser)
            .WithEnvironment("MessageQueueSettings__RabbitMqOptions__Password", rabbitMqPassword);

        return builder;
    }
}
