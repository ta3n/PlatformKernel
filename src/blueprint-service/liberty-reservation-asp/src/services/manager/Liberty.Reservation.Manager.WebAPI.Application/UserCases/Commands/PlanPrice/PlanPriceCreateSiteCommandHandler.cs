using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public class PlanPriceCreateSiteCommandHandler(
    ILogger<PlanPriceCreateSiteCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IPlanRoomGroupSiteService planRoomGroupSiteService,
    IFacilityService facilityService,
    IPlanRoomGroupSitePersonAgeTypeService planRoomGroupSitePersonAgeTypeService
) : UpdateCommandHandlerBase<PlanPriceCreateSiteCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanPriceCreateSiteCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var facilityId = securityContextAccessor.FacilityKey;

        var roomOfPlan = await planRoomGroupSiteService.FindByPlanIdAndRomTypeIdAsync(
            payload.PlanId,
            payload.RomTypeId,
            payload.SiteId,
            cancellationToken
        );
        if (roomOfPlan is not null)
        {
            return payload.PlanId;
        }

        var planRoomGroupSiteAdd = new PlanRoomGroupSite
        {
            PlanId = request.Payload.PlanId,
            RoomGroupId = request.Payload.RomTypeId,
            SiteId = request.Payload.SiteId,
            IsEnabled = true
        };

        var planRoomGroupSitePersonAgeTypes = new List<PlanRoomGroupSitePersonAgeType>();

        var existingFacility = await facilityService.FindByIdWithIncludePersonAgeTypeAsync(
            facilityId,
            cancellationToken
        );

        foreach (var facilityPersonAgeType in existingFacility.FacilityPersonAgeTypes!)
        {
            planRoomGroupSitePersonAgeTypes.Add(
                new PlanRoomGroupSitePersonAgeType
                {
                    PlanId = request.Payload.PlanId,
                    RoomGroupId = request.Payload.RomTypeId,
                    SiteId = request.Payload.SiteId,
                    PersonAgeTypeId = facilityPersonAgeType.PersonAgeTypeId,
                    IsEnabled = facilityPersonAgeType.PersonAgeType!.IsMain,
                    IsRegardAdult = facilityPersonAgeType.PersonAgeType!.IsMain,
                    PriceSettingType = facilityPersonAgeType.PersonAgeType!.IsMain ? PriceSettingTypes.Price : PriceSettingTypes.None,
                    Value = 0
                }
            );
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            var addSiteInPlanRoom = await planRoomGroupSiteService.CreateAsync(
                planRoomGroupSiteAdd,
                false,
                cancellationToken
            );

            await planRoomGroupSitePersonAgeTypeService.CreateRangeAsync(
                planRoomGroupSitePersonAgeTypes,
                false,
                cancellationToken
            );

            await UnitOfWork.CommitAsync(cancellationToken);

            return addSiteInPlanRoom.PlanId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Action} {ErrorMessage}", nameof(PlanPriceCreateSiteCommandHandler), ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
