using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

public class FacilityUpdateBasicSettingCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityService facilityService
) : UpdateCommandHandlerBase<FacilityUpdateBasicSettingCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        FacilityUpdateBasicSettingCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = securityContextAccessor.FacilityKey;

        var existingFacilityCount = await facilityService.CountByIdsAsync(
            [facilityId],
            cancellationToken
        );
        if (existingFacilityCount == 0)
        {
            throw new FacilityNotfoundException();
        }

        var entityToUpdate = Mapper.Map<Reservation.Application.Contexts.DataContexts.Entities.Data.Facility>(payload);
        entityToUpdate.Id = facilityId;
        entityToUpdate.Meta!.Logo = payload.File?.Code;

        var editFacility = await facilityService.UpdateBasicSettingAsync(
            entityToUpdate,
            cancellationToken: cancellationToken
        );

        return editFacility.Id;
    }
}
