using Mediator;

namespace SharedKernel.CQRS.Mediator;

/// <summary>
/// Represents the base contract for a request in the CQRS pattern.
/// </summary>
/// <typeparam name="TResponse">The type of the response produced by handling the request.</typeparam>
public interface IRequestBase<out TResponse> : IMessage
{
    Type ResponseType => typeof(TResponse);
}
