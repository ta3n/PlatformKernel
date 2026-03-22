# SharedKernel.CQRS

`SharedKernel.CQRS` now exposes two provider-specific implementations side by side:

- `SharedKernel.CQRS.MediatR`: legacy abstractions based on `MediatR`
- `SharedKernel.CQRS.Mediator`: new abstractions based on [`Mediator`](https://github.com/martinothamar/Mediator)

## Folder layout

- `MediatR/`: preserved legacy CQRS contracts and base handlers
- `Mediator/`: equivalent CQRS contracts and base handlers for `martinothamar/Mediator`

## Registration

For `Mediator`, add `Mediator.SourceGenerator` to the consuming application and call `AddMediator(...)` from the app project that owns your handlers.

```csharp
services.AddMediator(options =>
{
    options.Assemblies = [typeof(ApplicationAssemblyMarker)];
    options.PipelineBehaviors =
    [
        typeof(SharedKernel.UnitOfWork.Behaviors.Mediator.ExecutionStrategyBehavior<,>),
        typeof(SharedKernel.UnitOfWork.Behaviors.Mediator.LoggingBehavior<,>)
    ];
});
```

For `MediatR`, keep using the existing `AddMediatR(...)` registration in the consuming application and register the matching behaviors from `SharedKernel.UnitOfWork.Behaviors.MediatR`.
