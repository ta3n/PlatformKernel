using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItemInventory;

public class OptionItemInventoryGetAllQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IOptionItemAppDateRepository optionItemAppDateRepository
) : QueryPageBaseHandler<OptionItemInventoryGetAllQuery, OptionItemAppDateResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<OptionItemAppDateResponse>)> HandleAsync(
        OptionItemInventoryGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = optionItemAppDateRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.OptionItem)
            .Where(
                x => x.OptionItem!.FacilityOptionItems!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .Where(
                x => x.AppDateId >= request.StartAppDate
                    && x.AppDateId <= request.EndAppDate
            )
            .ProjectTo<OptionItemAppDateResponse>(Mapper.ConfigurationProvider);

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var headers = page.GeneratePaginationHttpHeaders();
        var data = page.Content.ToList();
        foreach (var value in data)
        {
            value.RemainNumber = value.SellNumber - value.ReservedNumber;
        }

        return (headers, data);
    }
}
