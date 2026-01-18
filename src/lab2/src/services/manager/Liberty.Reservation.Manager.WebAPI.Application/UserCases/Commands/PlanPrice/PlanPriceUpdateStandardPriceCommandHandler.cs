using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

public class PlanPriceUpdateStandardPriceCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<PlanPriceUpdateStandardPriceCommandHandler> logger,
    IServiceProvider serviceProvider
) : UpdateCommandHandlerBase<PlanPriceUpdateStandardPriceCommand, long>(unitOfWork, mapper)
{
    private readonly IPlanRoomGroupSiteAppDateTypePriceService _planRoomGroupSite =
        serviceProvider.GetRequiredService<IPlanRoomGroupSiteAppDateTypePriceService>();

    private readonly IAppDateTypeService _appDateTypeService =
        serviceProvider.GetRequiredService<IAppDateTypeService>();

    private readonly IPlanService _planService =
        serviceProvider.GetRequiredService<IPlanService>();

    private readonly IFacilityService _facilityService =
        serviceProvider.GetRequiredService<IFacilityService>();

    private readonly IPlanRoomGroupSiteService _planRoomGroupSiteService =
        serviceProvider.GetRequiredService<IPlanRoomGroupSiteService>();

    private readonly IPlanRoomGroupSitePriceDataService _planRoomGroupSitePriceDataService =
        serviceProvider.GetRequiredService<IPlanRoomGroupSitePriceDataService>();

    private readonly ISecurityContextAccessor _securityContextAccessor =
        serviceProvider.GetRequiredService<ISecurityContextAccessor>();

    protected override async Task<long> HandleAsync(
        PlanPriceUpdateStandardPriceCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var hasAppDateTypeId = true;

        if (payload.PriceDatas is not null && payload.PriceDatas.Count != 0)
        {
            var listAppDateTypeId = payload.PriceDatas!.Select(x => x.DateTypeId)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToArray();
            hasAppDateTypeId = listAppDateTypeId.Length > 0;
            if (listAppDateTypeId.Length > 0)
            {
                var existingCountAppDateType = await _appDateTypeService.CountByIdsAsync(
                    listAppDateTypeId,
                    cancellationToken
                );

                if (listAppDateTypeId.Length != existingCountAppDateType)
                {
                    throw new AppDateTypeNotfoundException();
                }

                await CheckValidationMinimumPriceAsync(request, cancellationToken);
            }
        }

        var listPriceDataOfSiteInPlanRoom = new List<PlanRoomGroupSiteAppDateTypePriceData>();
        var listRangePersonOfSiteInPlanRoom = new List<PlanRoomGroupSitePriceData>();

        var hashSet = new HashSet<(long PlanId, long RoomGroupId, long SiteId, int? PersonMin, int? PersonMax)>();
        foreach (var price in payload.PriceDatas!)
        {
            if (price.DateTypeId is not null)
            {
                var priceDataOfSiteInPlanRoom = new PlanRoomGroupSiteAppDateTypePriceData
                {
                    PlanId = request.PlanId,
                    RoomGroupId = request.RoomTypeId,
                    SiteId = request.SiteId,
                    AppDateTypeId = price.DateTypeId.Value,
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

            var key = (request.PlanId, request.RoomTypeId, request.SiteId, price.PersonMin, price.PersonMax);

            if (!hashSet.Add(key))
            {
                continue;
            }

            var planRoomGroupSitePriceData = new PlanRoomGroupSitePriceData
            {
                PlanId = request.PlanId,
                RoomGroupId = request.RoomTypeId,
                SiteId = request.SiteId,
                PersonMin = price.PersonMin,
                PersonMax = price.PersonMax,
                IsEnabled = true
            };

            listRangePersonOfSiteInPlanRoom.Add(planRoomGroupSitePriceData);
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            if (hasAppDateTypeId)
            {
                _ = await _planRoomGroupSite.ChangePriceDataOfSiteInPlanRoom(
                    request.PlanId,
                    request.RoomTypeId,
                    request.SiteId,
                    listPriceDataOfSiteInPlanRoom,
                    false,
                    cancellationToken
                );
            }

            _ = await _planRoomGroupSitePriceDataService.ChangeRangePersonOfSiteInPlanRoom(
                request.PlanId,
                request.RoomTypeId,
                request.SiteId,
                listRangePersonOfSiteInPlanRoom,
                false,
                cancellationToken
            );

            await _planService.UpdateLastModifiedAsync(request.PlanId, cancellationToken);
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
        var existingPlanRoomGroupSite = await _planRoomGroupSiteService.FindByPlanIdAndRomTypeIdAsync(
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

        var facilityId = _securityContextAccessor.FacilityKey;
        var facilityMinimumPriceData = await _facilityService.GetMinimumPriceAsync(facilityId, cancellationToken)
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
