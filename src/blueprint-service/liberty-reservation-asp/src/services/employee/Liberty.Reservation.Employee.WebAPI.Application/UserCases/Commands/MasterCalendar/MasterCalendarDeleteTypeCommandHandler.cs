using Liberty.Cache.Services;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MasterCalendar;

public class MasterCalendarDeleteTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheService cacheService,
    IDateTypeOfAppDateService dateTypeOfAppDateService
) : DeleteCommandHandlerBase<MasterCalendarDeleteTypeCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        MasterCalendarDeleteTypeCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var existingDateTypesOfAppDate = (await dateTypeOfAppDateService.FindAllByAppDateAsync(
            payload.AppDate,
            cancellationToken
        )).ToList();

        if (existingDateTypesOfAppDate.Count == 0 || existingDateTypesOfAppDate.TrueForAll(x => x.IsDeleted))
        {
            throw new AppDateTypeNotfoundException();
        }

        _ = await dateTypeOfAppDateService.DeleteRangeAsync(
            existingDateTypesOfAppDate,
            cancellationToken: cancellationToken
        );

        await cacheService.ResetAsync($"{nameof(AppDate)}*", cancellationToken: cancellationToken);

        return payload.AppDate;
    }
}
