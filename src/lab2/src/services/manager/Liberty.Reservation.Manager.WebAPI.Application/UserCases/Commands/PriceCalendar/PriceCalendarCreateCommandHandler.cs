using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PriceCalendar;

public class PriceCalendarCreateCommandHandler(
    ILogger<PriceCalendarCreateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    ICalendarAppDateAppDateTypeService calendarAppDateTypeService,
    IFacilityCalendarService facilityCalendarService,
    IAppDateTypeService appDateTypeService
) : CreateCommandHandlerBase<PriceCalendarCreateCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PriceCalendarCreateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = securityContextAccessor.FacilityKey;

        var appDateTypeIds = payload.Calendars.Select(x => x.PriceTypeId).Distinct().ToArray();
        var existingCountAppDateType = await appDateTypeService.CountByIdsAsync(
            appDateTypeIds,
            cancellationToken
        );

        if (appDateTypeIds.Length != existingCountAppDateType)
        {
            throw new AppDateTypeNotfoundException();
        }

        var facilityCalendar = await facilityCalendarService.FindByFacilityIdAsync(
            facilityId,
            cancellationToken
        );

        var calendar =
            facilityCalendar?.Calendar ?? new Calendar { Code = EntityUtil.CreateCode() };

        var listCalendarAppDateAppDateType = new List<CalendarAppDateAppDateType>();
        foreach (var item in payload.Calendars)
        {
            var newCalendarAppDateType = new CalendarAppDateAppDateType
            {
                AppDateTypeId = item.PriceTypeId,
                DateCalendar = item.DateCalendar,
                IsDeleted = item.IsDeleted
            };

            if (facilityCalendar is null)
            {
                newCalendarAppDateType.Calendar = calendar;
            }
            else
            {
                newCalendarAppDateType.CalendarId = calendar.Id;
            }

            listCalendarAppDateAppDateType.Add(newCalendarAppDateType);
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            if (facilityCalendar is null)
            {
                facilityCalendar = new FacilityCalendar
                {
                    FacilityId = securityContextAccessor.FacilityKey,
                    Calendar = calendar
                };

                await facilityCalendarService.CreateAsync(
                    facilityCalendar,
                    false,
                    cancellationToken
                );

                await UnitOfWork.CommitAsync(cancellationToken);
            }

            _ = await calendarAppDateTypeService.ChangeCalendarAppDate(
                calendar.Id,
                listCalendarAppDateAppDateType,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return calendar.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{Action} {Message}",
                nameof(PriceCalendarCreateCommandHandler),
                ex.Message
            );
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
