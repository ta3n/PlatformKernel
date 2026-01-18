namespace Liberty.ApplicationShared.Cqrs.BaseCommand;

public interface ICommandBase<out TResponse> : IRequestBase<TResponse>;

public interface ICreateCommandBase<TModel, out TResponse> : ICommandBase<TResponse>
    where TModel : class
{
    TModel Payload { get; set; }
}

public interface IUpdateCommandBase<TModel, out TResponse> : ICommandBase<TResponse>
    where TModel : class
{
    TModel Payload { get; set; }
}

public interface IDeleteCommandBase<TModel, out TResponse> : ICommandBase<TResponse>
{
    TModel Payload { get; set; }
}
