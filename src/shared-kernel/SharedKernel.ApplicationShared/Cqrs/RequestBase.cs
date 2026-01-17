namespace SharedKernel.ApplicationShared.Cqrs;

/// <summary>
/// Represents a base record type for defining CQRS requests.
/// </summary>
/// <typeparam name="TResponse">The type of response produced when the request is handled.</typeparam>
public abstract record RequestBase<TResponse> : IRequestBase<TResponse>;
