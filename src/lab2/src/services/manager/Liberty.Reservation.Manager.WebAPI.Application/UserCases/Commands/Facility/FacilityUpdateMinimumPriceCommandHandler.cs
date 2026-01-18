using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

public class FacilityUpdateMinimumPriceCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityService facilityService
) : UpdateCommandHandlerBase<FacilityUpdateMinimumPriceCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        FacilityUpdateMinimumPriceCommand request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var existingFacilityCount = await facilityService.CountByIdsAsync(
            [facilityId],
            cancellationToken
        );
        if (existingFacilityCount == 0)
        {
            throw new FacilityNotfoundException();
        }

        var entityToUpdate = Mapper.Map<Reservation.Application.Contexts.DataContexts.Entities.Data.Facility>(request.Payload);
        entityToUpdate.Id = facilityId;

        var editFacility = await facilityService.UpdateMinimumPriceAsync(
            entityToUpdate,
            cancellationToken: cancellationToken
        );

        return editFacility.Id;
    }
}
