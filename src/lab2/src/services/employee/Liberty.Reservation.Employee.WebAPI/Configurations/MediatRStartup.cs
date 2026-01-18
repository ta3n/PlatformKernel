using FluentValidation.AspNetCore;
using Liberty.ApplicationShared.Logging;
using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.UseCases.Queries.Timezone;
using Liberty.Reservation.Employee.WebAPI.Application;
using Liberty.UnitOfWork.Behaviors;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

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

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DbContextRoutingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExecutionStrategyBehavior<,>));

        services.AddTransient<
            IRequestHandler<GetAllTimeZoneQuery, (IHeaderDictionary, IEnumerable<TimeZoneResponse>)>,
            GetAllTimeZoneQueryHandler
        >();

        return services;
    }
}
