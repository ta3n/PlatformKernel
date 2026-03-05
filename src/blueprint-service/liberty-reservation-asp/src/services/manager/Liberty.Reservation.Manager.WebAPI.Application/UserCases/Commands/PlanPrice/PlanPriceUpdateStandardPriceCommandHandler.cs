using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public class PlanPriceUpdateStandardPriceCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPlanRoomGroupSiteAppDateTypePriceService planRoomGroupSite,
    IAppDateTypeService appDateTypeService,
    IPlanService planService,
    IFacilityService facilityService,
    IPlanRoomGroupSiteService planRoomGroupSiteService,
    ISecurityContextAccessor securityContextAccessor,
    ILogger<PlanPriceUpdateStandardPriceCommandHandler> logger
) : UpdateCommandHandlerBase<PlanPriceUpdateStandardPriceCommand, long>(unitOfWork, mapper)
{
    protected override async Task<long> HandleAsync(
        PlanPriceUpdateStandardPriceCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        await CheckValidationMinimumPriceAsync(request, cancellationToken);

        if (payload.PriceDatas is not null)
        {
            var listAppDateTypeId = payload.PriceDatas!.Select(x => x.DateTypeId).Distinct().ToArray();
            var existingCountAppDateType = await appDateTypeService.CountByIdsAsync(
                listAppDateTypeId,
                cancellationToken
            );

            if (listAppDateTypeId.Length != existingCountAppDateType)
            {
                throw new AppDateTypeNotfoundException();
            }
        }

        var listPriceDataOfSiteInPlanRoom = new List<PlanRoomGroupSiteAppDateTypePriceData>();

        foreach (var price in payload.PriceDatas!)
        {
            var priceDataOfSiteInPlanRoom = new PlanRoomGroupSiteAppDateTypePriceData
            {
                PlanId = request.PlanId,
                RoomGroupId = request.RoomTypeId,
                SiteId = request.SiteId,
                AppDateTypeId = price.DateTypeId,
                PriceData = new PriceData
                {
                    PersonMin = price.PersonMin,
                    PersonMax = price.PersonMax,
                    Price = price.Price,
                    IsEnabled = true
                },
                IsEnabled = true
            };
            listPriceDataOfSiteInPlanRoom.Add(priceDataOfSiteInPlanRoom);
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            _ = await planRoomGroupSite.ChangePriceDataOfSiteInPlanRoom(
                request.PlanId,
                request.RoomTypeId,
                request.SiteId,
                listPriceDataOfSiteInPlanRoom,
                false,
                cancellationToken
            );

            await planService.UpdateLastModifiedAsync(request.PlanId, cancellationToken);
            await UnitOfWork.CommitAsync(cancellationToken);

            return request.PlanId;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{Action} {Message}",
                nameof(PlanPriceUpdateStandardPriceCommandHandler),
                ex.Message
            );
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }

    private async Task CheckValidationMinimumPriceAsync(
        PlanPriceUpdateStandardPriceCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingPlanRoomGroupSite = await planRoomGroupSiteService.FindByPlanIdAndRomTypeIdAsync(
                request.PlanId,
                request.RoomTypeId,
                request.SiteId,
                cancellationToken
            )
            ?? throw new PlanRoomGroupSiteNotFoundException(request.PlanId, request.RoomTypeId, request.SiteId);

        var maxPrice = request.Payload.PriceDatas!.Max(p => p.Price);
        var minPrice = request.Payload.PriceDatas!.Min(p => p.Price);
        if (existingPlanRoomGroupSite.IsEnabledMinimumPrice)
        {
            ValidatePriceAgainstMinimum(maxPrice, minPrice, existingPlanRoomGroupSite.MinimumPrice);
            return;
        }

        var facilityId = securityContextAccessor.FacilityKey;
        var facilityMinimumPriceData = await facilityService.GetMinimumPriceAsync(facilityId, cancellationToken)
            ?? throw new FacilityNotfoundException();

        if (facilityMinimumPriceData.IsEnabledMinimumPrice)
        {
            ValidatePriceAgainstMinimum(maxPrice, minPrice, facilityMinimumPriceData.MinimumPrice);
        }
    }

    private static void ValidatePriceAgainstMinimum(
        int? maxPrice,
        int? minPrice,
        int? minimumPrice
    )
    {
        if (maxPrice < minimumPrice || minPrice < minimumPrice)
        {
            throw new PlanStandardPriceFacilityMinimumException();
        }
    }
}
