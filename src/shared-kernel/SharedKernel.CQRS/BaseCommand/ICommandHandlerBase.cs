using MediatR;

namespace SharedKernel.CQRS.BaseCommand;

/// <summary>
/// Represents the base interface for command handlers in the CQRS pattern.
/// </summary>
/// <typeparam name="TCommand">The type of the command being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
public interface ICommandHandlerBase<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;

/// <summary>
/// Defines a handler interface for processing create commands derived from <see cref="ICommandBase{TResponse}"/>.
/// </summary>
/// <typeparam name="TCommand">The type of the create command to be handled, inheriting from <see cref="ICommandBase{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The response type returned after the create command execution.</typeparam>
public interface ICreateCommandHandlerBase<in TCommand, TResponse> : ICommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;

/// <summary>
/// Represents a contract for handling update commands within the application.
/// </summary>
/// <typeparam name="TCommand">The type of the command that this handler processes.</typeparam>
/// <typeparam name="TResponse">The type of the result returned after processing the command.</typeparam>
/// <remarks>
/// This interface establishes a standard for implementing update command handlers, ensuring consistency
/// across various command-processing scenarios.
/// </remarks>
public interface IUpdateCommandHandlerBase<in TCommand, TResponse> : ICommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;

/// <summary>
/// Defines a contract for handling delete commands within the application.
/// </summary>
/// <typeparam name="TCommand">The type of the delete command being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response returned after the command is processed.</typeparam>
public interface IDeleteCommandHandlerBase<in TCommand, TResponse> : ICommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;

/// <summary>
/// Defines a base interface for handling action commands within the Command-Query Responsibility Segregation (CQRS) pattern.
/// </summary>
/// <typeparam name="TCommand">The type of the command being handled. Must implement <see cref="ICommandBase{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the command handler.</typeparam>
public interface IActionCommandHandlerBase<in TCommand, TResponse> : ICommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;
