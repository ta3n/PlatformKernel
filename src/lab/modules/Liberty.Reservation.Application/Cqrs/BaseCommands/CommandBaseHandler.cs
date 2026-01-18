using AutoMapper;
using Liberty.ApplicationShared.Cqrs.BaseCommand;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Application.Cqrs.BaseCommands;

public abstract class CommandBaseHandler<TCommand, TResponse>(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : ICommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>
{
    protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
    protected IMapper Mapper { get; } = mapper;

    public virtual Task<TResponse> Handle(
        TCommand request,
        CancellationToken cancellationToken
    )
    {
        return HandleAsync(request, cancellationToken);
    }

    protected abstract Task<TResponse> HandleAsync(
        TCommand request,
        CancellationToken cancellationToken
    );
}

public abstract class CreateCommandHandlerBase<TCommand, TResponse>(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : CommandBaseHandler<TCommand, TResponse>(
        unitOfWork,
        mapper
    ),
    ICreateCommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;

public abstract class UpdateCommandHandlerBase<TCommand, TResponse>(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : CommandBaseHandler<TCommand, TResponse>(
        unitOfWork,
        mapper
    ),
    IUpdateCommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;

public abstract class DeleteCommandHandlerBase<TCommand, TResponse>(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : CommandBaseHandler<TCommand, TResponse>(
        unitOfWork,
        mapper
    ),
    IDeleteCommandHandlerBase<TCommand, TResponse>
    where TCommand : ICommandBase<TResponse>;
