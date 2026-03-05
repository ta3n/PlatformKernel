using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MasterCalendar;

public class MasterCalendarUpdateDataCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheService cacheService,
    IAppDateService appDateService,
    IDateDataOfAppDateService dateDataOfAppDateService
) : UpdateCommandHandlerBase<MasterCalendarUpdateDataCommand, DateDataOfMasterCalendarResponse>(unitOfWork, mapper)
{
    protected override async Task<DateDataOfMasterCalendarResponse> HandleAsync(
        MasterCalendarUpdateDataCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var existingAppDate = await appDateService.FindByAppDateAsync(
            payload.AppDate,
            cancellationToken
        );

        var existingDateDataOfAppDate = await dateDataOfAppDateService.FindByAppDateAsync(
            payload.AppDate,
            cancellationToken
        );

        if (existingDateDataOfAppDate is null)
        {
            var dateDataOfAppDate = new AppDateAppDateData { AppDateData = new AppDateData { Name = payload.Name } };
            if (existingAppDate is null)
            {
                dateDataOfAppDate.AppDate = new AppDate
                {
                    Id = payload.AppDate,
                    DateTime = AppDate.GetDateTime(payload.AppDate)
                };
            }
            else
            {
                dateDataOfAppDate.AppDateId = existingAppDate.Id;
            }

            _ = await dateDataOfAppDateService.CreateAsync(
                dateDataOfAppDate,
                cancellationToken: cancellationToken
            );
        }
        else
        {
            if (existingDateDataOfAppDate.AppDateData is null)
            {
                existingDateDataOfAppDate.AppDateData = new AppDateData { Name = payload.Name };
            }
            else
            {
                existingDateDataOfAppDate.AppDateData!.Name = payload.Name;
            }

            _ = await dateDataOfAppDateService.UpdateAsync(
                existingDateDataOfAppDate,
                cancellationToken: cancellationToken
            );
        }

        await cacheService.ResetAsync($"{nameof(AppDate)}*", cancellationToken: cancellationToken);

        return new DateDataOfMasterCalendarResponse(
            payload.AppDate,
            existingDateDataOfAppDate?.AppDateData?.Name ?? payload.Name
        );
    }
}
