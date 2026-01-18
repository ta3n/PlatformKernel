using System.Globalization;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.SysException.Exceptions;
using Liberty.UnitOfWork.Abstractions;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;

public class FacilityUpdateCommandHandler(
    ILogger<FacilityUpdateCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMediator mediator,
    ICacheService cacheService,
    IFacilityService facilityService,
    ISiteService siteService,
    IFacilitySiteService facilitySiteService
) : UpdateCommandHandlerBase<FacilityUpdateCommand, FacilityResponse>(unitOfWork, mapper)
{
    protected override async Task<FacilityResponse> HandleAsync(
        FacilityUpdateCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var existingCountSites = await siteService.CountByIdsAsync(
            payload.SiteIds ?? [],
            cancellationToken
        );
        if (existingCountSites != (payload.SiteIds ?? []).Length)
        {
            throw new SiteNotfoundException();
        }

        var facility = Mapper.Map<Reservation.Application.Contexts.DataContexts.Entities.Data.Facility>(payload);
        facility.Meta!.SystemEMail = payload.SystemEMail;

        var existingCount = await facilityService.CountByIdsAsync(
            [payload.Id ?? 0],
            cancellationToken
        );

        if (existingCount is 0)
        {
            var requestCreate = new FacilityCreateRequest(
                payload.Id ?? 0
            )
            {
                CanOnLinePayment = facility.CanOnLinePayment,
                SystemEMail = payload.SystemEMail,
                Memo = payload.Memo
            };
            _ = await mediator.Send(
                new FacilityCreateCommand { Payload = requestCreate },
                cancellationToken
            );
        }

        var existingSitesInFacility = (await facilitySiteService.FindAllByFacilityIdAsync(
            payload.Id ?? 0,
            cancellationToken
        )).ToList();

        var keepSitesInFacility = existingSitesInFacility
            .Where(
                x => payload.SiteIds!.Contains(x.SiteId)
            )
            .ToList();

        var keepSiteIdsInFacility = keepSitesInFacility
            .Select(x => x.SiteId)
            .ToArray();

        var deleteSitesInFacility = existingSitesInFacility.Except(keepSitesInFacility).ToList();

        var addSitesInFacility = new List<FacilitySite>();
        if (payload.SiteIds is { Length: > 0 })
        {
            addSitesInFacility.AddRange(
                payload.SiteIds
                    .Where(
                        x => !keepSiteIdsInFacility.Contains(x)
                    )
                    .Select(
                        x => new FacilitySite
                        {
                            FacilityId = payload.Id ?? 0,
                            SiteId = x,
                            IsEnabled = true,
                            RecordMemo = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                        }
                    )
            );
        }

        try
        {
            await UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);

            if (existingCount > 0)
            {
                _ = await facilityService.UpdateAsync(
                    facility,
                    false,
                    cancellationToken: cancellationToken
                );
            }

            if (deleteSitesInFacility is { Count: > 0 })
            {
                await facilitySiteService.DeleteRangeAsync(
                    deleteSitesInFacility,
                    false,
                    cancellationToken
                );
            }

            if (addSitesInFacility is { Count: > 0 })
            {
                await facilitySiteService.CreateRangeAsync(
                    addSitesInFacility,
                    false,
                    cancellationToken
                );
            }

            await UnitOfWork.CommitAsync(cancellationToken);

            var response = Mapper.Map<FacilityResponse>(facility);

            await cacheService.ResetAsync(
                string.Format(CacheKeys.ResetPatternManagerFacilitySetting, facility.Id),
                false,
                cancellationToken
            );

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update facility failed: {Message}", ex.Message);
            await UnitOfWork.RollbackAsync(cancellationToken);
            throw new AppLibertyException(ex.Message, ex);
        }
    }
}
