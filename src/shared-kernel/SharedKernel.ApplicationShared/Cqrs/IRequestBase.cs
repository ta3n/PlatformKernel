using MediatR;

namespace SharedKernel.ApplicationShared.Cqrs;

/// <summary>
/// Represents the base contract for a request in the CQRS pattern.
/// </summary>
/// <typeparam name="TResponse">The type of the response produced by handling the request.</typeparam>
public interface IRequestBase<out TResponse> : IRequest<TResponse>;
