using Liberty.Cache.Services;
using Liberty.Cache.Utils;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.Models;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;

public class FacilityGetGroupDetailsQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IFacilityRepository facilityRepository
) : QuerySingleBaseHandler<FacilityGetGroupDetailsQuery, object>(mapper, cacheService)
{
    protected override string GetCacheKey(
        FacilityGetGroupDetailsQuery request
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        return CacheHelper.GetCacheKeyByEntity(
            nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.Facility),
            $"{string.Format(CacheKeys.FacilityPrefixKey, facilityId)}",
            CacheHelper.ComputeHash(
                [
                    nameof(FacilityGetGroupDetailsQueryHandler),
                    GetRequestJson(request)
                ]
            )
        );
    }

    protected override async Task<(IHeaderDictionary, object)> HandleAsync(
        FacilityGetGroupDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = facilityRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == facilityId);

        var queryableByGroup = QueryableByGroup(
            request.Group,
            queryable
        );

        var data = await queryableByGroup.SingleOrDefaultAsync(
                cancellationToken
            )
            ?? throw new FacilityNotfoundException();

        return (new HeaderDictionary(), data);
    }

    private IQueryable<dynamic> QueryableByGroup(
        GroupOfFacility group,
        IQueryable<Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Facility> queryable
    )
    {
        IQueryable<dynamic> queryableByGroup = group switch
        {
            GroupOfFacility.Accept => queryable.ProjectTo<FacilityDetailAcceptResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfFacility.Access => queryable.ProjectTo<FacilityDetailAccessResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfFacility.Bath => queryable.ProjectTo<FacilityDetailBathResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfFacility.Classification => queryable.ProjectTo<FacilityDetailClassificationResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfFacility.PaymentMethod => queryable.ProjectTo<FacilityDetailPaymentMethodResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfFacility.Publish => queryable.ProjectTo<FacilityDetailPublishResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfFacility.ReservationChange => queryable.ProjectTo<FacilityDetailReservationChangeResponse>(
                Mapper.ConfigurationProvider
            ),
            GroupOfFacility.ReservationSetting => queryable.ProjectTo<FacilityDetailReservationSettingResponse>(
                Mapper.ConfigurationProvider
            ),
            _ => throw new NotImplementedException()
        };

        return queryableByGroup;
    }
}
