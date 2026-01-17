using Blueprint.Service.Base.Application.Behaviors;
using Blueprint.Service.WebApi.Application;
using FluentValidation.AspNetCore;
using MediatR;
using SharedKernel.ApplicationShared.Logging;
using SharedKernel.UnitOfWork.Behaviors;

namespace PlatformKernel.Service.WebApi.Configurations;

public static class MediatRStartup
{
    public static IServiceCollection AddMediatRModule(
        this IServiceCollection services
    )
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        services.AddKeyedSingleton(
            "AssemblyForRequestValidationBehavior",
            typeof(AssemblyDefinition).Assembly
        );

        services.AddMediatR(typeof(AssemblyDefinition));

        RegisterBookingCommandHandlers(services);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DbContextRoutingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExecutionStrategyBehavior<,>));

        return services;
    }

    private static void RegisterBookingCommandHandlers(
        IServiceCollection services
    )
    {
        _ = services;
    }
}
