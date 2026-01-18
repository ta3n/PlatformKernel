using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingAggregateFaxAuditLog;

public class SaveBookingAggregateFaxAuditLogCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IBookingAggregateFaxAuditLogService faxAuditLogService
) : UpdateCommandHandlerBase<SaveBookingAggregateFaxAuditLogCommand, bool>(unitOfWork, mapper)
{
    protected override async Task<bool> HandleAsync(
        SaveBookingAggregateFaxAuditLogCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var newFaxAuditLog =
            Mapper.Map<Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs.BookingAggregateFaxAuditLog>(payload);

        await faxAuditLogService.CreateAsync(newFaxAuditLog, true, cancellationToken);

        return true;
    }
}
