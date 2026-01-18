namespace Liberty.ApplicationShared.Cqrs;

public abstract record RequestBase<TResponse> : IRequestBase<TResponse>;
