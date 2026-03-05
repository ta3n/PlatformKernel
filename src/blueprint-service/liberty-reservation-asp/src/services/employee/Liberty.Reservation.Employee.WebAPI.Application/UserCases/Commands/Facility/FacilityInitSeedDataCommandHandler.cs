using Liberty.Cache.Services;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;

public class FacilityInitSeedDataCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    ICacheService cacheService,
    IFacilityRepository facilityRepository,
    IFacilityExternalRepository facilityExternalRepository,
    IFacilityInitDefaultDataService facilityInitDefaultService,
    IFacilityInitStatusService facilityInitStatusService
) : UpdateCommandHandlerBase<FacilityInitSeedDataCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        FacilityInitSeedDataCommand request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = request.Payload.Id;

        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == facilityId);
        var facility = await queryable.SingleOrDefaultAsync(cancellationToken);

        if (facility is null)
        {
            var facilityExternal = await facilityExternalRepository.GetFacilityByIdAsync(
                    facilityId,
                    cancellationToken
                )
                ?? throw new FacilityNotfoundException();

            var requestCreate = new FacilityCreateRequest(facilityId)
            {
                SystemEMail = facilityExternal.Email,
                Memo = facilityExternal.Memo
            };
            var response = await mediator.Send(
                new FacilityCreateCommand { Payload = requestCreate },
                cancellationToken
            );

            return response.Id;
        }

        var facilityInitStatus = await facilityInitStatusService.GetFacilitySeedDataStatusAsync(
            facilityId,
            cancellationToken
        );
        var hasChanged = false;

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            if (!facilityInitStatus.HasSeededPersonAgeTypeData)
            {
                await facilityInitDefaultService.AddDefaultPersonAgeTypesAsync(
                    facility,
                    false,
                    cancellationToken
                );
                hasChanged = true;
            }

            if (hasChanged)
            {
                await cacheService.ResetAsync(
                    string.Format(CacheKeys.ResetPatternManagerFacilitySetting, facilityId),
                    false,
                    cancellationToken
                );
            }

            await UnitOfWork.CommitAsync(cancellationToken);

            return facilityId;
        }
        catch (Exception ex)
        {
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
