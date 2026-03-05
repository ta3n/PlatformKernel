using FluentValidation.AspNetCore;
using Liberty.ApplicationShared.Logging;
using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.UseCases.Commands.BookingReservation;
using Liberty.Reservation.Application.UseCases.Queries.AlertMessage;
using Liberty.Reservation.Application.UseCases.Queries.BookingReservation;
using Liberty.Reservation.User.WebAPI.Application;
using Liberty.UnitOfWork.Behaviors;
using MediatR;

namespace Liberty.Reservation.User.WebAPI.Configurations;

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
            IRequestHandler<BookingCreateCommand, Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>,
            BookingCreateCommandHandler
        >();

        services.AddTransient<
            IRequestHandler<BookingAdjustCommand, Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>,
            BookingAdjustCommandHandler
        >();

        services.AddTransient<
            IRequestHandler<BookingAdjustHeaderDataCommand, Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>,
            BookingAdjustHeaderDataCommandHandler
        >();

        services.AddTransient<
            IRequestHandler<BookingAdjustPriceCommand, Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>,
            BookingAdjustPriceCommandHandler
        >();
        services.AddTransient<IRequestHandler<BookingAbortCommand, BookingAbortResponse>, BookingAbortCommandHandler>();

        services.AddTransient<
            IRequestHandler<GetAllAlertMessageActiveQuery, (IHeaderDictionary, IEnumerable<AlertMessageResponse>)>,
            GetAllAlertMessageActiveQueryHandler
        >();

        services.AddTransient<
            IRequestHandler<GmoSearchTradeQuery, (IHeaderDictionary, SearchTradeResponse)>,
            GmoSearchTradeQueryHandler
        >();
    }
}
