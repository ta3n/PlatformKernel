using MediatR;

namespace Liberty.ApplicationShared.Cqrs.BaseCommand;

public interface ICommandHandlerBase<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;

public interface ICreateCommandHandlerBase<in TCommand, TResponse> : ICommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;

public interface IUpdateCommandHandlerBase<in TCommand, TResponse> : ICommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;

public interface IDeleteCommandHandlerBase<in TCommand, TResponse> : ICommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;
