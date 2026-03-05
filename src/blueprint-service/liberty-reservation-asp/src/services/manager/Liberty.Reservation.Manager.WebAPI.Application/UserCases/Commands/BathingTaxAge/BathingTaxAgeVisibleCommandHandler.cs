using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;

public class BathingTaxAgeVisibleCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IPersonAgeTypeService personAgeTypeService,
    IFacilityService facilityService
) : UpdateCommandHandlerBase<BathingTaxAgeVisibleCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        BathingTaxAgeVisibleCommand request,
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

        var facilityPersonAgeType = await personAgeTypeService.FindByIdAsync(
                payload.Id,
                cancellationToken
            )
            ?? throw new PersonAgeTypeNotfoundException();

        facilityPersonAgeType.IsVisible = request.Payload.IsVisible;

        await personAgeTypeService.UpdateAsync(
            facilityPersonAgeType,
            cancellationToken: cancellationToken
        );

        return facilityPersonAgeType.Id;
    }
}
