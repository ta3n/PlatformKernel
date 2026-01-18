using MediatR;

namespace Liberty.ApplicationShared.Cqrs;

public interface IRequestBase<out TResponse> : IRequest<TResponse>;
