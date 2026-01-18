using Liberty.Cache.Services;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.UnitOfWork.Abstractions;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;

public class FacilityEnableCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    IFacilityService facilityService,
    ICacheService cacheService,
    IFacilityExternalRepository facilityExternalRepository
) : UpdateCommandHandlerBase<FacilityEnableCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        FacilityEnableCommand request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = request.Payload.Id ?? 0;
        var existingCount = await facilityService.CountByIdsAsync([facilityId], cancellationToken);

        if (existingCount is 0)
        {
            var facilityExternal = await facilityExternalRepository.GetFacilityByIdAsync(
                    facilityId,
                    cancellationToken
                )
                ?? throw new FacilityNotfoundException();

            var requestCreate = new FacilityCreateRequest(facilityId)
            {
                SystemEMail = facilityExternal.Email,
                Memo = facilityExternal.Memo,
                IsEnabled = request.Payload.IsEnabled
            };
            var response = await mediator.Send(
                new FacilityCreateCommand { Payload = requestCreate },
                cancellationToken
            );

            return response.Id;
        }

        _ = await facilityService.EnableAsync(
            facilityId,
            request.Payload.IsEnabled,
            cancellationToken: cancellationToken
        );

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternManagerFacilitySetting, facilityId),
            false,
            cancellationToken
        );

        return facilityId;
    }
}
