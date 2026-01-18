using System.Text.Json;
using Liberty.SysIntegrationEvent.Base;
using MassTransit;
using MassTransit.Internals;

namespace Liberty.Reservation.Mail.Worker.Consumers.Base;

public abstract class BaseConsumer<TMessage>(
    ILogger<BaseConsumer<TMessage>> logger
) : IConsumer<TMessage> where TMessage : BaseJobEvent
{
    public virtual async Task Consume(
        ConsumeContext<TMessage> context
    )
    {
        var consumerName = typeof(BaseConsumer<TMessage>).GetTypeName();

        logger.LogInformation(
            "{ConsumerName} - Execute: {Message}",
            consumerName,
            JsonSerializer.Serialize(context.Message)
        );

        await ExecuteAsync(context);
    }

    protected abstract Task ExecuteAsync(
        ConsumeContext<TMessage> context
    );
}
