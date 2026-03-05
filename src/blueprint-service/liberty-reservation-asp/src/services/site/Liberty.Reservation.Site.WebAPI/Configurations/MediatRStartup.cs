using FluentValidation.AspNetCore;
using Liberty.ApplicationShared.Logging;
using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.UseCases.Commands.BookingReservation;
using Liberty.Reservation.Application.UseCases.Queries.AlertMessage;
using Liberty.Reservation.Application.UseCases.Queries.BookingReservation;
using Liberty.Reservation.Site.WebAPI.Application;
using Liberty.UnitOfWork.Behaviors;
using MediatR;

namespace Liberty.Reservation.Site.WebAPI.Configurations;

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
        services.AddTransient<
            IRequestHandler<BookingSearchByPlanQuery, (IHeaderDictionary, IEnumerable<BookingSearchByPlanResponse>)>,
            BookingSearchByPlanQueryHandler
        >();

        services.AddTransient<
            IRequestHandler<BookingCreateCommand, Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>,
            BookingCreateCommandHandler
        >();

        services.AddTransient<
            IRequestHandler<GetAllAlertMessageActiveQuery, (IHeaderDictionary, IEnumerable<AlertMessageResponse>)>,
            GetAllAlertMessageActiveQueryHandler
        >();
    }
}
