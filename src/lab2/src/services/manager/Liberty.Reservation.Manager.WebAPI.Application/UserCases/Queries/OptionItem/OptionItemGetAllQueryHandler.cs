using Liberty.Cache.Services;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;
using OptionItemEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.OptionItem;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItem;

public class OptionItemGetAllQueryHandler(
    IMapper mapper,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    IOptionItemRepository optionItemRepository
) : QueryPageBaseHandler<OptionItemGetAllQuery, OptionItemResponse>(mapper, cacheService)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<OptionItemResponse>)> HandleAsync(
        OptionItemGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = optionItemRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.FileOptionItems!)
            .ThenInclude(x => x.File)
            .Where(
                x => x.FacilityOptionItems!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .OrderByDescending(x => x.DisplayOrder);

        var page = await queryable.UsePageableAsDtoAsync<OptionItemEntity, OptionItemResponse>(
            request.Pageable,
            Mapper,
            cancellationToken: cancellationToken
        );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}
