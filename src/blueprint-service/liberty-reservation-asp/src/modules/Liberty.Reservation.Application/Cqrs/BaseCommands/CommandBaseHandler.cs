using AutoMapper;
using Liberty.ApplicationShared.Cqrs.BaseCommand;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Application.Cqrs.BaseCommands;

/// <summary>
/// Serves as an abstract base class for handling commands with a specified response type.
/// Encapsulates common functionality for command execution and dependency management.
/// </summary>
/// <typeparam name="TCommand">The type of the command being processed, restricted to implementations of ICommandBase&lt;TResponse&gt;.</typeparam>
/// <typeparam name="TResponse">The type of the result produced by the command handler.</typeparam>
public abstract class CommandBaseHandler<TCommand, TResponse>(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : ICommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>
{
    /// <summary>
    /// Represents the unit of work mechanism used for managing database transactions.
    /// This property supports transactional operations, enabling consistent and atomic
    /// execution of multiple operations within a command handler or similar context.
    /// </summary>
    protected IUnitOfWork UnitOfWork { get; } = unitOfWork;

    /// <summary>
    /// Provides access to the object-object mapping functionality encapsulated by the AutoMapper library.
    /// This property is used to map data between objects, particularly for transforming command payloads
    /// or entities into desired models for processing or persistence.
    /// </summary>
    protected IMapper Mapper { get; } = mapper;

    /// <summary>
    /// Handles the processing of the given command request and performs the required operations,
    /// including cache removal after processing. This method serves as the entry point for command handling logic.
    /// </summary>
    /// <param name="request">The command request containing the data necessary for processing.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous handling process, with the result being the response of the executed command.
    /// </returns>
    public virtual Task<TResponse> Handle(
        TCommand request,
        CancellationToken cancellationToken
    )
    {
        var response = HandleAsync(request, cancellationToken);

        RemoveCaches(request);

        return response;
    }

    /// <summary>
    /// Processes the asynchronous execution of a command and performs the required operations to generate a corresponding response.
    /// </summary>
    /// <param name="request">The command request containing all pertinent data necessary for processing.</param>
    /// <param name="cancellationToken">A token used to observe cancellation requests and halt the operation if necessary.</param>
    /// <returns>A task that represents the asynchronous operation, resulting in the response produced from the command execution.</returns>
    protected abstract Task<TResponse> HandleAsync(
        TCommand request,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Removes cached data related to the specified command instance.
    /// </summary>
    /// <param name="request">The command object that serves as the context for determining which cache entries should be removed.</param>
    protected virtual void RemoveCaches(
        TCommand request
    )
    {
    }
}

/// <summary>
/// Represents a base class for handling create commands and returning a response.
/// </summary>
/// <typeparam name="TCommand">The type of the create command being handled. Must implement <see cref="ICommandBase{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned after processing the create command.</typeparam>
/// <remarks>
/// Extends <see cref="CommandBaseHandler{TCommand, TResponse}"/> to provide foundational handling capabilities for commands,
/// while specifically implementing the <see cref="ICreateCommandHandlerBase{TCommand, TResponse}"/>
/// to focus on operations related to create commands.
/// </remarks>
public abstract class CreateCommandHandlerBase<TCommand, TResponse>(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : CommandBaseHandler<TCommand, TResponse>(
        unitOfWork,
        mapper
    ),
    ICreateCommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;

/// <summary>
/// Represents a base class for handling update commands in a CQRS pattern.
/// Provides functionality to implement command handlers that perform update operations,
/// leveraging a unit of work and object mapping capabilities.
/// </summary>
/// <typeparam name="TCommand">
/// The type of the command to be handled. Must implement <see cref="ICommandBase{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the command handler.
/// </typeparam>
public abstract class UpdateCommandHandlerBase<TCommand, TResponse>(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : CommandBaseHandler<TCommand, TResponse>(
        unitOfWork,
        mapper
    ),
    IUpdateCommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;

/// <summary>
/// Represents a base command handler for processing delete commands within the application.
/// Provides a foundation for handling delete operations, including dependency management for unit of work and object mapping.
/// </summary>
/// <typeparam name="TCommand">The type of the delete command to be handled. Must implement <see cref="ICommandBase{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the result produced by the delete command handler.</typeparam>
public abstract class DeleteCommandHandlerBase<TCommand, TResponse>(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : CommandBaseHandler<TCommand, TResponse>(
        unitOfWork,
        mapper
    ),
    IDeleteCommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;

/// <summary>
/// Represents the base class for handling action commands that do not return a result or involve modifying state.
/// </summary>
/// <typeparam name="TCommand">The type of the command being handled. Must implement <see cref="ICommandBase{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the command handler.</typeparam>
/// <remarks>
/// This class builds upon <see cref="CommandBaseHandler{TCommand, TResponse}"/>, providing support for action-oriented commands,
/// and implements the <see cref="IActionCommandHandlerBase{TCommand, TResponse}"/> interface for further specialization.
/// </remarks>
public abstract class ActionCommandHandlerBase<TCommand, TResponse>(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : CommandBaseHandler<TCommand, TResponse>(
        unitOfWork,
        mapper
    ),
    IActionCommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;
