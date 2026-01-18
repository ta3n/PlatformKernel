using Liberty.ApplicationShared.Cqrs;
using Liberty.ApplicationShared.Cqrs.BaseCommand;

namespace Liberty.Reservation.Application.Cqrs.BaseCommands;

public abstract record CommandBase<TResponse> : RequestBase<TResponse>, ICommandBase<TResponse>;

public abstract record CreateCommandBase<TModel, TResponse>
    : RequestBase<TResponse>, ICreateCommandBase<TModel, TResponse>
    where TModel : class
{
    public required TModel Payload { get; set; }
}

public abstract record UpdateCommandBase<TModel, TResponse>
    : RequestBase<TResponse>, IUpdateCommandBase<TModel, TResponse>
    where TModel : class
{
    public required TModel Payload { get; set; }
}

public abstract record DeleteCommandBase<TModel, TResponse>
    : RequestBase<TResponse>, IDeleteCommandBase<TModel, TResponse>
    where TModel : class
{
    public required TModel Payload { get; set; }
}
