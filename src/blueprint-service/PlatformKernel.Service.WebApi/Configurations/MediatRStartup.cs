using FluentValidation.AspNetCore;
using PlatformKernel.Service.WebApi.Application;
using MediatR;
using PlatformKernel.ApplicationShared.Logging;
using PlatformKernel.Service.Base.Application.Behaviors;
using PlatformKernel.UnitOfWork.Behaviors;

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
