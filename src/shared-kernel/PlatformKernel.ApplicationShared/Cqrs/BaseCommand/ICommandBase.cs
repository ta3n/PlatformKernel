namespace PlatformKernel.ApplicationShared.Cqrs.BaseCommand;

/// <summary>
/// Defines the base contract for a command in the CQRS pattern.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
public interface ICommandBase<out TResponse> : IRequestBase<TResponse>;

/// <summary>
/// Defines a base interface for create command operations, specifying the data model and response type.
/// </summary>
/// <typeparam name="TModel">
/// The type of the data model associated with the create command. This must be a reference type.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned from the command operation.
/// </typeparam>
public interface ICreateCommandBase<TModel, out TResponse> : ICommandBase<TResponse>
    where TModel : class
{
    /// <summary>
    /// Represents the data model or entity used as input for a command.
    /// </summary>
    /// <remarks>
    /// This property is a required component of commands derived from
    /// <see cref="ICreateCommandBase{TModel, TResponse}"/>,
    /// <see cref="IUpdateCommandBase{TModel, TResponse}"/>,
    /// <see cref="IDeleteCommandBase{TModel, TResponse}"/>, or other derived classes.
    /// It encapsulates the state or changes that the command will act upon.
    /// </remarks>
    /// <typeparam name="TModel">
    /// The type of the model or entity represented by the payload. It must be a reference type.
    /// </typeparam>
    TModel Payload { get; set; }
}

/// <summary>
/// Represents a command interface used for updating existing entities.
/// </summary>
/// <typeparam name="TModel">The type of the model being updated, typically defining the structure of the update payload.</typeparam>
/// <typeparam name="TResponse">The type of the response returned upon successful execution of the update command.</typeparam>
public interface IUpdateCommandBase<TModel, out TResponse> : ICommandBase<TResponse>
    where TModel : class
{
    /// <summary>
    /// Represents the payload for a command, encapsulating the data necessary to perform a specified operation.
    /// </summary>
    /// <remarks>
    /// The Payload property acts as a container for the core data required by the command implementation.
    /// Typically used in commands for creating, updating, deleting, or performing specific actions on a model.
    /// </remarks>
    /// <typeparam name="TModel">The type of the data model contained within the payload.</typeparam>
    /// <value>
    /// The data or entity instance that the command operates upon.
    /// </value>
    TModel Payload { get; set; }
}

/// <summary>
/// Represents a delete operation command in the context of the CQRS pattern.
/// </summary>
/// <typeparam name="TModel">The type of the model or payload being deleted.</typeparam>
/// <typeparam name="TResponse">The type of the response returned after the delete operation is executed.</typeparam>
public interface IDeleteCommandBase<TModel, out TResponse> : ICommandBase<TResponse>
{
    /// <summary>
    /// Represents the payload of the command. This property contains the data
    /// required to perform the associated operation, typically encapsulating
    /// the state or object to be created, updated, deleted, or manipulated.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of the model or data encapsulated within the payload,
    /// commonly used to define the target data structure for the command.
    /// </typeparam>
    TModel Payload { get; set; }
}
