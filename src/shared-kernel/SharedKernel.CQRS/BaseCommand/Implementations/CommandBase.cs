namespace SharedKernel.CQRS.BaseCommand.Implementations;

/// <summary>
/// Represents the base class for all commands in the CQRS pattern.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
public abstract record CommandBase<TResponse> : RequestBase<TResponse>, ICommandBase<TResponse>;

/// <summary>
/// Represents a base command for creating a resource of type <typeparamref name="TModel"/>
/// and processing a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TModel">
/// The type of the resource being created. It must be a reference type.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response that the command will return upon execution.
/// </typeparam>
public abstract record CreateCommandBase<TModel, TResponse>
    : RequestBase<TResponse>, ICreateCommandBase<TModel, TResponse>
    where TModel : class
{
    /// <summary>
    /// Represents the payload containing the data required to create a new entity or perform an associated operation.
    /// </summary>
    /// <remarks>
    /// The <c>Payload</c> property holds the model data associated with the command.
    /// This data is used by the command handler to process the request and perform the desired operations.
    /// </remarks>
    /// <typeparam name="TModel">The type of the model data contained in the payload.</typeparam>
    /// <seealso cref="CreateCommandBase{TModel, TResponse}"/>
    public required TModel Payload { get; set; }
}

/// <summary>
/// Represents a base class for update commands in the application that work with a specific model type.
/// This class provides a structure for handling update-related command operations and defines a required payload
/// containing the data to be updated.
/// </summary>
/// <typeparam name="TModel">The type of the model being updated. Must be a class.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the operation.</typeparam>
public abstract record UpdateCommandBase<TModel, TResponse>
    : RequestBase<TResponse>, IUpdateCommandBase<TModel, TResponse>
    where TModel : class
{
    /// <summary>
    /// Represents the data or model associated with an update command.
    /// This property is required and must be set with the relevant model instance
    /// that the command intends to update.
    /// </summary>
    public required TModel Payload { get; set; }
}

/// <summary>
/// Represents a base class for delete commands used in the application.
/// This class is intended to encapsulate the behavior and properties required to
/// perform a delete operation on a specified model type.
/// </summary>
/// <typeparam name="TModel">
/// The type of the model associated with the delete operation.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response that the delete operation will return.
/// </typeparam>
public abstract record DeleteCommandBase<TModel, TResponse>
    : RequestBase<TResponse>, IDeleteCommandBase<TModel, TResponse>
    where TModel : class
{
    /// <summary>
    /// Represents the data payload to be processed by the delete command.
    /// </summary>
    /// <remarks>
    /// This property holds the model required for execution of the delete command.
    /// It is mandatory and must be properly populated before the command is executed.
    /// Ensures that all necessary data for the specific delete operation is encapsulated.
    /// </remarks>
    public required TModel Payload { get; set; }
}

/// <summary>
/// Represents the base action command structure for handling delete operations.
/// Inherits from <see cref="RequestBase{TResponse}"/> and implements <see cref="IDeleteCommandBase{TModel, TResponse}"/>.
/// This abstract record is used as a foundation for commands that require an associated payload of type <typeparamref name="TModel"/>.
/// </summary>
/// <typeparam name="TModel">The type of the payload to be passed with the command. Must be a reference type.</typeparam>
/// <typeparam name="TResponse">The type of the response expected from the command.</typeparam>
public abstract record ActionCommandBase<TModel, TResponse>
    : RequestBase<TResponse>, IDeleteCommandBase<TModel, TResponse>
    where TModel : class
{
    /// <summary>
    /// Represents the data payload associated with the command.
    /// This property contains the core model data required for processing the command.
    /// </summary>
    public required TModel Payload { get; set; }
}
