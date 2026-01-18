using FluentValidation.AspNetCore;
using Liberty.ApplicationShared.Logging;
using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.UseCases.Queries.BookingReservation;
using Liberty.Reservation.Application.UseCases.Queries.Timezone;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application;
using Liberty.UnitOfWork.Behaviors;
using MediatR;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Configurations;

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
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExecutionStrategyBehavior<,>));

        return services;
    }

    private static void RegisterBookingCommandHandlers(
        IServiceCollection services
    )
    {
        services.AddTransient<
            IRequestHandler<BookingSearchByPlanQuery, (IHeaderDictionary, IEnumerable<BookingSearchByPlanResponse>)>,
            BookingSearchByPlanQueryHandler
        >();

        services.AddTransient<
            IRequestHandler<GetAllTimeZoneQuery, (IHeaderDictionary, IEnumerable<TimeZoneResponse>)>,
            GetAllTimeZoneQueryHandler
        >();
    }
}
